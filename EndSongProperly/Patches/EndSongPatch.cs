using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EndSongProperly.Patches
{
    internal class EndSongPatch
    {
        static bool ignoreIsSongPlaying = false;

        static List<string> SongIdsToEndProperly = new List<string>();
        public static void InitializeSongIdList()
        {
            SongIdsToEndProperly.Clear();
            var configFilePath = Plugin.Instance.ConfigSongIdListFilePath.Value;
            var takotakoCustomSongsFilePath = BepInExUtility.GetConfigString("com.fluto.takotako", "CustomSongs", "SongDirectory");

            bool readTakoTakoFile = false;
            bool readConfigFile = false;

            DirectoryInfo dirInfo = new DirectoryInfo(takotakoCustomSongsFilePath);
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("EndSongProperlySongIds.txt", SearchOption.AllDirectories).ToList();
                for (int i = 0; i < files.Count; i++)
                {
                    var lines = File.ReadAllLines(files[i].FullName);
                    SongIdsToEndProperly.AddRange(lines);
                    readTakoTakoFile = true;
                }
            }

            if (File.Exists(Plugin.Instance.ConfigSongIdListFilePath.Value))
            {
                var lines = File.ReadAllLines(Plugin.Instance.ConfigSongIdListFilePath.Value);
                SongIdsToEndProperly.AddRange(lines);
                readConfigFile = true;
            }
        }

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix(EnsoGameManager __instance)
        {
            if (SongIdsToEndProperly.Contains(__instance.settings.musicuid))
            {
                ignoreIsSongPlaying = true;
            }
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

        [HarmonyPatch(typeof(ResultPlayer))]
        [HarmonyPatch(nameof(ResultPlayer.Update))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void ResultPlayer_Update_Postfix(ResultPlayer __instance)
        {
            var gameManager = GameObject.Find("EnsoGameManager").GetComponent<EnsoGameManager>();
            gameManager.ensoSound.StopSong();
        }
    }
}
