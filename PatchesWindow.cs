using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace quicklaunch
{
    internal class PatchMinimize : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SplashScreenPanel), nameof(SplashScreenPanel.Awake));
        }

        [PatchPostfix]
        public static void Postfix()
        {
            if (!Plugin.MinimizeOnGameLaunch.Value)
                return;

            Screen.fullScreen = false;
            WindowsUtils.Minimize();
            Plugin.Log.LogInfo("Game window minimized.");
        }
    }

    internal class PatchMinimize2 : ModulePatch
    {
        static bool minimizedOnce;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameGraphicsClass), nameof(GameGraphicsClass.ApplyDisplaySettings));
        }

        [PatchPostfix]
        public static void Postfix()
        {
            if (minimizedOnce)
                return;
            minimizedOnce = true;

            if (!Plugin.MinimizeOnGameLaunch.Value)
                return;

            Screen.fullScreen = false;
            WindowsUtils.Minimize();
            Plugin.Log.LogInfo("Game window minimized.");
        }
    }

    internal class PatchFullscreen : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        public static void Postfix()
        {
            if (!Plugin.ForceFullscreenOnRaidStart.Value)
                return;

            Screen.fullScreen = true;
            Plugin.Log.LogInfo("Game window fullscreen set to true");
        }
    }
}
