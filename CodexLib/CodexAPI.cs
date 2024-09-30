using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CodexLib;

public static class CodexAPI
{
    private const string BaseUrl = "https://codex.aimsucks.space/api/v1/plugins/";
    private static readonly string EscapedPluginName = Uri.EscapeDataString(Codex.PluginName);
    private static readonly string APIUrl = $"{BaseUrl}/{EscapedPluginName}";

    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// Calls the Codex API "presets" endpoint for the plugin specified in initialization. This will query the database for all the top-level categories, subcategories, and associated presets down to a recursion of 4 subcategories.
    /// </summary>
    /// <returns>Asynchronous task that returns a list of categories, subcategories, and presets from the Codex API</returns>
    public static async Task<List<CodexCategory>?> GetPresets()
    {
        try
        {
            var url = APIUrl + "/presets";
            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<CodexCategory>>();

            return data;
        }
        catch (Exception ex)
        {
            Codex.PluginLog.Error(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Calls the Codex API "updates" endpoint with a query string containing all the preset IDs specified in the function parameters for the plugin specified in initialization. This queries the database for all presets from the specified plugin and filters the list based on the provided query.
    /// </summary>
    /// <param name="presetIds">A list of preset ID integers</param>
    /// <returns>Asynchronous task that returns a list presets from the Codex API</returns>
    /// <exception cref="ArgumentException">A list of preset ID integers was not provided</exception>
    public static async Task<List<CodexPreset>> GetPresetUpdates(List<int> presetIds)
    {
        if (presetIds.Count == 0) throw new ArgumentException("At least one preset ID must be provided.");

        try
        {
            var query = string.Join(",", presetIds);
            var url = APIUrl + "/updates?query=" + query;
            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<List<CodexPreset>>();

            if (data == null || data.Count == 0) return [];

            return data;
        }
        catch (Exception ex)
        {
            Codex.PluginLog.Error(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Calls the Codex API "updates" endpoint with a query string containing all the presets specified in the function parameters for the plugin specified in initialization. This queries the database for all presets from the specified plugin and filters the list based on the provided query.
    /// </summary>
    /// <param name="presets">A list of presets containing a Metadata object</param>
    /// <typeparam name="T">IPreset interface with a Metadata object with a Version integer and an ID integer </typeparam>
    /// <returns>Asynchronous task that returns a list presets from the Codex API</returns>
    /// <exception cref="ArgumentException">A list of preset ID integers was not provided, likely because the IPresets that were passed had incorrect Metadata</exception>
    public static async Task<List<CodexPreset>> GetPresetUpdates<T>(List<T> presets) where T : IPreset
    {
        if (presets.Count == 0) throw new ArgumentException("At least one preset must be provided.");

        var presetIds = presets
                        .Where(p => p.Metadata != null)
                        .Select(p => p.Metadata!.Id)
                        .ToList();

        return await GetPresetUpdates(presetIds);
    }

    public static void Dispose()
    {
        HttpClient.Dispose();
    }
}

public class CodexCategory
{
    public required string Name { get; set; }
    public List<CodexCategory>? Subcategories { get; set; }
    public List<CodexPreset>? Presets { get; set; }
}

public class CodexPreset
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required int Version { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public required string Data { get; set; }
}

public class PresetMetadata
{
    public int Id { get; set; }
    public int Version { get; set; }
}

public interface IPreset
{
    public PresetMetadata? Metadata { get; set; }
    public string Name { get; set; }
}
