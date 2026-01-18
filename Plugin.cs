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
        internal static ConfigEntry<MapId> Map;
        internal static ConfigEntry<EFT.Bots.EBotAmount> AIAmount;

        internal static ConfigEntry<bool> SkipSessionEndScreen;

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
            Sandbox,
            Labyrinth
        }

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();

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
        }
    }
}