using Cysharp.Threading.Tasks;
using HarmonyLib;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class EgosaExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtEgosaParam_Pure.json"));
        internal static List<EgosaMaster.Param> ExtList = new List<EgosaMaster.Param>();
        static List<EgosaMaster.Param> originalEgosa = new List<EgosaMaster.Param>();

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

        //[HarmonyTranspiler]
        //[HarmonyPatch(typeof(ImageViewer), nameof(ImageViewer.CashedSetUp), MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> SetEyeText_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> code = new List<CodeInstruction>(instructions);
            Label horrorEndingLabel = generator.DefineLabel();
            Label endBlinkLabel = generator.DefineLabel();

            object numField = null;
            bool labelsSet = false;

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Callvirt && (MethodInfo)code[i].operand == AccessTools.Method(typeof(Image), "set_sprite", new Type[] { typeof(Sprite) }))
                {
                    if (!labelsSet)
                    {
                        code[i + 1].labels.Add(horrorEndingLabel);
                        numField = code[i + 4].operand;
                        continue;
                    }
                    else
                    {
                        code.InsertRange(i + 1, new List<CodeInstruction>()
                            {
                                new CodeInstruction(OpCodes.Ldarg_0),
                                new CodeInstruction(OpCodes.Ldfld, numField),
                                new CodeInstruction(OpCodes.Brtrue, horrorEndingLabel),
                                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EgosaExtender), "eyeId")),
                                new CodeInstruction(OpCodes.Ldstr, ""),
                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string),"op_Inequality",new Type[]{typeof(string),typeof(string)})),
                                new CodeInstruction(OpCodes.Brfalse, horrorEndingLabel),
                                new CodeInstruction(OpCodes.Ldloc_1),
                                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EgosaExtender), "eyeId")),
                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SingletonMonoBehaviour<Settings>), "get_Instance")),
                                new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Settings), "CurrentLanguage")),
                                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ReactiveProperty<LanguageType>),"get_Value")),
                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EgosaExtender),"EgosaFromId", new Type[]{typeof(string),typeof(LanguageType)})),
                                new CodeInstruction(OpCodes.Call,  AccessTools.Method(typeof(ImageViewer), "Blink", new Type[]{typeof(string)})),
                                new CodeInstruction(OpCodes.Ldstr, ""),
                                new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(EgosaExtender), "eyeId")),
                                new CodeInstruction(OpCodes.Br_S, endBlinkLabel)

                            });
                        break;
                    }


                }
                if (!labelsSet && code[i].opcode == OpCodes.Call && (MethodInfo)code[i].operand == AccessTools.Method(typeof(ImageViewer), "Blink", new Type[] { typeof(string) }))
                {
                    code[i + 1].labels.Add(endBlinkLabel);
                    i -= 13;
                    labelsSet = true;
                    continue;
                }
            }
            return code;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(EgosaView2D), "UpdateContents")]
        static bool SetCustomSearches(EgosaRepView2D ___kusoPrefab, Transform ___kusoParent, VerticalGridLayout2D ___verticalGridLayout2D)
        {
            if (isCustomReply && customType != "")
            {
                List<EgosaMaster.Param> customEgosas = ExtList.FindAll((EgosaMaster.Param e) => e.Type == customType);
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getEgosas")]
        static void InitializeExtEgosa(ref List<EgosaMaster.Param> __result)
        {
            if (ExtList.Count != 0 && !__result.Exists(e => e.Id == ExtList[0].Id))
            {
                __result.AddRange(ExtList);
            }
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
