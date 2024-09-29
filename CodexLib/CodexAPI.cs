﻿using System;
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

    // Reusable HTTP client
    private static readonly HttpClient HttpClient = new();

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

    /*
     * In order to use this submodule, you will need to implement the interface IPreset in your config file and
     * assign it to your preset's class.
     */

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
