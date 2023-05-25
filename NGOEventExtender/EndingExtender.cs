using HarmonyLib;
using Newtonsoft.Json;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NGOEventExtender
{

    [HarmonyPatch]
    public class EndingExtender
    {
        public static List<EndingMaster.Param> ExtEndList = new List<EndingMaster.Param>();
        static List<EndingType> originalList = new List<EndingType>()
        {EndingType.Ending_Grand, EndingType.Ending_Happy, EndingType.Ending_Meta, EndingType.Ending_Normal, EndingType.Ending_Bad, EndingType.Ending_Work, EndingType.Ending_Needy, EndingType.Ending_Yami, EndingType.Ending_Av, EndingType.Ending_Healthy, EndingType.Ending_Lust, EndingType.Ending_Ntr, EndingType.Ending_Sukisuki, EndingType.Ending_Stressful, EndingType.Ending_Sucide, EndingType.Ending_Jine, EndingType.Ending_KowaiInternet, EndingType.Ending_Yarisute, EndingType.Ending_Kyouso, EndingType.Ending_Jisatu, EndingType.Ending_Jikka, EndingType.Ending_Ginga, EndingType.Ending_DarkAngel, EndingType.Ending_Ideon       };
        static List<EndingType> extEndHistory = new List<EndingType>();
        static List<string> exEndHistoryPaths = new List<string>();


        /// <summary>
        /// Sets a custom ending flag based on the custom ending's Id.
        /// </summary>
        /// <param name="id">The EndingMaster.Param's Id used to set the custom ending flag.</param>
        public static void SetExtEndingFlag(string id)
        {
            EndingType endingType = GetUniqueEndNum(id);
            SingletonMonoBehaviour<EventManager>.Instance.nowEnding = endingType;
        }

        /// <summary>
        /// Checks if a custom ending is currently playing.
        /// </summary>
        /// <returns></returns>
        public static bool IsCustomEnding()
        {
            EndingType end = SingletonMonoBehaviour<EventManager>.Instance.nowEnding;
            if ((int)end < 1000)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a specific custom ending is currently playing.
        /// </summary>
        /// <param name="id">The custom EndingMaster.Param's Id to check for.</param>
        /// <returns></returns>
        public static bool IsCustomEnding(string id)
        {
            EndingType end = SingletonMonoBehaviour<EventManager>.Instance.nowEnding;
            if ((int)end < 1000)
            {
                return false;
            }
            EndingMaster.Param param = NgoEx.EndingFromType(end);
            if (param.Id == id)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Adds custom endings to the Ending <c>ExtList</c>. This is required to load custom endings into the game.
        /// <br/> In addition, a path to a <c>json</c> file in your mod folder is required to save custom ending history.
        /// </summary>
        /// <remarks>The <c>json</c> path for custom ending history accounts for other mods that have custom endings. </remarks>
        /// <param name="exEndList"></param>
        /// <param name="exHistoryPath"></param>
        public static void AddExtEndings(List<EndingMaster.Param> exEndList, string exHistoryPath)
        {
            exEndHistoryPaths.Add(exHistoryPath);
            try { LoadExtEndHistory(); }
            catch
            {
                return;
            }
            ExtEndList.AddRange(exEndList);
        }

        /// <summary>
        /// Loads an EndingMaster.Param based on the parsed EndingType.
        /// </summary>
        /// <remarks>The parsed type is based off of the Id's unique number id at the end of the title.</remarks>
        /// <param name="type">The parsed type used to load the EndingMaster.Param.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static EndingMaster.Param GetCustomEndingName(EndingType type)
        {
            if ((int)type >= 1000)
            {
                int endIndex = (int)type - 1000;
                string endFindIndex = $"_{endIndex}";
                return ExtEndList.Find((EndingMaster.Param tw) => tw.Id.EndsWith(endFindIndex));
            }
            throw new ArgumentException("EndingType is not equal to or more than 1000.");
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Boot), "Start")]
        static void LoadExtEndHistory()
        {
            extEndHistory.Clear();
            foreach (string s in exEndHistoryPaths)
            {
                try
                {
                    var parse = JsonConvert.DeserializeObject<List<EndingType>>(File.ReadAllText(s)).Distinct();                   
                    extEndHistory.AddRange(parse);
                }
                catch
                {
                    Debug.LogError("This is not a valid JSON file!");
                    continue;
                }

            }
        }

        static List<EndingType> GetRequiredEndings()
        {
            List<EndingMaster.Param> endingIds = ExtEndList.FindAll((EndingMaster.Param tw) => tw.Id.StartsWith("!"));
            if (endingIds.Count >= 8)
            {
                endingIds = endingIds.GetRange(0, 8);
            }
            List<EndingType> typeList = new List<EndingType>();
            foreach (EndingMaster.Param param in endingIds)
            {
                EndingType endIndex = GetUniqueEndNum(param.Id);
                typeList.Add(endIndex);
            }
            return typeList;
        }

        static EndingType GetUniqueEndNum(string id)
        {
            EndingMaster.Param endingId = ExtEndList.Find((EndingMaster.Param tw) => tw.Id == id);
            string[] titleSplit = id.Split('_');
            bool parseWorked = int.TryParse(titleSplit[titleSplit.Count() - 1], out int index);
            int newInt = 0;
            if (parseWorked) { newInt = index + 1000; }
            else
            {
                throw new InvalidOperationException("Result is not a number. End of Id must be formatted like this: \n\"_#\" \nwhere # is a unique number.");
            }
            return (EndingType)newInt;
        }


        static void IsOptionalEnding(EndingType type)
        {
            if ((int)type >= 1000)
            {
                int endIndex = (int)type - 1000;
                string endFindIndex = $"_{endIndex}";
                EndingMaster.Param param = ExtEndList.Find((EndingMaster.Param tw) => tw.Id.EndsWith(endFindIndex));
                SingletonMonoBehaviour<Settings>.Instance.mitaEnd.Remove(type);
                SingletonMonoBehaviour<Settings>.Instance.Save();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Boot), "WaitChooseUser")]
        static void ReArrangeButtons(GameObject ___Hint)
        {
            if (ExtEndList.Count != 0)
            {
                ___Hint.GetComponent<RectTransform>().anchoredPosition = new Vector2(-574, 478);
                GameObject.Find("button_picture").GetComponent<RectTransform>().anchoredPosition = new Vector2(-64, 1020);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Boot), "Start")]
        static void ReqEndingsOnly(Button ___Data0)
        {
            List<EndingType> endHistory = SingletonMonoBehaviour<Settings>.Instance.mitaEnd.Distinct<EndingType>().ToList<EndingType>();
            List<EndingType> exHistory = extEndHistory.Distinct().ToList();
            List<EndingType> extReqEndings = GetRequiredEndings();
            List<EndingType> conditionList = new List<EndingType>();
            conditionList.AddRange(originalList);
            conditionList.AddRange(extReqEndings);
            //Debug.Log($"End History Count: {endHistory.Count}, Condition List Count: {conditionList.Count}");
            var isHistoryValid = conditionList.Except(endHistory.Concat(exHistory));
            if (isHistoryValid.Count() == 0)
            {
                ___Data0.gameObject.SetActive(true);
                return;
            }
            ___Data0.gameObject.SetActive(false);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Boot), "ShowEnds")]
        static void AddReqEndingsToList_Login(GameObject ____achievedBlock, GameObject ____unachievedBlock, Transform ___endingParent)
        {
            List<EndingType> typeList = GetRequiredEndings();
            foreach (EndingType type in typeList)
            {
                EndingMaster.Param param = NgoEx.EndingFromType(type);
                string id = param.Id;
                string text;
                string text2;
                switch (SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value)
                {
                    case LanguageType.JP:
                        text = param.EndingNameJp;
                        text2 = param.JissekiJp;
                        break;
                    case LanguageType.CN:
                        text = param.EndingNameCn;
                        text2 = param.JissekiCn;
                        break;
                    case LanguageType.KO:
                        text = param.EndingNameKo;
                        text2 = param.JissekiKo;
                        break;
                    case LanguageType.TW:
                        text = param.EndingNameTw;
                        text2 = param.JissekiTw;
                        break;
                    default:
                        text = param.EndingNameEn;
                        text2 = param.JissekiEn;
                        break;
                }
                if (extEndHistory.Exists((EndingType gotend) => gotend == type))
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(____achievedBlock, ___endingParent);
                    gameObject.GetComponent<TooltipCaller>().isShowTooltip = true;
                    gameObject.GetComponent<TooltipCaller>().textNakami = text + "\n" + text2;
                    continue;
                }
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(____unachievedBlock, ___endingParent);
                gameObject2.GetComponent<TooltipCaller>().isShowTooltip = true;
                gameObject2.GetComponent<TooltipCaller>().textNakami = text + "\n" + text2;
                continue;
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(App_LoadData), "Awake")]
        static void AddReqEndingsToList_Load(GameObject ____achievedBlock, GameObject ____unachievedBlock, Transform ___parent)
        {
            List<EndingType> typeList = GetRequiredEndings();
            foreach (EndingType type in typeList)
            {
                if (extEndHistory.Exists((EndingType gotend) => gotend == type))
                {
                    UnityEngine.Object.Instantiate<GameObject>(____achievedBlock, ___parent);
                }
                else
                {
                    UnityEngine.Object.Instantiate<GameObject>(____unachievedBlock, ___parent);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EndingManager), "Awake")]
        static void AddReqEndingsToList_Blue(GameObject ____achievedBlock, GameObject ____achievingBlock, GameObject ____unachievedBlock, GameObject ____endingAchievement, EndingType ___end)
        {
            List<EndingType> exHistory = extEndHistory;
            Transform transform = ____endingAchievement.transform;
            List<EndingType> typeList = GetRequiredEndings();
            foreach (EndingType type in typeList)
            {
                if (___end == type)
                {
                    UnityEngine.Object.Instantiate<GameObject>(____achievingBlock, transform);
                }
                else if (exHistory.Exists((EndingType gotend) => gotend == type))
                {
                    UnityEngine.Object.Instantiate<GameObject>(____achievedBlock, transform);
                }
                else
                {
                    UnityEngine.Object.Instantiate<GameObject>(____unachievedBlock, transform);
                }
            }
            IsOptionalEnding(___end);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NgoEx), "EndingFromType")]
        static bool GetCustomEndingName(ref EndingMaster.Param __result, EndingType type)
        {
            if ((int)type >= 1000)
            {
                __result = GetCustomEndingName(type);
                return false;
            }
            return true;
        }
    }
}
