using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace CodexLib;

public class Codex
{
    internal static string PluginName = null!;

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; } = null!;

    public static void Initialize(IDalamudPluginInterface pluginInterface, string pluginName)
    {
        pluginInterface.Create<Codex>();
        PluginName = pluginName;
    }

    public static void Dispose()
    {
        CodexAPI.Dispose();
    }
}
