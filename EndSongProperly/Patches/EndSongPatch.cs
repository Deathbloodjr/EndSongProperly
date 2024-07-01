using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndSongProperly.Patches
{
    internal class EndSongPatch
    {
        static bool ignoreIsSongPlaying = false;

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix(EnsoGameManager __instance)
        {
            ignoreIsSongPlaying = true;
        }

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExec))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExec_Postfix(EnsoGameManager __instance)
        {
            ignoreIsSongPlaying = false;
        }

        [HarmonyPatch(typeof(EnsoSound))]
        [HarmonyPatch(nameof(EnsoSound.IsSongPlaying))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoSound_IsSongPlaying_Postfix(EnsoSound __instance, ref bool __result)
        {
            if (ignoreIsSongPlaying)
            {
                __result = false;
            }
        }
    }
}
