# Feature: Snippet Sets (Collections)

**Created:** October 5, 2025  
**Status:** Planning  
**Priority:** High  
**Type:** Enhancement  
**Supersedes:** feature-multiple-texts.md (evolved version)

## Overview

Organize snippets into **Sets** (collections/categories). Each set contains up to 9 snippets with hotkeys CTRL+SHIFT+1-9. Users can switch between sets to access different snippet collections for different contexts (e.g., "Demo1" for demos, "Readme Notes" for documentation, "Email Templates" for correspondence).

## Conceptual Model

```
Application
├── Set 1: "Demo1"
│   ├── Snippet 1 (CTRL+SHIFT+1): "Hello Demo!"
│   ├── Snippet 2 (CTRL+SHIFT+2): "This is a test..."
│   └── Snippet 3 (CTRL+SHIFT+3): "Demo code here"
│
├── Set 2: "Readme Notes" ⭐ ACTIVE
│   ├── Snippet 1 (CTRL+SHIFT+1): "## Installation"
│   ├── Snippet 2 (CTRL+SHIFT+2): "## Usage"
│   └── Snippet 4 (CTRL+SHIFT+4): "## Contributing"
│
└── Set 3: "Email Templates"
    ├── Snippet 1 (CTRL+SHIFT+1): "Dear Sir/Madam,"
    └── Snippet 2 (CTRL+SHIFT+2): "Best regards,\nJohn"
```

**Key Concept:** Only ONE set is "active" at a time. Active set's hotkeys (1-9) are registered globally.

## Data Structure

### Class Hierarchy

```csharp
// Top-level application settings
public class AppSettings
{
    public List<SnippetSet> Sets { get; set; } = new();
    public string ActiveSetId { get; set; }  // Which set is currently active
    public bool MinimizeToTray { get; set; } = false;
    public int SettingsVersion { get; set; } = 2;  // For migration
}

// A collection of related snippets
public class SnippetSet
{
    public string Id { get; set; }                    // Unique identifier (GUID)
    public string Name { get; set; }                  // Display name (e.g., "Demo1", "Readme Notes")
    public string Description { get; set; }           // Optional description
    public List<Snippet> Snippets { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string ColorHex { get; set; }              // Optional: UI color coding
    public string IconName { get; set; }              // Optional: icon identifier
    
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
}

// Individual text snippet
public class Snippet
{
    public string Id { get; set; }              // Unique identifier (GUID)
    public string Name { get; set; }            // Display name (e.g., "Email Signature")
    public string Content { get; set; }         // The actual text content
    public int HotkeyNumber { get; set; }       // 1-9 for CTRL+SHIFT+1-9 (within parent set)
    public int TypingSpeed { get; set; }        // 1-10
    public bool HasCode { get; set; }           // Code mode flag
    public bool UseFile { get; set; }           // File mode flag
    public string FilePath { get; set; }        // Optional file path
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
```

## UI Design

### **Layout Option A: Three-Panel Split View** (RECOMMENDED)

```
┌───────────────────────────────────────────────────────────────────┐
│  Hotkey Typer - Snippet Sets                           [_][□][X]  │
├──────────────┬──────────────────┬─────────────────────────────────┤
│  Sets        │  Snippets        │  Editor                         │
│              │                  │                                 │
│ ⭐Demo1      │  [1] Hello Demo  │  Name: Email Signature          │
│   Readme     │  [2] Test Text   │  Set: Readme Notes              │
│   Emails     │  [3] Demo Code   │  Hotkey: CTRL+SHIFT+[1▼]        │
│              │                  │                                 │
│ [New Set]    │  [New Snippet]   │  ┌───────────────────────────┐  │
│ [Delete]     │  [Delete]        │  │ Text content here...      │  │
│ [Rename]     │  [Move to Set]   │  │                           │  │
│ [Duplicate]  │                  │  │ Best regards,             │  │
│              │  Search: [____]  │  │ John Doe                  │  │
│ [Import]     │                  │  └───────────────────────────┘  │
│ [Export]     │                  │                                 │
│              │                  │  Typing Speed: [====○====]      │
│              │                  │  ☑ Has Code  ☐ Use File         │
│              │                  │                                 │
│              │                  │  [Save]  [Apply]  [Cancel]      │
├──────────────┴──────────────────┴─────────────────────────────────┤
│ Active Set: Readme Notes | 3 snippets | Hotkeys 1,2,4 registered │
└───────────────────────────────────────────────────────────────────┘
```

### **Layout Option B: Tree View with Split Editor**

```
┌───────────────────────────────────────────────────────────────────┐
│  Hotkey Typer                                          [_][□][X]  │
├──────────────────────────────┬────────────────────────────────────┤
│  Collection Tree             │  Snippet Editor                    │
│                              │                                    │
│  📁 All Sets                 │  Name: Installation Instructions   │
│    ├─ ⭐ Demo1 (3 snippets)  │  Set: Readme Notes                 │
│    │   ├─ [1] Hello Demo    │  Hotkey: CTRL+SHIFT+1              │
│    │   ├─ [2] Test Text     │                                    │
│    │   └─ [3] Demo Code     │  ┌──────────────────────────────┐  │
│    ├─ 📂 Readme Notes        │  │ ## Installation              │  │
│    │   ├─ [1] Installation  │  │                              │  │
│    │   ├─ [2] Usage         │  │ Run the following...         │  │
│    │   └─ [4] Contributing  │  │                              │  │
│    └─ 📂 Emails (2)          │  └──────────────────────────────┘  │
│        ├─ [1] Dear Sir/Madam │                                    │
│        └─ [2] Best Regards   │  Speed: [====○====] Normal         │
│                              │  ☑ Has Code  ☐ Use File            │
│  [New Set]  [New Snippet]    │                                    │
│  [Delete]   [Set Active]     │  [Save]  [Delete]                  │
├──────────────────────────────┴────────────────────────────────────┤
│ Active: Readme Notes | 3 snippets active | Last typed: Usage      │
└───────────────────────────────────────────────────────────────────┘
```

**Recommendation: Option A (Three-Panel)** - More explicit and easier to understand

## Settings File Structure

### New Format (with Sets)

```json
{
  "settingsVersion": 2,
  "activeSetId": "abc-123-def",
  "minimizeToTray": true,
  "sets": [
    {
      "id": "abc-123-def",
      "name": "Demo1",
      "description": "Snippets for product demos",
      "colorHex": "#3498db",
      "iconName": "presentation",
      "createdDate": "2025-10-05T10:00:00",
      "modifiedDate": "2025-10-05T15:30:00",
      "snippets": [
        {
          "id": "snippet-001",
          "name": "Hello Demo",
          "content": "Hello! This is my demo application.",
          "hotkeyNumber": 1,
          "typingSpeed": 5,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T10:00:00",
          "modifiedDate": "2025-10-05T10:00:00"
        },
        {
          "id": "snippet-002",
          "name": "Test Text",
          "content": "This is a test of the typing feature...",
          "hotkeyNumber": 2,
          "typingSpeed": 7,
          "hasCode": false,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T10:05:00",
          "modifiedDate": "2025-10-05T10:05:00"
        }
      ]
    },
    {
      "id": "xyz-789-ghi",
      "name": "Readme Notes",
      "description": "Documentation snippets",
      "colorHex": "#2ecc71",
      "iconName": "document",
      "createdDate": "2025-10-05T11:00:00",
      "modifiedDate": "2025-10-05T11:00:00",
      "snippets": [
        {
          "id": "snippet-003",
          "name": "Installation",
          "content": "## Installation\n\nRun the following command...",
          "hotkeyNumber": 1,
          "typingSpeed": 4,
          "hasCode": true,
          "useFile": false,
          "filePath": "",
          "createdDate": "2025-10-05T11:00:00",
          "modifiedDate": "2025-10-05T11:00:00"
        }
      ]
    }
  ]
}
```

### Migration from Old Format

**Old Format (single text):**
```json
{
  "predefinedText": "Hello, world!",
  "typingSpeed": 5,
  "hasCodeMode": false
}
```

**Migration Logic:**
```csharp
public void MigrateFromOldFormat()
{
    if (SettingsVersion < 2 && Sets.Count == 0)
    {
        var defaultSet = new SnippetSet
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Default",
            Description = "Migrated from previous version",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
        
        if (!string.IsNullOrEmpty(oldPredefinedText))
        {
            defaultSet.Snippets.Add(new Snippet
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Default Text",
                Content = oldPredefinedText,
                HotkeyNumber = 1,
                TypingSpeed = oldTypingSpeed,
                HasCode = oldHasCodeMode,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });
        }
        
        Sets.Add(defaultSet);
        ActiveSetId = defaultSet.Id;
        SettingsVersion = 2;
    }
}
```

## User Workflows

### Workflow 1: Create a New Set
1. User clicks "New Set" button
2. Dialog appears: "Enter set name"
3. User types "Readme Notes"
4. System creates empty set with that name
5. Set appears in the Sets list
6. User can now add snippets to this set

### Workflow 2: Add Snippet to Active Set
1. User selects "Readme Notes" set (becomes active ⭐)
2. User clicks "New Snippet" button
3. System creates new snippet with default name "New Snippet 1"
4. System assigns first available hotkey (e.g., 1)
5. User edits name to "Installation"
6. User types/pastes content
7. User clicks "Save"
8. Snippet appears in Snippets list for "Readme Notes" set
9. CTRL+SHIFT+1 now types this content (because "Readme Notes" is active)

### Workflow 3: Switch Active Set
1. User is working with "Demo1" set (active)
   - CTRL+SHIFT+1 = "Hello Demo"
   - CTRL+SHIFT+2 = "Test Text"
2. User clicks on "Email Templates" set
3. System shows confirmation: "Switch active set to 'Email Templates'? This will change your hotkeys."
4. User confirms
5. System:
   - Unregisters old hotkeys (Demo1's 1,2,3)
   - Registers new hotkeys (Email Templates' 1,2)
   - Updates UI to show Email Templates snippets
   - Marks "Email Templates" with ⭐
6. Now CTRL+SHIFT+1 = "Dear Sir/Madam" (from Email Templates)

### Workflow 4: Move Snippet Between Sets
1. User selects snippet "Hello Demo" in "Demo1" set
2. User clicks "Move to Set" button
3. Dropdown appears showing all sets: [Readme Notes, Email Templates]
4. User selects "Email Templates"
5. System:
   - Removes snippet from Demo1
   - Adds to Email Templates
   - Assigns first available hotkey in Email Templates
   - Shows confirmation
6. Snippet now appears in Email Templates list

### Workflow 5: Export/Import a Set
**Export:**
1. User right-clicks on "Readme Notes" set
2. User selects "Export Set..."
3. File dialog appears
4. User saves as "readme-snippets.json"
5. Set (with all snippets) is exported

**Import:**
1. User clicks "Import" button
2. User selects "readme-snippets.json"
3. Dialog: "Import as new set or merge with existing?"
4. User chooses "New Set"
5. System creates new set with imported snippets
6. Set appears in list

## Set CRUD Operations

### Create Set
- **Button:** "New Set"
- **Input:** Set name (required)
- **Validation:** Name cannot be empty, should be unique
- **Result:** New empty set created, not active by default

### Read/View Set
- **Action:** Click on set in list
- **Result:** Shows set's snippets in middle panel

### Update/Rename Set
- **Button:** "Rename" (with set selected)
- **Input:** New name
- **Validation:** Name required, must be unique
- **Result:** Set name updated in list

### Delete Set
- **Button:** "Delete" (with set selected)
- **Confirmation:** "Delete set 'Demo1' and all its snippets? This cannot be undone."
- **Validation:** Cannot delete if it's the only set
- **Result:** Set and all snippets removed
- **Special:** If deleting active set, prompt to choose new active set

### Set Active
- **Button:** "Set Active" or click star icon
- **Action:** Makes selected set the active one
- **Result:** Hotkeys re-registered for new active set

### Duplicate Set
- **Button:** "Duplicate"
- **Result:** Creates copy of set with " - Copy" suffix
- **Behavior:** All snippets duplicated with new IDs

## Storage Location Options

### Option 1: Single File (Simple)
```
%LOCALAPPDATA%\HotkeyTyper\
└── settings.json  (contains all sets and snippets)
```

### Option 2: Set-Based Files (Organized)
```
%LOCALAPPDATA%\HotkeyTyper\
├── settings.json          (app settings, active set ID)
└── sets\
    ├── demo1.json        (Demo1 set with snippets)
    ├── readme-notes.json (Readme Notes set)
    └── email-templates.json
```

### Option 3: Hybrid (Best for Performance)
```
%LOCALAPPDATA%\HotkeyTyper\
├── settings.json          (app settings, set metadata)
├── sets\
│   ├── abc-123.json      (individual set files)
│   └── xyz-789.json
└── backups\              (automatic backups)
    └── 2025-10-05-settings.json
```

**Recommendation: Option 1** for initial implementation (simpler), can migrate to Option 3 later if needed.

## Implementation Phases

### Phase 1: Core Set Infrastructure (Foundation)
**Estimated Time: 3-4 hours**

1. Create `SnippetSet` class
2. Create `Snippet` class
3. Update `AppSettings` class
4. Implement migration from old format
5. Add unit tests for migration
6. Update settings load/save to handle sets

### Phase 2: Basic Set UI (Essential)
**Estimated Time: 4-5 hours**

7. Add Sets ListBox to left panel
8. Add "New Set" button
9. Add "Delete Set" button with confirmation
10. Add "Rename Set" button
11. Implement set selection (shows snippets)
12. Add visual indicator for active set (⭐)

### Phase 3: Set Activation & Hotkeys
**Estimated Time: 3-4 hours**

13. Implement "Set Active" functionality
14. Unregister old hotkeys when switching sets
15. Register new hotkeys for active set
16. Update status bar to show active set
17. Add confirmation dialog for set switching

### Phase 4: Snippet Management Within Sets
**Estimated Time: 4-5 hours**

18. Add Snippets ListBox (middle panel)
19. Show snippets for selected set
20. Add "New Snippet" button (adds to current set)
21. Add "Delete Snippet" button
22. Implement snippet editing (right panel)
23. Save snippet changes to parent set

### Phase 5: Advanced Set Operations
**Estimated Time: 3-4 hours**

24. Add "Duplicate Set" functionality
25. Add "Move Snippet to Set" functionality
26. Implement drag & drop between sets (optional)
27. Add set color/icon support (optional)

### Phase 6: Import/Export
**Estimated Time: 3-4 hours**

28. Export single set to .json
29. Export all sets to .json
30. Import set (new or merge)
31. File dialogs for import/export

### Phase 7: Polish & Testing
**Estimated Time: 4-5 hours**

32. Add keyboard shortcuts
33. Add tooltips
34. Search/filter snippets
35. Comprehensive testing
36. Update documentation

**Total Estimated Time: 24-31 hours**

## Technical Considerations

### Active Set Management
- Only ONE set can be active at a time
- Active set's hotkeys are globally registered
- Switching sets requires hotkey re-registration (slight delay)
- Need to handle switching gracefully (unregister → register)

### Hotkey Conflicts
- Each set has independent 1-9 hotkeys
- No conflicts between sets (only active set matters)
- Same hotkey number can exist in multiple sets

### Performance
- Lazy load snippet content if very large
- Cache active set for fast hotkey response
- Efficient set switching

### Error Handling
- Empty sets allowed
- Cannot delete last set
- Cannot delete active set without choosing replacement
- Handle corrupted settings gracefully

### Backward Compatibility
- Auto-migrate old single-text settings
- Create default set from old data
- Keep backup of old settings before migration

## Future Enhancements

1. **Set Templates**: Pre-made sets (e.g., "Programming", "Business")
2. **Set Import from Library**: Community-shared sets
3. **Quick Set Switching**: Hotkey to cycle through sets
4. **Set Tags**: Categorize sets with tags
5. **Cloud Sync**: Sync sets across devices
6. **Set Sharing**: Export/share sets with team
7. **Set Statistics**: Track usage per set
8. **Smart Suggestions**: Suggest which set to activate based on active window

## Questions & Decisions

### Q1: Can a snippet exist in multiple sets?
**A:** No (Phase 1). Each snippet belongs to exactly one set. User can duplicate snippet to another set.

### Q2: What happens if user has no sets?
**A:** Create a "Default" set automatically on first run.

### Q3: How many sets can a user create?
**A:** Unlimited (Phase 1). Can add limits later if needed (e.g., 50 sets).

### Q4: Can user rename a set while it's active?
**A:** Yes. Renaming doesn't affect hotkeys.

### Q5: Should switching sets be quick-action or require confirmation?
**A:** Show confirmation first time, then add "Don't ask again" checkbox.

### Q6: Can sets have the same name?
**A:** No. Set names must be unique.

## Success Metrics

- ✅ Users can create unlimited sets
- ✅ Each set can have up to 9 snippets
- ✅ Only active set's hotkeys are registered
- ✅ Switching sets works smoothly
- ✅ Old settings migrate automatically
- ✅ Export/import works for sets
- ✅ UI clearly shows which set is active
- ✅ No performance degradation with many sets

---

**Next Steps:**
1. Review and approve this design
2. Choose storage location strategy
3. Start Phase 1 implementation
4. Iterate and gather feedback
