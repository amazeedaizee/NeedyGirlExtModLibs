using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class KitsuneExtender
    {
        //public static string jsonOne = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtKitsuneParam.json"));
        //public static string jsonTwo = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtKitsuneTitleParam.json"));
        public static List<KituneMaster.Param> ExtList = new List<KituneMaster.Param>();
        public static List<KituneSuretaiMaster.Param> ExtList_Title = new List<KituneSuretaiMaster.Param>();


        public static bool isCustomThread;
        public static bool canPost;
        public static string customTopic = "";
        static List<KituneMaster.Param> originalKitune = new List<KituneMaster.Param>();
        static List<KituneSuretaiMaster.Param> originalKituneTitles = new List<KituneSuretaiMaster.Param>();

        /// <summary>
        /// Sets if an /st/ thread is custom or not.
        /// </summary>
        /// <param name="isCustom">It is a custom thread if true, otherwise loads the normal thread.</param>
        /// <param name="topic"> The KituneMaster.Param and KitsuneSuretai.Param </param>
        /// <param name="isCanPost"></param>
        public static void IsCustomKitsuneThread(bool isCustom, string topic = null, bool isCanPost = true)
        {
            canPost = isCanPost;
            if (isCustom && (topic != null || topic != ""))
            {
                isCustomThread = true;
                customTopic = topic;
            }
            else
            {
                isCustomThread = false;
                customTopic = "";
            }
        }

        /// <summary>
        /// Opens a Window with a custom /st/ thread made by the user. Posting in /st/ is disabled.
        /// </summary>
        /// <remarks><c>IsCustomKitsuneThread</c> is set to false after using this method.</remarks>
        /// <param name="isMini"> Is smaller and placed at a random position if true.
        /// <br/> Is normal sized otherwise.</param>
        /// <param name="topic"> The custom topic of the /st/ thread.</param>
        public static void OpenCustomKituneWindow(bool isMini, string topic)
        {
            IsCustomKitsuneThread(true, topic, false);
            IWindow window;
            switch (isMini)
            {
                case true:
                    window = SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.KeijibanMini, true);
                    window.setRandomPosition();
                    break;
                case false:
                    window = SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Keijiban, true);
                    break;
            }

            IsCustomKitsuneThread(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Boot), "Awake")]
        [HarmonyPatch(typeof(EventManager), "Load")]
        [HarmonyPatch(typeof(DayPassing), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void ResetCustomKitsuneThread()
        {
            IsCustomKitsuneThread(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getKitunes")]
        static void SetExtKitsunes(ref List<KituneMaster.Param> __result)
        {
            if (originalKitune.Count == 0)
            {
                originalKitune = __result;
            }
            List<KituneMaster.Param> newList = new List<KituneMaster.Param>();
            newList.AddRange(originalKitune);
            newList.AddRange(KitsuneExtender.ExtList);
            __result = newList;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getSuretais")]
        static void SetExtKitsune(ref List<KituneSuretaiMaster.Param> __result)
        {
            if (originalKituneTitles.Count == 0)
            {
                originalKituneTitles = __result;
            }
            List<KituneSuretaiMaster.Param> newList = new List<KituneSuretaiMaster.Param>();
            newList.AddRange(originalKituneTitles);
            newList.AddRange(KitsuneExtender.ExtList_Title);
            __result = newList;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(KitsuneView), "UpdateContents")]
        static bool SetCustomKituneThread(KitsuneView __instance, Text ____suretai, ScrollRect ____scrollRect, GameObject ____buttonRoot)
        {
            if (isCustomThread && customTopic != "")
            {
                try
                {
                    string findTitle = $"Suretai_{customTopic}";
                    KituneSuretaiMaster.Param titleId = KitsuneExtender.ExtList_Title.Find((KituneSuretaiMaster.Param k) => k.Id == findTitle);
                }
                catch
                {
                    Debug.LogError("An error has occurred! That title can't be found.");
                    return true;
                }
                if (!canPost)
                {
                    ____scrollRect.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                    ____buttonRoot.SetActive(false);
                }
                string text2 = NgoEx.SuretaiFromType("Suretai_" + customTopic, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
                ____suretai.text = text2;
                foreach (KituneMaster.Param param in NgoEx.KituneFromRank(customTopic))
                {
                    string str = NgoEx.KituneFromType(param.Id, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
                    GetResInstance(__instance, false).SetData(NgoEx.SystemTextFromType(SystemTextType.Kitsune_Handle, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value), param.ResNumber, str);
                }
                return false;
            }
            return true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(KitsuneView), "GetResInstance")]
        static KitsuneResView GetResInstance(KitsuneView instance, bool isJien = false)
        {
            throw new NotImplementedException();
        }

    }
}
