# Feature: Multiple Predefined Texts

**Created:** October 5, 2025  
**Status:** Planning  
**Priority:** High  
**Type:** Enhancement

## Overview

Extend HotkeyTyper to support saving and managing multiple predefined texts, each with its own hotkey assignment. This allows users to have quick access to different text snippets without having to constantly edit and save.

## Current Limitations

- Only one predefined text can be saved at a time
- Users must edit and overwrite to change the text
- Single hotkey (CTRL+SHIFT+1) is hardcoded
- No way to organize or categorize different text snippets

## Proposed Solution

### Core Features

#### 1. Multiple Text Entries
- Support up to 9 text entries (CTRL+SHIFT+1 through CTRL+SHIFT+9)
- Each entry has:
  - **Name/Label**: User-friendly identifier (e.g., "Email Signature", "Code Template")
  - **Text Content**: The actual text to type
  - **Hotkey**: Assigned keyboard shortcut (CTRL+SHIFT+1-9)
  - **Typing Speed**: Individual speed setting per entry
  - **Has Code**: Individual flag for code mode per entry
  - **File Path**: Optional linked file (if using file mode)

#### 2. User Interface Changes

**Layout Option A: Split View (Recommended)**
```
┌─────────────────────────────────────────────┐
│  Hotkey Typer - Manage Multiple Texts       │
├──────────────┬──────────────────────────────┤
│ Text List    │  Text Editor                 │
│              │                              │
│ • [1] Email  │  Name: Email Signature       │
│ • [2] Code   │  Hotkey: CTRL+SHIFT+1        │
│   [3] Empty  │                              │
│   [4] Empty  │  ┌────────────────────────┐  │
│   ...        │  │ Text content here...   │  │
│              │  │                        │  │
│ [New]        │  │                        │  │
│ [Delete]     │  └────────────────────────┘  │
│ [Import]     │                              │
│ [Export]     │  Typing Speed: [====○====]   │
│              │  ☑ Has Code  ☐ Use File      │
│              │                              │
│              │  [Save]  [Apply]  [Cancel]   │
├──────────────┴──────────────────────────────┤
│ Status: 2 texts saved, 7 slots available    │
└─────────────────────────────────────────────┘
```

**Layout Option B: Tabbed View**
```
┌─────────────────────────────────────────────┐
│  Hotkey Typer                               │
├─────────────────────────────────────────────┤
│ [1: Email] [2: Code] [3: Empty] ... [+New]  │
├─────────────────────────────────────────────┤
│  Active: Email Signature (CTRL+SHIFT+1)     │
│                                             │
│  ┌───────────────────────────────────────┐  │
│  │ Text content here...                  │  │
│  │                                       │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  Typing Speed: [====○====] Normal           │
│  ☑ Has Code  ☐ Use File                     │
│                                             │
│  [Save]  [Delete]  [Minimize to Tray]       │
├─────────────────────────────────────────────┤
│ Status: Hotkeys 1-2 active, 7 available     │
└─────────────────────────────────────────────┘
```

**Recommendation: Option A (Split View)** - Provides better overview and management capabilities

#### 3. Data Structure

```csharp
public class TextEntry
{
    public string Id { get; set; }              // Unique identifier (GUID)
    public string Name { get; set; }            // Display name
    public string Content { get; set; }         // The text content
    public int HotkeyNumber { get; set; }       // 1-9 for CTRL+SHIFT+1-9
    public int TypingSpeed { get; set; }        // 1-10
    public bool HasCode { get; set; }           // Code mode flag
    public bool UseFile { get; set; }           // File mode flag
    public string FilePath { get; set; }        // Optional file path
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class AppSettings
{
    public List<TextEntry> TextEntries { get; set; } = new();
    public bool MinimizeToTray { get; set; }
    
    // For backward compatibility
    public void MigrateFromOldFormat(string oldText, int oldSpeed, bool oldHasCode)
    {
        if (TextEntries.Count == 0 && !string.IsNullOrEmpty(oldText))
        {
            TextEntries.Add(new TextEntry
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
        }
    }
}
```

#### 4. Settings File Format (settings.json)

**New Format:**
```json
{
  "textEntries": [
    {
      "id": "a1b2c3d4-...",
      "name": "Email Signature",
      "content": "Best regards,\nJohn Doe",
      "hotkeyNumber": 1,
      "typingSpeed": 5,
      "hasCode": false,
      "useFile": false,
      "filePath": "",
      "createdDate": "2025-10-05T10:30:00",
      "modifiedDate": "2025-10-05T10:30:00"
    },
    {
      "id": "e5f6g7h8-...",
      "name": "Code Template",
      "content": "public class MyClass\n{\n    // TODO\n}",
      "hotkeyNumber": 2,
      "typingSpeed": 3,
      "hasCode": true,
      "useFile": false,
      "filePath": "",
      "createdDate": "2025-10-05T10:35:00",
      "modifiedDate": "2025-10-05T10:35:00"
    }
  ],
  "minimizeToTray": true
}
```

**Migration from Old Format:**
```json
{
  "predefinedText": "Hello, this is my text",
  "typingSpeed": 5,
  "hasCodeMode": false
}
```
→ Automatically converted to new format with single entry at hotkey 1

## Implementation Plan

### Phase 1: Core Infrastructure (Foundation)
**Goal:** Set up data structures and basic multi-text support

1. ✅ Create `TextEntry` class
2. ✅ Create `AppSettings` class with migration logic
3. ✅ Update `SettingsManager` to handle new format
4. ✅ Add backward compatibility for old settings.json
5. ✅ Write unit tests for settings migration

**Estimated Time:** 2-3 hours

### Phase 2: UI - List View (Essential UI)
**Goal:** Allow users to see and select multiple texts

6. ✅ Redesign form layout with SplitContainer
7. ✅ Add ListBox/ListView for text entries (left panel)
8. ✅ Show entry name and hotkey in list
9. ✅ Implement selection change to load text details
10. ✅ Add visual indicators (active/inactive)

**Estimated Time:** 3-4 hours

### Phase 3: UI - Entry Management (CRUD Operations)
**Goal:** Create, edit, delete text entries

11. ✅ Add "New Entry" button
12. ✅ Add "Delete Entry" button with confirmation
13. ✅ Add "Rename Entry" functionality
14. ✅ Add "Duplicate Entry" button
15. ✅ Implement entry name validation (no duplicates)
16. ✅ Add Save/Cancel/Apply buttons for editor

**Estimated Time:** 3-4 hours

### Phase 4: Hotkey Assignment
**Goal:** Assign and manage hotkeys for each entry

17. ✅ Add ComboBox for hotkey selection (1-9)
18. ✅ Prevent duplicate hotkey assignments
19. ✅ Show available vs. assigned hotkeys
20. ✅ Allow "None" option (text without hotkey)
21. ✅ Visual feedback for conflicts

**Estimated Time:** 2-3 hours

### Phase 5: Multiple Hotkey Registration
**Goal:** Register and handle multiple global hotkeys

22. ✅ Refactor hotkey registration to support array
23. ✅ Register CTRL+SHIFT+1 through CTRL+SHIFT+9
24. ✅ Map hotkey events to correct text entry
25. ✅ Update status messages with active hotkeys
26. ✅ Test hotkey handling with multiple entries

**Estimated Time:** 3-4 hours

### Phase 6: Import/Export (Nice to Have)
**Goal:** Backup and share text collections

27. ✅ Add "Export All" button (save to .json file)
28. ✅ Add "Import" button (load from .json file)
29. ✅ File dialog for export/import
30. ✅ Merge vs. Replace options on import
31. ✅ Validation of imported data

**Estimated Time:** 2-3 hours

### Phase 7: Polish & Testing
**Goal:** Improve UX and ensure stability

32. ✅ Add search/filter box (optional)
33. ✅ Improve status messages
34. ✅ Add keyboard shortcuts (Ctrl+N for new, Del for delete)
35. ✅ Add tooltips and help text
36. ✅ Test all scenarios thoroughly
37. ✅ Update documentation

**Estimated Time:** 3-4 hours

**Total Estimated Time:** 18-25 hours

## User Workflows

### Workflow 1: Adding a New Text Entry
1. User clicks "New Entry" button
2. System creates new entry with default name "New Text X"
3. System assigns first available hotkey (or None)
4. User enters custom name
5. User types/pastes text content
6. User adjusts typing speed if needed
7. User clicks "Save"
8. System saves to settings.json
9. Hotkey is immediately active

### Workflow 2: Editing Existing Entry
1. User selects entry from list
2. System loads entry details in editor panel
3. User modifies text/settings
4. User clicks "Save" or "Apply"
5. System updates entry and saves to disk
6. Changes take effect immediately

### Workflow 3: Reassigning Hotkeys
1. User selects entry from list
2. User changes hotkey dropdown to different number
3. If hotkey is already assigned:
   - System shows warning
   - User can: swap hotkeys, or cancel
4. User clicks "Save"
5. System re-registers hotkeys
6. New hotkey is active

### Workflow 4: Using Multiple Hotkeys
1. User has 3 entries configured:
   - Entry 1: Email signature (CTRL+SHIFT+1)
   - Entry 2: Code template (CTRL+SHIFT+2)
   - Entry 3: Meeting notes (CTRL+SHIFT+3)
2. User presses CTRL+SHIFT+2 in any application
3. System types Entry 2's text
4. Status updates: "Typed: Code template (CTRL+SHIFT+2)"

## Technical Considerations

### Hotkey Limits
- Windows supports limited global hotkeys
- CTRL+SHIFT+1-9 = 9 possible entries (reasonable limit)
- Consider allowing users to disable hotkeys they don't use

### Performance
- Loading all entries at startup
- Ensure fast hotkey response time
- Lazy load file contents if using file mode

### Error Handling
- Duplicate hotkey prevention
- Invalid hotkey assignments
- File I/O errors (settings.json)
- Corrupted settings recovery

### Backward Compatibility
- Detect old settings format
- Auto-migrate on first launch
- Keep backup of old settings

## Future Enhancements (Beyond Initial Release)

1. **Categories/Folders**: Organize texts into groups
2. **Custom Hotkeys**: Allow other key combinations (CTRL+ALT, etc.)
3. **Cloud Sync**: Sync texts across devices
4. **Templates with Variables**: Insert date, username, etc.
5. **Rich Text Support**: Format, colors, etc.
6. **Snippets Library**: Pre-made collection of common texts
7. **Usage Statistics**: Track which texts are used most
8. **Quick Preview**: Tooltip showing text content on hover
9. **Drag & Drop Reordering**: Change hotkey assignments visually
10. **Tags**: Tag entries for better organization

## Success Metrics

- ✅ Users can save at least 9 different texts
- ✅ Hotkeys 1-9 all work independently
- ✅ Settings persist across app restarts
- ✅ Old settings automatically migrate
- ✅ No performance degradation with max entries
- ✅ UI is intuitive and easy to navigate
- ✅ Export/Import works reliably

## Questions to Answer

1. **What happens if user tries to assign same hotkey to two entries?**
   - Show warning and prevent save, OR
   - Auto-reassign conflicting entry to next available, OR
   - Allow swap with confirmation

2. **Should we limit entry names to certain length?**
   - Suggested: 50 characters max for display

3. **What if user has no entries?**
   - Show welcome message
   - Prompt to create first entry
   - Or auto-create a default entry

4. **Should empty hotkey slots be shown in the list?**
   - Option A: Show all 9 slots (1-9), mark empty
   - Option B: Show only created entries
   - **Recommendation: Option B** (cleaner UI)

5. **Can users reorder entries in the list?**
   - Phase 1: No (sorted by hotkey number)
   - Future: Drag & drop support

## Files to Modify

### New Files
- `HotkeyTyper/Models/TextEntry.cs` - Data model
- `HotkeyTyper/Models/AppSettings.cs` - Settings container
- `HotkeyTyper/Managers/SettingsManager.cs` - Load/Save logic

### Modified Files
- `HotkeyTyper/Form1.cs` - Main form logic
- `HotkeyTyper/Form1.Designer.cs` - UI layout
- `HotkeyTyper/Program.cs` - Hotkey registration

### Test Files
- `HotkeyTyper.Tests/SettingsMigrationTests.cs`
- `HotkeyTyper.Tests/TextEntryTests.cs`

## References

- Current single-text implementation in `Form1.cs`
- Settings persistence in `settings.json`
- Hotkey registration in `Program.cs`

---

**Next Steps:**
1. Review and approve this plan
2. Start with Phase 1 (Core Infrastructure)
3. Iterate through phases in order
4. Get user feedback after Phase 3
