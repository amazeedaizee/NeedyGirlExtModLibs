using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class KRepExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtKRepParam_Num.json"));
        //public static List<KRepMaster.Param> ExtList = JsonConvert.DeserializeObject<List<KRepMaster.Param>>(json);
        public static List<KRepMaster.Param> ExtList = new List<KRepMaster.Param>();

        static List<KusoRepType> extKRepSmallList = new List<KusoRepType>();
        static List<KusoRepType> extKRepMiddleList = new List<KusoRepType>();
        static List<KusoRepType> extKRepLargeList = new List<KusoRepType>();

        /// <summary>
        /// Re-initializes custom Tweet Replies.
        /// </summary>
        public static void ResetExtKReps()
        {
            ClearExtKReps();
            InitializeExtKReps();
        }

        static void ClearExtKReps()
        {
            extKRepSmallList.Clear();
            extKRepMiddleList.Clear();
            extKRepLargeList.Clear();
        }

        static void InitializeExtKReps()
        {
            foreach (KRepMaster.Param param in ExtList)
            {
                KusoRepType kIndex = ExtTextManager.GetUniqueIdNum<KusoRepType>(param.Id);
                if ((int)kIndex == 999)
                {
                    continue;
                }
                if (param.ParentID == "random")
                {
                    extKRepSmallList.Add(kIndex);
                    extKRepMiddleList.Add(kIndex);
                    extKRepLargeList.Add(kIndex);
                }
                if (param.ParentID == "small")
                {
                    extKRepSmallList.Add(kIndex);
                }
                if (param.ParentID == "middle")
                {
                    extKRepMiddleList.Add(kIndex);
                }
                if (param.ParentID == "large")
                {
                    extKRepLargeList.Add(kIndex);
                }
            }
        }

        static List<KusoRepType> GetFinalList()
        {
            int follower = SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.Follower);
            if (follower < 10000)
            {
                List<KusoRepType> smallKList = new List<KusoRepType>();
                smallKList.AddRange(NgoEx.smallKusoreps);
                smallKList.AddRange(KRepExtender.extKRepSmallList);
                return smallKList;
            }
            else if (follower < 100000)
            {
                List<KusoRepType> middleKList = new List<KusoRepType>();
                middleKList.AddRange(NgoEx.middleKusoreps);
                middleKList.AddRange(KRepExtender.extKRepMiddleList);
                return middleKList;
            }
            else
            {
                List<KusoRepType> largeKList = new List<KusoRepType>();
                largeKList.AddRange(NgoEx.largeKusoreps);
                largeKList.AddRange(KRepExtender.extKRepLargeList);
                return largeKList;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PoketterManager), "AddKusorepToTweet", new Type[] { typeof(TweetData), typeof(KusoRepType) })]
        static bool AddExtNormalKusoreps(TweetData tw, KusoRepType k)
        {
            List<KusoRepType> KList = GetFinalList();
            if (!tw.IsOmote)
            {
                return true;
            }
            else if (SingletonMonoBehaviour<EventManager>.Instance.nowEnding != EndingType.Ending_None)
            {
                return true;
            }
            else if (SingletonMonoBehaviour<EventManager>.Instance.isHorror)
            {
                return true;
            }
            else if (tw.Type == TweetType.AfterTweet_Angel1 || tw.Type == TweetType.AfterTweet_Angel2 || tw.Type == TweetType.AfterTweet_Angel3 || tw.Type == TweetType.AfterTweet_Angel4 || tw.Type == TweetType.AfterTweet_Angel5)
            {
                return true;
            }
            else
            {
                KusoRepType randomKRep = KList[UnityEngine.Random.Range(0, (KList.Count))];
                while (tw.kusoReps.Contains(randomKRep))
                {
                    randomKRep = KList[UnityEngine.Random.Range(0, (KList.Count))];
                    if (!tw.kusoReps.Contains(randomKRep))
                    {
                        break;
                    }
                }
                tw.kusoReps.Add(randomKRep);
            }
            return false;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TweetFetcher), "ConvertTypeToKusorep")]
        static bool GetRawExtendedKRepData(ref KRepMaster.Param __result, KusoRepType t)
        {
            if ((int)t >= 10000)
            {
                int kRepIndex = (int)t - 10000;
                string kRepFindIndex = $"_{kRepIndex}";
                __result = KRepExtender.ExtList.Find((KRepMaster.Param tw) => tw.Id.EndsWith(kRepFindIndex));
                return false;
            }
            return true;
        }

    }
}
