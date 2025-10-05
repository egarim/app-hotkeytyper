namespace HotkeyTyper.Models;

public class UISettings
{
    public UIScale Scale { get; set; } = UIScale.Normal;
    public int BaseFontSize { get; set; } = 9; // Base font size for UI
    public int ContentFontSize { get; set; } = 10; // Font size for content editor
    public bool AutoSave { get; set; } = false; // Auto-save changes without clicking Save button
}

public enum UIScale
{
    Small = 0,   // 90% - For high DPI (150%+)
    Normal = 1,  // 100% - Default
    Large = 2    // 110% - For low DPI or accessibility
}
