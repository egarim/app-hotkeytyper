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
        ClientSize = new Size(600, 650); // Increased size for better layout
        MinimumSize = new Size(550, 600);
        Text = "Hotkey Typer - CTRL+SHIFT+1 to Type Text";
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;
        Padding = new Padding(15); // Add padding around the form
        
        // Create controls
        CreateControls();
    }
    
    private void CreateControls()
    {
        // Create main layout panel
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            Padding = new Padding(5),
            AutoSize = true
        };
        
        // Configure row styles
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Instructions
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Text box (expandable)
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Typing speed controls
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Checkboxes
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // File path
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
        
        // Row 0: Instructions Label
        var lblInstructions = new Label
        {
            Text = "Configure your predefined text below.\nPress CTRL+SHIFT+1 anywhere to type it:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            Margin = new Padding(0, 0, 0, 10),
            Dock = DockStyle.Top
        };
        
        // Row 1: TextBox for predefined text
        var txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Text = predefinedText,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Segoe UI", 9F),
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 15)
        };
        
        // Row 2: Typing speed controls panel
        var speedPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 15)
        };
        
        var lblTypingSpeed = new Label
        {
            Text = "Typing Speed:",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 5, 10, 0),
            TextAlign = ContentAlignment.MiddleLeft
        };
        
        var sliderTypingSpeed = new LimitedTrackBar
        {
            Name = "sliderTypingSpeed",
            Width = 220,
            Height = 45,
            Minimum = 1,
            Maximum = 10,
            Value = typingSpeed,
            TickFrequency = 1,
            TickStyle = TickStyle.None,
            SoftMax = hasCodeMode ? 8 : null,
            Margin = new Padding(0, 0, 10, 0)
        };
        sliderTypingSpeed.ValueChanged += TypingSpeedSlider_ValueChanged;
        
        var lblSpeedIndicator = new Label
        {
            Name = "lblSpeedIndicator",
            Text = "Normal",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            Margin = new Padding(0, 5, 0, 0),
            TextAlign = ContentAlignment.MiddleLeft
        };
        
        speedPanel.Controls.AddRange(new Control[] { lblTypingSpeed, sliderTypingSpeed, lblSpeedIndicator });
        
        // Row 3: Checkboxes panel
        var checkboxPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 10)
        };
        
        var chkHasCode = new CheckBox
        {
            Name = "chkHasCode",
            Text = "Has Code (limit speed)",
            AutoSize = true,
            Checked = hasCodeMode,
            Font = new Font("Segoe UI", 9F),
            Margin = new Padding(0, 0, 20, 0)
        };
        chkHasCode.CheckedChanged += ChkHasCode_CheckedChanged;
        
        var chkUseFile = new CheckBox
        {
            Name = "chkUseFile",
            Text = "Use File (.md/.txt)",
            AutoSize = true,
            Checked = false,
            Font = new Font("Segoe UI", 9F)
        };
        chkUseFile.CheckedChanged += ChkUseFile_CheckedChanged;
        
        checkboxPanel.Controls.AddRange(new Control[] { chkHasCode, chkUseFile });
        
        // Row 4: File path panel
        var filePanel = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 15)
        };
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        
        var txtFilePath = new TextBox
        {
            Name = "txtFilePath",
            Text = string.Empty,
            Enabled = false,
            ReadOnly = true,
            Font = new Font("Segoe UI", 9F),
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 10, 0)
        };
        
        var btnBrowseFile = new Button
        {
            Name = "btnBrowseFile",
            Text = "Browse…",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Enabled = false,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 5, 15, 5)
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        
        filePanel.Controls.Add(txtFilePath, 0, 0);
        filePanel.Controls.Add(btnBrowseFile, 1, 0);
        
        // Row 5: Buttons panel
        var buttonPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 15)
        };
        
        var btnUpdate = new Button
        {
            Text = "Save",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(20, 8, 20, 8),
            Margin = new Padding(0, 0, 10, 0)
        };
        btnUpdate.Click += BtnUpdate_Click;
        
        var btnMinimize = new Button
        {
            Text = "Minimize to Tray",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(20, 8, 20, 8),
            Margin = new Padding(0, 0, 10, 0)
        };
        btnMinimize.Click += BtnMinimize_Click;
        
        var btnStop = new Button
        {
            Name = "btnStop",
            Text = "Stop Typing",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(20, 8, 20, 8),
            Enabled = false
        };
        btnStop.Click += BtnStop_Click;
        
        buttonPanel.Controls.AddRange(new Control[] { btnUpdate, btnMinimize, btnStop });
        
        // Row 6: Status label
        var lblStatus = new Label
        {
            Text = "Status: Hotkey CTRL+SHIFT+1 is active",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            ForeColor = Color.Green,
            Dock = DockStyle.Fill
        };
        
        // Add all rows to main layout
        mainLayout.Controls.Add(lblInstructions, 0, 0);
        mainLayout.Controls.Add(txtPredefinedText, 0, 1);
        mainLayout.Controls.Add(speedPanel, 0, 2);
        mainLayout.Controls.Add(checkboxPanel, 0, 3);
        mainLayout.Controls.Add(filePanel, 0, 4);
        mainLayout.Controls.Add(buttonPanel, 0, 5);
        mainLayout.Controls.Add(lblStatus, 0, 6);
        
        // Add main layout to form
        Controls.Add(mainLayout);
        
        // Store references for later use
        this.txtPredefinedText = txtPredefinedText;
        this.sliderTypingSpeed = sliderTypingSpeed;
        this.lblSpeedIndicator = lblSpeedIndicator;
        this.lblStatus = lblStatus;
        this.btnStop = btnStop;
        this.chkHasCode = chkHasCode;
        this.chkUseFile = chkUseFile;
        this.txtFilePath = txtFilePath;
        this.btnBrowseFile = btnBrowseFile;
    }
    
    private TextBox txtPredefinedText;
    private TrackBar sliderTypingSpeed;
    private Label lblSpeedIndicator;
    private Label lblStatus;
    private Button btnStop;
    private CheckBox chkHasCode;
    private CheckBox chkUseFile;
    private TextBox txtFilePath;
    private Button btnBrowseFile;

    #endregion
}
