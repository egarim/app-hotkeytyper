namespace HotkeyTyper;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    // Dispose method is implemented in Form1.cs

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        
        // Form properties
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1400, 850); // Increased from 1100x700 for high DPI
        MinimumSize = new Size(1100, 750); // Increased from 950x650
        Text = "Hotkey Typer";
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;
        Padding = new Padding(5); // Reduced from 10 to save space
        BackColor = Color.White;
        
        // Create controls
        CreateControls();
    }
    
    private void CreateControls()
    {
        // Main layout
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0)
        };
        
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Tab header with buttons
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Tab content
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status bar
        
        // Row 0: Tab header panel with buttons
        var tabHeaderPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(0),
            Height = 45
        };
        tabHeaderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // TabControl
        tabHeaderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F)); // Spacer
        tabHeaderPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Buttons
        
        // Tab control
        tabControlSets = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F),
            Padding = new Point(8, 4),
            Margin = new Padding(0, 0, 0, 5) // Add bottom margin
        };
        tabControlSets.SelectedIndexChanged += TabControlSets_SelectedIndexChanged;
        
        // Spacer panel (invisible)
        var spacerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Width = 10
        };
        
        // Buttons panel
        var tabButtonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 5, 8), // Vertical padding to center buttons
            Margin = new Padding(0)
        };
        
        btnNewSet = new Button
        {
            Text = "+ New Set",
            AutoSize = true,
            MinimumSize = new Size(0, 30), // Set minimum height
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 4, 0),
            Padding = new Padding(10, 4, 10, 4) // Increased padding
        };
        btnNewSet.Click += BtnNewSet_Click;
        
        btnRenameSet = new Button
        {
            Text = "Rename",
            AutoSize = true,
            MinimumSize = new Size(0, 30),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 4, 0),
            Padding = new Padding(10, 4, 10, 4),
            Enabled = false
        };
        btnRenameSet.Click += BtnRenameSet_Click;
        
        btnDeleteSet = new Button
        {
            Text = "Delete",
            AutoSize = true,
            MinimumSize = new Size(0, 30),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 12, 0),
            Padding = new Padding(10, 4, 10, 4),
            Enabled = false
        };
        btnDeleteSet.Click += BtnDeleteSet_Click;

        btnSettings = new Button
        {
            Text = "⚙ Settings",
            AutoSize = true,
            MinimumSize = new Size(0, 30),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 4, 0),
            Padding = new Padding(10, 4, 10, 4)
        };
        btnSettings.Click += BtnSettings_Click;
        
        tabButtonsPanel.Controls.AddRange(new Control[] { btnNewSet, btnRenameSet, btnDeleteSet, btnSettings });
        
        tabHeaderPanel.Controls.Add(tabControlSets, 0, 0);
        tabHeaderPanel.Controls.Add(spacerPanel, 1, 0); // Add spacer
        tabHeaderPanel.Controls.Add(tabButtonsPanel, 2, 0); // Buttons in column 2
        
        // Row 2: Status bar
        lblStatus = new Label
        {
            Name = "lblStatus",
            Text = "Status: Ready",
            AutoSize = false,
            Height = 25,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.Green,
            Padding = new Padding(5, 0, 0, 0)
        };
        
        var statusPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 35,
            Padding = new Padding(0, 10, 0, 0)
        };
        
        btnMinimize = new Button
        {
            Name = "btnMinimize",
            Text = "📌 Minimize to Tray",
            Dock = DockStyle.Right,
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 5, 15, 5),
            FlatStyle = FlatStyle.System
        };
        btnMinimize.Click += BtnMinimize_Click;
        
        statusPanel.Controls.Add(btnMinimize);
        statusPanel.Controls.Add(lblStatus);
        
        mainLayout.Controls.Add(tabHeaderPanel, 0, 0);
        mainLayout.Controls.Add(tabControlSets, 0, 1);
        mainLayout.Controls.Add(statusPanel, 0, 2);
        
        Controls.Add(mainLayout);
    }
    
    public TabPage CreateSetTabPage(HotkeyTyper.Models.SnippetSet set)
    {
        var tabPage = new TabPage
        {
            Text = set.Name,
            Tag = set,
            Padding = new Padding(8), // Reduced from 12
            BackColor = Color.White
        };
        
        // Main layout for tab content
        var tabLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0)
        };
        
        tabLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
        tabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F)); // Snippets list - reduced from 150F
        tabLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Editor (takes remaining space)
        
        // Row 0: Snippets header (no buttons - moved to editor toolbar)
        var snippetsHeaderPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 38,
            Padding = new Padding(0, 0, 0, 8)
        };
        
        var lblSnippetsHeader = new Label
        {
            Text = "Snippets in this set:",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            AutoSize = true,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(51, 51, 51),
            Padding = new Padding(0, 8, 0, 0)
        };
        
        snippetsHeaderPanel.Controls.Add(lblSnippetsHeader);
        
        // Row 1: Snippets ListView
        var listViewSnippets = new ListView
        {
            Name = "listViewSnippets_" + set.Id,
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9.5F),
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            Tag = set
        };
        
        listViewSnippets.Columns.Add("Hotkey", 70);
        listViewSnippets.Columns.Add("Name", 120);
        listViewSnippets.Columns.Add("Speed", 60);
        listViewSnippets.Columns.Add("Preview", 1000); // Large width to fill remaining space
        listViewSnippets.SelectedIndexChanged += ListViewSnippets_SelectedIndexChanged;
        
        // Populate snippets
        foreach (var snippet in set.Snippets)
        {
            var item = new ListViewItem(snippet.GetHotkeyDisplay());
            item.SubItems.Add(snippet.Name);
            item.SubItems.Add(GetSpeedText(snippet.TypingSpeed));
            item.SubItems.Add(snippet.GetContentPreview());
            item.Tag = snippet;
            listViewSnippets.Items.Add(item);
        }
        
        // Row 2: Editor panel
        var editorPanel = CreateEditorPanel();
        
        tabLayout.Controls.Add(snippetsHeaderPanel, 0, 0);
        tabLayout.Controls.Add(listViewSnippets, 0, 1);
        tabLayout.Controls.Add(editorPanel, 0, 2);
        
        tabPage.Controls.Add(tabLayout);
        
        return tabPage;
    }
    
    private Panel CreateEditorPanel()
    {
        var editorPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 6, 0, 0),
            AutoScroll = true
        };
        
        var editorLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,  // Changed from 8 to 9 (added toolbar row)
            Padding = new Padding(10, 8, 10, 8),
            BackColor = Color.FromArgb(250, 250, 250)
        };
        
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header + Toolbar
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Name field
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Content textbox
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Speed
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Checkboxes
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // File path
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Spacer (removed buttons)
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hotkey display
        
        // Row 0: Editor header with toolbar
        var headerToolbarPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
        };
        headerToolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        headerToolbarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        
        var lblEditorHeader = new Label
        {
            Text = "Snippet Editor",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 51, 51),
            AutoSize = true,
            Dock = DockStyle.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };
        
        // CRUD Toolbar
        var toolbarPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0)
        };
        
        btnNewSnippet = new Button
        {
            Text = "➕ New",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 6, 15, 6),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 0, 0)
        };
        btnNewSnippet.Click += BtnNewSnippet_Click;
        
        btnSaveSnippet = new Button
        {
            Text = "💾 Save",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Padding = new Padding(15, 6, 15, 6),
            Margin = new Padding(8, 0, 0, 0),
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        btnSaveSnippet.FlatAppearance.BorderSize = 0;
        btnSaveSnippet.Click += BtnSaveSnippet_Click;
        
        btnCancelSnippet = new Button
        {
            Text = "❌ Cancel",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 6, 15, 6),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(8, 0, 0, 0),
            Enabled = false
        };
        btnCancelSnippet.Click += BtnCancelSnippet_Click;
        
        btnDeleteSnippet = new Button
        {
            Text = "🗑️ Delete",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 6, 15, 6),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(8, 0, 0, 0),
            Enabled = false
        };
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        
        // Add buttons in reverse order for RightToLeft flow
        toolbarPanel.Controls.AddRange(new Control[] { btnNewSnippet, btnDeleteSnippet, btnCancelSnippet, btnSaveSnippet });
        
        headerToolbarPanel.Controls.Add(lblEditorHeader, 0, 0);
        headerToolbarPanel.Controls.Add(toolbarPanel, 1, 0);
        
        // Row 1: Name field
        var namePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 6) // Reduced from 8
        };
        namePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Auto-size for label
        namePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        
        var lblName = new Label
        {
            Text = "Name:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 4, 8, 0), // Reduced from 6, 10
            MinimumSize = new Size(60, 0) // Ensure minimum width but allow expansion
        };
        
        txtSnippetName = new TextBox
        {
            Name = "txtSnippetName",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5F),
            Enabled = false,
            Height = 26
        };
        txtSnippetName.TextChanged += TxtSnippetName_TextChanged;
        
        namePanel.Controls.Add(lblName, 0, 0);
        namePanel.Controls.Add(txtSnippetName, 1, 0);
        
        // Row 2: Content
        var contentPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 6) // Reduced from 8
        };
        contentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Textbox
        
        var contentLabel = new Label
        {
            Text = "Content:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 4) // Reduced from 5
        };
        
        txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10F),
            Enabled = false,
            BorderStyle = BorderStyle.FixedSingle
        };
        txtPredefinedText.TextChanged += TxtPredefinedText_TextChanged;
        
        contentPanel.Controls.Add(contentLabel, 0, 0);
        contentPanel.Controls.Add(txtPredefinedText, 0, 1);
        
        // Row 3: Typing speed
        var speedPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4) // Reduced from 6
        };
        speedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        speedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        speedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        
        var lblSpeed = new Label
        {
            Text = "Typing Speed:",
            Font = new Font("Segoe UI", 9F),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 4, 8, 0) // Reduced from 5, 10
        };
        
        sliderTypingSpeed = new TrackBar
        {
            Minimum = 1,
            Maximum = 10,
            Value = 5,
            TickFrequency = 1,
            Dock = DockStyle.Fill,
            Enabled = false
        };
        sliderTypingSpeed.ValueChanged += SliderTypingSpeed_ValueChanged;
        
        lblSpeedIndicator = new Label
        {
            Text = "Normal",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(8, 4, 0, 0), // Reduced from 10, 5
            MinimumSize = new Size(80, 0)
        };
        
        speedPanel.Controls.Add(lblSpeed, 0, 0);
        speedPanel.Controls.Add(sliderTypingSpeed, 1, 0);
        speedPanel.Controls.Add(lblSpeedIndicator, 2, 0);
        
        // Row 4: Checkboxes
        var checkboxPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 4) // Reduced from 6
        };
        
        chkHasCode = new CheckBox
        {
            Text = "Has Code (limit speed)",
            Font = new Font("Segoe UI", 9F),
            AutoSize = true,
            Enabled = false,
            Margin = new Padding(0, 0, 15, 0) // Reduced from 20
        };
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;
        
        chkUseFile = new CheckBox
        {
            Text = "Use File (.md/.txt)",
            Font = new Font("Segoe UI", 9F),
            AutoSize = true,
            Enabled = false
        };
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;
        
        checkboxPanel.Controls.AddRange(new Control[] { chkHasCode, chkUseFile });
        
        // Row 5: File path
        var filePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 6), // Reduced from 8
            Visible = false
        };
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        
        txtFilePath = new TextBox
        {
            Name = "txtFilePath",
            ReadOnly = true,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 0, 8, 0), // Reduced from 10
            Enabled = false
        };
        txtFilePath.TextChanged += TxtFilePath_TextChanged;
        
        btnBrowseFile = new Button
        {
            Text = "Browse...",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(12, 4, 12, 4), // Reduced from 15, 5
            Enabled = false
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        
        filePanel.Controls.Add(txtFilePath, 0, 0);
        filePanel.Controls.Add(btnBrowseFile, 1, 0);
        
        // Store reference to toggle visibility
        chkUseFile.Tag = filePanel;
        
        // Row 6: Stop button (removed other buttons - now in toolbar)
        var stopButtonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 2, 0, 6)
        };
        
        btnStop = new Button
        {
            Text = "Stop Typing",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 8, 15, 8),
            FlatStyle = FlatStyle.System,
            Enabled = false
        };
        btnStop.Click += BtnStop_Click;
        
        stopButtonPanel.Controls.Add(btnStop);
        
        // Row 7: Hotkey display
        lblHotkeyDisplay = new Label
        {
            Text = "Hotkey: (select a snippet)",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = Color.Gray,
            AutoSize = true,
            Dock = DockStyle.Top
        };
        
        editorLayout.Controls.Add(headerToolbarPanel, 0, 0);
        editorLayout.Controls.Add(namePanel, 0, 1);
        editorLayout.Controls.Add(contentPanel, 0, 2);
        editorLayout.Controls.Add(speedPanel, 0, 3);
        editorLayout.Controls.Add(checkboxPanel, 0, 4);
        editorLayout.Controls.Add(filePanel, 0, 5);
        editorLayout.Controls.Add(stopButtonPanel, 0, 6);
        editorLayout.Controls.Add(lblHotkeyDisplay, 0, 7);
        
        editorPanel.Controls.Add(editorLayout);
        
        return editorPanel;
    }

    // Field declarations
    private TabControl tabControlSets;
    private Button btnNewSet;
    private Button btnRenameSet;
    private Button btnDeleteSet;
    private Button btnSettings;
    private Button btnMinimize;
    private Label lblStatus;
    
    // Editor controls
    private TextBox txtSnippetName;
    private TextBox txtPredefinedText;
    private TrackBar sliderTypingSpeed;
    private Label lblSpeedIndicator;
    private Label lblHotkeyDisplay;
    private Button btnStop;
    private Button btnNewSnippet;
    private Button btnSaveSnippet;
    private Button btnCancelSnippet;
    private Button btnDeleteSnippet;
    private CheckBox chkHasCode;
    private CheckBox chkUseFile;
    private TextBox txtFilePath;
    private Button btnBrowseFile;

    #endregion
}
