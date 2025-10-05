using System.Runtime.InteropServices;
using System.Text.Json;
using HotkeyTyper.Models;
using HotkeyTyper.Managers;

namespace HotkeyTyper;

public partial class Form1 : Form
{
    // Windows API constants for hotkey registration
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID_BASE = 1000; // Base ID for hotkeys 1-9
    
    // NEW: Settings Manager and Configuration
    private SettingsManager settingsManager;
    private AppConfiguration appConfig;
    private SnippetSet? currentSet;
    private Snippet? currentSnippet;
    
    // Settings file path (OLD - for backward compatibility during migration)
    private readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
    
    // Predefined text to type
    private string predefinedText = "Hello, this is my predefined text!";
    
    // Typing speed (1 = slowest, 10 = fastest)
    private int typingSpeed = 5;
    
    // Cancellation for in-progress typing
    private CancellationTokenSource? typingCts;
    private readonly KeyboardInputSender inputSender = new();
    
    // Code mode flag (limits maximum speed)
    private bool hasCodeMode = false;
    private int lastNonCodeSpeed = 10; // persisted
    private ToolTip? speedToolTip;
    
    // File source mode
    private bool fileSourceMode = false;
    private string fileSourcePath = string.Empty;
    
    // Flag to prevent auto-save during snippet loading
    private bool isLoadingSnippet = false;
    
    // Reliability heuristics for high-speed typing
    private const int MinFastDelayMs = 35; // Minimum delay enforced when speed >= 8
    private const int ContextualPauseMs = 140; // Extra pause after space following certain boundary chars
    private static readonly char[] ContextBoundaryChars = new[] { '>', ')', '}', ']' };
    
    // System tray components
    private NotifyIcon? trayIcon;
    private ContextMenuStrip? trayMenu;
    private Icon? appIcon; // custom generated icon
    private IntPtr appIconHandle = IntPtr.Zero; // native handle for cleanup
    
    // Windows API imports
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
    
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    public Form1()
    {
        // Initialize Settings Manager and load configuration
        settingsManager = new SettingsManager();
        appConfig = settingsManager.LoadSettings();
        
        InitializeComponent();
        InitializeSystemTray();
        LoadSettings(); // Load old format for backward compatibility
        // NOTE: Global hotkey registration is now handled in OnHandleCreated so that
        // it is automatically re-registered if the window handle is recreated (e.g.
        // when toggling ShowInTaskbar while minimizing to tray). Previously the hotkey
        // stopped working after minimizing because ShowInTaskbar can force a handle
        // recreation which invalidated the original RegisterHotKey binding.
        
        // Update UI after form is fully loaded
        this.Load += Form1_Load;
    }
    
    private void Form1_Load(object? sender, EventArgs e)
    {
        UpdateUIFromSettings();
        LoadSetsIntoTabs();
    }
    
    private void LoadSetsIntoTabs()
    {
        tabControlSets.TabPages.Clear();
        
        foreach (var set in appConfig.Sets)
        {
            var tabPage = CreateSetTabPage(set);
            tabControlSets.TabPages.Add(tabPage);
        }
        
        // Select active set
        if (!string.IsNullOrEmpty(appConfig.ActiveSetId))
        {
            var activeSet = appConfig.GetActiveSet();
            if (activeSet != null)
            {
                for (int i = 0; i < tabControlSets.TabPages.Count; i++)
                {
                    if (tabControlSets.TabPages[i].Tag == activeSet)
                    {
                        tabControlSets.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        else if (tabControlSets.TabPages.Count > 0)
        {
            tabControlSets.SelectedIndex = 0;
        }
        
        UpdateSetButtons();
    }
    
    private void UpdateSetButtons()
    {
        bool hasSelection = tabControlSets.SelectedTab != null;
        btnRenameSet.Enabled = hasSelection;
        btnDeleteSet.Enabled = hasSelection && appConfig.Sets.Count > 1;
    }
    
    private void UpdateUIFromSettings()
    {
        // Apply UI scale settings
        ApplyUIScale(appConfig.UISettings);
        
        // Update text box with loaded predefined text
        if (txtPredefinedText != null)
        {
            txtPredefinedText.Text = predefinedText;
        }
        
        // Update typing speed slider with loaded value
        if (sliderTypingSpeed != null)
        {
            if (sliderTypingSpeed is LimitedTrackBar lt)
            {
                lt.SoftMax = hasCodeMode ? 8 : null;
            }
            if (hasCodeMode && typingSpeed > 8)
            {
                typingSpeed = 8; // clamp from settings if needed
            }
            sliderTypingSpeed.Value = Math.Min(Math.Max(typingSpeed, sliderTypingSpeed.Minimum), sliderTypingSpeed.Maximum);
        }
        
        // Update speed indicator label
        if (lblSpeedIndicator != null)
        {
            lblSpeedIndicator.Text = GetSpeedText(typingSpeed) + (hasCodeMode ? " (code mode)" : string.Empty);
        }
        if (chkHasCode != null)
        {
            chkHasCode.Checked = hasCodeMode;
        }
        if (chkUseFile != null)
        {
            chkUseFile.Checked = fileSourceMode;
        }
        if (txtFilePath != null)
        {
            txtFilePath.Text = fileSourcePath;
            txtFilePath.Enabled = fileSourceMode;
        }
        if (btnBrowseFile != null)
        {
            btnBrowseFile.Enabled = fileSourceMode;
        }
        if (txtPredefinedText != null)
        {
            txtPredefinedText.Enabled = !fileSourceMode;
        }
        UpdateTooltips();
    }
    
    private void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    if (!string.IsNullOrEmpty(settings.PredefinedText))
                    {
                        predefinedText = settings.PredefinedText;
                    }
                    if (settings.TypingSpeed > 0 && settings.TypingSpeed <= 10)
                    {
                        typingSpeed = settings.TypingSpeed;
                    }
                    hasCodeMode = settings.HasCode;
                    if (settings.LastNonCodeSpeed >= 1 && settings.LastNonCodeSpeed <= 10)
                    {
                        lastNonCodeSpeed = settings.LastNonCodeSpeed;
                    }
                    fileSourceMode = settings.UseFileSource;
                    if (!string.IsNullOrWhiteSpace(settings.FileSourcePath))
                    {
                        fileSourcePath = settings.FileSourcePath;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading settings: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    
    private void SaveSettings()
    {
        try
        {
            var settings = new AppSettings { PredefinedText = predefinedText, TypingSpeed = typingSpeed, HasCode = hasCodeMode, LastNonCodeSpeed = lastNonCodeSpeed, UseFileSource = fileSourceMode, FileSourcePath = fileSourcePath };
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void InitializeSystemTray()
    {
        // Create system tray icon
        if (appIcon == null)
        {
            // Generate & cache custom icon (shared between form & tray)
            appIcon = IconFactory.Create(out appIconHandle);
            this.Icon = appIcon; // taskbar / alt-tab icon
            // Export icon for user customization if not already present
            try
            {
                string exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HotkeyTyper.ico");
                IconFactory.TryExportIcon(appIcon, exportPath);
            }
            catch { /* ignore export errors */ }
        }
        trayIcon = new NotifyIcon()
        {
            Text = "Hotkey Typer - CTRL+SHIFT+1 Active",
            Visible = true,
            Icon = appIcon ?? SystemIcons.Application
        };
        
        // Create context menu for tray icon
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Show", null, ShowForm_Click);
        trayMenu.Items.Add("Exit", null, Exit_Click);
        
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.DoubleClick += ShowForm_Click;
    }
    
    private void ShowForm_Click(object? sender, EventArgs e)
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.BringToFront();
    }
    
    private void Exit_Click(object? sender, EventArgs e)
    {
        Application.Exit();
    }
    
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            int hotkeyId = m.WParam.ToInt32();
            
            // Check if it's one of our hotkeys (1000-1008 for CTRL+SHIFT+1 through CTRL+SHIFT+9)
            if (hotkeyId >= HOTKEY_ID_BASE && hotkeyId < HOTKEY_ID_BASE + 9)
            {
                int hotkeyNumber = hotkeyId - HOTKEY_ID_BASE + 1; // Convert to 1-9
                StartTypingForHotkey(hotkeyNumber);
            }
        }
        
        base.WndProc(ref m);
    }
    
    private void StartTypingForHotkey(int hotkeyNumber)
    {
        // Ignore if already typing
        if (typingCts != null)
        {
            return;
        }
        
        // Get the active set
        var activeSet = appConfig.GetActiveSet();
        if (activeSet == null)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: No active snippet set";
                lblStatus.ForeColor = Color.Red;
            }
            return;
        }
        
        // Find the snippet with this hotkey number
        var snippet = activeSet.GetSnippetByHotkey(hotkeyNumber);
        if (snippet == null)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = $"Status: No snippet assigned to CTRL+SHIFT+{hotkeyNumber}";
                lblStatus.ForeColor = Color.Orange;
            }
            return;
        }
        
        // Start typing with this snippet
        StartTypingWithSnippet(snippet);
    }
    
    private void StartTypingWithSnippet(Snippet snippet)
    {
        // Ignore if already typing
        if (typingCts != null)
        {
            return;
        }

        typingCts = new CancellationTokenSource();
        var token = typingCts.Token;

        // Determine content to type from snippet
        string? contentToType;
        
        if (snippet.UseFile && !string.IsNullOrWhiteSpace(snippet.FilePath))
        {
            // Load content from file
            bool truncated;
            contentToType = LoadFileContent(snippet.FilePath, out truncated);
            if (contentToType == null)
            {
                // Abort typing if file not available
                typingCts.Dispose();
                typingCts = null;
                if (lblStatus != null)
                {
                    lblStatus.Text = $"Status: File not found: {snippet.FilePath}";
                    lblStatus.ForeColor = Color.Red;
                }
                return;
            }
            if (truncated && lblStatus != null)
            {
                lblStatus.Text = "Status: File truncated for typing";
                lblStatus.ForeColor = Color.DarkOrange;
            }
        }
        else
        {
            // Use snippet content
            contentToType = snippet.Content;
        }

        // Update UI state
        if (lblStatus != null)
        {
            lblStatus.Text = $"Status: Typing '{snippet.Name}' at speed {snippet.TypingSpeed}...";
            lblStatus.ForeColor = Color.DarkOrange;
        }
        if (btnStop != null)
        {
            btnStop.Enabled = true;
        }

        // Fire and forget task using snippet's speed and hasCode settings
        _ = TypePredefinedTextAsync(token, contentToType, snippet.TypingSpeed, snippet.HasCode);
    }
    
    private string? LoadFileContent(string filePath, out bool truncated)
    {
        truncated = false;
        if (string.IsNullOrWhiteSpace(filePath)) return null;
        try
        {
            if (!File.Exists(filePath)) return null;
            string text = File.ReadAllText(filePath);
            const int maxLen = 50000;
            if (text.Length > maxLen)
            {
                text = text.Substring(0, maxLen);
                truncated = true;
            }
            return text;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }
    
    private async Task TypePredefinedTextAsync(CancellationToken token, string content, int speed, bool hasCode)
    {
        try
        {
            // Flush any pending keys from the hotkey press
            SendKeys.Flush();
            
            // Capture current foreground window so we can restore focus after initial delay
            IntPtr targetWindow = GetForegroundWindow();

            // Wait for hotkey to be fully released and target window to be ready
            await Task.Delay(250, token);

            // Attempt to restore focus to previously active window (ignore failures)
            if (targetWindow != IntPtr.Zero)
            {
                _ = SetForegroundWindow(targetWindow);
            }
            
            // Calculate delay based on typing speed (1=slowest, 10=fastest)
            // Use the speed parameter, not the instance variable
            int baseDelay = 310 - (speed * 30); // Base delay calculation
            int variation = Math.Max(10, baseDelay / 3); // Variation amount
            Random random = new Random();
            
            char prev = '\0';
            for (int index = 0; index < content.Length; index++)
            {
                if (token.IsCancellationRequested) break;
                char c = content[index];

                // Contextual pause heuristic: if previous char was a boundary and current is a space, pause before continuing
                if (prev != '\0' && c == ' ' && ContextBoundaryChars.Contains(prev) && speed >= 7)
                {
                    try { await Task.Delay(ContextualPauseMs, token); } catch (OperationCanceledException) { break; }
                }

                // Use SendInput for improved reliability at high speeds.
                // Fallback to SendKeys only if character requires special translation (currently none, Unicode covers most cases).
                bool ok = inputSender.SendChar(c);
                if (!ok)
                {
                    // Fallback to SendKeys for this character
                    string fallback = EscapeForSendKeys(c);
                    if (fallback.Length > 0)
                    {
                        SendKeys.SendWait(fallback);
                    }
                }

                int delay = Math.Max(20, random.Next(Math.Max(10, baseDelay - variation), baseDelay + variation));
                if (speed >= 8 && delay < MinFastDelayMs)
                {
                    delay = MinFastDelayMs; // enforce safety margin
                }
                try
                {
                    await Task.Delay(delay, token);
                }
                catch (OperationCanceledException)
                {
                    break; // exit loop promptly on cancellation
                }

                prev = c;
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when user clicks Stop
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error typing text: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            // Reset UI state
            typingCts?.Dispose();
            typingCts = null;
            if (btnStop != null)
            {
                btnStop.Enabled = false;
            }
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Hotkey CTRL+SHIFT+1 is active";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }

    /// <summary>
    /// Escapes a single character for SendKeys so that source code or other literal text
    /// (including parentheses and plus/caret/percent signs) can be typed without triggering
    /// SendKeys grouping or modifier semantics.
    /// </summary>
    internal static string EscapeForSendKeys(char c)
    {
        return c switch
        {
            // Modifier / special grouping characters that must be wrapped in braces to be literal
            '+' => "{+}",
            '^' => "{^}",
            '%' => "{%}",
            '~' => "{~}",
            '(' => "{(}",
            ')' => "{)}",
            // Braces require doubling pattern
            '{' => "{{}",
            '}' => "{}}",
            // Translate newlines / tabs to their SendKeys representations
            '\n' => "{ENTER}",
            '\r' => string.Empty, // ignore CR in CRLF to avoid double-enter
            '\t' => "{TAB}",
            _ => c.ToString()
        };
    }
    
    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (fileSourceMode)
        {
            // In file mode, saving text box content is irrelevant; show passive status.
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: File mode active (text box not saved)";
                lblStatus.ForeColor = Color.DarkOrange;
            }
            return;
        }

        if (txtPredefinedText != null)
        {
            predefinedText = txtPredefinedText.Text;
            SaveSettings();
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Text saved";
                lblStatus.ForeColor = Color.Green;
            }
        }
    }
    
    private void TypingSpeedSlider_ValueChanged(object? sender, EventArgs e)
    {
        if (sender is TrackBar slider)
        {
            if (hasCodeMode && slider.Value > 8)
            {
                slider.Value = 8; // clamp
            }
            typingSpeed = slider.Value;
            if (lblSpeedIndicator != null)
            {
                lblSpeedIndicator.Text = GetSpeedText(typingSpeed) + (hasCodeMode ? " (code mode)" : string.Empty);
            }
            SaveSettings();
        }
    }

    private void ChkHasCode_CheckedChanged(object? sender, EventArgs e)
    {
        if (chkHasCode == null || currentSnippet == null) return;
        
        // Update speed indicator label
        if (lblSpeedIndicator != null && sliderTypingSpeed != null)
        {
            lblSpeedIndicator.Text = GetSpeedText(sliderTypingSpeed.Value) + (chkHasCode.Checked ? " (code mode)" : string.Empty);
        }
        
        // Auto-save if enabled
        AutoSaveCurrentSnippet();
    }

    private void ChkUseFile_CheckedChanged(object? sender, EventArgs e)
    {
        if (chkUseFile == null || currentSnippet == null) return;
        
        // Enable/disable file path controls
        if (txtFilePath != null) txtFilePath.Enabled = chkUseFile.Checked;
        if (btnBrowseFile != null) btnBrowseFile.Enabled = chkUseFile.Checked;
        if (txtPredefinedText != null) txtPredefinedText.Enabled = !chkUseFile.Checked;
        
        // Auto-save if enabled
        AutoSaveCurrentSnippet();
    }

    private void BtnBrowseFile_Click(object? sender, EventArgs e)
    {
        if (currentSnippet == null) return;
        
        using var ofd = new OpenFileDialog
        {
            Title = "Select text or markdown file",
            Filter = "Text/Markdown (*.txt;*.md)|*.txt;*.md|All Files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            // Auto-save file path to current snippet
            currentSnippet.FilePath = ofd.FileName;
            currentSnippet.UpdateModifiedDate();
            settingsManager.SaveSettings(appConfig);
            
            if (txtFilePath != null) txtFilePath.Text = ofd.FileName;
        }
    }

    private string? LoadFileContentForTyping(out bool truncated)
    {
        truncated = false;
        if (string.IsNullOrWhiteSpace(fileSourcePath)) return null;
        try
        {
            if (!File.Exists(fileSourcePath)) return null;
            string text = File.ReadAllText(fileSourcePath);
            const int maxLen = 50000;
            if (text.Length > maxLen)
            {
                text = text.Substring(0, maxLen);
                truncated = true;
            }
            return text;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    private void NotifyClamped()
    {
        if (lblStatus != null)
        {
            lblStatus.Text = "Status: Speed limited in code mode";
            lblStatus.ForeColor = Color.DarkOrange;
        }
    }

    private void UpdateTooltips()
    {
        if (speedToolTip == null && sliderTypingSpeed != null)
        {
            speedToolTip = new ToolTip();
        }
        if (speedToolTip != null && sliderTypingSpeed != null && lblSpeedIndicator != null)
        {
            string msg = hasCodeMode
                ? "Code mode: max speed limited to Fast (8) to improve reliability while typing code."
                : "Normal mode: speeds 1 (Very Slow) to 10 (Very Fast).";
            speedToolTip.SetToolTip(sliderTypingSpeed, msg);
            speedToolTip.SetToolTip(lblSpeedIndicator, msg);
        }
    }
    
    private string GetSpeedText(int speed)
    {
        return speed switch
        {
            1 or 2 => "Very Slow",
            3 or 4 => "Slow", 
            5 or 6 => "Normal",
            7 or 8 => "Fast",
            9 or 10 => "Very Fast",
            _ => "Normal"
        };
    }
    
    private void BtnMinimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }
    
    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (typingCts != null && !typingCts.IsCancellationRequested)
        {
            typingCts.Cancel();
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Typing cancelled";
                lblStatus.ForeColor = Color.Red;
            }
            if (btnStop != null)
            {
                btnStop.Enabled = false;
            }
        }
    }
    
    private void SliderTypingSpeed_ValueChanged(object? sender, EventArgs e)
    {
        if (sliderTypingSpeed == null || lblSpeedIndicator == null) return;
        
        int speed = sliderTypingSpeed.Value;
        lblSpeedIndicator.Text = GetSpeedText(speed);
        
        // Auto-save speed to current snippet (so hotkeys use the updated speed immediately)
        if (currentSnippet != null)
        {
            currentSnippet.TypingSpeed = speed;
            currentSnippet.UpdateModifiedDate();
            settingsManager.SaveSettings(appConfig);
        }
    }
    
    private void AutoSaveCurrentSnippet()
    {
        // Don't auto-save if disabled, no snippet selected, or currently loading a snippet
        if (!appConfig.UISettings.AutoSave || currentSnippet == null || isLoadingSnippet) return;
        
        // Validate snippet name
        if (string.IsNullOrWhiteSpace(txtSnippetName.Text)) return;
        
        // Update snippet from editor
        currentSnippet.Name = txtSnippetName.Text.Trim();
        currentSnippet.Content = txtPredefinedText.Text;
        currentSnippet.TypingSpeed = sliderTypingSpeed.Value;
        currentSnippet.HasCode = chkHasCode.Checked;
        currentSnippet.UseFile = chkUseFile.Checked;
        currentSnippet.FilePath = chkUseFile.Checked ? txtFilePath.Text : null;
        
        settingsManager.SaveSettings(appConfig);
        
        // Update the ListView to reflect the changes
        UpdateListViewItem(currentSnippet);
        
        lblStatus.Text = $"Status: Auto-saved '{currentSnippet.Name}'";
        lblStatus.ForeColor = Color.Green;
    }
    
    private void UpdateListViewItem(Snippet snippet)
    {
        // Find the ListView in the active tab
        if (tabControlSets.SelectedTab == null) return;
        
        ListView? listView = null;
        foreach (Control control in tabControlSets.SelectedTab.Controls)
        {
            if (control is TableLayoutPanel tableLayout)
            {
                foreach (Control child in tableLayout.Controls)
                {
                    if (child is ListView lv)
                    {
                        listView = lv;
                        break;
                    }
                }
            }
        }
        
        if (listView == null) return;
        
        // Find the ListViewItem for this snippet
        foreach (ListViewItem item in listView.Items)
        {
            if (item.Tag == snippet)
            {
                // Update the item's display
                item.SubItems[0].Text = snippet.GetHotkeyDisplay();
                item.SubItems[1].Text = snippet.Name;
                item.SubItems[2].Text = GetSpeedText(snippet.TypingSpeed);
                item.SubItems[3].Text = snippet.GetContentPreview();
                break;
            }
        }
    }
    
    private void TxtSnippetName_TextChanged(object? sender, EventArgs e)
    {
        AutoSaveCurrentSnippet();
    }
    
    private void TxtPredefinedText_TextChanged(object? sender, EventArgs e)
    {
        AutoSaveCurrentSnippet();
    }

    private void TxtFilePath_TextChanged(object? sender, EventArgs e)
    {
        AutoSaveCurrentSnippet();
    }
    
    // ===== SET MANAGEMENT EVENT HANDLERS =====
    
    // ===== TAB CONTROL EVENT HANDLERS =====
    
    private void TabControlSets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (tabControlSets.SelectedTab?.Tag is SnippetSet selectedSet)
        {
            currentSet = selectedSet;
            appConfig.SetActiveSet(selectedSet.Id);
            settingsManager.SaveSettings(appConfig);
            UpdateSetButtons();
        }
    }
    
    private void BtnNewSet_Click(object? sender, EventArgs e)
    {
        string? setName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter a name for the new snippet set:",
            "New Snippet Set",
            "New Set");
            
        if (!string.IsNullOrWhiteSpace(setName))
        {
            var newSet = new SnippetSet
            {
                Id = Guid.NewGuid().ToString(),
                Name = setName,
                Description = "",
                Snippets = new List<Snippet>()
            };
            
            appConfig.Sets.Add(newSet);
            settingsManager.SaveSettings(appConfig);
            
            var tabPage = CreateSetTabPage(newSet);
            tabControlSets.TabPages.Add(tabPage);
            tabControlSets.SelectedTab = tabPage;
        }
    }
    
    private void BtnRenameSet_Click(object? sender, EventArgs e)
    {
        if (tabControlSets.SelectedTab?.Tag is not SnippetSet selectedSet) return;
        
        string? newName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter a new name for this snippet set:",
            "Rename Snippet Set",
            selectedSet.Name);
            
        if (!string.IsNullOrWhiteSpace(newName) && newName != selectedSet.Name)
        {
            selectedSet.Name = newName;
            tabControlSets.SelectedTab.Text = newName;
            settingsManager.SaveSettings(appConfig);
        }
    }
    
    private void BtnDeleteSet_Click(object? sender, EventArgs e)
    {
        if (tabControlSets.SelectedTab?.Tag is not SnippetSet selectedSet) return;
        
        if (appConfig.Sets.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last remaining set.", "Delete Set", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete the set '{selectedSet.Name}' and all its snippets?",
            "Delete Snippet Set",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
            
        if (result == DialogResult.Yes)
        {
            appConfig.Sets.Remove(selectedSet);
            settingsManager.SaveSettings(appConfig);
            tabControlSets.TabPages.Remove(tabControlSets.SelectedTab);
        }
    }

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        var settingsDialog = new SettingsDialog(appConfig.UISettings);
        if (settingsDialog.ShowDialog(this) == DialogResult.OK && settingsDialog.Applied)
        {
            // Copy the modified settings back to appConfig
            appConfig.UISettings.Scale = settingsDialog.UISettings.Scale;
            appConfig.UISettings.BaseFontSize = settingsDialog.UISettings.BaseFontSize;
            appConfig.UISettings.ContentFontSize = settingsDialog.UISettings.ContentFontSize;
            appConfig.UISettings.AutoSave = settingsDialog.UISettings.AutoSave;
            
            // Save settings
            settingsManager.SaveSettings(appConfig);
            
            // Refresh editor controls to reflect auto-save setting immediately
            EnableEditorControls(currentSnippet != null);
            
            // Apply UI scale changes
            ApplyUIScale(appConfig.UISettings);
            
            // Show feedback about auto-save status
            if (appConfig.UISettings.AutoSave)
            {
                lblStatus.Text = "Status: Auto-save enabled - changes will be saved automatically";
                lblStatus.ForeColor = Color.Green;
            }
            else
            {
                lblStatus.Text = "Status: Auto-save disabled - use Save button to save changes";
                lblStatus.ForeColor = Color.Blue;
            }
            
            // Show message about restart for complete UI refresh (only if needed for fonts/scale)
            MessageBox.Show(
                "Settings have been saved and applied.\n\nNote: Some UI changes may require an application restart to fully take effect.",
                "Settings Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
    
    // ===== SNIPPET EVENT HANDLERS =====
    
    private void ListViewSnippets_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is not ListView listView) return;
        if (listView.SelectedItems.Count == 0)
        {
            currentSnippet = null;
            EnableEditorControls(false);
            return;
        }
        
        if (listView.SelectedItems[0].Tag is Snippet selectedSnippet)
        {
            currentSnippet = selectedSnippet;
            currentSet = listView.Tag as SnippetSet;
            LoadSnippetIntoEditor();
            EnableEditorControls(true);
        }
    }
    
    private void LoadSnippetIntoEditor()
    {
        if (currentSnippet == null) return;
        
        // Set flag to prevent auto-save during load
        isLoadingSnippet = true;
        
        txtSnippetName.Text = currentSnippet.Name;
        txtPredefinedText.Text = currentSnippet.Content;
        sliderTypingSpeed.Value = currentSnippet.TypingSpeed;
        chkHasCode.Checked = currentSnippet.HasCode;
        chkUseFile.Checked = currentSnippet.UseFile;
        txtFilePath.Text = currentSnippet.FilePath ?? "";
        lblHotkeyDisplay.Text = $"Hotkey: {currentSnippet.GetHotkeyDisplay()}";
        
        // Set status message based on auto-save mode
        if (appConfig.UISettings.AutoSave)
        {
            lblStatus.Text = $"Status: Editing '{currentSnippet.Name}' (auto-save enabled)";
            lblStatus.ForeColor = Color.Green;
        }
        else
        {
            lblStatus.Text = $"Status: Editing snippet '{currentSnippet.Name}'";
            lblStatus.ForeColor = Color.Blue;
        }
        
        // Clear flag after load is complete
        isLoadingSnippet = false;
    }
    
    private void EnableEditorControls(bool enabled)
    {
        txtSnippetName.Enabled = enabled;
        txtPredefinedText.Enabled = enabled;
        sliderTypingSpeed.Enabled = enabled;
        chkHasCode.Enabled = enabled;
        chkUseFile.Enabled = enabled;
        txtFilePath.Enabled = enabled && chkUseFile.Checked;
        btnBrowseFile.Enabled = enabled && chkUseFile.Checked;
        
        // Handle auto-save mode
        bool autoSaveEnabled = appConfig.UISettings.AutoSave;
        if (autoSaveEnabled && enabled)
        {
            btnSaveSnippet.Enabled = false;
            btnSaveSnippet.Text = "✓ Auto Save";
        }
        else
        {
            btnSaveSnippet.Enabled = enabled;
            btnSaveSnippet.Text = "💾 Save";
        }
        
        btnCancelSnippet.Enabled = enabled && !autoSaveEnabled; // No cancel needed in auto-save mode
        btnDeleteSnippet.Enabled = enabled;
    }
    
    private void BtnNewSnippet_Click(object? sender, EventArgs e)
    {
        // Get the currently selected tab's set
        if (tabControlSets.SelectedTab?.Tag is not SnippetSet targetSet) return;
        
        var availableHotkeys = targetSet.GetAvailableHotkeys();
        if (availableHotkeys.Count == 0)
        {
            MessageBox.Show("This set already has 9 snippets (maximum allowed).", "New Snippet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        string? snippetName = Microsoft.VisualBasic.Interaction.InputBox(
            "Enter a name for the new snippet:",
            "New Snippet",
            "New Snippet");
            
        if (!string.IsNullOrWhiteSpace(snippetName))
        {
            var newSnippet = new Snippet
            {
                Id = Guid.NewGuid().ToString(),
                Name = snippetName,
                Content = "",
                HotkeyNumber = availableHotkeys[0],
                TypingSpeed = 5
            };
            
            targetSet.AddSnippet(newSnippet);
            settingsManager.SaveSettings(appConfig);
            
            // Refresh the tab to show new snippet
            RefreshActiveTab();
        }
    }
    
    private void BtnDeleteSnippet_Click(object? sender, EventArgs e)
    {
        if (currentSnippet == null || currentSet == null) return;
        
        var result = MessageBox.Show(
            $"Are you sure you want to delete the snippet '{currentSnippet.Name}'?",
            "Delete Snippet",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
            
        if (result == DialogResult.Yes)
        {
            currentSet.RemoveSnippet(currentSnippet.Id);
            settingsManager.SaveSettings(appConfig);
            
            currentSnippet = null;
            EnableEditorControls(false);
            
            // Refresh the tab
            RefreshActiveTab();
        }
    }
    
    private void BtnSaveSnippet_Click(object? sender, EventArgs e)
    {
        if (currentSnippet == null) return;
        
        // Validate snippet name
        if (string.IsNullOrWhiteSpace(txtSnippetName.Text))
        {
            MessageBox.Show("Snippet name cannot be empty.", "Save Snippet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        // Update snippet from editor
        currentSnippet.Name = txtSnippetName.Text.Trim();
        currentSnippet.Content = txtPredefinedText.Text;
        currentSnippet.TypingSpeed = sliderTypingSpeed.Value;
        currentSnippet.HasCode = chkHasCode.Checked;
        currentSnippet.UseFile = chkUseFile.Checked;
        currentSnippet.FilePath = chkUseFile.Checked ? txtFilePath.Text : null;
        
        settingsManager.SaveSettings(appConfig);
        
        // Refresh the tab to show updated snippet
        RefreshActiveTab();
        
        lblStatus.Text = $"Status: Snippet '{currentSnippet.Name}' saved successfully";
        lblStatus.ForeColor = Color.Green;
    }
    
    private void BtnCancelSnippet_Click(object? sender, EventArgs e)
    {
        // Reload the current snippet from saved settings to discard changes
        if (currentSnippet != null)
        {
            LoadSnippetIntoEditor();
            lblStatus.Text = "Status: Changes discarded";
            lblStatus.ForeColor = Color.Orange;
        }
    }
    
    private void RefreshActiveTab()
    {
        if (tabControlSets.SelectedTab?.Tag is not SnippetSet selectedSet) return;
        
        int selectedIndex = tabControlSets.SelectedIndex;
        string? selectedSnippetId = currentSnippet?.Id;
        
        // Recreate tab
        tabControlSets.TabPages.RemoveAt(selectedIndex);
        var newTab = CreateSetTabPage(selectedSet);
        tabControlSets.TabPages.Insert(selectedIndex, newTab);
        tabControlSets.SelectedIndex = selectedIndex;
        
        // Reselect snippet if we had one
        if (selectedSnippetId != null)
        {
            var listView = newTab.Controls[0].Controls.OfType<ListView>().FirstOrDefault();
            if (listView != null)
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    if (listView.Items[i].Tag is Snippet snippet && snippet.Id == selectedSnippetId)
                    {
                        listView.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        // Unregister all hotkeys when form is disposed
        for (int i = 0; i < 9; i++)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_BASE + i);
        }
        
        // Clean up system tray
        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }
        trayMenu?.Dispose();
        if (appIcon != null)
        {
            IconFactory.Destroy(ref appIcon, ref appIconHandle);
        }
        
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Ensures the global hotkey is (re)registered whenever the form's native handle is created.
    /// This fixes a bug where minimizing to tray (setting ShowInTaskbar = false) can recreate the
    /// window handle and silently detach the previously registered hotkey.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TryRegisterGlobalHotkey();
    }

    /// <summary>
    /// Attempt to register CTRL+SHIFT+1 through CTRL+SHIFT+9 as global hotkeys.
    /// If hotkeys are already registered (stale from a previous handle) we first try to unregister.
    /// Any failure is surfaced in the status label and via a tray balloon tip for visibility.
    /// </summary>
    private void TryRegisterGlobalHotkey()
    {
        // Unregister any existing registrations from previous handle
        for (int i = 0; i < 9; i++)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_BASE + i);
        }

        // Register all 9 hotkeys (CTRL+SHIFT+1 through CTRL+SHIFT+9)
        int successCount = 0;
        List<int> failedHotkeys = new List<int>();
        
        for (int i = 0; i < 9; i++)
        {
            int hotkeyNumber = i + 1; // 1-9
            int hotkeyId = HOTKEY_ID_BASE + i;
            Keys key = (Keys)((int)Keys.D1 + i); // D1 through D9
            
            if (RegisterHotKey(this.Handle, hotkeyId, MOD_CONTROL | MOD_SHIFT, (int)key))
            {
                successCount++;
            }
            else
            {
                failedHotkeys.Add(hotkeyNumber);
            }
        }

        // Update status based on results
        if (successCount == 9)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: All hotkeys (CTRL+SHIFT+1-9) registered";
                lblStatus.ForeColor = Color.Green;
            }
        }
        else if (successCount > 0)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = $"Status: {successCount}/9 hotkeys registered. Failed: {string.Join(", ", failedHotkeys)}";
                lblStatus.ForeColor = Color.Orange;
            }
            trayIcon?.ShowBalloonTip(3000, "Hotkey Typer", 
                $"Some hotkeys failed to register: CTRL+SHIFT+{string.Join(", ", failedHotkeys)}. They may be in use by another application.", 
                ToolTipIcon.Warning);
        }
        else
        {
            if (lblStatus != null)
            {
                lblStatus.Text = "Status: Failed to register any hotkeys";
                lblStatus.ForeColor = Color.Red;
            }
            trayIcon?.ShowBalloonTip(3000, "Hotkey Typer", "Failed to register global hotkeys. They may be in use by another application.", ToolTipIcon.Error);
        }
    }

    // ===== UI SCALE METHODS =====
    
    private void ApplyUIScale(UISettings uiSettings)
    {
        float scaleFactor = uiSettings.Scale switch
        {
            UIScale.Small => 0.90f,
            UIScale.Large => 1.10f,
            _ => 1.0f
        };

        // Apply to all controls recursively
        ApplyFontScaleToControl(this, uiSettings.BaseFontSize, scaleFactor);
        
        // Apply content font size specifically to content textbox
        if (txtPredefinedText != null)
        {
            txtPredefinedText.Font = new Font("Consolas", uiSettings.ContentFontSize * scaleFactor);
        }
    }

    private void ApplyFontScaleToControl(Control control, int baseFontSize, float scaleFactor)
    {
        // Skip content textbox - it has its own setting
        if (control == txtPredefinedText) return;

        // Apply font size to this control
        if (control.Font != null)
        {
            var currentFont = control.Font;
            var newSize = baseFontSize * scaleFactor;
            
            // Preserve font style
            control.Font = new Font(currentFont.FontFamily, newSize, currentFont.Style);
        }

        // Recursively apply to children
        foreach (Control child in control.Controls)
        {
            ApplyFontScaleToControl(child, baseFontSize, scaleFactor);
        }
    }
}

// Settings class for JSON serialization
public class AppSettings
{
    public string PredefinedText { get; set; } = string.Empty;
    public int TypingSpeed { get; set; } = 5;
    public bool HasCode { get; set; } = false;
    public int LastNonCodeSpeed { get; set; } = 10;
    public bool UseFileSource { get; set; } = false;
    public string FileSourcePath { get; set; } = string.Empty;
}
