using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using SPT.Reflection.Patching;

namespace tarkin.quicklaunch
{
    [BepInPlugin("com.tarkin.quicklaunch", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<MapId> Map;

        internal static ConfigEntry<bool> LoadBots;
        internal static ConfigEntry<bool> LoadLoot;
        internal static ConfigEntry<bool> OnlyEssentialScenes;

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

        private PatchManager patchManager;

        private void Awake()
        {
            Logger = base.Logger;

            if (Chainloader.PluginInfos.ContainsKey("com.fika.core"))
            {
                Logger.LogError("Not compatible with Fika!");

                Destroy(this);
                return;
            }

            patchManager = new PatchManager(this, autoPatch: true);
            patchManager.EnablePatches();

            InitConfiguration();
        }

        private void InitConfiguration()
        {
            Enabled = Config.Bind("1. Autostart", "Enable autostart", false, "Trigger auto start raid right after main menu load");
            Map = Config.Bind("1. Autostart", "Autostart map", MapId.factory4_day, "");

            LoadBots = Config.Bind("2. Loading", "Enable bots", true, "");
            LoadLoot = Config.Bind("2. Loading", "Enable loot", true, "Loads faster with no loot");
            OnlyEssentialScenes = Config.Bind("2. Loading", "OnlyEssentialScenes", false, "Load only scenes that are required for the location to work");

            SkipSessionEndScreen = Config.Bind("3. Raid end", "Skip Session End Screen", true, "");
        }
    }
}