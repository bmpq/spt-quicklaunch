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
        public class Patch_ScenesPreset_ShouldLoadScene : ModulePatch
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
                return AccessTools.Method(typeof(ScenesPreset), nameof(ScenesPreset.ShouldLoadScene));
            }

            [PatchPostfix]
            private static void PatchPostfix(ISceneResource scene, ref bool __result)
            {
                if (!Plugin.OnlyEssentialScenes.Value)
                    return;

                if (!__result)
                    return;

                if (scene is SceneResourceKey sceneKey)
                {
                    foreach (var suffix in Whitelist)
                    {
                        if (sceneKey.path.EndsWith(suffix + ".unity", System.StringComparison.OrdinalIgnoreCase))
                        {
                            __result = true;
                            Plugin.Logger.LogWarning("Whitelisting scene: " + sceneKey.path);
                            return;
                        }
                    }
                }

                __result = false; 
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
