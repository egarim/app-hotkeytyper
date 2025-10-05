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
    private Button btnOK;
    private Button btnCancel;
    private Button btnApply;
    
    public UISettings UISettings { get; private set; }
    public bool Applied { get; private set; } = false;

    public SettingsDialog(UISettings currentSettings)
    {
        UISettings = new UISettings
        {
            Scale = currentSettings.Scale,
            BaseFontSize = currentSettings.BaseFontSize,
            ContentFontSize = currentSettings.ContentFontSize
        };

        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Settings";
        this.Size = new Size(480, 320);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(15)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

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
            RowCount = 3,
            Padding = new Padding(10)
        };
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        // UI Scale
        var lblScale = new Label
        {
            Text = "UI Scale:",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 15)
        };

        cmbUIScale = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F),
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 15)
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
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 15)
        };

        numBaseFontSize = new NumericUpDown
        {
            Minimum = 7,
            Maximum = 14,
            Font = new Font("Segoe UI", 9F),
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 15)
        };

        // Content Font Size
        var lblContentFontSize = new Label
        {
            Text = "Content Font Size:",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 8, 10, 15)
        };

        numContentFontSize = new NumericUpDown
        {
            Minimum = 8,
            Maximum = 16,
            Font = new Font("Segoe UI", 9F),
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 15)
        };

        settingsLayout.Controls.Add(lblScale, 0, 0);
        settingsLayout.Controls.Add(cmbUIScale, 1, 0);
        settingsLayout.Controls.Add(lblBaseFontSize, 0, 1);
        settingsLayout.Controls.Add(numBaseFontSize, 1, 1);
        settingsLayout.Controls.Add(lblContentFontSize, 0, 2);
        settingsLayout.Controls.Add(numContentFontSize, 1, 2);

        settingsPanel.Controls.Add(settingsLayout);

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
        mainLayout.Controls.Add(buttonPanel, 0, 1);

        this.Controls.Add(mainLayout);
        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void LoadSettings()
    {
        cmbUIScale.SelectedIndex = (int)UISettings.Scale;
        numBaseFontSize.Value = UISettings.BaseFontSize;
        numContentFontSize.Value = UISettings.ContentFontSize;
    }

    private void SaveSettings()
    {
        UISettings.Scale = (UIScale)cmbUIScale.SelectedIndex;
        UISettings.BaseFontSize = (int)numBaseFontSize.Value;
        UISettings.ContentFontSize = (int)numContentFontSize.Value;
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
}
