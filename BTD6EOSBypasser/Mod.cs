using HarmonyLib;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Store;
using Il2CppPlayEveryWare.EpicOnlineServices;
using Il2CppSystem.Threading.Tasks;
using MelonLoader;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(BTD6EOSBypasser.Mod), "BTD6 EOS Bypasser", "1.0.0", "Baydock")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6-Epic")]

namespace BTD6EOSBypasser {
    [HarmonyPatch]
    public class Mod : MelonMod {
        public static MelonLogger.Instance Logger { get; private set; }

        private const string DisabledStoreMessage = "In order to get mods to run on Epic Games, I had to gut Epic Store functionality.";
        private const string DisabledEpicLoginMessage = "In order to get mods to run on Epic Games, I had to gut Epic Login functionality.\n" +
                                                        "Get a linking code for an account by logging into BTD Monkey City, BTD Battles, BTD6, Bloons Adventure Time TD, or BTD Battles 2 on Steam or a mobile device.";

        public override void OnInitializeMelon() {
            Logger = LoggerInstance;
        }

        [HarmonyPatch(typeof(EOSHostManager), nameof(EOSHostManager.WaitTillReady))]
        [HarmonyPrefix]
        // Stops from stalling on step 1 because EOSSDK is missing
        public static bool BypassWaitForEOS(out Task __result) {
            __result = Task.CompletedTask;
            return false;
        }

        [HarmonyPatch(typeof(EOSHostManager), nameof(EOSHostManager.Start))]
        [HarmonyPrefix]
        // Stops EOSHostManager from setting up
        public static bool BypassEOSHostManagerStart() => false;

        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Awake))]
        [HarmonyPrefix]
        // Stops EOSManager from setting up
        public static bool BypassEOSManagerAwake() => false;

        [HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
        [HarmonyPrefix]
        // Stops EOSManager from updating
        public static bool BypassEOSManagerUpdate() => false;

        [HarmonyPatch(typeof(Purchaser), nameof(Purchaser.InitializePurchasing))]
        [HarmonyPrefix]
        // EOS is unfunctional, so no purchasing can be done
        public static bool NoPurchasing() => false;

        [HarmonyPatch(typeof(Modding), nameof(Modding.CheckForMods))]
        [HarmonyPrefix]
        // Idk why EOS is used here
        public static bool BypassModCheck(out bool __result) {
            // you silly, why wouldn't I return that you are in fact modding
            __result = true;
            return false;
        }

        [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Start))]
        [HarmonyPostfix]
        // Some cosmetic changes to better show that the store is unfunctional
        public static void DisableStoreButton(MainMenu __instance) {
            Button storeButton = __instance.storeBtn;

            // Make image look disabled to indicate it is not functional
            storeButton.gameObject.GetComponent<Image>().color = storeButton.colors.disabledColor;

            // Remove previous functionality, and add a message to explain why
            MainMenu.__c.__9__21_10 = null; // Where previous UnityAction is stored
            storeButton.onClick.RemoveAllListeners();
            storeButton.onClick.AddListener(new System.Action(() => PopupScreen.instance.ShowOkPopup(DisabledStoreMessage)));
        }

        [HarmonyPatch(typeof(MainAccountPopup), nameof(MainAccountPopup.Awake))]
        [HarmonyPostfix]
        // Stop the user from trying to log in with epic since EOS is unfunctional
        public static void DisableEpicLogin(MainAccountPopup __instance) {
            Button loginEpicButton = __instance.loginEpicBtn;

            // Make image and text look disabled to indicate it is not functional
            loginEpicButton.gameObject.GetComponent<Image>().color = loginEpicButton.colors.disabledColor;
            loginEpicButton.gameObject.GetComponentInChildren<Image>().color = loginEpicButton.colors.disabledColor;

            // Remove previous functionality, and add a message to explain why
            loginEpicButton.onClick.RemoveAllListeners();
            loginEpicButton.onClick.AddListener(new System.Action(() => PopupScreen.instance.ShowOkPopup(DisabledEpicLoginMessage)));
        }
    }
}
