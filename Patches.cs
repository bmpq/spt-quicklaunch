using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.Matchmaker;
using EFT.UI.SessionEnd;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace tarkin.quicklaunch
{
    internal class PatchMenuScreenShow : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MenuScreen), nameof(MenuScreen.Show), new Type[] { typeof(Profile), typeof(MatchmakerPlayerControllerClass), typeof(ESessionMode) });
        }

        [PatchPostfix]
        public static void Postfix(DefaultUIButton ____playButton, DefaultUIButton ____hideoutButton)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.Map.Value == Plugin.MapId.hideout)
            {
                ____hideoutButton.OnClick.Invoke();
                return;
            }
            
            ____playButton.OnClick.Invoke();
        }
    }

    internal class PatchMatchMakerSideSelectionScreenShow : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSideSelectionScreen), nameof(MatchMakerSideSelectionScreen.Show), new Type[] { typeof(ISession), typeof(RaidSettings), typeof(IHealthController), typeof(InventoryController) });
        }

        [PatchPostfix]
        public static void Postfix(UIAnimatedToggleSpawner ____pmcButton, DefaultUIButton ____nextButton)
        {
            if (!Plugin.Enabled.Value)
                return;

            ____pmcButton.SpawnedObject.Set(true);
            ____nextButton.OnClick.Invoke();
        }
    }

    internal class PatchLocationButton : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LocationButton).GetMethod(nameof(LocationButton.Show));
        }

        [PatchPostfix]
        public static void Postfix(ref LocationButton __instance, LocationSettingsClass.Location location)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.Map.Value == Plugin.MapId.Mod)
            {
                if (location.Id == Plugin.CustomMapName.Value)
                    __instance.Select(true);
                return;
            }

            if (location.Id == Plugin.Map.Value.ToString())
            {
                __instance.Select(true);
            }
        }
    }

    internal class PatchMatchMakerSelectionLocationScreenShow : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchMakerSelectionLocationScreen), nameof(MatchMakerSelectionLocationScreen.Show), new Type[] { typeof(ISession), typeof(RaidSettings), typeof(MatchmakerPlayerControllerClass) });
        }

        [PatchPostfix]
        public static void Postfix(DefaultUIButton ____acceptButton)
        {
            if (!Plugin.Enabled.Value)
                return;

            // couldn't track why we need to wait just know that we have to
            ____acceptButton.ClickAfterDelay(100);
        }
    }

    internal class PatchMatchMakerOfflineRaidScreenShow : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchmakerOfflineRaidScreen), nameof(MatchmakerOfflineRaidScreen.Show), new Type[] { typeof(InfoClass), typeof(RaidSettings), typeof(RaidSettings) });
        }

        [PatchPostfix]
        public static void Postfix(DefaultUIButton ____changeSettingsButton, DefaultUIButton ____readyButton)
        {
            if (!Plugin.Enabled.Value)
                return;

            ____changeSettingsButton.ClickAfterDelay(100);
            ____readyButton.ClickAfterDelay(400);
        }
    }

    internal class PatchRaidSettingsWindow : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(RaidSettingsWindow), nameof(RaidSettingsWindow.Show));
        }

        [PatchPostfix]
        public static void Postfix(DropDownBox ____aiAmountDropdown)
        {
            if (!Plugin.Enabled.Value)
                return;

            ____aiAmountDropdown.UpdateValue((int)Plugin.AIAmount.Value);
        }
    }

    internal class PatchSessionResultExitStatus : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SessionResultExitStatus), nameof(SessionResultExitStatus.Show), [
                typeof(Profile),
                typeof(LastPlayerStateClass),
                typeof(ESideType),
                typeof(ExitStatus),
                typeof(TimeSpan),
                typeof(ISession),
                typeof(bool)
                ]);
        }

        [PatchPostfix]
        public static void Postfix(DefaultUIButton ____mainMenuButton)
        {
            if (!Plugin.SkipSessionEndScreen.Value)
                return;

            ____mainMenuButton.ClickAfterDelay(100);
        }
    }

    internal static class Extension
    {
        public async static void ClickAfterDelay(this DefaultUIButton button, int delay)
        {
            await Task.Delay(100);
            button.OnClick.Invoke();
        }
    }
}
