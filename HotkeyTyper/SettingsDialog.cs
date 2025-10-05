using HotkeyTyper.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotkeyTyper;

public class SettingsDialog : Form
{
    private ComboBox cmbUIScale;
    private NumericUpDown numBaseFontSize;
    private NumericUpDown numContentFontSize;
    private CheckBox chkAutoSave;
    private Button btnOK;
    private Button btnCancel;
    private Button btnApply;
    private Button btnResetData;
    
    public UISettings UISettings { get; private set; }
    public bool Applied { get; private set; } = false;
    public bool ResetRequested { get; private set; } = false;

    public SettingsDialog(UISettings currentSettings)
    {
        UISettings = new UISettings
        {
            Scale = currentSettings.Scale,
            BaseFontSize = currentSettings.BaseFontSize,
            ContentFontSize = currentSettings.ContentFontSize,
            AutoSave = currentSettings.AutoSave
        };

        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Settings";
        this.Size = new Size(600, 650);
        this.MinimumSize = new Size(550, 580);
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(15),
            AutoScroll = true
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Display settings
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Data management
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

        // Settings panel
        var settingsPanel = new GroupBox
        {
            Text = "Display Settings",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
        };

        var settingsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(15),
            AutoSize = true
        };
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // UI Scale
        var lblScale = new Label
        {
            Text = "UI Scale:",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 8)
        };

        cmbUIScale = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F),
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(0, 5, 0, 5)
        };
        cmbUIScale.Items.AddRange(new object[] {
            "Small (90%) - For high DPI (150%+)",
            "Normal (100%) - Default",
            "Large (110%) - For low DPI or larger text"
        });

        // Base Font Size
        var lblBaseFontSize = new Label
        {
            Text = "UI Font Size:",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 8)
        };

        numBaseFontSize = new NumericUpDown
        {
            Minimum = 7,
            Maximum = 14,
            Font = new Font("Segoe UI", 9F),
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(0, 5, 0, 5)
        };

        // Content Font Size
        var lblContentFontSize = new Label
        {
            Text = "Content Font Size:",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 8)
        };

        numContentFontSize = new NumericUpDown
        {
            Minimum = 8,
            Maximum = 16,
            Font = new Font("Segoe UI", 9F),
            Anchor = AnchorStyles.Left | AnchorStyles.Right,
            Margin = new Padding(0, 5, 0, 5)
        };

        // Auto Save
        var lblAutoSave = new Label
        {
            Text = "Auto Save:",
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 8)
        };

        chkAutoSave = new CheckBox
        {
            Text = "Automatically save changes without clicking Save button",
            Font = new Font("Segoe UI", 9F),
            Anchor = AnchorStyles.Left,
            AutoSize = true,
            Margin = new Padding(0, 5, 0, 5)
        };

        settingsLayout.Controls.Add(lblScale, 0, 0);
        settingsLayout.Controls.Add(cmbUIScale, 1, 0);
        settingsLayout.Controls.Add(lblBaseFontSize, 0, 1);
        settingsLayout.Controls.Add(numBaseFontSize, 1, 1);
        settingsLayout.Controls.Add(lblContentFontSize, 0, 2);
        settingsLayout.Controls.Add(numContentFontSize, 1, 2);
        settingsLayout.Controls.Add(lblAutoSave, 0, 3);
        settingsLayout.Controls.Add(chkAutoSave, 1, 3);

        settingsPanel.Controls.Add(settingsLayout);

        // Data Management panel
        var dataPanel = new GroupBox
        {
            Text = "Data Management",
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Margin = new Padding(0, 10, 0, 0),
            AutoSize = true
        };

        var dataLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Padding = new Padding(15, 10, 15, 10)
        };

        btnResetData = new Button
        {
            Text = "🔄 Reset All Data",
            AutoSize = true,
            Padding = new Padding(15, 8, 15, 8),
            Font = new Font("Segoe UI", 9F),
            BackColor = Color.FromArgb(220, 53, 69), // Red color for warning
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnResetData.FlatAppearance.BorderSize = 0;
        btnResetData.Click += BtnResetData_Click;

        var lblResetInfo = new Label
        {
            Text = "⚠️ This will delete all snippet sets and restore default configuration",
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 100, 100),
            Margin = new Padding(10, 8, 0, 0)
        };

        dataLayout.Controls.Add(btnResetData);
        dataLayout.Controls.Add(lblResetInfo);
        dataPanel.Controls.Add(dataLayout);

        // Buttons panel
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 0)
        };

        btnOK = new Button
        {
            Text = "OK",
            AutoSize = true,
            Padding = new Padding(20, 5, 20, 5),
            Margin = new Padding(5, 0, 0, 0),
            DialogResult = DialogResult.OK
        };
        btnOK.Click += BtnOK_Click;

        btnCancel = new Button
        {
            Text = "Cancel",
            AutoSize = true,
            Padding = new Padding(20, 5, 20, 5),
            Margin = new Padding(5, 0, 0, 0),
            DialogResult = DialogResult.Cancel
        };

        btnApply = new Button
        {
            Text = "Apply",
            AutoSize = true,
            Padding = new Padding(20, 5, 20, 5),
            Margin = new Padding(5, 0, 0, 0)
        };
        btnApply.Click += BtnApply_Click;

        buttonPanel.Controls.Add(btnOK);
        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Controls.Add(btnApply);

        mainLayout.Controls.Add(settingsPanel, 0, 0);
        mainLayout.Controls.Add(dataPanel, 0, 1);
        mainLayout.Controls.Add(buttonPanel, 0, 2);

        this.Controls.Add(mainLayout);
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void LoadSettings()
    {
        cmbUIScale.SelectedIndex = (int)UISettings.Scale;
        numBaseFontSize.Value = UISettings.BaseFontSize;
        numContentFontSize.Value = UISettings.ContentFontSize;
        chkAutoSave.Checked = UISettings.AutoSave;
    }

    private void SaveSettings()
    {
        UISettings.Scale = (UIScale)cmbUIScale.SelectedIndex;
        UISettings.BaseFontSize = (int)numBaseFontSize.Value;
        UISettings.ContentFontSize = (int)numContentFontSize.Value;
        UISettings.AutoSave = chkAutoSave.Checked;
    }

    private void BtnApply_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        Applied = true;
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        SaveSettings();
        Applied = true;
    }

    private void BtnResetData_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "⚠️ WARNING: This will permanently delete ALL snippet sets and reset the application to its default state.\n\n" +
            "This action CANNOT be undone!\n\n" +
            "Are you absolutely sure you want to continue?",
            "Reset All Data - Confirmation Required",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2); // Default to "No"

        if (result == DialogResult.Yes)
        {
            // Double confirmation for safety
            var confirmResult = MessageBox.Show(
                "⚠️ FINAL WARNING!\n\n" +
                "This is your last chance to cancel.\n\n" +
                "Click YES to permanently delete all your snippets and sets.\n" +
                "Click NO to keep your data safe.",
                "Reset All Data - Final Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2); // Default to "No"

            if (confirmResult == DialogResult.Yes)
            {
                ResetRequested = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
