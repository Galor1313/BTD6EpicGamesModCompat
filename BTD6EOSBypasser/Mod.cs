using BTD6EOSBypasser;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using Il2CppAssets.Scripts.Unity.UI_New.Main.HeroSelect;
using Il2CppAssets.Scripts.Unity.UI_New.Pause;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Store;
using Il2CppAssets.Scripts.Unity.UI_New.Upgrade;
using Il2CppPlayEveryWare.EpicOnlineServices;
using Il2CppSystem.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(Mod), "BTD6 EOS Bypasser", "1.0.2", "Baydock")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6-Epic")]

namespace BTD6EOSBypasser;

[HarmonyPatch]
public class Mod : MelonMod {
    private const string DisabledStoreMessage =
        "In order to get mods to run on Epic Games, I had to gut Epic Store functionality.";

    private const string DisabledEpicLoginMessage =
        "In order to get mods to run on Epic Games, I had to gut Epic Login functionality.\n" +
        "Get a linking code for an account by logging into BTD Battles, BTD6, BTD Monkey City, Bloons Adventure Time TD, or BTD Battles 2 on Steam or a mobile device.";

    public static MelonLogger.Instance Logger { get; private set; }

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
        var storeButton = __instance.storeBtn;

        // Make image look disabled to indicate it is not functional
        storeButton.gameObject.GetComponent<Image>().color = storeButton.colors.disabledColor;

        // Remove previous functionality, and add a message to explain why
        MainMenu.__c.__9__21_10 = null; // Where previous UnityAction is stored
        storeButton.onClick.RemoveAllListeners();
        storeButton.onClick.AddListener(new System.Action(() =>
            PopupScreen.instance.ShowOkPopup(DisabledStoreMessage)));
    }

    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Open))]
    [HarmonyPrefix]
    // I got an error here with a fresh account, idk, patching adds error catching so it's fine now
    public static void BypassError() { }

    [HarmonyPatch(typeof(MainAccountPopup), nameof(MainAccountPopup.Awake))]
    [HarmonyPostfix]
    // Stop the user from trying to log in with epic since EOS is unfunctional
    public static void DisableEpicLogin(MainAccountPopup __instance) {
        var loginEpicButton = __instance.loginEpicBtn;

        // Make image and text look disabled to indicate it is not functional
        loginEpicButton.gameObject.GetComponent<Image>().color = loginEpicButton.colors.disabledColor;
        loginEpicButton.gameObject.GetComponentInChildren<NK_TextMeshProUGUI>().color =
            loginEpicButton.colors.disabledColor;

        // Remove previous functionality, and add a message to explain why
        loginEpicButton.onClick.RemoveAllListeners();
        loginEpicButton.onClick.AddListener(new System.Action(() =>
            PopupScreen.instance.ShowOkPopup(DisabledEpicLoginMessage)));
    }

    [HarmonyPatch(typeof(PauseScreen), nameof(PauseScreen.Open))]
    [HarmonyPostfix]
    // Some cosmetic changes to better show that the store is unfunctional
    public static void DisablePauseStore(PauseScreen __instance) {
        // Disable store button
        __instance.storeButton.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(UpgradeScreen), nameof(UpgradeScreen.Open))]
    [HarmonyPostfix]
    // Some cosmetic changes to better show that the store is unfunctional
    public static void DisableUpgradePurchases(UpgradeScreen __instance) {
        // Destroy purchasing button
        Object.Destroy(__instance.purchaseAllTowerUpgradesIncludingParagon.gameObject);

        //oh come on, another one
        Object.Destroy(__instance.purchaseAllTowerUpgrades.gameObject);

        //even more
        Object.Destroy(__instance.purchaseParagonTowerUpgrades.gameObject);

        //lots of purchasing buttons
        Object.Destroy(__instance.purchaseTowerXP.gameObject);
    }

    [HarmonyPatch(typeof(UpgradeScreen), nameof(UpgradeScreen.SetIapButtons))]
    [HarmonyPrefix]
    // Disable upgrade iaps
    public static bool DisableUpgradeIaps() => false;

    [HarmonyPatch(typeof(TowerProductButton), nameof(TowerProductButton.StartPurchase))]
    [HarmonyPrefix]
    // Disable purchasing
    public static bool DisableTowerProductButton() => false;

    [HarmonyPatch(typeof(HeroUpgradeDetails), nameof(HeroUpgradeDetails.Awake))]
    [HarmonyPostfix]
    // Disable hero bundles
    public static void RemoveHeroBundles(HeroUpgradeDetails __instance) {
        Object.Destroy(__instance.heroBundleBtn.transform.parent.gameObject);
    }
}
