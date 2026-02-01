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
        internal static ConfigEntry<string> CustomMapName;
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
            Labyrinth,
            Mod
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

            CustomMapName = Config.Bind("2. Custom map", "Custom map name", "de_dust2", "Will try to load map with this name if MapID is set to 'Mod'");

            SkipSessionEndScreen = Config.Bind("3. Raid end", "Skip Session End Screen", true, "");
        }
    }
}