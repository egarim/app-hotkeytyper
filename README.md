# Hotkey Typer

A powerful Windows Forms utility that manages multiple snippet sets and types predefined text into any application using global hotkeys. Perfect for demos, tutorials, customer support, development workflows, or any scenario where you need to reproduce text quickly and naturally.

## Key Features

### Multi-Set Organization
- **Tabbed Interface**: Organize snippets into multiple sets (e.g., "Customer Support", "Development", "Demos")
- **Easy Set Management**: Create, rename, and delete snippet sets with simple toolbar buttons
- **Default Sample**: Each new set includes a sample snippet to get you started
- **Persistent Storage**: All sets and snippets are automatically saved to `settings.json`

### Snippet Management
- **9 Hotkeys Per Set**: Each set supports up to 9 snippets (CTRL+SHIFT+1 through CTRL+SHIFT+9)
- **Individual Typing Speeds**: Configure typing speed (1-10) per snippet for realistic playback
- **Code-Aware Mode**: Enable "Has Code" to limit maximum speed for accurate code reproduction
- **File Source Support**: Load snippet content from external files (auto-updates on typing)
- **Rich Preview**: Wide preview column shows snippet content at a glance
- **Auto-Save**: Optionally enable auto-save to persist changes immediately without clicking Save

### User Experience
- **Customizable UI**: Adjust UI scale (75%-150%) and font sizes for your display
- **Human-Like Typing**: Randomized delays and contextual pauses for natural-looking text entry
- **System Tray Integration**: Minimize to tray for unobtrusive background operation
- **Reset to Default**: Safely clear all data and restore default configuration
- **Custom Icons**: Runtime-generated icons with first-run export to `HotkeyTyper.ico`

### Special Character Support
- **Automatic Escaping**: Safely type source code with parentheses, braces, and special characters
- **Tab & Newline Support**: Real Tab and Enter key presses for proper formatting
- **SendKeys Compatible**: Handles `+ ^ % ~ ( ) { }` without manual escaping

## Requirements

- Windows 10 or later
- .NET SDK (10.0 or later)

## Installation

Clone, build, and run (PowerShell):

```powershell
git clone https://github.com/jamesmontemagno/app-hotkeytyper.git
cd app-hotkeytyper/HotkeyTyper
dotnet build
dotnet run
```

## Usage

### Getting Started
1. **Launch the app** - The default set includes a sample snippet
2. **Create snippet sets** - Click "+ New Set" to organize snippets by category
3. **Add snippets** - Click "+ New" to create snippets with custom content and hotkeys
4. **Configure settings** - Click ⚙️ Settings to adjust UI scale, fonts, and auto-save

### Working with Snippets
1. **Select a snippet** from the list to edit it
2. **Edit the content** in the text area
3. **Adjust typing speed** using the slider (1=slowest, 10=fastest)
4. **Enable "Has Code"** to limit speed for accurate code typing
5. **Use "Use File"** to load content from an external file
6. **Save changes** - Click "💾 Save" or enable auto-save in settings

### Using Hotkeys
- **CTRL+SHIFT+1-9**: Type the corresponding snippet from the active set
- Hotkeys work globally - switch to any application and press the hotkey
- Each snippet set has its own hotkey mapping (1-9)
- Only the active (selected) set's hotkeys are functional

### Managing Sets
- **Create**: Click "+ New Set" and enter a name
- **Rename**: Click "Rename" while viewing a set
- **Delete**: Click "Delete" to remove the current set (confirmation required)
- **Switch**: Click any tab to make that set active

### Settings & Customization
- **UI Scale**: Adjust from 75% to 150% for your display DPI
- **Font Sizes**: Customize UI and content font sizes independently
- **Auto-Save**: Enable to save snippet changes automatically
- **Reset All Data**: Clear all sets and restore default configuration (double confirmation)

### System Tray
- Click "Minimize to Tray" to run in the background
- Right-click tray icon for quick access to Show/Hide/Exit
- Hotkeys remain active while minimized
- The app restores to its previous state when shown

## Notes & Troubleshooting

### Security & Permissions
- Some applications or elevated processes may block automated input
- Run Hotkey Typer with matching elevation if needed
- Security tools may flag keystroke simulation - only run in trusted environments

### Icons & Customization
- Tray and taskbar icons are generated at runtime (see `IconFactory.cs`)
- First run exports `HotkeyTyper.ico` for customization
- To use a custom icon, add an .ico file and set `<ApplicationIcon>` in the .csproj

### Performance Tips
- Use "Has Code" mode for code snippets to ensure accuracy
- Lower typing speeds (1-5) are more reliable for complex text
- For very long snippets, consider using "Use File" mode
- Wide preview column shows more content - resize columns as needed

## Technical Details

### Typing Mechanism
Hotkey Typer uses `SendKeys` with automatic escaping for special characters. The following are handled automatically:
- **Special Keys**: `+ ^ % ~ ( ) { }`
- **Tabs**: Converted to real Tab key presses
- **Newlines**: Converted to Enter key presses
- **Source Code**: Parentheses, braces, and operators are properly escaped

### Reliability Features
- **Contextual Pauses**: Extra delays after punctuation for better reliability
- **Minimum Delays**: Enforced minimum timing at high speeds (8-10)
- **Code Mode**: Limits maximum speed when "Has Code" is enabled
- **Character Grouping**: Intelligent batching for optimal performance

### Data Storage
All configuration is stored in `settings.json` including:
- Snippet sets and their snippets
- UI preferences (scale, fonts, auto-save)
- Active set selection
- Window position and size

## Repository

https://github.com/jamesmontemagno/app-hotkeytyper

## Changelog

### Version 2.0 (Current)
- ✨ Multi-set organization with tabbed interface
- ✨ 9 hotkeys per set (CTRL+SHIFT+1-9)
- ✨ Individual typing speed per snippet
- ✨ Auto-save option for snippets
- ✨ Customizable UI scale and fonts
- ✨ Reset to default functionality
- ✨ Wide preview column for better content visibility
- ✨ Default sample snippet in new sets
- 🐛 Fixed settings persistence across restarts
- 🐛 Fixed auto-save grid updates
- 🐛 Fixed button state management

### Version 1.0
- 🎉 Initial release
- Single snippet with CTRL+SHIFT+1 hotkey
- Basic typing with speed control
- System tray integration
- Settings persistence
