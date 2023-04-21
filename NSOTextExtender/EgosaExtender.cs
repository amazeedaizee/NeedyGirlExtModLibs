using Cysharp.Threading.Tasks;
using HarmonyLib;
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
    public class EgosaExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtEgosaParam_Pure.json"));
        public static List<EgosaMaster.Param> ExtList = new List<EgosaMaster.Param>();
        static List<EgosaMaster.Param> originalEgosa = new List<EgosaMaster.Param>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getEgosas")]
        static void InitializeExtEgosa(ref List<EgosaMaster.Param> __result)
        {
            if (originalEgosa.Count == 0)
            {
                originalEgosa = __result;
            }
            List<EgosaMaster.Param> newList = new List<EgosaMaster.Param>();
            newList.AddRange(originalEgosa);
            newList.AddRange(ExtList);
            __result = newList;

        }

        static bool isCustomReply = false;
        static string customType = "";
        static string customJouken = "";
        static string eyeId = "";


        /// <summary>
        /// Sets if a Vanity Search Thread is custom or not.
        /// </summary>
        /// <param name="isCustom">If the thread is custom.</param>
        /// <param name="type">The type used to search any matching EgosaMaster.Params. (optional)</param>
        /// <param name="jouken">The condition used to search for any matching EgosaMaster.Params, in addition to type (optional).</param>
        public static void IsCustomSearch(bool isCustom, string type = null, string jouken = null)
        {
            isCustomReply = isCustom;
            if (isCustomReply && type != null)
            {
                if (jouken != null)
                {
                    customJouken = jouken;
                }
                else { customJouken = ""; };
                customType = type;
            }
            else
            {
                isCustomReply = false;
                customType = "";
                customJouken = "";
            }
        }


        /// <summary>
        /// Creates a Window with custom search results made by the user. These search results are pre-set and are not randomized.
        /// </summary>
        /// <remarks>Note: Search results load the first param at the bottom of the result list, and vice-versa. 
        /// <br/> <c>IsCustomSearch</c> also sets to false after using this method.</remarks>
        /// <param name="type"> The type used to search any matching EgosaMaster.Params. </param>
        /// <param name="jouken">The condition used to search for any matching EgosaMaster.Params, in addition to type (optional).</param>
        public static async UniTask OpenCustomSearchWindow(string type, string jouken = null)
        {
            IsCustomSearch(true, type, jouken);
            SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.EgosaResult, true);
            await UniTask.Delay(670, false, PlayerLoopTiming.Update, default(CancellationToken));
            IsCustomSearch(false);
        }

        /// <summary>
        /// Creates an Eye window with a custom search result made by the user.
        /// </summary>
        /// <remarks>Only 22 Eye windows can be loaded per save instance. The counter will only reset when a save is loaded through the Login screen or the Load window.</remarks>
        /// <param name="id">The EgosaMaster.Param's Id used to load the search result.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void OpenEyeSearchWindow(string id)
        {
            eyeId = id;

            IChachedWindowParent chachedWindowParent = SingletonMonoBehaviour<ChachedWindowStore>.Instance.GetChachedWindowParent(ChachedWindowType.HORROR_DAY_3);
            ChachedWindowObject chachedWindowObject = chachedWindowParent.Pop();
            chachedWindowObject.Open();
            chachedWindowObject.ExecNumberEvent(0);
        }


        /// <summary>
        /// Creates a lot of Eye windows with custom search results made by the user. Search results will only appear with EgosaMaster.Params that has a <c>Type</c> of "eyes".
        /// </summary>
        /// <remarks>Only 22 Eye windows can be loaded per save instance. The counter will only reset when a save is loaded through the Login screen or the Load window.</remarks>
        public static async UniTask OpenALotOfEyeWindows()
        {
            List<EgosaMaster.Param> eyeList = EgosaExtender.ExtList.FindAll((EgosaMaster.Param e) => e.Type == "eyes");
            for (int i = 0; i < eyeList.Count; i++)
            {
                if (i == 23)
                {
                    break;
                }
                OpenEyeSearchWindow(eyeList[i].Id);
                await UniTask.Delay(120, false, PlayerLoopTiming.Update, default(CancellationToken));
                continue;
            }
        }

        static string EgosaFromId(string id, LanguageType lang)
        {
            EgosaMaster.Param param = NgoEx.getEgosas().FirstOrDefault((EgosaMaster.Param x) => x.Id == id);
            switch (lang)
            {
                case LanguageType.JP:
                    return param.BodyJp;
                case LanguageType.CN:
                    return param.BodyCn;
                case LanguageType.KO:
                    return param.BodyKo;
                case LanguageType.TW:
                    return param.BodyTw;
            }
            return param.BodyEn;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ImageViewer), "CashedSetUp")]
        static bool SetEyeText(int number, ImageViewer __instance, RectMask2D ____mask, Image ___bg, Image ____image, Sprite ___medama, Text ___kowai)
        {
            float height = ____mask.transform.parent.GetComponent<RectTransform>().rect.height;
            ____mask.padding = new Vector4(0f, height, 0f, 0f);
            ___bg.color = new Color(0f, 0f, 0f);
            ____image.sprite = ___medama;
            if (number == 0 && eyeId != "")
            {
                Blink(__instance, EgosaFromId(eyeId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value));
                eyeId = "";
            }
            else { Blink(__instance, NgoEx.SystemTextFromTypeString("KowaiInternet" + number.ToString(), SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value)); }
            ___kowai.text = "";
            Eye_cont();
            return false;

            async UniTask Eye_cont()
            {
                await UniTask.Delay(1100, false, PlayerLoopTiming.Update, default(CancellationToken));
                Show(__instance);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EgosaView2D), "UpdateContents")]
        static bool SetCustomSearches(EgosaRepView2D ___kusoPrefab, Transform ___kusoParent, VerticalGridLayout2D ___verticalGridLayout2D)
        {
            if (isCustomReply && customType != "")
            {
                List<EgosaMaster.Param> customEgosas = EgosaExtender.ExtList.FindAll((EgosaMaster.Param e) => e.Type == customType);
                for (int i = 0; i < customEgosas.Count; i++)
                {
                    EgosaRepView2D kusoRepView = UnityEngine.Object.Instantiate<EgosaRepView2D>(___kusoPrefab, ___kusoParent);
                    kusoRepView.gameObject.transform.SetAsLastSibling();
                    if (customJouken != "") { kusoRepView.SetData(new KusoRepDrawable(NgoEx.EgosaString(SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value, i, customType, customJouken))); }
                    else { kusoRepView.SetData(new KusoRepDrawable(NgoEx.EgosaString(SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value, i, customType))); }
                    kusoRepView.Show();
                    ___verticalGridLayout2D.AddLayoutObject(kusoRepView);
                }
                return false;
            }
            return true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ImageViewer), "Blink")]
        static void Blink(ImageViewer instance, string nakami)
        {
            throw new NotImplementedException();
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ImageViewer), "Show")]
        static void Show(ImageViewer instance)
        {
            throw new NotImplementedException();
        }

    }
}
