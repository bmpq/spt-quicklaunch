using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

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

        internal static ConfigEntry<bool> SkipSessionEndScreen;

        internal static ConfigEntry<bool> SkipLoadingScenesEnabled;
        internal static ConfigEntry<string> EssentialSceneNameKeywords;

        internal enum MapId
        {
            hideout,
            factory4_day,
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

            new PatchSessionResultExitStatus().Enable();
        }

        private void InitConfiguration()
        {
            const string CATAUTOSTART = "1. Autostart raid";
            Enabled = Config.Bind(CATAUTOSTART, "- Enabled", false, "");
            Map = Config.Bind(CATAUTOSTART, "Map", MapId.factory4_day, "");
            AIAmount = Config.Bind(CATAUTOSTART, "AI Amount", EFT.Bots.EBotAmount.AsOnline, "");

            SkipSessionEndScreen = Config.Bind("2. Raid end", "Skip Session End Screen", true, "");

            SkipLoadingScenesEnabled = Config.Bind("3. Bare scene mode", "- Enabled", false, "You don't need this.");
            EssentialSceneNameKeywords = Config.Bind("3. Bare scene mode", "Essential Scene Name Keywords",
                "DesignMain, DesignStuff, AI, Scripts, Sound, Culling, Light",
                "Only scenes that have these words in their name will be loaded");

            MinimizeOnGameLaunch = Config.Bind("4. Application", "Minimize on game launch", false, "");
            ForceFullscreenOnRaidStart = Config.Bind("4. Application", "Force fullscreen on raid start", false, "");
        }
    }
}