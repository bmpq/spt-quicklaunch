using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using EFT;
using EFT.UI.SessionEnd;
using JsonType;

namespace tarkin.raidloadingtweaks
{
    internal class Patch_TarkovApplication_OnApplicationLoaded : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.OnApplicationLoaded));
        }

        [PatchPostfix]
        public static void Postfix(MainMenuShowOperation ____menuOperation)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.Map.Value == Plugin.MapId.hideout)
            {
                ____menuOperation.ShowScreen(EFT.UI.EMenuType.Hideout, turnOn: true);
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
            return AccessTools.Method(typeof(LocalGame), nameof(LocalGame.SpawnLoot));
        }

        [PatchPrefix]
        private static void PatchPrefix(LocationSettings.Location location)
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
            return AccessTools.Method(typeof(SessionResultExitStatus), nameof(SessionResultExitStatus.Show), 
                [typeof(SessionResultExitStatus.ExitStatusScreenController)]);
        }

        [PatchPostfix]
        public static void Postfix(SessionResultExitStatus.ExitStatusScreenController controller)
        {
            if (!Plugin.SkipSessionEndScreen.Value)
                return;

            controller.GoToMainMenu();
        }
    }
}
