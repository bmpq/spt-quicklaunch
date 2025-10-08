using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using quicklaunch;

namespace tarkin.quicklaunch
{
    [BepInPlugin("com.tarkin.quicklaunch", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Log;

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<bool> MinimizeOnGameLaunch;
        internal static ConfigEntry<bool> ForceFullscreenOnRaidStart;
        internal static ConfigEntry<MapId> Map;
        internal static ConfigEntry<EFT.Bots.EBotAmount> AIAmount;

        internal static ConfigEntry<bool> SkipLoadingScenesEnabled;
        internal static ConfigEntry<string> SkipLoadingScenes;

        internal enum MapId
        {
            hideout,
            factory4_day,
            factory4_night,
            bigmap,
            RezervBase,
            Woods,
            Shoreline,
            Interchange,
            laboratory,
            Lighthouse,
            TarkovStreets,
            Sandbox
        }

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();

            new Patch_ScenesPreset_ScenesResourceKeys().Enable();

            new PatchMinimize().Enable();
            new PatchMinimize2().Enable();
            new PatchFullscreen().Enable();

            new PatchMenuScreenShow().Enable();
            new PatchMatchMakerSideSelectionScreenShow().Enable();
            new PatchLocationButton().Enable();
            new PatchMatchMakerSelectionLocationScreenShow().Enable();
            new PatchMatchMakerOfflineRaidScreenShow().Enable();
            new PatchRaidSettingsWindow().Enable();
        }

        private void InitConfiguration()
        {
            MinimizeOnGameLaunch = Config.Bind("Application", "Minimize on game launch", false, "");
            ForceFullscreenOnRaidStart = Config.Bind("Application", "Force fullscreen on raid start", true, "");

            Enabled = Config.Bind("Auto-start raid", "_Enabled", false, "");
            Map = Config.Bind("Auto-start raid", "Map", MapId.factory4_day, "");
            AIAmount = Config.Bind("Auto-start raid", "AI Amount", EFT.Bots.EBotAmount.AsOnline, "");

            SkipLoadingScenesEnabled = Config.Bind("Skip loading scenes", "_Enabled", false, "");
            SkipLoadingScenes = Config.Bind("Skip loading scenes", "Scenes to skip", "Shopping_Mall_IDEA,Reserve_Base_Bunkers", "List of Unity scenes to skip loading, separated by comma");
        }
    }
}