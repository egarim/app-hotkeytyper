# Data Model Quick Reference

## Class Hierarchy

```
AppConfiguration (Root)
│
├── SettingsVersion: int (2)
├── ActiveSetId: string (GUID)
├── MinimizeToTray: bool
│
└── Sets: List<SnippetSet>
    │
    ├── SnippetSet
    │   ├── Id: string (GUID)
    │   ├── Name: string
    │   ├── Description: string
    │   ├── ColorHex: string (#RRGGBB)
    │   ├── IconName: string
    │   ├── CreatedDate: DateTime
    │   ├── ModifiedDate: DateTime
    │   │
    │   └── Snippets: List<Snippet>
    │       │
    │       └── Snippet
    │           ├── Id: string (GUID)
    │           ├── Name: string
    │           ├── Content: string
    │           ├── HotkeyNumber: int (1-9)
    │           ├── TypingSpeed: int (ms)
    │           ├── HasCode: bool
    │           ├── UseFile: bool
    │           ├── FilePath: string? (nullable)
    │           ├── CreatedDate: DateTime
    │           └── ModifiedDate: DateTime
```

---

## Quick Usage Examples

### 1. Create a New Snippet

```csharp
var snippet = new Snippet
{
    Name = "Hello World",
    Content = "Hello, World!",
    HotkeyNumber = 1,
    TypingSpeed = 10,
    HasCode = false
};

// Validation
if (snippet.IsValid(out string error))
{
    // Good to use
}
else
{
    Console.WriteLine($"Invalid: {error}");
}
```

### 2. Create a Snippet Set

```csharp
var set = new SnippetSet
{
    Name = "Work Emails",
    Description = "Common email templates",
    ColorHex = "#4285F4"
};

// Add snippets
set.AddSnippet(snippet);

// Check available hotkeys
var available = set.GetAvailableHotkeys(); // [2, 3, 4, 5, 6, 7, 8, 9]

// Get snippet by hotkey
var snippet1 = set.GetSnippetByHotkey(1);
```

### 3. Create Configuration

```csharp
var config = new AppConfiguration();
config.CreateDefaultSet(); // Creates "Default" set with sample

// Add custom set
var workSet = new SnippetSet { Name = "Work" };
config.AddSet(workSet);

// Set active
config.SetActiveSet(workSet.Id);
```

### 4. Save and Load Settings

```csharp
var manager = new SettingsManager();

// Save
manager.SaveSettings(config);
// Saves to: %LOCALAPPDATA%\HotkeyTyper\settings.json

// Load
var loaded = manager.LoadSettings();
// Automatically migrates from old format if needed

// Export/Import
manager.ExportSettings(config, "C:\\backup.json");
var imported = manager.ImportSettings("C:\\backup.json");
```

### 5. Migration from Old Format

```csharp
// Old format (automatically detected by SettingsManager)
var oldSettings = new AppSettings
{
    PredefinedText = "Old text",
    TypingSpeed = 8,
    HasCode = true
};

// Automatic migration in SettingsManager.LoadSettings()
// Or manual migration:
var newConfig = new AppConfiguration();
newConfig.MigrateFromOldFormat(oldSettings);
// Creates "Default" set with old text as snippet #1
```

---

## Helper Methods

### Snippet

| Method | Returns | Purpose |
|--------|---------|---------|
| `GetHotkeyDisplay()` | `string` | "Ctrl+Shift+1" |
| `GetContentPreview()` | `string` | First 50 chars + "..." |
| `Clone(newName?)` | `Snippet` | Deep copy with new GUID |
| `IsValid(out error)` | `bool` | Validates all properties |

### SnippetSet

| Method | Returns | Purpose |
|--------|---------|---------|
| `GetSnippetByHotkey(num)` | `Snippet?` | Find by hotkey 1-9 |
| `GetAvailableHotkeys()` | `List<int>` | Unused hotkeys |
| `HasHotkeyConflict(num)` | `bool` | Check if assigned |
| `GetNextAvailableHotkey()` | `int?` | First free slot |
| `AddSnippet(snippet)` | `void` | Add to collection |
| `RemoveSnippet(id)` | `bool` | Remove by GUID |
| `Clone(newName?)` | `SnippetSet` | Deep copy |
| `IsValid(out error)` | `bool` | Validates set + snippets |

### AppConfiguration

| Method | Returns | Purpose |
|--------|---------|---------|
| `GetActiveSet()` | `SnippetSet?` | Current active set |
| `SetActiveSet(id)` | `void` | Change active set |
| `GetSetById(id)` | `SnippetSet?` | Find by GUID |
| `GetSetByName(name)` | `SnippetSet?` | Find by name |
| `IsSetNameTaken(name)` | `bool` | Check duplicates |
| `AddSet(set)` | `void` | Add to collection |
| `RemoveSet(id)` | `bool` | Remove by GUID |
| `MigrateFromOldFormat(old)` | `void` | Convert V1 → V2 |
| `CreateDefaultSet()` | `void` | Initialize first set |
| `EnsureValid()` | `void` | Fix invalid state |
| `IsValid(out error)` | `bool` | Validate everything |

### SettingsManager

| Method | Returns | Purpose |
|--------|---------|---------|
| `LoadSettings()` | `AppConfiguration` | Load from JSON |
| `SaveSettings(config)` | `bool` | Save to JSON |
| `ExportSettings(config, path)` | `bool` | Manual export |
| `ImportSettings(path)` | `AppConfiguration?` | Manual import |
| `GetSettingsFolder()` | `string` | AppData path |
| `GetSettingsFilePath()` | `string` | settings.json path |

---

## Validation Rules

### Snippet
- ✅ `Name` is not null/empty
- ✅ `Content` is not null/empty (unless `UseFile` is true)
- ✅ `HotkeyNumber` is 1-9
- ✅ `TypingSpeed` is > 0
- ✅ `FilePath` is not null/empty when `UseFile` is true

### SnippetSet
- ✅ `Name` is not null/empty
- ✅ No duplicate hotkey numbers
- ✅ All snippets are valid
- ✅ Maximum 9 snippets

### AppConfiguration
- ✅ At least one set exists
- ✅ No duplicate set names
- ✅ `ActiveSetId` exists in Sets (or is empty)
- ✅ All sets are valid

---

## JSON Property Names

| C# Property | JSON Key |
|-------------|----------|
| `SettingsVersion` | `settingsVersion` |
| `ActiveSetId` | `activeSetId` |
| `MinimizeToTray` | `minimizeToTray` |
| `Sets` | `sets` |
| `Id` | `id` |
| `Name` | `name` |
| `Content` | `content` |
| `HotkeyNumber` | `hotkeyNumber` |
| `TypingSpeed` | `typingSpeed` |
| `HasCode` | `hasCode` |
| `UseFile` | `useFile` |
| `FilePath` | `filePath` |
| `CreatedDate` | `createdDate` |
| `ModifiedDate` | `modifiedDate` |
| `Description` | `description` |
| `ColorHex` | `colorHex` |
| `IconName` | `iconName` |
| `Snippets` | `snippets` |

---

## Storage Locations

| Type | Path |
|------|------|
| **Settings** | `%LOCALAPPDATA%\HotkeyTyper\settings.json` |
| **Backup** | `%LOCALAPPDATA%\HotkeyTyper\settings.backup.json` |
| **Old V1** | `<app dir>\settings.json` (migrated to `.migrated`) |

Actual path example:
```
C:\Users\<username>\AppData\Local\HotkeyTyper\settings.json
```

---

## Common Patterns

### Loading Settings on Startup

```csharp
public partial class Form1 : Form
{
    private SettingsManager settingsManager;
    private AppConfiguration appConfig;

    public Form1()
    {
        InitializeComponent();
        
        settingsManager = new SettingsManager();
        appConfig = settingsManager.LoadSettings();
        
        // Now use appConfig to populate UI
        LoadSetsIntoUI();
    }
}
```

### Saving Settings on Change

```csharp
private void SaveSettings()
{
    if (settingsManager.SaveSettings(appConfig))
    {
        lblStatus.Text = "Settings saved";
    }
    else
    {
        MessageBox.Show("Failed to save settings!", "Error", 
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

### Switching Active Set

```csharp
private void lstSets_SelectedIndexChanged(object sender, EventArgs e)
{
    if (lstSets.SelectedItem is SnippetSet selectedSet)
    {
        appConfig.SetActiveSet(selectedSet.Id);
        LoadSnippetsIntoUI(selectedSet);
        SaveSettings(); // Persist the change
    }
}
```

### Adding a New Snippet

```csharp
private void btnAddSnippet_Click(object sender, EventArgs e)
{
    var activeSet = appConfig.GetActiveSet();
    if (activeSet == null) return;

    var snippet = new Snippet
    {
        Name = txtSnippetName.Text,
        Content = txtContent.Text,
        HotkeyNumber = activeSet.GetNextAvailableHotkey() ?? 1,
        TypingSpeed = (int)numTypingSpeed.Value
    };

    if (snippet.IsValid(out string error))
    {
        activeSet.AddSnippet(snippet);
        SaveSettings();
        RefreshSnippetsList();
    }
    else
    {
        MessageBox.Show($"Invalid snippet: {error}");
    }
}
```

---

## Migration Flow Diagram

```
┌─────────────────────────────────────┐
│  SettingsManager.LoadSettings()    │
└─────────────┬───────────────────────┘
              │
              ▼
    ┌──────────────────┐
    │ File exists?     │
    └─────┬────────────┘
          │
    ┌─────┴─────┐
    NO          YES
    │           │
    ▼           ▼
┌───────┐  ┌──────────────┐
│Check  │  │Parse JSON    │
│old    │  │              │
│loc.   │  └──────┬───────┘
└───┬───┘         │
    │       ┌─────┴─────┐
    │       │Version?   │
    │       └─────┬─────┘
    │             │
    │      ┌──────┴──────┐
    │      V1            V2
    │      │             │
    ▼      ▼             ▼
┌─────────────┐    ┌────────┐
│Migrate from │    │Return  │
│old format   │    │config  │
└─────┬───────┘    └────────┘
      │
      ▼
┌────────────────┐
│Create Default  │
│AppConfiguration│
└────────────────┘
```

---

## Next Steps

See `FEATURE-PLAN-snippet-sets.md` for full 8-phase plan.

**Current Status**: Phase 1 Complete ✅  
**Next Phase**: Phase 2 - Basic UI (Sets Panel)

