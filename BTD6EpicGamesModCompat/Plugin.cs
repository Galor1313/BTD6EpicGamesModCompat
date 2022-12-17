using MelonLoader;
using System;

[assembly: MelonInfo(typeof(BTD6EpicGamesModCompat.Plugin), "BTD6 Epic Games Mod Compat", "1.0.0", "Baydock")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6-Epic")]

namespace BTD6EpicGamesModCompat {
    public sealed class Plugin : MelonPlugin {
        public static MelonLogger.Instance Logger { get; private set; }

        // Runs before crash caused by EOSSDK thankfully
        public override void OnPreInitialization() {
            Logger = LoggerInstance;

            // Avoid crash
            EOSSDK.Remove();

            // Used as a backup quit event, sometimes one and sometimes the other is called based on how the application is forcefully closed
            // There are still some ways that this could not end up being called.
            // Any help on how to get this to call on the following would be appreciated:
            // - A hard crash (eg. an uncaught exception)
            // - Task manager force close
            // - Selecting text in melonloader console (pausing execution), and then closing from the melonloader console
            // - Probably some other ways I haven't tried
            AppDomain.CurrentDomain.ProcessExit += (s, e) => EOSSDK.Restore();
        }

        public override void OnApplicationQuit() => EOSSDK.Restore();
    }
}
