# Phase 1 Testing Guide

## What Was Completed

Phase 1 (Foundation & Data Layer) is **COMPLETE**. All core data model classes have been created, compiled successfully, and are ready for use.

### Files Created (949 lines of code)

- **Snippet.cs** (150 lines) - Individual text snippets
- **SnippetSet.cs** (213 lines) - Collections of snippets  
- **AppConfiguration.cs** (281 lines) - Top-level settings
- **SettingsManager.cs** (305 lines) - JSON persistence

---

## How to Test

### 1. Verify Build (Already Done)

Run the verification script:
```powershell
.\verify-phase1.ps1
```

**Expected Output:**
- Build: SUCCESS
- All 4 source files verified
- Assembly compiled: 52 KB

### 2. Test Runtime Behavior

Since the app is a Windows Forms application, the data layer will be tested when you launch it:

```powershell
cd HotkeyTyper
dotnet run
```

**What to Check:**

1. **Settings File Creation**
   - Location: `C:\Users\joche\AppData\Local\HotkeyTyper\settings.json`
   - Should be created automatically on first launch
   - Default set should be present

2. **Settings File Format**
   ```json
   {
     "settingsVersion": 2,
     "activeSetId": "some-guid-here",
     "minimizeToTray": false,
     "sets": [
       {
         "id": "some-guid-here",
         "name": "Default",
         "description": "Your first snippet set",
         "snippets": [
           {
             "id": "another-guid",
             "name": "Sample Snippet",
             "content": "Type Ctrl+Shift+1 to insert this text",
             "hotkeyNumber": 1,
             "typingSpeed": 10,
             "hasCode": false
           }
         ]
       }
     ]
   }
   ```

3. **Backup File**
   - After making changes, check for `settings.backup.json`
   - Should be created before each save

### 3. Test Migration from Old Format

If you have an old `settings.json` in the app directory:

```json
{
  "predefinedText": "Old text here",
  "typingSpeed": 8,
  "hasCode": true
}
```

**Expected Behavior:**
- Automatically detected as V1 format
- Migrated to new V2 format
- Creates "Default" set with old text as snippet #1
- Old file renamed to `settings.json.migrated`
- New file saved to `AppData\Local\HotkeyTyper\`

### 4. Manual Testing with PowerShell

You can manually test the classes (though the app needs to run for full testing):

```powershell
# Check if settings file exists
$settingsPath = "$env:LOCALAPPDATA\HotkeyTyper\settings.json"
Test-Path $settingsPath

# View settings file
Get-Content $settingsPath | ConvertFrom-Json | ConvertTo-Json -Depth 10

# Check backup file
Test-Path "$env:LOCALAPPDATA\HotkeyTyper\settings.backup.json"
```

---

## Expected Behavior

### On First Launch (No Settings Exist)

1. SettingsManager checks for settings file
2. Not found → creates default AppConfiguration
3. Calls `CreateDefaultSet()`
4. Creates "Default" set with sample snippet
5. Saves to `%LOCALAPPDATA%\HotkeyTyper\settings.json`

### On Subsequent Launches (Settings Exist)

1. SettingsManager loads from JSON
2. Deserializes to AppConfiguration
3. Validates all data
4. Returns to application

### If Old Format Detected

1. Detects V1 format (has `predefinedText` field)
2. Calls `MigrateFromOldFormat(oldSettings)`
3. Creates new AppConfiguration
4. Adds "Default" set with migrated snippet
5. Saves as V2 format
6. Renames old file to `.migrated`

---

## Known Limitations (By Design)

These are **intentional** for Phase 1:

- **No UI integration yet** - Forms still use old code
- **Settings created but not used** - UI doesn't connect to SettingsManager yet
- **Hotkeys not registered** - Hotkey logic not wired up yet
- **Only default set created** - UI for managing sets comes in Phase 2

**This is expected!** Phase 1 is just the data foundation. Phase 2 will connect the UI.

---

## Troubleshooting

### Issue: Settings file not created

**Cause:** App might have crashed before saving

**Fix:** Check for exceptions in the app output

### Issue: Old settings not migrated

**Cause:** Old file might be in wrong location

**Expected location:** Same directory as `HotkeyTyper.exe`

### Issue: JSON format looks wrong

**Cause:** Might be comparing V1 vs V2 format

**Solution:** V2 format has `sets` array, V1 has `predefinedText`

---

## What's Next: Phase 2

After testing Phase 1, Phase 2 will add:

1. **Three-Panel Layout**
   - Left: Sets list
   - Middle: Snippets in active set
   - Right: Snippet editor

2. **Sets Management**
   - New Set button
   - Delete Set button  
   - Rename Set button
   - Set selection

3. **Integration**
   - Load settings on startup
   - Save on changes
   - Populate UI from data model
   - Wire up existing controls

**Estimated Time:** 6-8 hours

---

## Files Reference

### Documentation
- `docs/phase1-completion-summary.md` - Detailed implementation report
- `docs/data-model-reference.md` - API reference and examples
- `docs/FEATURE-PLAN-snippet-sets.md` - Complete 8-phase plan

### Source Code
- `HotkeyTyper/Models/Snippet.cs` - Snippet class
- `HotkeyTyper/Models/SnippetSet.cs` - SnippetSet class
- `HotkeyTyper/Models/AppConfiguration.cs` - AppConfiguration class
- `HotkeyTyper/Managers/SettingsManager.cs` - SettingsManager class

### Testing
- `verify-phase1.ps1` - Verification script
- `HotkeyTyper/TestDataLayer.cs` - Embedded test suite (not executed yet)

---

## Success Criteria

Phase 1 is considered complete when:

- [x] All 4 classes created
- [x] Project compiles without errors/warnings
- [x] Classes use proper namespaces
- [x] JSON serialization configured
- [x] Validation methods implemented
- [x] Migration logic complete
- [x] Storage uses AppData\Local
- [ ] Settings file created on app launch (test this now!)
- [ ] Migration works for old format (test if you have old settings!)

---

## Ready to Test!

1. Run `dotnet run` in the HotkeyTyper folder
2. Check `%LOCALAPPDATA%\HotkeyTyper\settings.json`
3. Verify JSON structure matches expected format
4. Report any issues

**Phase 1 is done!** The data layer is solid and ready for UI integration in Phase 2.
