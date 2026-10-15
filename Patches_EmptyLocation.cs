using System.Reflection;
using HarmonyLib;

#if SPT_4_1
using System.Collections.Generic;
using SPT.Reflection.Patching;
#elif SPT_5_0
using SPTushonka.Reflection.Patching;
using Il2CppSystem.Collections.Generic;
#endif

using EFT;

namespace tarkin.raidloadingtweaks
{
    internal class Patches_EmptyLocation
    {
        public class Patch_LoadScenesFromPresetOperation_ScenesLoadCoroutine : ModulePatch
        {
            readonly static string[] Whitelist =
            {
                "scripts",
                "DesignMain",
                "Design_Main",
                "Sound",
                "AI"
            };

            protected override MethodBase GetTargetMethod()
            {
                return AccessTools.Method(typeof(LoadScenesFromPresetOperation), nameof(LoadScenesFromPresetOperation.ScenesLoadCoroutine));
            }

            [PatchPrefix]
            private static void PatchPrefix(ref IList<ResourceKey> scenes)
            {
                if (!Plugin.OnlyEssentialScenes.Value) return;

                if (scenes == null || scenes.Count == 0)
                {
                    return;
                }

                List<ResourceKey> filteredScenes = new List<ResourceKey>();

                foreach (ResourceKey scene in scenes)
                {
                    foreach (string suffix in Whitelist)
                    {
                        if (scene.path.EndsWith(suffix + ".unity", System.StringComparison.OrdinalIgnoreCase))
                        {
                            filteredScenes.Add(scene);
                            Plugin.Logger.LogWarning("Whitelisting scene: " + scene.path);
                            break;
                        }
                    }
                }

                scenes = filteredScenes;
            }
        }

        public class Patch_BotDoorsController_RefreshData : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return AccessTools.Method(typeof(BotDoorsController), nameof(BotDoorsController.RefreshData));
            }

            [PatchPrefix]
            private static void PatchPrefix(BotDoorsController __instance)
            {
                if (!Plugin.OnlyEssentialScenes.Value) return;

                // crashes without this
                __instance._navMeshDoorLinks = new List<NavMeshDoorLink>();
            }
        }

        internal class Patch_LocalGame_SpawnLoot : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return AccessTools.Method(typeof(BaseLocalGame<EftGamePlayerOwner>), nameof(BaseLocalGame<EftGamePlayerOwner>.SpawnLoot));
            }

            [PatchPrefix]
            private static void PatchPrefix(JsonType.LocationSettings.Location location)
            {
                if (Plugin.LoadLoot.Value)
                    return;

                location.Loot?.Clear();
                Plugin.Logger.LogWarning("Disabled loot in this raid");
            }
        }
    }
}
