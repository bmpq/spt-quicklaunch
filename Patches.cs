using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using EFT;
using EFT.UI.SessionEnd;

#if SPT_4_0
using MainMenuShowOperation = MainMenuControllerClass;
using ExitStatusScreenController = EFT.UI.SessionEnd.SessionResultExitStatus.GClass3899;
#elif SPT_4_1
using ExitStatusScreenController = EFT.UI.SessionEnd.SessionResultExitStatus.ExitStatusScreenController;
#endif

namespace tarkin.raidloadingtweaks
{
    internal class Patch_TarkovApplication_OnApplicationLoaded : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication),
#if SPT_4_0
                nameof(TarkovApplication.method_38));
#elif SPT_4_1
                nameof(TarkovApplication.OnApplicationLoaded));
#endif
        }

        [PatchPostfix]
        public static void Postfix(MainMenuShowOperation ___mainMenuControllerClass)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.Map.Value == Plugin.MapId.hideout)
            {
                ___mainMenuControllerClass.ShowScreen(EFT.UI.EMenuType.Hideout, turnOn: true);
                return;
            }

            if (TarkovApplication.Exist(out var app))
            {
                app.InternalStartGame(Plugin.Map.Value.ToString(), isLocalGame: true, isBotEnabled: Plugin.LoadBots.Value);
            }
        }
    }

    internal class Patch_LocalGame_SpawnLoot : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocalGame), nameof(LocalGame.method_12));
        }

        [PatchPrefix]
        private static void PatchPrefix(LocationSettingsClass.Location location)
        {
            if (Plugin.LoadLoot.Value)
                return;

            location.Loot?.Clear();
            Plugin.Logger.LogWarning("Disabled loot in this raid");
        }
    }

    internal class Patch_SessionResultExitStatus_Show : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SessionResultExitStatus), nameof(SessionResultExitStatus.Show), [typeof(ExitStatusScreenController)]);
        }

        [PatchPostfix]
        public static void Postfix(ExitStatusScreenController ___ScreenController)
        {
            if (!Plugin.SkipSessionEndScreen.Value)
                return;

            ___ScreenController.GoToMainMenu();
        }
    }
}
