using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;

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
            Logger = base.Logger;

            if (Chainloader.PluginInfos.ContainsKey("com.fika.core"))
            {
                Logger.LogError("Not compatible with Fika!");

                Destroy(this);
                return;
            }

            InitConfiguration();

            new Patch_TarkovApplication_OnApplicationLoaded().Enable();

            new Patch_SessionResultExitStatus_Show().Enable();
        }

        private void InitConfiguration()
        {
            Enabled = Config.Bind("1. Autostart", "- Enabled", false, "");
            Map = Config.Bind("1. Autostart", "Map", MapId.factory4_day, "");

            LoadBots = Config.Bind("2. Loading", "Enable bots", true, "");
            LoadLoot = Config.Bind("2. Loading", "Enable loot", true, "Loads faster with no loot");

            SkipSessionEndScreen = Config.Bind("3. Raid end", "Skip Session End Screen", true, "");
        }
    }
}