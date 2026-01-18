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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        [PatchPrefix]
        private static bool PatchPrefix(ref SceneResourceKey[] __result, List<SceneResourceKey> ____scenesResourceKeys)
        {
            if (!Plugin.SkipLoadingScenesEnabled.Value)
                return true;

            string[] sceneNameKeywordsToKeep = Plugin.EssentialSceneNameKeywords.Value
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (sceneNameKeywordsToKeep.Length == 0)
            {
                return true;
            }

            var scenesToKeep = new List<SceneResourceKey>();
            var scenesToSkip = new List<SceneResourceKey>();

            foreach (var scene in ____scenesResourceKeys)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

                bool shouldKeep = false;

                foreach (var keyword in sceneNameKeywordsToKeep)
                {
                    if (sceneName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shouldKeep = true;
                        break;
                    }
                }

                if (shouldKeep)
                {
                    scenesToKeep.Add(scene);
                }
                else
                {
                    scenesToSkip.Add(scene);
                }
            }

            if (scenesToSkip.Any())
            {
                Plugin.Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME}: Skipping the following scenes based on configuration:");
                foreach (var skippedScene in scenesToSkip)
                {
                    Plugin.Log.LogInfo($" - Skipped: {skippedScene.path}");
                }
            }

            __result = scenesToKeep.ToArray();
            return false;
        }
    }
}
