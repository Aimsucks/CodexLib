using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace CodexLib;

public class Codex
{
    internal static string PluginName = null!;

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; } = null!;

    /// <summary>
    /// Initializes the Codex API connector with a plugin name. Needs to connect to the primary plugin's interface to send exceptions to the log.
    /// </summary>
    /// <param name="pluginInterface">IDalamudPluginInterface of primary plugin</param>
    /// <param name="pluginName">Plugin name string as it appears on Codex, does not need to be URL encoded</param>
    public static void Initialize(IDalamudPluginInterface pluginInterface, string pluginName)
    {
        pluginInterface.Create<Codex>();
        PluginName = pluginName;
    }

    /// <summary>
    /// Dispose function that should be called when unloading the plugin.
    /// </summary>
    public static void Dispose()
    {
        CodexAPI.Dispose();
    }
}
