using System.Reflection;
using HarmonyLib;

#if SPT_4_1
using SPT.Reflection.Patching;
#elif SPT_5_0
using SPTushonka.Reflection.Patching;
using Il2CppSystem.Threading.Tasks;

using EFT.UI;
using Comfort.Common;
#endif

using EFT;
using EFT.UI.SessionEnd;

namespace tarkin.raidloadingtweaks
{
#if SPT_5_0
    internal class Patch_TarkovApplication_ShowCharacterSelectionScreen : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.ShowCharacterSelectionScreen));
        }

        [PatchPostfix]
        public static void Postfix(TarkovApplication __instance, CharacterSelectionDataResponse data, ref Task<CharacterSelectionScreen.CharacterSelectionResult> __result)
        {
            if (!Plugin.Enabled.Value)
                return;

            var result = new CharacterSelectionScreen.CharacterSelectionResult()
            {
                Cancelled = false,
                GameMode = EGameMode.Pve,
                ProfileData = data[EGameMode.Pve]
            };

            Singleton<MenuUI>.Instance.CharacterSelectionScreen.Close();

            __result.TrySetResult(result);
        }
    }
#endif

    internal class Patch_TarkovApplication_OnApplicationLoaded : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.OnApplicationLoaded));
        }

        [PatchPostfix]
        public static void Postfix(TarkovApplication __instance)
        {
            if (!Plugin.Enabled.Value)
                return;

            if (Plugin.Map.Value == Plugin.MapId.hideout)
            {
#if SPT_4_1
                ((MainMenuShowOperation)AccessTools.Field(typeof(TarkovApplication), "_menuOperation").GetValue(__instance))
#elif SPT_5_0
                __instance._menuOperation
#endif
                    .ShowScreen(EFT.UI.EMenuType.Hideout, turnOn: true);
                return;
            }

            __instance.InternalStartGame(Plugin.Map.Value.ToString(), isLocalGame: true, isBotEnabled: Plugin.LoadBots.Value);
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
