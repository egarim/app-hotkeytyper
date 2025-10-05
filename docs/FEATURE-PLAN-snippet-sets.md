# Master Feature Plan: Snippet Sets with Collections

**Created:** October 5, 2025  
**Status:** Planning → Ready for Implementation  
**Priority:** High  
**Type:** Major Enhancement  
**Replaces:** feature-multiple-texts.md, feature-snippet-sets.md

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Vision & Goals](#vision--goals)
3. [Architecture Overview](#architecture-overview)
4. [Data Structures](#data-structures)
5. [User Interface Design](#user-interface-design)
6. [Storage & Persistence](#storage--persistence)
7. [Implementation Roadmap](#implementation-roadmap)
8. [User Workflows](#user-workflows)
9. [Technical Specifications](#technical-specifications)
10. [Testing & Validation](#testing--validation)
11. [Future Enhancements](#future-enhancements)

---

## Executive Summary

Transform HotkeyTyper from a single-text typing tool into a powerful snippet management system with hierarchical organization.

### Evolution Path

```
Current State                Phase 1                    Phase 2 (Final)
─────────────               ─────────────               ─────────────
Single Text                 Multiple Snippets           Organized Sets
                                                        
[One text]        →        [Snippet 1-9]      →        Set 1: "Demo"
CTRL+SHIFT+1               CTRL+SHIFT+1-9                ├─ Snippet 1-9
                                                         
                                                        Set 2: "Readme"
                                                          ├─ Snippet 1-9
                                                         
                                                        Set 3: "Emails"
                                                          └─ Snippet 1-9
```

### What We're Building

**Snippet Sets**: Organize your text snippets into collections (sets) for different contexts.

- **Example 1:** "Demo1" set with product demo snippets
- **Example 2:** "Readme Notes" set with documentation snippets  
- **Example 3:** "Email Templates" set with common email responses

**Key Innovation:** Only ONE set is active at a time. Switch between sets to access different snippet collections using the same hotkeys (CTRL+SHIFT+1-9).

---

## Vision & Goals

### Primary Goals

1. ✅ **Organization**: Group related snippets into logical sets
2. ✅ **Context Switching**: Quick switching between snippet collections
3. ✅ **Scalability**: Support unlimited sets with 9 snippets each
4. ✅ **Usability**: Intuitive UI with clear visual hierarchy
5. ✅ **Portability**: Easy import/export for backup and sharing

### User Benefits

- **For Developers**: Separate sets for different projects/languages
- **For Writers**: Different sets for various clients/topics
- **For Support**: Sets for different products/scenarios
- **For Demos**: Dedicated sets for presentations

### Success Criteria

- ✅ Users can create unlimited sets
- ✅ Each set supports up to 9 snippets (CTRL+SHIFT+1-9)
- ✅ Active set switching is smooth and fast (< 1 second)
- ✅ Old single-text settings migrate automatically
- ✅ No data loss during migration
- ✅ Export/Import works reliably
- ✅ UI is intuitive for first-time users

---

## Architecture Overview

### Conceptual Model

```
HotkeyTyper Application
│
├─── Application Settings (Global)
│    ├─ Active Set ID
│    ├─ Window preferences
│    └─ Minimize to tray setting
│
├─── Set Collection
│    │
│    ├─── Set 1: "Demo1" ⭐ ACTIVE
│    │    ├─ Metadata (name, description, colors)
│    │    └─ Snippets
│    │        ├─ Snippet 1 (CTRL+SHIFT+1): "Hello Demo!"
│    │        ├─ Snippet 2 (CTRL+SHIFT+2): "Test text..."
│    │        └─ Snippet 3 (CTRL+SHIFT+3): "Demo code..."
│    │
│    ├─── Set 2: "Readme Notes"
│    │    ├─ Metadata
│    │    └─ Snippets
│    │        ├─ Snippet 1 (CTRL+SHIFT+1): "## Installation"
│    │        ├─ Snippet 2 (CTRL+SHIFT+2): "## Usage"
│    │        └─ Snippet 4 (CTRL+SHIFT+4): "## Contributing"
│    │
│    └─── Set 3: "Email Templates"
│         ├─ Metadata
│         └─ Snippets
│             ├─ Snippet 1 (CTRL+SHIFT+1): "Dear Sir/Madam,"
│             └─ Snippet 2 (CTRL+SHIFT+2): "Best regards,\nJohn"
│
└─── Global Hotkey Registry (Runtime)
     └─ Registers hotkeys only for ACTIVE set
```

### Key Concepts

**Set (Collection)**
- Container for related snippets
- Has unique ID, name, description
- Contains 0-9 snippets
- Has metadata (creation date, colors, icons)

**Snippet (Text Entry)**
- Individual text content
- Belongs to exactly ONE set
- Has hotkey assignment (1-9 within its set)
- Has individual settings (speed, code mode, etc.)

**Active Set**
- Only ONE set can be active at a time
- Active set's hotkeys are globally registered
- Switching sets = unregister old + register new hotkeys
- Marked with ⭐ in UI

**Hotkey Mapping**
- Each set has independent 1-9 hotkeys
- CTRL+SHIFT+1 in "Demo1" ≠ CTRL+SHIFT+1 in "Readme Notes"
- No conflicts between sets (only active set matters)

---

## Data Structures

### Class Hierarchy

```csharp
/// <summary>
/// Top-level application settings container
/// </summary>
public class AppSettings
{
    public int SettingsVersion { get; set; } = 2;
    public List<SnippetSet> Sets { get; set; } = new();
    public string ActiveSetId { get; set; }
    public bool MinimizeToTray { get; set; } = false;
    
    // Helper methods
    public SnippetSet GetActiveSet()
    {
        return Sets.FirstOrDefault(s => s.Id == ActiveSetId);
    }
    
    public void SetActiveSet(string setId)
    {
        if (Sets.Any(s => s.Id == setId))
        {
            ActiveSetId = setId;
        }
    }
    
    // Migration from old format
    public void MigrateFromOldFormat(string oldText, int oldSpeed, bool oldHasCode)
    {
        if (SettingsVersion < 2 && Sets.Count == 0 && !string.IsNullOrEmpty(oldText))
        {
            var defaultSet = new SnippetSet
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Default",
                Description = "Migrated from previous version",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            
            defaultSet.Snippets.Add(new Snippet
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Default Text",
                Content = oldText,
                HotkeyNumber = 1,
                TypingSpeed = oldSpeed,
                HasCode = oldHasCode,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });
            
            Sets.Add(defaultSet);
            ActiveSetId = defaultSet.Id;
            SettingsVersion = 2;
        }
    }
}

/// <summary>
/// A collection/set of related snippets
/// </summary>
public class SnippetSet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<Snippet> Snippets { get; set; } = new();
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
    
    // Optional visual customization
    public string ColorHex { get; set; } = "#3498db";
    public string IconName { get; set; } = "folder";
    
    // Helper methods
    public Snippet GetSnippetByHotkey(int hotkeyNumber)
    {
        return Snippets.FirstOrDefault(s => s.HotkeyNumber == hotkeyNumber);
    }
    
    public List<int> GetAvailableHotkeys()
    {
        var assigned = Snippets.Select(s => s.HotkeyNumber).ToHashSet();
        return Enumerable.Range(1, 9).Where(n => !assigned.Contains(n)).ToList();
    }
    
    public bool HasHotkeyConflict(int hotkeyNumber, string excludeSnippetId = null)
    {
        return Snippets.Any(s => s.HotkeyNumber == hotkeyNumber && s.Id != excludeSnippetId);
    }
    
    public int GetSnippetCount() => Snippets.Count;
    
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.Now;
    }
}

/// <summary>
/// Individual text snippet/entry
/// </summary>
public class Snippet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Content { get; set; }
    public int HotkeyNumber { get; set; }
    public int TypingSpeed { get; set; } = 5;
    public bool HasCode { get; set; } = false;
    public bool UseFile { get; set; } = false;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
    
    // Helper methods
    public string GetHotkeyDisplay()
    {
        return $"CTRL+SHIFT+{HotkeyNumber}";
    }
    
    public string GetContentPreview(int maxLength = 50)
    {
        if (string.IsNullOrEmpty(Content))
            return "(empty)";
        
        var preview = Content.Replace("\n", " ").Replace("\r", "");
        return preview.Length <= maxLength 
            ? preview 
            : preview.Substring(0, maxLength) + "...";
    }
    
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.Now;
    }
}
```

### Validation Rules

**SnippetSet**
- ✅ Name: Required, 1-100 characters, must be unique
- ✅ Description: Optional, max 500 characters
- ✅ Snippets: 0-9 snippets allowed
- ✅ At least one set must exist in application

**Snippet**
- ✅ Name: Required, 1-100 characters
- ✅ Content: Optional (can be empty for placeholders)
- ✅ HotkeyNumber: 1-9, must be unique within parent set
- ✅ TypingSpeed: 1-10
- ✅ FilePath: Valid path if UseFile is true

---

## User Interface Design

### Layout: Three-Panel Split View (RECOMMENDED)

```
┌─────────────────────────────────────────────────────────────────────────┐
│  HotkeyTyper - Snippet Sets                                  [_][□][X]  │
├──────────────────┬───────────────────────┬──────────────────────────────┤
│ SETS             │ SNIPPETS              │ SNIPPET EDITOR               │
│ (Left Panel)     │ (Middle Panel)        │ (Right Panel)                │
├──────────────────┼───────────────────────┼──────────────────────────────┤
│                  │                       │                              │
│ ⭐ Demo1 (3)     │ For Set: Demo1        │ Snippet Details              │
│   Readme (3)     │                       │                              │
│   Emails (2)     │ [1] 🔥 Hello Demo     │ Name: [Hello Demo        ]   │
│                  │ [2] 📝 Test Text      │ Set:  Demo1 (active)         │
│                  │ [3] 💻 Demo Code      │ Hotkey: CTRL+SHIFT+[1 ▼]     │
│ ───────────────  │                       │                              │
│ [➕ New Set]     │ ───────────────────── │ Content:                     │
│ [🗑️ Delete]      │ [➕ New Snippet]      │ ┌──────────────────────────┐ │
│ [✏️ Rename]      │ [🗑️ Delete]           │ │ Hello! This is my demo   │ │
│ [📋 Duplicate]   │ [📋 Duplicate]        │ │ application. Let me      │ │
│                  │ [↗️ Move to...]        │ │ show you the features... │ │
│ ───────────────  │                       │ │                          │ │
│ [📥 Import]      │ 🔍 Search:            │ │                          │ │
│ [📤 Export]      │ [____________]        │ │                          │ │
│                  │                       │ └──────────────────────────┘ │
│                  │                       │                              │
│                  │                       │ Typing Speed: [====●====]    │
│                  │                       │              Normal          │
│                  │                       │                              │
│                  │                       │ ☑ Has Code  ☐ Use File       │
│                  │                       │                              │
│                  │                       │ ─────────────────────────    │
│                  │                       │ [💾 Save] [✓ Apply] [✖ Cancel] │
├──────────────────┴───────────────────────┴──────────────────────────────┤
│ Status: Set "Demo1" active | 3 snippets | Hotkeys 1,2,3 registered     │
│ Last Action: Snippet "Hello Demo" saved                                 │
└─────────────────────────────────────────────────────────────────────────┘
```

### Panel Breakdown

#### **Left Panel: Sets List**
- **Purpose:** Show all snippet sets, indicate active set
- **Controls:**
  - ListBox with set names
  - Visual indicator (⭐) for active set
  - Snippet count badge (e.g., "Demo1 (3)")
  - Management buttons below
- **Interactions:**
  - Single click: Select set (shows its snippets in middle panel)
  - Double click: Set as active
  - Right click: Context menu (Set Active, Rename, Delete, etc.)

#### **Middle Panel: Snippets List**
- **Purpose:** Show snippets for selected set
- **Controls:**
  - ListBox with snippet entries
  - Each entry shows: [Hotkey#] Icon Name
  - Search box at bottom
  - Management buttons
- **Interactions:**
  - Single click: Select snippet (loads in editor)
  - Double click: Quick edit mode
  - Right click: Context menu

#### **Right Panel: Snippet Editor**
- **Purpose:** View/edit selected snippet
- **Controls:**
  - Name textbox
  - Set name (read-only, shows parent set)
  - Hotkey dropdown (1-9)
  - Content textbox (multi-line, expandable)
  - Typing speed slider
  - Has Code checkbox
  - Use File checkbox + path/browse
  - Save/Apply/Cancel buttons
- **Behaviors:**
  - Auto-save on focus lost (optional)
  - Unsaved changes warning
  - Real-time validation

### Window Dimensions

**Minimum Size:** 900 x 650 pixels  
**Default Size:** 1100 x 700 pixels  
**Recommended:** 1200 x 750 pixels  

### Visual Design Elements

**Active Set Indicator:**
- ⭐ Star icon next to active set name
- Highlighted background color
- Bold text

**Snippet Icons:**
- 📝 Regular text
- 💻 Code snippet (Has Code = true)
- 📄 File-based (Use File = true)
- Custom icons based on content type (future)

**Color Coding (Optional):**
- Sets can have custom colors for visual distinction
- Color stripe on left side of set item

---

## Storage & Persistence

### Storage Location

**Recommended: AppData\Local**

```
C:\Users\{username}\AppData\Local\HotkeyTyper\
├── settings.json          (all app data)
├── settings.backup.json   (automatic backup)
└── logs\                  (error logs - future)
    └── error-2025-10-05.log
```

**Access Path in Code:**
```csharp
private readonly string settingsFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "HotkeyTyper"
);

private readonly string settingsFilePath = Path.Combine(
    settingsFolder,
    "settings.json"
);
```

### File Format: settings.json

**New Format (Version 2 - with Sets):**

```json
{
  "settingsVersion": 2,
  "activeSetId": "abc-123-def-456",
  "minimizeToTray": true,
  "sets": [
    {
      "id": "abc-123-def-456",
      "name": "Demo1",
      "description": "Snippets for product demonstrations",
      "colorHex": "#3498db",
      "iconName": "presentation",
      "createdDate": "2025-10-05T10:00:00Z",
      "modifiedDate": "2025-10-05T15:30:00Z",
      "snippets": [
        {
          "id": "snippet-001",
          "name": "Hello Demo",
          "content": "Hello! This is my demo application. Let me show you the features...",
          "hotkeyNumber": 1,
          "typingSpeed": 5,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T10:00:00Z",
          "modifiedDate": "2025-10-05T10:00:00Z"
        },
        {
          "id": "snippet-002",
          "name": "Test Text",
          "content": "This is a test of the typing feature. Speed can be adjusted.",
          "hotkeyNumber": 2,
          "typingSpeed": 7,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T10:05:00Z",
          "modifiedDate": "2025-10-05T10:05:00Z"
        },
        {
          "id": "snippet-003",
          "name": "Demo Code",
          "content": "public class Example\n{\n    public void Demo()\n    {\n        Console.WriteLine(\"Hello\");\n    }\n}",
          "hotkeyNumber": 3,
          "typingSpeed": 3,
          "hasCode": true,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T10:10:00Z",
          "modifiedDate": "2025-10-05T10:10:00Z"
        }
      ]
    },
    {
      "id": "xyz-789-ghi-012",
      "name": "Readme Notes",
      "description": "Documentation snippets for README files",
      "colorHex": "#2ecc71",
      "iconName": "document",
      "createdDate": "2025-10-05T11:00:00Z",
      "modifiedDate": "2025-10-05T11:00:00Z",
      "snippets": [
        {
          "id": "snippet-004",
          "name": "Installation",
          "content": "## Installation\n\nRun the following command to install:\n\n```bash\nnpm install my-package\n```",
          "hotkeyNumber": 1,
          "typingSpeed": 4,
          "hasCode": true,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T11:00:00Z",
          "modifiedDate": "2025-10-05T11:00:00Z"
        },
        {
          "id": "snippet-005",
          "name": "Usage",
          "content": "## Usage\n\nImport the library and use it like this:\n\n```javascript\nimport { MyClass } from 'my-package';\n```",
          "hotkeyNumber": 2,
          "typingSpeed": 4,
          "hasCode": true,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T11:05:00Z",
          "modifiedDate": "2025-10-05T11:05:00Z"
        },
        {
          "id": "snippet-006",
          "name": "Contributing",
          "content": "## Contributing\n\nWe welcome contributions! Please:\n\n1. Fork the repository\n2. Create a feature branch\n3. Submit a pull request",
          "hotkeyNumber": 4,
          "typingSpeed": 5,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T11:10:00Z",
          "modifiedDate": "2025-10-05T11:10:00Z"
        }
      ]
    },
    {
      "id": "mno-345-pqr-678",
      "name": "Email Templates",
      "description": "Common email responses",
      "colorHex": "#e74c3c",
      "iconName": "email",
      "createdDate": "2025-10-05T12:00:00Z",
      "modifiedDate": "2025-10-05T12:00:00Z",
      "snippets": [
        {
          "id": "snippet-007",
          "name": "Formal Greeting",
          "content": "Dear Sir/Madam,\n\nThank you for your inquiry. ",
          "hotkeyNumber": 1,
          "typingSpeed": 6,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T12:00:00Z",
          "modifiedDate": "2025-10-05T12:00:00Z"
        },
        {
          "id": "snippet-008",
          "name": "Signature",
          "content": "Best regards,\nJohn Doe\nSenior Developer\nAcme Corporation\njohn.doe@acme.com",
          "hotkeyNumber": 2,
          "typingSpeed": 5,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T12:05:00Z",
          "modifiedDate": "2025-10-05T12:05:00Z"
        }
      ]
    }
  ]
}
```

### Migration from Old Format

**Old Format (Version 1 - Single Text):**

```json
{
  "predefinedText": "Hello, this is my predefined text!",
  "typingSpeed": 5,
  "hasCodeMode": false
}
```

**Migration Logic:**

```csharp
public class SettingsManager
{
    public AppSettings LoadSettings()
    {
        if (!File.Exists(settingsFilePath))
        {
            return CreateDefaultSettings();
        }
        
        var json = File.ReadAllText(settingsFilePath);
        var settings = JsonSerializer.Deserialize<AppSettings>(json);
        
        // Check if migration needed
        if (settings.SettingsVersion < 2)
        {
            MigrateFromV1(settings, json);
            SaveSettings(settings); // Save migrated version
        }
        
        return settings;
    }
    
    private void MigrateFromV1(AppSettings settings, string oldJson)
    {
        try
        {
            // Try to extract old format fields
            var oldSettings = JsonSerializer.Deserialize<OldSettings>(oldJson);
            
            if (!string.IsNullOrEmpty(oldSettings.PredefinedText))
            {
                // Create default set with old data
                var defaultSet = new SnippetSet
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Default",
                    Description = "Migrated from previous version",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                
                defaultSet.Snippets.Add(new Snippet
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Default Text",
                    Content = oldSettings.PredefinedText,
                    HotkeyNumber = 1,
                    TypingSpeed = oldSettings.TypingSpeed,
                    HasCode = oldSettings.HasCodeMode,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                });
                
                settings.Sets.Add(defaultSet);
                settings.ActiveSetId = defaultSet.Id;
            }
            
            settings.SettingsVersion = 2;
            
            // Create backup of old settings
            File.Copy(settingsFilePath, 
                     settingsFilePath + ".v1.backup", 
                     overwrite: true);
        }
        catch (Exception ex)
        {
            // Log error and create fresh settings
            Console.WriteLine($"Migration failed: {ex.Message}");
        }
    }
    
    private AppSettings CreateDefaultSettings()
    {
        var settings = new AppSettings
        {
            SettingsVersion = 2,
            MinimizeToTray = false
        };
        
        // Create default set with sample snippet
        var defaultSet = new SnippetSet
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Default",
            Description = "Your first snippet set",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
        
        defaultSet.Snippets.Add(new Snippet
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sample Snippet",
            Content = "Hello, this is a sample snippet!\nEdit this to get started.",
            HotkeyNumber = 1,
            TypingSpeed = 5,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        });
        
        settings.Sets.Add(defaultSet);
        settings.ActiveSetId = defaultSet.Id;
        
        return settings;
    }
}

// Old settings format for migration
public class OldSettings
{
    [JsonPropertyName("predefinedText")]
    public string PredefinedText { get; set; }
    
    [JsonPropertyName("typingSpeed")]
    public int TypingSpeed { get; set; } = 5;
    
    [JsonPropertyName("hasCodeMode")]
    public bool HasCodeMode { get; set; }
}
```

---

## Implementation Roadmap

### Phase 1: Foundation & Data Layer (Week 1)
**Goal:** Build core data structures and persistence  
**Estimated Time:** 6-8 hours

#### Tasks:
1. ✅ Create `Models` folder in project
2. ✅ Implement `Snippet` class with all properties and helpers
3. ✅ Implement `SnippetSet` class with collection management
4. ✅ Implement `AppSettings` class with migration logic
5. ✅ Create `SettingsManager` class for load/save operations
6. ✅ Implement migration from old single-text format
7. ✅ Add JSON serialization/deserialization
8. ✅ Create backup mechanism before migration
9. ✅ Write unit tests for migration logic
10. ✅ Test settings persistence

**Deliverable:** Working data layer that can load/save settings

---

### Phase 2: Basic UI - Sets Panel (Week 1)
**Goal:** Display and manage sets  
**Estimated Time:** 6-8 hours

#### Tasks:
11. ✅ Update Form1.Designer.cs with SplitContainer layout
12. ✅ Add left panel (Sets) with ListBox
13. ✅ Bind sets to ListBox with custom rendering
14. ✅ Add "New Set" button with dialog
15. ✅ Implement CreateSet() method
16. ✅ Add "Delete Set" button with confirmation
17. ✅ Implement DeleteSet() method with validation
18. ✅ Add "Rename Set" button with dialog
19. ✅ Implement RenameSet() method
20. ✅ Add "Duplicate Set" button
21. ✅ Implement DuplicateSet() method
22. ✅ Add visual indicator for active set (⭐)
23. ✅ Handle set selection (load snippets)

**Deliverable:** Working sets panel with full CRUD

---

### Phase 3: Snippets Panel (Week 2)
**Goal:** Display and manage snippets within sets  
**Estimated Time:** 6-8 hours

#### Tasks:
24. ✅ Add middle panel (Snippets) with ListBox
25. ✅ Bind snippets to ListBox for selected set
26. ✅ Add custom rendering (hotkey number + icon + name)
27. ✅ Add "New Snippet" button
28. ✅ Implement CreateSnippet() in active set
29. ✅ Add "Delete Snippet" button with confirmation
30. ✅ Implement DeleteSnippet() method
31. ✅ Handle snippet selection (load in editor)
32. ✅ Add search/filter textbox
33. ✅ Implement search functionality

**Deliverable:** Working snippets panel with CRUD

---

### Phase 4: Editor Panel (Week 2)
**Goal:** Edit snippet details  
**Estimated Time:** 5-7 hours

#### Tasks:
34. ✅ Add right panel (Editor) with form controls
35. ✅ Add Name textbox with validation
36. ✅ Add Set name label (read-only, shows parent)
37. ✅ Add Hotkey dropdown (1-9)
38. ✅ Prevent duplicate hotkeys in same set
39. ✅ Add Content textbox (multi-line, scrollable)
40. ✅ Add Typing Speed slider
41. ✅ Add Has Code checkbox
42. ✅ Add Use File checkbox + path + browse button
43. ✅ Add Save, Apply, Cancel buttons
44. ✅ Implement save logic
45. ✅ Add unsaved changes warning

**Deliverable:** Fully functional snippet editor

---

### Phase 5: Set Activation & Hotkeys (Week 3)
**Goal:** Implement active set switching and hotkey registration  
**Estimated Time:** 8-10 hours

#### Tasks:
46. ✅ Add "Set Active" button/menu item
47. ✅ Implement SetActiveSet() method
48. ✅ Add confirmation dialog for switching
49. ✅ Refactor hotkey registration for multiple hotkeys
50. ✅ Implement UnregisterHotkeys() method
51. ✅ Implement RegisterHotkeys() for active set
52. ✅ Update hotkey handler to map to correct snippet
53. ✅ Handle hotkey events (type snippet content)
54. ✅ Update status bar when active set changes
55. ✅ Add "Don't ask again" option to confirmation
56. ✅ Test switching between sets
57. ✅ Test all 9 hotkeys work correctly

**Deliverable:** Working set activation and hotkey switching

---

### Phase 6: Advanced Features (Week 3)
**Goal:** Add move, duplicate, and advanced operations  
**Estimated Time:** 5-7 hours

#### Tasks:
58. ✅ Add "Move Snippet to Set" button
59. ✅ Show dialog with list of other sets
60. ✅ Implement MoveSnippet() method
61. ✅ Add "Duplicate Snippet" button
62. ✅ Implement DuplicateSnippet() (within same set)
63. ✅ Add drag & drop support (optional, future)
64. ✅ Add set color picker (optional)
65. ✅ Add set icon selector (optional)
66. ✅ Add keyboard shortcuts (Ctrl+N, Del, etc.)

**Deliverable:** Advanced snippet management features

---

### Phase 7: Import/Export (Week 4)
**Goal:** Backup and sharing functionality  
**Estimated Time:** 6-8 hours

#### Tasks:
67. ✅ Add "Export Set" button
68. ✅ Show SaveFileDialog for export
69. ✅ Implement ExportSet() to JSON
70. ✅ Add "Export All Sets" option
71. ✅ Implement ExportAllSets() method
72. ✅ Add "Import" button
73. ✅ Show OpenFileDialog for import
74. ✅ Add import options dialog (New Set / Merge / Replace)
75. ✅ Implement ImportSet() method
76. ✅ Validate imported JSON structure
77. ✅ Handle import errors gracefully
78. ✅ Add success/failure messages

**Deliverable:** Working import/export system

---

### Phase 8: Polish & Testing (Week 4)
**Goal:** Refinement, testing, documentation  
**Estimated Time:** 8-10 hours

#### Tasks:
79. ✅ Add tooltips to all buttons
80. ✅ Improve status messages
81. ✅ Add progress indicators for long operations
82. ✅ Implement error handling throughout
83. ✅ Add logging for debugging
84. ✅ Test migration from old settings
85. ✅ Test with 0 sets (edge case)
86. ✅ Test with 1 set, 1 snippet
87. ✅ Test with multiple sets, all hotkeys used
88. ✅ Test import/export roundtrip
89. ✅ Performance test with large content
90. ✅ Update README documentation
91. ✅ Create user guide
92. ✅ Fix any bugs found during testing

**Deliverable:** Production-ready application

---

### **Total Estimated Time: 50-60 hours (6-8 weeks part-time)**

---

## User Workflows

### Workflow 1: First-Time User Setup

**Scenario:** User installs app for first time

1. App launches with default set "Default" containing sample snippet
2. User sees UI tour/welcome message (optional)
3. User clicks "Rename" on "Default" set → renames to "My Snippets"
4. User selects sample snippet in middle panel
5. User edits content in right panel
6. User clicks "Save"
7. User presses CTRL+SHIFT+1 → text is typed!

**Time to first success: < 2 minutes**

---

### Workflow 2: Creating a New Set with Snippets

**Scenario:** Developer wants to create a "Code Snippets" set

1. User clicks "New Set" button
2. Dialog appears: "Enter set name"
3. User types "Code Snippets" → clicks OK
4. New set appears in left panel
5. User double-clicks "Code Snippets" → becomes active (⭐)
6. User clicks "New Snippet"
7. New snippet created with default name "New Snippet 1", hotkey 1
8. User enters:
   - Name: "For Loop"
   - Content: `for (int i = 0; i < 10; i++)\n{\n    // TODO\n}`
   - Typing Speed: 3 (slower for code)
   - Has Code: ✅ checked
9. User clicks "Save"
10. User presses CTRL+SHIFT+1 → code is typed slowly!

**Time to complete: < 3 minutes**

---

### Workflow 3: Switching Between Sets

**Scenario:** User has 3 sets and wants to switch contexts

**Current State:**
- Active set: "Demo1" ⭐
- CTRL+SHIFT+1 = "Hello Demo"
- User now needs to write documentation

**Actions:**
1. User clicks on "Readme Notes" set in left panel
2. Dialog appears: "Switch active set to 'Readme Notes'? Your hotkeys will change."
3. User clicks "Yes"
4. System:
   - Unregisters Demo1's hotkeys (1,2,3)
   - Registers Readme Notes' hotkeys (1,2,4)
   - Updates ⭐ to "Readme Notes"
   - Shows Readme Notes snippets in middle panel
5. User presses CTRL+SHIFT+1 → "## Installation" is typed

**Time to switch: < 5 seconds**

---

### Workflow 4: Moving Snippet Between Sets

**Scenario:** User wants to move a snippet from one set to another

1. User selects snippet "Email Template" in "Demo1" set
2. User clicks "Move to Set" button
3. Dialog shows dropdown with all other sets:
   - [Email Templates]
   - [Readme Notes]
4. User selects "Email Templates"
5. System:
   - Removes from Demo1
   - Adds to Email Templates
   - Assigns first available hotkey (or lets user choose)
   - Refreshes UI
6. Confirmation: "Moved 'Email Template' to 'Email Templates' set"

**Time to complete: < 10 seconds**

---

### Workflow 5: Backup with Export

**Scenario:** User wants to backup all snippets before making changes

1. User clicks "Export" button
2. Dropdown appears: [Export Current Set | Export All Sets]
3. User clicks "Export All Sets"
4. SaveFileDialog appears with default name "HotkeyTyper-Backup-2025-10-05.json"
5. User chooses location and saves
6. Success message: "Exported 3 sets with 8 snippets to [path]"
7. User can now edit with confidence

**Time to complete: < 15 seconds**

---

### Workflow 6: Restore from Import

**Scenario:** User wants to import snippets on new computer

1. User clicks "Import" button
2. OpenFileDialog appears
3. User selects "HotkeyTyper-Backup-2025-10-05.json"
4. Dialog: "How to import?"
   - ○ Create new sets (keeps existing)
   - ○ Merge with existing sets
   - ○ Replace all (deletes existing)
5. User selects "Replace all"
6. Confirmation: "This will delete all current sets. Continue?"
7. User clicks "Yes"
8. System imports all sets and snippets
9. Success: "Imported 3 sets with 8 snippets"

**Time to complete: < 30 seconds**

---

## Technical Specifications

### Hotkey Registration

```csharp
public class HotkeyManager
{
    private Dictionary<int, string> registeredHotkeys = new();
    
    public void RegisterHotkeysForSet(SnippetSet set)
    {
        UnregisterAllHotkeys();
        
        foreach (var snippet in set.Snippets)
        {
            if (snippet.HotkeyNumber >= 1 && snippet.HotkeyNumber <= 9)
            {
                RegisterHotkey(snippet.HotkeyNumber, snippet.Id);
            }
        }
    }
    
    private void RegisterHotkey(int number, string snippetId)
    {
        // MOD_CONTROL = 0x0002, MOD_SHIFT = 0x0004
        bool success = RegisterHotKey(
            this.Handle, 
            number,           // Hotkey ID
            0x0002 | 0x0004,  // CTRL + SHIFT
            (Keys)(0x30 + number) // '1' through '9'
        );
        
        if (success)
        {
            registeredHotkeys[number] = snippetId;
        }
    }
    
    private void UnregisterAllHotkeys()
    {
        foreach (var hotkeyId in registeredHotkeys.Keys)
        {
            UnregisterHotKey(this.Handle, hotkeyId);
        }
        registeredHotkeys.Clear();
    }
    
    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;
        
        if (m.Msg == WM_HOTKEY)
        {
            int hotkeyId = m.WParam.ToInt32();
            
            if (registeredHotkeys.TryGetValue(hotkeyId, out string snippetId))
            {
                OnHotkeyPressed(snippetId);
            }
        }
        
        base.WndProc(ref m);
    }
    
    private void OnHotkeyPressed(string snippetId)
    {
        var activeSet = appSettings.GetActiveSet();
        var snippet = activeSet?.Snippets.FirstOrDefault(s => s.Id == snippetId);
        
        if (snippet != null)
        {
            TypeSnippet(snippet);
        }
    }
}
```

### Performance Considerations

**Hotkey Registration:**
- Average time: < 50ms per hotkey
- Max 9 hotkeys to register
- Total switch time: < 500ms

**Set Switching:**
- Unregister old: ~200ms
- Register new: ~200ms
- UI update: ~100ms
- **Total: < 500ms** (acceptable)

**Large Content Handling:**
- Lazy load snippet content > 100KB
- Stream file-based snippets
- Background typing for very long text

**Memory Usage:**
- Estimated: 10-50 KB per snippet (depending on content)
- 100 snippets ≈ 1-5 MB (acceptable)

---

## Testing & Validation

### Unit Tests

```csharp
[TestFixture]
public class SnippetSetTests
{
    [Test]
    public void GetAvailableHotkeys_ReturnsCorrectList()
    {
        var set = new SnippetSet();
        set.Snippets.Add(new Snippet { HotkeyNumber = 1 });
        set.Snippets.Add(new Snippet { HotkeyNumber = 3 });
        
        var available = set.GetAvailableHotkeys();
        
        Assert.That(available, Is.EquivalentTo(new[] { 2, 4, 5, 6, 7, 8, 9 }));
    }
    
    [Test]
    public void HasHotkeyConflict_DetectsConflict()
    {
        var set = new SnippetSet();
        set.Snippets.Add(new Snippet { Id = "1", HotkeyNumber = 1 });
        
        Assert.IsTrue(set.HasHotkeyConflict(1));
        Assert.IsFalse(set.HasHotkeyConflict(2));
    }
}

[TestFixture]
public class MigrationTests
{
    [Test]
    public void MigrateFromOldFormat_CreatesDefaultSet()
    {
        var settings = new AppSettings();
        settings.MigrateFromOldFormat("Old text", 5, false);
        
        Assert.AreEqual(1, settings.Sets.Count);
        Assert.AreEqual("Default", settings.Sets[0].Name);
        Assert.AreEqual(1, settings.Sets[0].Snippets.Count);
        Assert.AreEqual("Old text", settings.Sets[0].Snippets[0].Content);
    }
}
```

### Integration Tests

1. **End-to-End Workflow Test**
   - Create set → add snippet → set active → trigger hotkey
   - Verify text is typed correctly

2. **Import/Export Test**
   - Export all sets → delete all → import → verify identical

3. **Migration Test**
   - Start with old format → load app → verify migration → verify functionality

### Manual Test Cases

| Test Case | Steps | Expected Result |
|-----------|-------|----------------|
| **TC-001** | Create new set with name "Test" | Set appears in list |
| **TC-002** | Rename set to empty string | Error: Name required |
| **TC-003** | Delete only remaining set | Error: Cannot delete last set |
| **TC-004** | Create snippet with duplicate hotkey | Error: Hotkey already used |
| **TC-005** | Switch active set | Hotkeys re-register correctly |
| **TC-006** | Press CTRL+SHIFT+5 (no snippet) | No action (graceful) |
| **TC-007** | Import invalid JSON file | Error message shown |
| **TC-008** | Export set with 0 snippets | Exports successfully |
| **TC-009** | Move snippet to same set | Error or no-op |
| **TC-010** | Type 10,000 character snippet | Types correctly (may be slow) |

---

## Future Enhancements

### Phase 9+: Advanced Features (Post-Launch)

1. **Cloud Sync** - Sync sets across devices
2. **Set Templates** - Pre-built sets for common scenarios
3. **Snippet Variables** - Insert date, time, username, clipboard
4. **Custom Hotkeys** - Beyond CTRL+SHIFT+1-9
5. **Rich Text** - Formatting, colors, images
6. **Snippet Preview** - Hover tooltip showing content
7. **Usage Statistics** - Track most-used snippets
8. **Categories/Tags** - Additional organization layer
9. **Search Across Sets** - Global snippet search
10. **Snippet Sharing** - Community marketplace
11. **Auto-Backup** - Automatic periodic backups
12. **Version History** - Track snippet changes over time
13. **Snippet Macros** - Combine multiple snippets
14. **Context-Aware Activation** - Auto-switch sets based on active window
15. **Mobile Companion** - View/edit snippets on phone

---

## Success Metrics & KPIs

### Technical Metrics
- ✅ Zero data loss during migration
- ✅ < 500ms set switching time
- ✅ < 100ms hotkey response time
- ✅ 100% test coverage for critical paths
- ✅ No memory leaks over 24hr usage

### User Experience Metrics
- ✅ < 5 min to create first set
- ✅ < 2 min to add first snippet
- ✅ < 10 sec to switch sets
- ✅ Intuitive UI (minimal documentation needed)
- ✅ No user errors during normal operations

### Adoption Metrics (Post-Release)
- Average snippets per user
- Average sets per user
- Most popular features
- Feature usage distribution
- User retention rate

---

## Summary & Next Steps

### What We're Building

A complete snippet management system with:
- ✅ Hierarchical organization (Sets → Snippets)
- ✅ Quick context switching
- ✅ Full CRUD operations
- ✅ Import/Export capabilities
- ✅ Backward compatibility
- ✅ Professional UI

### Implementation Strategy

**8 Phases over 6-8 weeks:**
1. Foundation (Week 1)
2. Sets UI (Week 1)
3. Snippets UI (Week 2)
4. Editor (Week 2)
5. Activation & Hotkeys (Week 3)
6. Advanced Features (Week 3)
7. Import/Export (Week 4)
8. Polish & Testing (Week 4)

### Ready to Start?

**Next Actions:**
1. ✅ Review and approve this plan
2. ✅ Choose storage location (AppData\Local recommended)
3. ✅ Start Phase 1: Create data models
4. ✅ Iterate with user feedback

---

**Document Version:** 1.0  
**Last Updated:** October 5, 2025  
**Status:** Ready for Implementation ✅
