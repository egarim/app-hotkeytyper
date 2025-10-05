using System;
using System.IO;
using System.Text.Json;
using HotkeyTyper.Models;

namespace HotkeyTyper.Managers;

/// <summary>
/// Manages loading and saving application settings to/from JSON files.
/// Handles migration from old format and automatic backups.
/// </summary>
public class SettingsManager
{
    private readonly string settingsFolder;
    private readonly string settingsFilePath;
    private readonly string backupFilePath;

    /// <summary>
    /// JSON serialization options for pretty-printed, indented output
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public SettingsManager()
    {
        // Use AppData\Local for storing settings (Windows best practice)
        settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HotkeyTyper"
        );

        settingsFilePath = Path.Combine(settingsFolder, "settings.json");
        backupFilePath = Path.Combine(settingsFolder, "settings.backup.json");

        // Ensure the folder exists
        EnsureSettingsFolderExists();
    }

    /// <summary>
    /// Creates the settings folder if it doesn't exist
    /// </summary>
    private void EnsureSettingsFolderExists()
    {
        if (!Directory.Exists(settingsFolder))
        {
            Directory.CreateDirectory(settingsFolder);
        }
    }

    /// <summary>
    /// Loads settings from the settings file.
    /// If file doesn't exist or is invalid, returns default settings.
    /// Handles migration from old format automatically.
    /// </summary>
    /// <returns>Loaded or default AppConfiguration</returns>
    public AppConfiguration LoadSettings()
    {
        try
        {
            // Check if settings file exists
            if (!File.Exists(settingsFilePath))
            {
                // Try to migrate from old location (application directory)
                var migrated = TryMigrateFromOldLocation();
                if (migrated != null)
                    return migrated;

                // No settings found, return defaults
                var defaultSettings = new AppConfiguration();
                defaultSettings.CreateDefaultSet();
                return defaultSettings;
            }

            // Read and parse the JSON file
            string json = File.ReadAllText(settingsFilePath);
            
            // First, try to deserialize as new format
            try
            {
                var settings = JsonSerializer.Deserialize<AppConfiguration>(json, JsonOptions);
                
                if (settings == null)
                {
                    return CreateDefaultSettings();
                }

                // Check if migration from old format is needed
                if (settings.SettingsVersion < 2)
                {
                    MigrateFromV1(settings, json);
                }

                // Ensure settings are valid
                settings.EnsureValid();

                return settings;
            }
            catch (JsonException)
            {
                // Failed to parse as new format, try old format
                return TryParseOldFormat(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
            return CreateDefaultSettings();
        }
    }

    /// <summary>
    /// Saves settings to the settings file.
    /// Creates a backup of the previous settings before saving.
    /// </summary>
    /// <param name="settings">The settings to save</param>
    /// <returns>True if saved successfully, false otherwise</returns>
    public bool SaveSettings(AppConfiguration settings)
    {
        try
        {
            // Validate before saving
            if (!settings.IsValid(out string errorMessage))
            {
                Console.WriteLine($"Cannot save invalid settings: {errorMessage}");
                return false;
            }

            // Create backup of existing settings
            if (File.Exists(settingsFilePath))
            {
                File.Copy(settingsFilePath, backupFilePath, overwrite: true);
            }

            // Serialize to JSON
            string json = JsonSerializer.Serialize(settings, JsonOptions);

            // Write to file
            File.WriteAllText(settingsFilePath, json);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            
            // Try to restore from backup if save failed
            if (File.Exists(backupFilePath))
            {
                try
                {
                    File.Copy(backupFilePath, settingsFilePath, overwrite: true);
                }
                catch
                {
                    // Ignore backup restore errors
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Attempts to migrate settings from the old location (application directory)
    /// </summary>
    /// <returns>Migrated settings, or null if not found</returns>
    private AppConfiguration? TryMigrateFromOldLocation()
    {
        try
        {
            // Old location: same directory as the executable
            string oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

            if (!File.Exists(oldPath))
                return null;

            Console.WriteLine($"Found settings in old location: {oldPath}");
            Console.WriteLine("Migrating to new location...");

            // Read old settings
            string json = File.ReadAllText(oldPath);
            
            // Try to parse
            var settings = TryParseOldFormat(json);

            if (settings != null)
            {
                // Save to new location
                SaveSettings(settings);

                // Rename old file to .migrated
                File.Move(oldPath, oldPath + ".migrated");

                Console.WriteLine("Migration successful!");
                return settings;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error migrating from old location: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Migrates settings from Version 1 (old single-text format) to Version 2
    /// </summary>
    /// <param name="settings">The settings object to update</param>
    /// <param name="originalJson">The original JSON string</param>
    private void MigrateFromV1(AppConfiguration settings, string originalJson)
    {
        try
        {
            Console.WriteLine("Migrating settings from Version 1 to Version 2...");

            // Try to parse old AppSettings format from JSON
            var oldSettings = JsonSerializer.Deserialize<AppSettings>(originalJson, JsonOptions);

            if (oldSettings != null)
            {
                // Use the new migration method
                settings.MigrateFromOldFormat(oldSettings);

                // Save the migrated settings
                SaveSettings(settings);

                Console.WriteLine("Migration complete!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            // Ensure we have at least a default set
            settings.EnsureValid();
        }
    }

    /// <summary>
    /// Attempts to parse JSON as old format (Version 1) and migrate
    /// </summary>
    /// <param name="json">The JSON string to parse</param>
    /// <returns>Migrated AppConfiguration, or default settings if parsing fails</returns>
    private AppConfiguration TryParseOldFormat(string json)
    {
        try
        {
            // Try to parse as old AppSettings format
            var oldSettings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);

            if (oldSettings != null)
            {
                Console.WriteLine("Detected old settings format, migrating...");

                var settings = new AppConfiguration();
                settings.MigrateFromOldFormat(oldSettings);

                return settings;
            }

            // Not old format, return defaults
            return CreateDefaultSettings();
        }
        catch
        {
            return CreateDefaultSettings();
        }
    }

    /// <summary>
    /// Creates default settings with a sample set and snippet
    /// </summary>
    /// <returns>Default AppConfiguration</returns>
    private AppConfiguration CreateDefaultSettings()
    {
        var settings = new AppConfiguration();
        settings.CreateDefaultSet();
        return settings;
    }

    /// <summary>
    /// Gets the path to the settings folder
    /// </summary>
    public string GetSettingsFolder()
    {
        return settingsFolder;
    }

    /// <summary>
    /// Gets the path to the settings file
    /// </summary>
    public string GetSettingsFilePath()
    {
        return settingsFilePath;
    }

    /// <summary>
    /// Exports settings to a specified file path
    /// </summary>
    /// <param name="settings">The settings to export</param>
    /// <param name="exportPath">Full path to export file</param>
    /// <returns>True if exported successfully</returns>
    public bool ExportSettings(AppConfiguration settings, string exportPath)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(exportPath, json);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting settings: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Imports settings from a specified file path
    /// </summary>
    /// <param name="importPath">Full path to import file</param>
    /// <returns>Imported settings, or null if failed</returns>
    public AppConfiguration? ImportSettings(string importPath)
    {
        try
        {
            if (!File.Exists(importPath))
                return null;

            string json = File.ReadAllText(importPath);
            var settings = JsonSerializer.Deserialize<AppConfiguration>(json, JsonOptions);

            if (settings != null)
            {
                settings.EnsureValid();
            }

            return settings;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing settings: {ex.Message}");
            return null;
        }
    }
}
