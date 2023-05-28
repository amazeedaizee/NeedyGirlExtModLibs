using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class TweetExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtTweetParam_Num.json"));
        //public static List<TweetMaster.Param> ExtList = JsonConvert.DeserializeObject<List<TweetMaster.Param>>(json);
        public static List<TweetMaster.Param> ExtList = new List<TweetMaster.Param>();

        /// <summary>
        /// Creates a custom Tweet on Tweeter/Poketter with normal (random) replies attached to it.
        /// </summary>
        /// <param name="id"> The TweetMaster.Param's Id used to load the tweet.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void StartExtTweetWithRandomReps(string id)
        {

            TweetMaster.Param exTweetId = TweetExtender.ExtList.Find((TweetMaster.Param t) => t.Id == id);
            TweetType exType = ExtTextManager.GetUniqueIdNum<TweetType>(exTweetId.Id);
            SingletonMonoBehaviour<PoketterManager>.Instance.AddQueueWithKusoreps(exType, null, null);
        }

        /// <summary>
        /// Creates a custom Tweet on Tweeter/Poketter with no replies attached to it.
        /// </summary>
        /// <param name="id"> The TweetMaster.Param's Id used to load the tweet.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void StartExtTweet(string id)
        {

            TweetMaster.Param exTweetId = TweetExtender.ExtList.Find((TweetMaster.Param t) => t.Id == id);
            TweetType exType = ExtTextManager.GetUniqueIdNum<TweetType>(exTweetId.Id);
            SingletonMonoBehaviour<PoketterManager>.Instance.AddTweet(exType);
        }

        public static void StartTweet(TweetType tweet)
        {
            SingletonMonoBehaviour<PoketterManager>.Instance.AddTweet(tweet);
        }

        /// <summary>
        /// Creates a normal or custom Tweet on Tweeter/Poketter with custom replies attached to it. These replies are pre-set and are not randomized. Tweet replies are loaded based on their <c>ParentID</c>.
        /// </summary>
        /// <remarks>Note: Tweet Replies loads the first param at the bottom of the reply thread, and vice-versa.</remarks>
        /// <param name="tweet"> The TweetType used to load the tweet. If loading custom text, either use <c>ExtTextExtender.GetUniqueIdNum()</c> or use your unique number at the end of your ID and add 10000 to it, then cast it as a TweetType.</param>
        /// <param name="kusoRepParId">The KRepMaster.Param's <c>ParentID</c> that's used to find matching params.</param>
        /// /// <exception cref="NullReferenceException"></exception>
        public static void StartTweetWithExtReps(TweetType tweet, string kusoRepParId)
        {
            List<KRepMaster.Param> customList = KRepExtender.ExtList.FindAll((KRepMaster.Param kr) => kr.ParentID == kusoRepParId);
            List<KusoRepType> newList = new List<KusoRepType>();
            foreach (KRepMaster.Param param in customList)
            {
                KusoRepType krIndex = ExtTextManager.GetUniqueIdNum<KusoRepType>(param.Id);
                if ((int)krIndex != 999) { newList.Add(krIndex); }
            }
            SingletonMonoBehaviour<PoketterManager>.Instance.AddQueueWithKusoreps(tweet, newList);
        }

        /// <summary>
        /// Creates a normal or custom Tweet on Tweeter/Poketter with custom replies attached to it. These replies are pre-set and are not randomized. Tweet replies are loaded based on their <c>ParentID</c>.
        /// </summary>
        /// <remarks>Note: Tweet Replies loads the first param at the bottom of the reply thread, and vice-versa.</remarks>
        /// <param name="tweet"> The TweetType used to load the tweet. If loading custom text, either use <c>ExtTextExtender.GetUniqueIdNum()</c> or use your unique number at the end of your ID and add 10000 to it, then cast it as a TweetType.</param>
        /// <param name="repList">The list of KusoRepType's used to load in the pre-set tweet replies. You can also place custom tweet replies in the list by either using <c>ExtTextExtender.GetUniqueIdNum()</c> or by using your unique number at the end of your ID and add 10000 to it, then cast it as a KusoRepType. </param>
        /// /// <exception cref="NullReferenceException"></exception>
        public static void StartTweetWithPresetReps(TweetType tweet, List<KusoRepType> repList)
        {
            SingletonMonoBehaviour<PoketterManager>.Instance.AddQueueWithKusoreps(tweet, repList);
        }

        /// <summary>
        /// Creates a custom Tweet on Tweeter/Poketter with custom replies attached to it. These replies are pre-set and are not randomized. Tweet replies are loaded based on their <c>ParentID</c>.
        /// </summary>
        /// <remarks>Note: Tweet Replies loads the first param at the bottom of the reply thread, and vice-versa.</remarks>
        /// <param name="tweetId"> The TweetMaster.Param's Id used to load the tweet.</param>
        /// <param name="kusoRepParId">The KRepMaster.Param's <c>ParentID</c> that's used to find matching params.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static void StartExtTweetWithExtReps(string tweetId, string kusoRepParId)
        {
            TweetMaster.Param tweet = ExtList.Find((TweetMaster.Param t) => t.Id == tweetId);
            TweetType twIndex = ExtTextManager.GetUniqueIdNum<TweetType>(tweet.Id);
            StartTweetWithExtReps(twIndex, kusoRepParId);
        }

        /// <summary>
        /// Creates a custom Tweet on Tweeter/Poketter with replies attached to it. These replies are pre-set and are not randomized. Tweet replies are loaded based on their <c>ParentID</c>.
        /// </summary>
        /// <param name="tweetId"> The TweetMaster.Param's Id used to load the tweet.</param>
        /// <param name="repList">The list of KusoRepType's used to load in the pre-set tweet replies. You can also place custom tweet replies in the list by either using <c>ExtTextExtender.GetUniqueIdNum()</c> or by using your unique number at the end of your ID and add 10000 to it, then cast it as a KusoRepType. </param>
        /// /// <exception cref="NullReferenceException"></exception>
        public static void StartExtTweetWithPresetReps(string tweetId, List<KusoRepType> repList)
        {
            TweetMaster.Param tweet = ExtList.Find((TweetMaster.Param t) => t.Id == tweetId);
            TweetType twIndex = ExtTextManager.GetUniqueIdNum<TweetType>(tweet.Id);
            StartTweetWithPresetReps(twIndex, repList);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TweetFetcher), "ConvertTypeToTweet")]
        static bool GetRawExtendedTweetData(ref TweetMaster.Param __result, TweetType t)
        {
            if ((int)t >= 10000)
            {
                int twIndex = (int)t - 10000;
                string twFindIndex = $"_{twIndex}";
                __result = TweetExtender.ExtList.Find((TweetMaster.Param tw) => tw.Id.EndsWith(twFindIndex));
                return false;

            }
            return true;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(TweetFetcher), "FetchAvailable")]
        static void FetchExtAvailable(ref List<TweetType> __result, CommandType commandType, CommandResult commandResult, int DayPart = 0, WindowType windowType = WindowType.None)
        {
            List<TweetMaster.Param> exList = TweetExtender.ExtList.FindAll((TweetMaster.Param t) => IsEqualCommandId(t, commandType) && IsEqualResult(t, commandResult) && IsMatchTime(t, DayPart));
            List<TweetType> newList = new List<TweetType>();
            List<TweetType> originalList = __result;
            foreach (TweetMaster.Param param in exList)
            {
                TweetType twIndex = ExtTextManager.GetUniqueIdNum<TweetType>(param.Id);
                if ((int)twIndex != 2000) { newList.Add(twIndex); }
            }
            newList.AddRange(originalList);
            __result = newList;
            newList = null;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TweetFetcher), "IsEqualCommandId")]
        static bool IsEqualCommandId(TweetMaster.Param t, CommandType c)
        {
            throw new NotImplementedException();
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TweetFetcher), "IsEqualResult")]
        static bool IsEqualResult(TweetMaster.Param t, CommandResult r)
        {
            throw new NotImplementedException();
        }
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TweetFetcher), "IsMatchTime")]
        static bool IsMatchTime(TweetMaster.Param t, int DayPart)
        {
            throw new NotImplementedException();
        }


    }
}
