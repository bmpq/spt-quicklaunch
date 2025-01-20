using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT;
using quicklaunch;

[BepInPlugin("com.tarkin.quicklaunch", "quicklaunch", "1.0.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Log;

    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<MapId> Map;

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

    private void Start()
    {
        Log = base.Logger;

        InitConfiguration();

        new PatchMenuScreenShow().Enable();
        new PatchMatchMakerSideSelectionScreenShow().Enable();
        new PatchLocationButton().Enable();
        new PatchMatchMakerSelectionLocationScreenShow().Enable();
        new PatchMatchMakerOfflineRaidScreenShow().Enable();
    }

    private void InitConfiguration()
    {
        Enabled = Config.Bind("General", "Enabled", true, "");
        Map = Config.Bind("General", "Map", MapId.factory4_day, "");
    }
}