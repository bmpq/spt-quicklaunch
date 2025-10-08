using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace tarkin.quicklaunch
{
    internal class Patch_ScenesPreset_ScenesResourceKeys : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(ScenesPreset), nameof(ScenesPreset.ScenesResourceKeys));
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref SceneResourceKey[] __result, List<SceneResourceKey> ____scenesResourceKeys)
        {
            string[] sceneNamesToSkip = Plugin.SkipLoadingScenes.Value
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (sceneNamesToSkip.Length == 0)
            {
                return true;
            }

            var scenesToKeep = new List<SceneResourceKey>();
            var scenesThatWereSkipped = new List<SceneResourceKey>();

            foreach (var scene in ____scenesResourceKeys)
            {
                bool shouldSkip = sceneNamesToSkip.Any(nameToSkip =>
                    scene.path.IndexOf(nameToSkip, StringComparison.OrdinalIgnoreCase) >= 0
                );

                if (shouldSkip)
                {
                    scenesThatWereSkipped.Add(scene);
                }
                else
                {
                    scenesToKeep.Add(scene);
                }
            }

            if (scenesThatWereSkipped.Any())
            {
                Plugin.Log.LogInfo("tarkin.quicklaunch: Skipping the following scenes based on configuration:");
                foreach (var skippedScene in scenesThatWereSkipped)
                {
                    Plugin.Log.LogInfo($" - Skipped: {skippedScene.path}");
                }
            }

            __result = scenesToKeep.ToArray();
            return false;
        }
    }
}
