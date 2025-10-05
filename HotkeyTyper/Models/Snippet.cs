using System;
using System.Text.Json.Serialization;

namespace HotkeyTyper.Models;

/// <summary>
/// Represents an individual text snippet/entry within a snippet set.
/// Each snippet has content, a hotkey assignment (1-9), and individual settings.
/// </summary>
public class Snippet
{
    /// <summary>
    /// Unique identifier for this snippet (GUID)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Display name for the snippet (e.g., "Email Signature", "Code Template")
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The actual text content to be typed
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Hotkey number (1-9) for CTRL+SHIFT+1 through CTRL+SHIFT+9
    /// Must be unique within the parent SnippetSet
    /// </summary>
    [JsonPropertyName("hotkeyNumber")]
    public int HotkeyNumber { get; set; } = 1;

    /// <summary>
    /// Typing speed (1-10, where 1=slowest, 10=fastest)
    /// </summary>
    [JsonPropertyName("typingSpeed")]
    public int TypingSpeed { get; set; } = 5;

    /// <summary>
    /// Whether this snippet contains code (enables slower, more careful typing)
    /// </summary>
    [JsonPropertyName("hasCode")]
    public bool HasCode { get; set; } = false;

    /// <summary>
    /// Whether to load content from a file instead of using Content property
    /// </summary>
    [JsonPropertyName("useFile")]
    public bool UseFile { get; set; } = false;

    /// <summary>
    /// Path to the file containing the content (when UseFile is true)
    /// </summary>
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// When this snippet was created
    /// </summary>
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// When this snippet was last modified
    /// </summary>
    [JsonPropertyName("modifiedDate")]
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets a display string for the hotkey (e.g., "CTRL+SHIFT+1")
    /// </summary>
    public string GetHotkeyDisplay()
    {
        return $"CTRL+SHIFT+{HotkeyNumber}";
    }

    /// <summary>
    /// Gets a preview of the content (first N characters, with line breaks removed)
    /// </summary>
    /// <param name="maxLength">Maximum length of the preview</param>
    /// <returns>Content preview string</returns>
    public string GetContentPreview(int maxLength = 50)
    {
        if (string.IsNullOrEmpty(Content))
            return "(empty)";

        // Replace line breaks and carriage returns with spaces
        var preview = Content.Replace("\n", " ").Replace("\r", "").Trim();

        // Truncate if too long
        if (preview.Length <= maxLength)
            return preview;

        return preview.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// Updates the ModifiedDate to current time
    /// </summary>
    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.Now;
    }

    /// <summary>
    /// Creates a copy of this snippet with a new ID
    /// </summary>
    /// <param name="newName">Optional new name for the copy</param>
    /// <returns>A new Snippet instance</returns>
    public Snippet Clone(string? newName = null)
    {
        return new Snippet
        {
            Id = Guid.NewGuid().ToString(),
            Name = newName ?? $"{Name} - Copy",
            Content = Content,
            HotkeyNumber = HotkeyNumber,
            TypingSpeed = TypingSpeed,
            HasCode = HasCode,
            UseFile = UseFile,
            FilePath = FilePath,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }

    /// <summary>
    /// Validates the snippet's data
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            errorMessage = "Snippet name cannot be empty";
            return false;
        }

        if (Name.Length > 100)
        {
            errorMessage = "Snippet name cannot exceed 100 characters";
            return false;
        }

        if (HotkeyNumber < 1 || HotkeyNumber > 9)
        {
            errorMessage = "Hotkey number must be between 1 and 9";
            return false;
        }

        if (TypingSpeed < 1 || TypingSpeed > 10)
        {
            errorMessage = "Typing speed must be between 1 and 10";
            return false;
        }

        if (UseFile && string.IsNullOrWhiteSpace(FilePath))
        {
            errorMessage = "File path is required when 'Use File' is enabled";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    public override string ToString()
    {
        return $"[{HotkeyNumber}] {Name}";
    }
}
