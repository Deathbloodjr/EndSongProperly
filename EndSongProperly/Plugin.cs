using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using BepInEx.Configuration;
using EndSongProperly.Patches;

#if TAIKO_IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace EndSongProperly
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
#if TAIKO_MONO
    public class Plugin : BaseUnityPlugin
#elif TAIKO_IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<bool> ConfigExamplesEnabled;

        public ConfigEntry<bool> ConfigLoggingEnabled;
        public ConfigEntry<int> ConfigLoggingDetailLevelEnabled;

#if TAIKO_MONO
        private void Awake()
#elif TAIKO_IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if TAIKO_MONO
            Log = Logger;
#elif TAIKO_IL2CPP
            Log = base.Log;
#endif

            SetupConfig();
            SetupHarmony();
        }

        private void SetupConfig()
        {
            // I never really used this
            // I'd rather just use a folder in BepInEx's folder for storing information
            // I tend to put data in BepInEx/data/<ModName> now

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");


            ConfigLoggingEnabled = Config.Bind("Debug",
                "LoggingEnabled",
                true,
                "Enables logs to be sent to the console.");

            ConfigLoggingDetailLevelEnabled = Config.Bind("Debug",
                "LoggingDetailLevelEnabled",
                0,
                "Enables more detailed logs to be sent to the console. The higher the number, the more logs will be displayed. Mostly for my own debugging.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                _harmony.PatchAll(typeof(EndSongPatch));
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        // I never used these, but they may come in handy at some point
        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCustomCoroutine(IEnumerator enumerator)
        {
#if TAIKO_MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif TAIKO_IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }

        public void LogInfoInstance(string value, int detailLevel = 0)
        {
            // Only print if Detailed Enabled is true, or if DetailedEnabled is false and isDetailed is false
            if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
            {
                Log.LogInfo("[" + detailLevel + "] " + value);
            }
        }
        public static void LogInfo(string value, int detailLevel = 0)
        {
            Instance.LogInfoInstance(value, detailLevel);
        }


        public void LogWarningInstance(string value, int detailLevel = 0)
        {
            if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
            {
                Log.LogWarning("[" + detailLevel + "] " + value);
            }
        }
        public static void LogWarning(string value, int detailLevel = 0)
        {
            Instance.LogWarningInstance(value, detailLevel);
        }


        public void LogErrorInstance(string value, int detailLevel = 0)
        {
            if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
            {
                Log.LogError("[" + detailLevel + "] " + value);
            }
        }
        public static void LogError(string value, int detailLevel = 0)
        {
            Instance.LogErrorInstance(value, detailLevel);
        }

    }
}