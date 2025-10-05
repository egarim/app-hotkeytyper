using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HotkeyTyper.Models;

/// <summary>
/// Represents a collection/set of related snippets.
/// Each set can contain up to 9 snippets with hotkeys 1-9.
/// Only one set can be active at a time.
/// </summary>
public class SnippetSet
{
    /// <summary>
    /// Unique identifier for this set (GUID)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Display name for the set (e.g., "Demo1", "Readme Notes", "Email Templates")
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of what this set contains
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Collection of snippets in this set
    /// </summary>
    [JsonPropertyName("snippets")]
    public List<Snippet> Snippets { get; set; } = new List<Snippet>();

    /// <summary>
    /// When this set was created
    /// </summary>
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// When this set was last modified
    /// </summary>
    [JsonPropertyName("modifiedDate")]
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Optional color for visual customization (hex format, e.g., "#3498db")
    /// </summary>
    [JsonPropertyName("colorHex")]
    public string ColorHex { get; set; } = "#3498db";

    /// <summary>
    /// Optional icon name for visual customization
    /// </summary>
    [JsonPropertyName("iconName")]
    public string IconName { get; set; } = "folder";

    /// <summary>
    /// Gets a snippet by its hotkey number
    /// </summary>
    /// <param name="hotkeyNumber">The hotkey number (1-9)</param>
    /// <returns>The snippet with that hotkey, or null if not found</returns>
    public Snippet? GetSnippetByHotkey(int hotkeyNumber)
    {
        return Snippets.FirstOrDefault(s => s.HotkeyNumber == hotkeyNumber);
    }

    /// <summary>
    /// Gets a list of available (unassigned) hotkey numbers in this set
    /// </summary>
    /// <returns>List of integers 1-9 that are not currently assigned</returns>
    public List<int> GetAvailableHotkeys()
    {
        var assigned = Snippets.Select(s => s.HotkeyNumber).ToHashSet();
        return Enumerable.Range(1, 9).Where(n => !assigned.Contains(n)).ToList();
    }

    /// <summary>
    /// Checks if a hotkey number is already assigned to another snippet
    /// </summary>
    /// <param name="hotkeyNumber">The hotkey number to check</param>
    /// <param name="excludeSnippetId">Optional snippet ID to exclude from the check (for editing existing snippets)</param>
    /// <returns>True if there's a conflict, false otherwise</returns>
    public bool HasHotkeyConflict(int hotkeyNumber, string? excludeSnippetId = null)
    {
        return Snippets.Any(s => 
            s.HotkeyNumber == hotkeyNumber && 
            s.Id != excludeSnippetId
        );
    }

    /// <summary>
    /// Gets the first available hotkey number
    /// </summary>
    /// <returns>First available hotkey (1-9), or 0 if all are assigned</returns>
    public int GetNextAvailableHotkey()
    {
        var available = GetAvailableHotkeys();
        return available.Count > 0 ? available[0] : 0;
    }

    /// <summary>
    /// Gets the number of snippets in this set
    /// </summary>
    /// <returns>Count of snippets</returns>
    public int GetSnippetCount()
    {
        return Snippets.Count;
    }

    /// <summary>
    /// Updates the ModifiedDate to current time
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.Now;
    }

    /// <summary>
    /// Adds a snippet to this set
    /// </summary>
    /// <param name="snippet">The snippet to add</param>
    /// <returns>True if added successfully, false if hotkey conflict</returns>
    public bool AddSnippet(Snippet snippet)
    {
        if (HasHotkeyConflict(snippet.HotkeyNumber, snippet.Id))
        {
            return false;
        }

        Snippets.Add(snippet);
        UpdateModifiedDate();
        return true;
    }

    /// <summary>
    /// Removes a snippet from this set
    /// </summary>
    /// <param name="snippetId">ID of the snippet to remove</param>
    /// <returns>True if removed, false if not found</returns>
    public bool RemoveSnippet(string snippetId)
    {
        var snippet = Snippets.FirstOrDefault(s => s.Id == snippetId);
        if (snippet == null)
            return false;

        Snippets.Remove(snippet);
        UpdateModifiedDate();
        return true;
    }

    /// <summary>
    /// Creates a copy of this set with new IDs
    /// </summary>
    /// <param name="newName">Optional new name for the copy</param>
    /// <returns>A new SnippetSet instance</returns>
    public SnippetSet Clone(string? newName = null)
    {
        var clonedSet = new SnippetSet
        {
            Id = Guid.NewGuid().ToString(),
            Name = newName ?? $"{Name} - Copy",
            Description = Description,
            ColorHex = ColorHex,
            IconName = IconName,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        // Clone all snippets
        foreach (var snippet in Snippets)
        {
            clonedSet.Snippets.Add(snippet.Clone());
        }

        return clonedSet;
    }

    /// <summary>
    /// Validates the set's data
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            errorMessage = "Set name cannot be empty";
            return false;
        }

        if (Name.Length > 100)
        {
            errorMessage = "Set name cannot exceed 100 characters";
            return false;
        }

        if (Description.Length > 500)
        {
            errorMessage = "Description cannot exceed 500 characters";
            return false;
        }

        if (Snippets.Count > 9)
        {
            errorMessage = "A set cannot contain more than 9 snippets";
            return false;
        }

        // Check for hotkey conflicts
        var hotkeyGroups = Snippets.GroupBy(s => s.HotkeyNumber);
        foreach (var group in hotkeyGroups)
        {
            if (group.Count() > 1)
            {
                errorMessage = $"Multiple snippets assigned to hotkey {group.Key}";
                return false;
            }
        }

        // Validate each snippet
        foreach (var snippet in Snippets)
        {
            if (!snippet.IsValid(out string snippetError))
            {
                errorMessage = $"Snippet '{snippet.Name}': {snippetError}";
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }

    public override string ToString()
    {
        return $"{Name} ({GetSnippetCount()} snippets)";
    }
}
