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
        ClientSize = new Size(1000, 650);
        MinimumSize = new Size(900, 600);
        Text = "Hotkey Typer";
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        FormBorderStyle = FormBorderStyle.Sizable;
        Padding = new Padding(15);
        
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
        
        // Row 0: Tab header panel
        var tabHeaderPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 45,
            Padding = new Padding(0, 0, 0, 10)
        };
        
        // Tab control
        tabControlSets = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5F),
            Padding = new Point(10, 5)
        };
        tabControlSets.SelectedIndexChanged += TabControlSets_SelectedIndexChanged;
        
        // Buttons panel (positioned over tab control)
        var tabButtonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(5, 8, 5, 0)
        };
        
        btnNewSet = new Button
        {
            Text = "➕",
            Width = 35,
            Height = 28,
            Font = new Font("Segoe UI", 10F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 3, 0)
        };
        btnNewSet.Click += BtnNewSet_Click;
        
        btnRenameSet = new Button
        {
            Text = "✏️",
            Width = 35,
            Height = 28,
            Font = new Font("Segoe UI", 10F),
            FlatStyle = FlatStyle.System,
            Margin = new Padding(0, 0, 3, 0),
            Enabled = false
        };
        btnRenameSet.Click += BtnRenameSet_Click;
        
        btnDeleteSet = new Button
        {
            Text = "🗑️",
            Width = 35,
            Height = 28,
            Font = new Font("Segoe UI", 10F),
            FlatStyle = FlatStyle.System,
            Enabled = false
        };
        btnDeleteSet.Click += BtnDeleteSet_Click;
        
        tabButtonsPanel.Controls.AddRange(new Control[] { btnNewSet, btnRenameSet, btnDeleteSet });
        
        tabHeaderPanel.Controls.Add(tabControlSets);
        tabHeaderPanel.Controls.Add(tabButtonsPanel);
        
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
            Text = "Minimize to Tray",
            Dock = DockStyle.Right,
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 5, 15, 5)
        };
        btnMinimize.Click += BtnMinimize_Click;
        
        statusPanel.Controls.Add(lblStatus);
        statusPanel.Controls.Add(btnMinimize);
        
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
            Padding = new Padding(15)
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
        tabLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Snippets list
        tabLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // Editor
        
        // Row 0: Snippets header + New button
        var snippetsHeaderPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 40,
            Padding = new Padding(0, 0, 0, 10)
        };
        
        var lblSnippetsHeader = new Label
        {
            Text = "Snippets in this set:",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Dock = DockStyle.Left,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft
        };
        
        var btnNewSnippet = new Button
        {
            Text = "➕ Add New Snippet",
            Dock = DockStyle.Right,
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 5, 15, 5),
            Tag = set
        };
        btnNewSnippet.Click += BtnNewSnippet_Click;
        
        snippetsHeaderPanel.Controls.Add(lblSnippetsHeader);
        snippetsHeaderPanel.Controls.Add(btnNewSnippet);
        
        // Row 1: Snippets ListView
        var listViewSnippets = new ListView
        {
            Name = "listViewSnippets_" + set.Id,
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Segoe UI", 9.5F),
            Tag = set
        };
        
        listViewSnippets.Columns.Add("Hotkey", 100);
        listViewSnippets.Columns.Add("Name", 300);
        listViewSnippets.Columns.Add("Preview", 400);
        listViewSnippets.SelectedIndexChanged += ListViewSnippets_SelectedIndexChanged;
        
        // Populate snippets
        foreach (var snippet in set.Snippets)
        {
            var item = new ListViewItem(snippet.GetHotkeyDisplay());
            item.SubItems.Add(snippet.Name);
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
            Padding = new Padding(0, 15, 0, 0)
        };
        
        var editorLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(245, 245, 245)
        };
        
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Name field
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Content textbox
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Speed
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Checkboxes
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // File path
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons
        editorLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hotkey display
        
        // Row 0: Editor header
        var lblEditorHeader = new Label
        {
            Text = "Snippet Editor",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 10)
        };
        
        // Row 1: Name field
        var namePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
        };
        namePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        namePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        
        var lblName = new Label
        {
            Text = "Name:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 5, 10, 0)
        };
        
        txtSnippetName = new TextBox
        {
            Name = "txtSnippetName",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5F),
            Enabled = false
        };
        
        namePanel.Controls.Add(lblName, 0, 0);
        namePanel.Controls.Add(txtSnippetName, 1, 0);
        
        // Row 2: Content
        var contentLabel = new Label
        {
            Text = "Content:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 5)
        };
        
        txtPredefinedText = new TextBox
        {
            Name = "txtPredefinedText",
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9.5F),
            Enabled = false,
            Margin = new Padding(0, 0, 0, 10)
        };
        
        var contentPanel = new Panel
        {
            Dock = DockStyle.Fill
        };
        contentPanel.Controls.Add(txtPredefinedText);
        contentPanel.Controls.Add(contentLabel);
        
        // Row 3: Typing speed
        var speedPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
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
            Margin = new Padding(0, 5, 10, 0)
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
            Margin = new Padding(10, 5, 0, 0),
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
            Margin = new Padding(0, 0, 0, 10)
        };
        
        chkHasCode = new CheckBox
        {
            Text = "Has Code (limit speed)",
            Font = new Font("Segoe UI", 9F),
            AutoSize = true,
            Enabled = false,
            Margin = new Padding(0, 0, 20, 0)
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
            Margin = new Padding(0, 0, 0, 15),
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
            Margin = new Padding(0, 0, 10, 0),
            Enabled = false
        };
        
        btnBrowseFile = new Button
        {
            Text = "Browse...",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 5, 15, 5),
            Enabled = false
        };
        btnBrowseFile.Click += BtnBrowseFile_Click;
        
        filePanel.Controls.Add(txtFilePath, 0, 0);
        filePanel.Controls.Add(btnBrowseFile, 1, 0);
        
        // Store reference to toggle visibility
        chkUseFile.Tag = filePanel;
        
        // Row 6: Action buttons
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 10)
        };
        
        btnSaveSnippet = new Button
        {
            Text = "💾 Save Changes",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Padding = new Padding(20, 8, 20, 8),
            Margin = new Padding(0, 0, 10, 0),
            Enabled = false
        };
        btnSaveSnippet.Click += BtnSaveSnippet_Click;
        
        btnDeleteSnippet = new Button
        {
            Text = "🗑️ Delete This Snippet",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 8, 15, 8),
            Margin = new Padding(0, 0, 10, 0),
            Enabled = false
        };
        btnDeleteSnippet.Click += BtnDeleteSnippet_Click;
        
        btnStop = new Button
        {
            Text = "Stop Typing",
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Padding = new Padding(15, 8, 15, 8),
            Enabled = false
        };
        btnStop.Click += BtnStop_Click;
        
        buttonPanel.Controls.AddRange(new Control[] { btnSaveSnippet, btnDeleteSnippet, btnStop });
        
        // Row 7: Hotkey display
        lblHotkeyDisplay = new Label
        {
            Text = "Hotkey: (select a snippet)",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = Color.Gray,
            AutoSize = true,
            Dock = DockStyle.Top
        };
        
        editorLayout.Controls.Add(lblEditorHeader, 0, 0);
        editorLayout.Controls.Add(namePanel, 0, 1);
        editorLayout.Controls.Add(contentPanel, 0, 2);
        editorLayout.Controls.Add(speedPanel, 0, 3);
        editorLayout.Controls.Add(checkboxPanel, 0, 4);
        editorLayout.Controls.Add(filePanel, 0, 5);
        editorLayout.Controls.Add(buttonPanel, 0, 6);
        editorLayout.Controls.Add(lblHotkeyDisplay, 0, 7);
        
        editorPanel.Controls.Add(editorLayout);
        
        return editorPanel;
    }

    // Field declarations
    private TabControl tabControlSets;
    private Button btnNewSet;
    private Button btnRenameSet;
    private Button btnDeleteSet;
    private Button btnMinimize;
    private Label lblStatus;
    
    // Editor controls
    private TextBox txtSnippetName;
    private TextBox txtPredefinedText;
    private TrackBar sliderTypingSpeed;
    private Label lblSpeedIndicator;
    private Label lblHotkeyDisplay;
    private Button btnStop;
    private Button btnSaveSnippet;
    private Button btnDeleteSnippet;
    private CheckBox chkHasCode;
    private CheckBox chkUseFile;
    private TextBox txtFilePath;
    private Button btnBrowseFile;

    #endregion
}
