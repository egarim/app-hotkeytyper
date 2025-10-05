using System;
using HotkeyTyper.Models;
using HotkeyTyper.Managers;

namespace HotkeyTyper;

/// <summary>
/// Simple test class to verify data layer functionality
/// Run this to test: AppConfiguration, SnippetSet, Snippet, SettingsManager
/// </summary>
public static class DataLayerTest
{
    public static void RunTests()
    {
        Console.WriteLine("=== HotkeyTyper Data Layer Tests ===\n");

        TestSnippetCreation();
        TestSnippetSetCreation();
        TestAppConfigurationCreation();
        TestSettingsManagerSaveLoad();
        TestMigrationFromOldFormat();

        Console.WriteLine("\n=== All Tests Complete ===");
    }

    private static void TestSnippetCreation()
    {
        Console.WriteLine("Test 1: Snippet Creation");
        
        var snippet = new Snippet
        {
            Name = "Test Snippet",
            Content = "This is a test snippet\nwith multiple lines",
            HotkeyNumber = 1,
            TypingSpeed = 10,
            HasCode = false
        };

        Console.WriteLine($"  ✓ Created snippet: {snippet.Name}");
        Console.WriteLine($"  ✓ Hotkey display: {snippet.GetHotkeyDisplay()}");
        Console.WriteLine($"  ✓ Content preview: {snippet.GetContentPreview()}");
        Console.WriteLine($"  ✓ Is valid: {snippet.IsValid(out string error)}");
        
        if (!string.IsNullOrEmpty(error))
            Console.WriteLine($"  ✗ Validation error: {error}");
        
        // Test cloning
        var clone = snippet.Clone("Cloned Snippet");
        Console.WriteLine($"  ✓ Cloned snippet: {clone.Name}");
        Console.WriteLine();
    }

    private static void TestSnippetSetCreation()
    {
        Console.WriteLine("Test 2: SnippetSet Creation");
        
        var set = new SnippetSet
        {
            Name = "Test Set",
            Description = "A test snippet set",
            ColorHex = "#FF5733"
        };

        // Add some snippets
        var snippet1 = new Snippet
        {
            Name = "Snippet 1",
            Content = "First snippet",
            HotkeyNumber = 1,
            TypingSpeed = 10
        };

        var snippet2 = new Snippet
        {
            Name = "Snippet 2",
            Content = "Second snippet",
            HotkeyNumber = 2,
            TypingSpeed = 15
        };

        set.AddSnippet(snippet1);
        set.AddSnippet(snippet2);

        Console.WriteLine($"  ✓ Created set: {set.Name}");
        Console.WriteLine($"  ✓ Snippet count: {set.Snippets.Count}");
        Console.WriteLine($"  ✓ Available hotkeys: {string.Join(", ", set.GetAvailableHotkeys())}");
        Console.WriteLine($"  ✓ Has hotkey conflict for 1: {set.HasHotkeyConflict(1)}");
        Console.WriteLine($"  ✓ Has hotkey conflict for 3: {set.HasHotkeyConflict(3)}");
        Console.WriteLine($"  ✓ Is valid: {set.IsValid(out string error)}");
        
        if (!string.IsNullOrEmpty(error))
            Console.WriteLine($"  ✗ Validation error: {error}");
        
        Console.WriteLine();
    }

    private static void TestAppConfigurationCreation()
    {
        Console.WriteLine("Test 3: AppConfiguration Creation");
        
        var config = new AppConfiguration();
        config.CreateDefaultSet();

        Console.WriteLine($"  ✓ Settings version: {config.SettingsVersion}");
        Console.WriteLine($"  ✓ Sets count: {config.Sets.Count}");
        
        var activeSet = config.GetActiveSet();
        Console.WriteLine($"  ✓ Active set: {activeSet?.Name ?? "None"}");
        Console.WriteLine($"  ✓ Is valid: {config.IsValid(out string error)}");
        
        if (!string.IsNullOrEmpty(error))
            Console.WriteLine($"  ✗ Validation error: {error}");

        // Test adding a new set
        var newSet = new SnippetSet
        {
            Name = "My Custom Set",
            Description = "A custom test set"
        };

        config.AddSet(newSet);
        Console.WriteLine($"  ✓ Added new set, total: {config.Sets.Count}");
        Console.WriteLine($"  ✓ Set name taken 'Default': {config.IsSetNameTaken("Default")}");
        Console.WriteLine($"  ✓ Set name taken 'Unused': {config.IsSetNameTaken("Unused")}");
        
        Console.WriteLine();
    }

    private static void TestSettingsManagerSaveLoad()
    {
        Console.WriteLine("Test 4: SettingsManager Save/Load");
        
        var manager = new SettingsManager();
        Console.WriteLine($"  ✓ Settings folder: {manager.GetSettingsFolder()}");
        Console.WriteLine($"  ✓ Settings file: {manager.GetSettingsFilePath()}");

        // Create test configuration
        var config = new AppConfiguration();
        config.CreateDefaultSet();

        // Add a custom set with snippets
        var customSet = new SnippetSet
        {
            Name = "Test Set for Save/Load",
            Description = "Testing persistence"
        };

        customSet.AddSnippet(new Snippet
        {
            Name = "Test Snippet 1",
            Content = "Hello, World!",
            HotkeyNumber = 1,
            TypingSpeed = 10
        });

        customSet.AddSnippet(new Snippet
        {
            Name = "Test Snippet 2",
            Content = "Goodbye, World!",
            HotkeyNumber = 2,
            TypingSpeed = 15,
            HasCode = true
        });

        config.AddSet(customSet);
        config.SetActiveSet(customSet.Id);

        // Save
        bool saved = manager.SaveSettings(config);
        Console.WriteLine($"  ✓ Save successful: {saved}");

        // Load
        var loadedConfig = manager.LoadSettings();
        Console.WriteLine($"  ✓ Load successful: {loadedConfig != null}");
        
        if (loadedConfig != null)
        {
            Console.WriteLine($"  ✓ Loaded sets count: {loadedConfig.Sets.Count}");
            Console.WriteLine($"  ✓ Loaded active set: {loadedConfig.GetActiveSet()?.Name ?? "None"}");
            
            var loadedSet = loadedConfig.GetSetByName("Test Set for Save/Load");
            if (loadedSet != null)
            {
                Console.WriteLine($"  ✓ Custom set loaded with {loadedSet.Snippets.Count} snippets");
            }
            else
            {
                Console.WriteLine($"  ✗ Failed to load custom set");
            }
        }
        else
        {
            Console.WriteLine($"  ✗ Failed to load settings");
        }

        Console.WriteLine();
    }

    private static void TestMigrationFromOldFormat()
    {
        Console.WriteLine("Test 5: Migration from Old Format");
        
        // Create old format AppSettings (from Form1.cs)
        var oldSettings = new AppSettings
        {
            PredefinedText = "This is the old predefined text",
            TypingSpeed = 8,
            HasCode = true,
            UseFileSource = false,
            FileSourcePath = string.Empty
        };

        Console.WriteLine($"  ✓ Created old format: '{oldSettings.PredefinedText.Substring(0, 20)}...'");

        // Create new configuration and migrate
        var newConfig = new AppConfiguration();
        newConfig.MigrateFromOldFormat(oldSettings);

        Console.WriteLine($"  ✓ Migration complete");
        Console.WriteLine($"  ✓ New version: {newConfig.SettingsVersion}");
        Console.WriteLine($"  ✓ Sets created: {newConfig.Sets.Count}");
        
        var defaultSet = newConfig.GetActiveSet();
        if (defaultSet != null)
        {
            Console.WriteLine($"  ✓ Default set: {defaultSet.Name}");
            Console.WriteLine($"  ✓ Snippets in default set: {defaultSet.Snippets.Count}");
            
            if (defaultSet.Snippets.Count > 0)
            {
                var snippet = defaultSet.Snippets[0];
                Console.WriteLine($"  ✓ Migrated snippet name: {snippet.Name}");
                Console.WriteLine($"  ✓ Migrated content: '{snippet.Content.Substring(0, 20)}...'");
                Console.WriteLine($"  ✓ Migrated speed: {snippet.TypingSpeed}");
                Console.WriteLine($"  ✓ Migrated hasCode: {snippet.HasCode}");
            }
        }

        Console.WriteLine();
    }
}
