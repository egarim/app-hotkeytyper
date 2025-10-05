using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HotkeyTyper.Models;

/// <summary>
/// Top-level application settings container.
/// Contains all snippet sets and application preferences.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Settings file version for migration purposes
    /// Version 1: Old single-text format
    /// Version 2: New sets-based format
    /// </summary>
    [JsonPropertyName("settingsVersion")]
    public int SettingsVersion { get; set; } = 2;

    /// <summary>
    /// Collection of all snippet sets
    /// </summary>
    [JsonPropertyName("sets")]
    public List<SnippetSet> Sets { get; set; } = new List<SnippetSet>();

    /// <summary>
    /// ID of the currently active set (only this set's hotkeys are registered)
    /// </summary>
    [JsonPropertyName("activeSetId")]
    public string ActiveSetId { get; set; } = string.Empty;

    /// <summary>
    /// Whether to minimize the app to system tray instead of taskbar
    /// </summary>
    [JsonPropertyName("minimizeToTray")]
    public bool MinimizeToTray { get; set; } = false;

    /// <summary>
    /// UI preferences and display settings
    /// </summary>
    [JsonPropertyName("uiSettings")]
    public UISettings UISettings { get; set; } = new UISettings();

    /// <summary>
    /// Gets the currently active snippet set
    /// </summary>
    /// <returns>The active SnippetSet, or null if not found</returns>
    public SnippetSet? GetActiveSet()
    {
        if (string.IsNullOrEmpty(ActiveSetId))
            return Sets.FirstOrDefault();

        return Sets.FirstOrDefault(s => s.Id == ActiveSetId);
    }

    /// <summary>
    /// Sets a set as the active one
    /// </summary>
    /// <param name="setId">ID of the set to activate</param>
    /// <returns>True if set was activated, false if set not found</returns>
    public bool SetActiveSet(string setId)
    {
        if (Sets.Any(s => s.Id == setId))
        {
            ActiveSetId = setId;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a set by its ID
    /// </summary>
    /// <param name="setId">ID of the set to find</param>
    /// <returns>The SnippetSet, or null if not found</returns>
    public SnippetSet? GetSetById(string setId)
    {
        return Sets.FirstOrDefault(s => s.Id == setId);
    }

    /// <summary>
    /// Gets a set by its name
    /// </summary>
    /// <param name="name">Name of the set to find</param>
    /// <returns>The SnippetSet, or null if not found</returns>
    public SnippetSet? GetSetByName(string name)
    {
        return Sets.FirstOrDefault(s => 
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
        );
    }

    /// <summary>
    /// Checks if a set name is already in use
    /// </summary>
    /// <param name="name">Name to check</param>
    /// <param name="excludeSetId">Optional set ID to exclude from check (for renaming)</param>
    /// <returns>True if name is already used, false otherwise</returns>
    public bool IsSetNameTaken(string name, string? excludeSetId = null)
    {
        return Sets.Any(s => 
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && 
            s.Id != excludeSetId
        );
    }

    /// <summary>
    /// Adds a new set to the collection
    /// </summary>
    /// <param name="set">The set to add</param>
    /// <param name="setAsActive">Whether to make this the active set</param>
    /// <returns>True if added successfully</returns>
    public bool AddSet(SnippetSet set, bool setAsActive = false)
    {
        if (IsSetNameTaken(set.Name, set.Id))
            return false;

        Sets.Add(set);

        // If this is the first set or setAsActive is true, make it active
        if (Sets.Count == 1 || setAsActive)
        {
            ActiveSetId = set.Id;
        }

        return true;
    }

    /// <summary>
    /// Removes a set from the collection
    /// </summary>
    /// <param name="setId">ID of the set to remove</param>
    /// <returns>True if removed, false if not found or it's the last set</returns>
    public bool RemoveSet(string setId)
    {
        // Cannot remove the last set
        if (Sets.Count <= 1)
            return false;

        var set = GetSetById(setId);
        if (set == null)
            return false;

        Sets.Remove(set);

        // If we removed the active set, activate the first remaining set
        if (ActiveSetId == setId)
        {
            ActiveSetId = Sets[0].Id;
        }

        return true;
    }

    /// <summary>
    /// Migrates settings from old single-text format (Version 1) to new format (Version 2)
    /// </summary>
    /// <param name="oldSettings">The old AppSettings object from Form1.cs</param>
    public void MigrateFromOldFormat(HotkeyTyper.AppSettings oldSettings)
    {
        // Only migrate if we're on an old version and have no sets yet
        if (SettingsVersion >= 2 || Sets.Count > 0)
            return;

        if (string.IsNullOrEmpty(oldSettings.PredefinedText))
        {
            // Create empty default set
            CreateDefaultSet();
        }
        else
        {
            // Create a "Default" set with the old text as a snippet
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
                HasCode = oldSettings.HasCode,
                UseFile = oldSettings.UseFileSource,
                FilePath = oldSettings.FileSourcePath,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            });

            Sets.Add(defaultSet);
            ActiveSetId = defaultSet.Id;
        }

        SettingsVersion = 2;
    }

    /// <summary>
    /// Creates a default set with a sample snippet
    /// Used for first-time initialization
    /// </summary>
    public void CreateDefaultSet()
    {
        if (Sets.Count > 0)
            return;

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

        Sets.Add(defaultSet);
        ActiveSetId = defaultSet.Id;
    }

    /// <summary>
    /// Ensures settings are in a valid state
    /// Creates default set if needed, sets active set if missing, etc.
    /// </summary>
    public void EnsureValid()
    {
        // Ensure we have at least one set
        if (Sets.Count == 0)
        {
            CreateDefaultSet();
        }

        // Ensure we have an active set
        if (string.IsNullOrEmpty(ActiveSetId) || GetActiveSet() == null)
        {
            ActiveSetId = Sets[0].Id;
        }

        // Validate all sets
        foreach (var set in Sets.ToList())
        {
            if (!set.IsValid(out _))
            {
                // Remove invalid sets (should rarely happen)
                Sets.Remove(set);
            }
        }

        // Re-check after removing invalid sets
        if (Sets.Count == 0)
        {
            CreateDefaultSet();
        }
    }

    /// <summary>
    /// Gets the total number of snippets across all sets
    /// </summary>
    public int GetTotalSnippetCount()
    {
        return Sets.Sum(s => s.GetSnippetCount());
    }

    /// <summary>
    /// Validates the settings
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (Sets.Count == 0)
        {
            errorMessage = "At least one set is required";
            return false;
        }

        if (string.IsNullOrEmpty(ActiveSetId))
        {
            errorMessage = "Active set ID cannot be empty";
            return false;
        }

        if (GetActiveSet() == null)
        {
            errorMessage = "Active set not found";
            return false;
        }

        // Check for duplicate set names
        var duplicates = Sets.GroupBy(s => s.Name.ToLower())
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key);

        if (duplicates.Any())
        {
            errorMessage = $"Duplicate set names found: {string.Join(", ", duplicates)}";
            return false;
        }

        // Validate all sets
        foreach (var set in Sets)
        {
            if (!set.IsValid(out string setError))
            {
                errorMessage = $"Set '{set.Name}': {setError}";
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }
}
