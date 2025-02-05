using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using NGO;
using NGOTxtExtender;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

namespace NGOEventExtender
{
    public class PlayingBlock
    {

        /// <summary>
        /// Dialogue from KAngel.
        /// </summary>
        /// <param name="chatId">The TenComment.Param's Id used to load the dialogue.</param>
        /// <param name="animId">The animation that KAngel uses when she speaks.</param>
        /// <returns></returns>
        public static Playing AngelTalk(string chatId, string animId = "")
        {
            string chat = chatId == "" || chatId == null ? "" : NgoEx.TenTalk(chatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            return new Playing(true, chat, StatusType.Tension, 1, 0, "", "", animId, false, SuperchatType.White, false, "");
        }

        /// <summary>
        /// A hate comment which KAngel responds to. 
        /// </summary>
        /// <param name="aChatId">The TenComment.Param's Id used to load the dialogue.</param>
        /// <param name="hChatId">The TenComment.Param's Id used to load the comment.</param>
        /// <param name="animId">The animation that KAngel uses when she speaks.</param>
        /// <returns></returns>
        public static List<Playing> AngelTalkToHaters(string aChatId, string hChatId, string animId = "")
        {
            string aChat = aChatId == "" || aChatId == null ? "" : NgoEx.TenTalk(aChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            string hChat = hChatId == "" || hChatId == null ? "" : NgoEx.TenTalk(hChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            List<Playing> list = new List<Playing>();
            list.Add(new Playing(true, "", StatusType.Tension, 1, 0, "", "", "", true, SuperchatType.White, true, hChat));
            list.Add(new Playing(true, aChat, StatusType.Tension, 1, 0, "", "", animId, false, SuperchatType.White, false, ""));
            return list;
        }

        /// <summary>
        /// An ordinary chat comment.
        /// </summary>
        /// <param name="chatId"> The KusoComment.Param's Id used to load the comment. </param>
        /// <returns></returns>
        public static Playing KTalk(string chatId)
        {
            string chat = chatId == "" || chatId == null ? "" : NgoEx.Kome(chatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            return new Playing(false, chat, StatusType.Tension, 1, 0, "", "", "", true, SuperchatType.White, false, "");
        }

        /// <summary>
        /// A Super Chat comment, which KAngel can read near the end of the stream.
        /// </summary>
        /// <param name="kChatId">The KusoComment.Param's Id used to load the comment.</param>
        /// <param name="aChatId">The TenComment.Param's Id used to load her response.</param>
        /// <param name="aAnimId">The animation that KAngel uses when she speaks.</param>
        /// <returns></returns>
        public static Playing SuperKTalk(string kChatId, string aChatId, string aAnimId = "")
        {
            string kChat = kChatId == "" || kChatId == null ? "" : NgoEx.TenTalk(kChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            string aChat = aChatId == "" || aChatId == null ? "" : NgoEx.TenTalk(aChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            return new Playing(false, kChat, StatusType.Tension, 0, 10, aChat, aAnimId, "", true, SuperchatType.White, false, "");
        }


        /// <summary>
        ///  A Super Chat comment, which KAngel can read near the end of the stream. Gives out two or more responses to the same comment.
        /// </summary>
        /// <param name="kChatId">The KusoComment.Param's Id used to load the comment.</param>
        /// <param name="aChatId">The <c>List</c> of TenComment.Param Ids used to load her responses.</param>
        /// <param name="aAnimId">The <c>List</c> of animation Ids that KAngel does when she speaks, per response.</param>
        /// <returns></returns>
        public static Playing SuperKTalk_RepMore(string kChatId, List<string> aChatId, List<string> aAnimId)
        {
            string kChat = kChatId == "" || kChatId == null ? "" : NgoEx.Kome(kChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            string chat = "";
            string anim = "";

            for (int i = 0; i < aChatId.Count; i++)
            {
                if (aChatId[i] == "" || aChatId[i] == null)
                {
                    continue;
                }
                if (i == 0)
                {
                    chat.Concat(NgoEx.TenTalk(aChatId[0], SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value));
                    continue;
                }
                chat.Concat($"___{NgoEx.TenTalk(aChatId[i], SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value)}");
            }
            for (int j = 0; j < aAnimId.Count; j++)
            {
                if (j == 0)
                {
                    chat.Concat(aAnimId[0]);
                    continue;
                }
                chat.Concat($"___{aAnimId[j]}");
            }

            return new Playing(false, kChat, StatusType.Tension, 0, 10, chat, anim, "", false, SuperchatType.White, false, "");
        }


        /// <summary>
        /// A stressful chat comment. Delete these to reduce Stress by -1.
        /// </summary>
        /// <param name="chatId">The KusoComment.Param's Id used to load the comment.</param>
        /// <returns></returns>
        public static Playing KusoKTalk(string chatId)
        {
            string chat = chatId == "" || chatId == null ? "" : NgoEx.Kome(chatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            return new Playing(false, true, chat);
        }

        /// <summary>
        /// Used to load random chat comments, based on follower count. 
        /// <br/><c>"first":</c> Comments that usually appear at the start of the stream.
        /// <br/><c>"middle":</c> Comments that usually appear around the middle of the stream.
        /// <br/><c>"last":</c> Comments that usually appear when the stream ends.
        /// <br/><c>"rainbow":</c> Creates a super chat rainbow.
        /// </summary>
        /// <remarks>You can also use other states such as <c>"delete"</c> and <c>"deleteAll"</c> to remove stream comments during a stream. 
        /// <br/>"delete" and "deleteAll" will not delete superchats, in addition, any stressful comments deleted with this command will not apply -1 Stress.</remarks>
        /// <param name="state">What random chat comments are loaded. Refer to the summary for more details.</param>
        /// <returns></returns>
        public static Playing SetMobs(string state = "middle")
        {
            if (!(state == "first" || state == "last" || state == "rainbow" || state == "delete" || state == "deleteAll"))
            {
                state = "middle";
            }
            return new Playing(state);
        }


        /// <summary>
        /// The time when KAngel starts reading super chat comments.
        /// </summary>
        /// <returns></returns>
        public static Playing ReadComments()
        {

            return new Playing(true);
        }


        /// <summary>
        /// Plays a sound during a stream.
        /// </summary>
        /// <param name="sound">The sound that plays.</param>
        /// <param name="looping">If the sound loops or not.</param>
        /// <returns></returns>
        public static Playing PlaySound(SoundType sound, bool looping)
        {
            return new Playing(sound, looping);
        }

        /// <summary>
        /// Plays a border effect during a stream.
        /// </summary>
        /// <param name="chanceEffect">The type of border effect.</param>
        /// <param name="state">The border effect state.</param>
        /// <returns></returns>
        public static Playing PlayEffect(ChanceEffectType chanceEffect, CEffectState state)

        {
            Dictionary<CEffectState, string> dict = new Dictionary<CEffectState, string>()
            {
                {CEffectState.In, "in" },
                {CEffectState.Win_Stop, "win_stop" },
                {CEffectState.Win, "win" },
                {CEffectState.Out, "out"}
            };
            dict.TryGetValue(state, out string value);
            return new Playing(chanceEffect, value);
        }
    }

    /// <summary>
    /// Different behaviour of how stream chat comments show in a stream.
    /// <br/><c>Normal:</c> Comments can be selected and deleted.
    /// <br/><c>Uncontrollable:</c> Comments are unselectable and are grayed out.
    /// <br/><c>Celebration:</c> Comments are unselectable and are either super chats or grayed out randomly.
    /// </summary>
    public enum StreamChatType
    {
        Normal, Uncontrollable, Celebration
    }

    /// <summary>
    /// The states used to load in a border effect.
    /// <list type="bullet">
    /// <item>
    /// <term>In</term>
    /// <description>Effect loads in and is set into its first state.</description>
    /// </item>
    ///<item>
    /// <term>Win_Stop</term>
    /// <description>Animation is paused.</description>
    /// </item>
    ///  <item>
    /// <term>Win</term>
    /// <description>If effect is loaded in, it is set into its primary state.</description>
    /// </item>
    ///  <item>
    /// <term>Out</term>
    /// <description>Exits the effect.</description>
    /// </item>
    /// </list>
    /// </summary>
    public enum CEffectState
    {
        In, Win_Stop, Win, Out
    }

    /// <summary>
    /// In-game sprites that are used for a stream's background. You can load these into a custom stream using <c>SetExtStreamBG</c>.
    /// </summary>
    public enum StreamBackground
    {
        Default, Silver, Gold, MileOne, MileTwo, MileThree, MileFour, MileFive, Guru, Horror, BigHouse, Roof, Black, Void, None = 1000
    }

    public class StreamSettings
    {
        /// <summary>
        /// Sets the stream title.
        /// </summary>
        /// <param name="id">The id associated with a TenComment param object.</param>
        public static string SetStreamTitle(string id)
        {
            LanguageType lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            return NgoEx.TenTalk(id, lang);
        }
        /// <summary>
        /// Sets the behaviour of stream chat comments in a stream.
        /// </summary>
        /// <param name="chatType">The StreamChatType of the stream.</param>
        public static void SetStreamChatType(StreamChatType chatType)
        {
            switch (chatType)
            {
                case StreamChatType.Uncontrollable:
                    SingletonMonoBehaviour<Live>.Instance.isUncontrollable = true;
                    SingletonMonoBehaviour<Live>.Instance.isOiwai = false;
                    break;
                case StreamChatType.Celebration:
                    SingletonMonoBehaviour<Live>.Instance.isOiwai = true;
                    SingletonMonoBehaviour<Live>.Instance.isUncontrollable = false;
                    break;
                default:
                    SingletonMonoBehaviour<Live>.Instance.isOiwai = false;
                    SingletonMonoBehaviour<Live>.Instance.isUncontrollable = false;
                    return;
            }
        }

        /// <summary>
        /// Sets the animation for when KAngel starts reading super chat comments.
        /// </summary>
        /// <param name="reactAnim">The name of the animation to play.</param>
        public static void SetCustomReactAnim(string reactAnim)
        {
            StreamExtender.currentReactAnim = reactAnim;
        }

        /// <summary>
        /// Sets whether the splash screen after a stream ends shows up or not.
        /// </summary>
        /// <remarks> Note: This only affects the end screen if it is planned to show up after a stream, 
        /// as some endings affect how the end screen is shown.</remarks>
        /// <param name="hasScreen">If false, the end screen will not appear after a stream.</param>
        public static void HasEndSplashScreen(bool hasScreen)
        {
            StreamExtender.withEndScreen = hasScreen;
        }

        /// <summary>
        /// Sets the base number of people that are watching a custom stream. <br/>Number cannot be lower than 192, however it can be set to 0.
        /// </summary>
        /// <remarks>If set to 0, the number will not randomly refresh throughout the stream.</remarks>
        /// <param name="watchNum">The base number of people that are watching a custom stream.</param>
        public static void SetExtWatchingNum(int watchNum)
        {
            StreamExtender.watchingNum = watchNum;
        }

        /// <summary>
        /// Sets a custom background for a custom stream.
        /// </summary>
        /// <param name="sprite">The custom sprite used to set the image.</param>
        /// <exception cref="ArgumentException">Throws if the sprite is null.</exception>
        public static void SetExtStreamBG(Sprite sprite)
        {
            if (sprite == null)
            {
                throw new ArgumentException("\"sprite\" can't be null."); ;
            }
            StreamExtender.customStreamBG = sprite;
        }
        /// <summary>
        /// Sets a custom background for a custom stream.
        /// </summary>
        /// <param name="bg">The in-game sprite used to set the image.</param>
        /// <exception cref="ArgumentException">Throws if the sprite is null.</exception>
        public static void SetExtStreamBG(StreamBackground bg)
        {
            if (bg == StreamBackground.None)
            {
                throw new ArgumentException("\"bg\" can't be StreamBackground.None.");
            }
            StreamExtender.streamBG = bg;
        }
    }



    [HarmonyPatch]
    public class StreamExtender
    {
        internal static bool isCustom = false;
        internal static bool isDarkAngel = false;

        internal static string currentFirstAnim = "";
        internal static string currentReactAnim = "";
        internal static bool withEndScreen = true;

        internal static int? watchingNum = null;
        internal static StreamBackground streamBG = StreamBackground.None;
        internal static Sprite customStreamBG = null;

        internal static string actionStreamId = null;
        internal static List<TweetType> actionTweetType = null;

        static ExtNgoStream currentCustomStream;
        internal static List<ExtActionNgoStream> actionStreamList = new List<ExtActionNgoStream>();
        static List<ExtNgoStream> conditionalStreamList = new List<ExtNgoStream>();

        /// <summary>
        /// Caches the first animation to be used in a later stream. 
        /// Recommnended if KAngel is the first one to speak in a stream. 
        /// </summary>
        /// <remarks><c>Action_HaishinStart</c> already does this process.</remarks>
        /// <param name="anim"></param>
        /// <returns></returns>
        public static async UniTask InitializeFirstAnim(string anim)
        {
            isCustom = true;
            currentFirstAnim = anim;
            HaishinFirstAnimation.LoadHaishinFirstAnimation().Forget();
        }
        /// <summary>
        /// Starts a custom stream, based on the contents from your <c>ExtNgoStream.</c> <br/>Make sure the <c>ExtNgoStream</c> you use isn't static.
        /// </summary>
        /// <typeparam name="T">The <c>ExtNgoStream</c> data used to load your custom stream.</typeparam>
        /// <param name="isDarkUI"> If true, loads in the Dark UI for the stream, otherwise uses the default stream UI.</param>
        /// <param name="isDarkAnim">If true, uses the Dark Angel transformation before a stream, otherwise uses the default KAngel transformation.</param>
        public static void StartCustomStream<T>(bool isDarkUI = false, bool isDarkAnim = false) where T : ExtNgoStream
        {
            Type type = typeof(T);
            ExtNgoStream stream = Activator.CreateInstance(type) as ExtNgoStream;

            StartCustomStream(stream, isDarkUI, isDarkAnim);

        }

        /// <summary>
        /// Saves a custom stream to be used later when the Broadcast window is opened. 
        /// Useful if you want to start a stream without the transformation playing.
        /// </summary>
        /// <remarks>Will only be saved for one upcoming stream only. (it will reset after a stream)</remarks>
        /// <typeparam name="T"> The custom stream to load later on.</typeparam>
        /// <returns></returns>
        public static async UniTask AwaitCustomStream<T>() where T : ExtNgoStream
        {
            Type type = typeof(T);
            ExtNgoStream stream = Activator.CreateInstance(type) as ExtNgoStream;
            await AwaitCustomStream(stream);
        }

        /// <summary>
        /// Saves a custom stream to be used later when the Broadcast window is opened. 
        /// Useful if you want to start a stream without the transformation playing.
        /// </summary>
        /// <remarks>Will only be saved for one upcoming stream only. (it will reset after a stream)</remarks>
        /// <param name="stream"> The custom stream to load later on.</param>
        /// <returns></returns>
        public static async UniTask AwaitCustomStream(ExtNgoStream stream)
        {
            isCustom = true;
            currentCustomStream = stream;
            if (!(stream.StartingAnim == "" || stream.StartingAnim == null))
            {
                currentFirstAnim = stream.StartingAnim;
            }
            HaishinFirstAnimation.LoadHaishinFirstAnimation().Forget();
        }

        /// <summary>
        /// Sets the Dark Angel transformation scene to play instead of the normal transformation scene the next time the Transform! window appears.
        /// </summary>
        /// <remarks>Will only play for one upcoming Transform! scene only. (it will reset after a stream)</remarks>
        public static void AwaitDarkAngelTransform()
        {
            isDarkAngel = true;
        }

        /// <summary>
        /// Starts a custom stream, based on the contents from your <c>ExtNgoStream.</c> <br/>Make sure the <c>ExtNgoStream</c> you use isn't static.
        /// </summary>
        /// <param name="stream">The <c>ExtNgoStream</c> data used to load your custom stream. Make sure the <c>ExtNgoStream</c> you use isn't static.</param>
        /// <param name="isDarkUI"> If true, loads in the Dark UI for the stream, otherwise uses the default stream UI.</param>
        /// <param name="isDarkAnim">If true, uses the Dark Angel transformation before a stream, otherwise uses the default KAngel transformation.</param>
        public static void StartCustomStream(ExtNgoStream stream, bool isDarkUI = false, bool isDarkAnim = false)
        {

            isCustom = true;
            currentCustomStream = stream;
            isDarkAngel = isDarkAnim;
            if (!(stream.StartingAnim == "" || stream.StartingAnim == null))
            {
                currentFirstAnim = stream.StartingAnim;
            }
            SingletonMonoBehaviour<EventManager>.Instance.SetShortcutState(false, 0.2f);
            SingletonMonoBehaviour<EventManager>.Instance.alpha = AlphaType.none;
            if (isDarkUI)
            {
                InitializeFirstAnim(currentFirstAnim);
                SingletonMonoBehaviour<EventManager>.Instance.AddEvent<Action_HaishinDark>();
                return;
            }
            SingletonMonoBehaviour<EventManager>.Instance.AddEvent<Action_HaishinStart>();
        }


        /// <summary>
        /// Adds a custom normal stream to the game, where it will only play if its conditions are met.
        /// <br/> (Normal: the streams you can choose during a night.)
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <typeparam name="T">The custom stream to add.</typeparam>
        public static void AddExtActionStream<T>() where T : ExtActionNgoStream
        {
            Type type = typeof(T);
            ExtActionNgoStream stream = Activator.CreateInstance(type) as ExtActionNgoStream;
            actionStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom normal stream to the game, where it will only play if its conditions are met.
        /// <br/> (Normal: the streams you can choose during a night.)
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <param name="stream">The custom stream to add.</param>
        public static void AddExtActionStream(ExtActionNgoStream stream)
        {
            actionStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom stream to the game, where it will only play if its conditions are met.
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <typeparam name="T">The custom stream to add.</typeparam>
        public static void AddConditionalStream<T>() where T : ExtNgoStream
        {
            Type type = typeof(T);
            ExtNgoStream stream = Activator.CreateInstance(type) as ExtNgoStream;
            conditionalStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom stream to the game, where it will only play if its conditions are met.
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <param name="stream">The custom stream to add.</param>
        public static void AddConditionalStream(ExtNgoStream stream)
        {
            conditionalStreamList.Add(stream);

        }

        /// <summary>
        /// Ends a custom stream. Calculates stats if an ExtActionNgoStream is playing, otherwise just closes the Stream window.
        /// </summary>
        public static void EndCustomStream()
        {
            if (isCustom)
            {
                if (actionStreamId != null)
                {
                    SingletonMonoBehaviour<Live>.Instance.EndHaishin();
                    return;
                }
                SingletonMonoBehaviour<Live>.Instance.HaishinClean();
                return;
            }
            Debug.LogError("No custom stream is currently playing.");
        }

        //Sets any settings and NowPlaying Lists
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TestScenario), "Awake")]
        static bool SetStreamList(TestScenario __instance, ref List<Playing> ___playing, ref Live ____Live)
        {
            if (!isCustom)
            {
                return true;
            }
            __instance.title = currentCustomStream.StreamTitle;
            currentCustomStream.SetStreamSettings();
            Awake_Stub(__instance);
            ___playing.AddRange(currentCustomStream.NowPlaying);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TestScenario), "StartScenario")]
        static bool InitializeExtStream(TestScenario __instance, List<Playing> ___playing, Live ____Live, string ___title)
        {
            if (!isCustom)
            {
                return true;
            }
            AsyncAction();

            async UniTask AsyncAction()
            {
                await StartScenario_Stub(__instance);
                await currentCustomStream.AfterStream();
                ResetCustomStream();
            }
            return false;


        }

        static Sprite ChangeStreamBackground(StreamBackground bg)
        {
            TenchanView view = SingletonMonoBehaviour<TenchanView>.Instance;
            Dictionary<StreamBackground, Sprite> spriteBg = new Dictionary<StreamBackground, Sprite>() {
                { StreamBackground.Default, view.background_no_shield },
                { StreamBackground.Silver, view.background_silver_shield },
                {StreamBackground.Gold, view.background_gold_shield },
                {StreamBackground.MileOne, view.background_kinen1 },
                {StreamBackground.MileTwo, view.background_kinen2 },
                {StreamBackground.MileThree, view.background_kinen3 },
                {StreamBackground.MileFour, view.background_kinen4 },
                {StreamBackground.MileFive, view.background_kinen5 },
                {StreamBackground.Guru, view._background_kyouso },
                {StreamBackground.Horror, view._background_horror },
                {StreamBackground.BigHouse, view._background_happy },
                {StreamBackground.Roof, view._background_sayonara1 }

            };

            if (bg == StreamBackground.Black || bg == StreamBackground.Void || bg == StreamBackground.None)
            {
                return null;
            }
            return spriteBg[bg];
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Live), "Awake")]
        static void SetCustomWatching(Live __instance, ref int ___watcher)
        {

            if (!isCustom || watchingNum == null || watchingNum == 0)
            {
                return;
            }
            if (watchingNum < 0)
            {
                Debug.LogError("Watching number can't be a negative number.");
                return;
            }
            if (watchingNum > 0 && watchingNum < 192)
            {
                Debug.LogError("Watching number can't be lower than 192.");
                return;
            }
            if (watchingNum > 10000000)
            {
                Debug.LogError("Watching number can't be higher than 10 million. Setting number to ten million.");
                ___watcher = 10000000;
                UpdateDetail_Stub(__instance);
                return;
            }
            ___watcher = (int)watchingNum;
            UpdateDetail_Stub(__instance);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Live), "Awake")]
        static void SetCustomBG(Live __instance)
        {
            if (!isCustom)
            {
                return;
            }
            if (customStreamBG != null)
            {
                streamBG = StreamBackground.None;
                __instance.Tenchan._backGround.sprite = customStreamBG;
                return;
            }
            if (streamBG != StreamBackground.None && customStreamBG == null)
            {
                if (streamBG == StreamBackground.Black)
                {
                    __instance.Tenchan._backGround.color = new Color(0f, 0f, 0f, 1f);
                    return;
                }
                else if (streamBG == StreamBackground.Void)
                {
                    __instance.Tenchan._backGround.color = new Color(0f, 0f, 0f, 0f);
                    return;
                }
                else
                {
                    __instance.Tenchan._backGround.sprite = ChangeStreamBackground(streamBG);
                    return;
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Live), "UpdateDetail")]
        static bool NoWatching(Live __instance, ref TMP_Text ___haisinDetail, LanguageType ____lang)
        {
            if (watchingNum == 0)
            {
                ___haisinDetail.text = string.Concat(new string[]
                         {
                    "0 ",
            NgoEx.SystemTextFromType(SystemTextType.Haisin_Watching_Number, ____lang),
            " ・ ",
            NgoEx.SystemTextFromType(SystemTextType.Haisin_Started_Day, ____lang),
            " ",
            NgoEx.DayText(SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.DayIndex), ____lang)
                           });
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HaishinFirstAnimation), "GetEndingHaishinFirstAnimationKey")]
        [HarmonyPatch(typeof(HaishinFirstAnimation), "GetNormalHaishinFirstAnimationKey")]
        static void GetEndingFirstAnim(ref string __result)
        {
            if (currentFirstAnim == "" || currentFirstAnim == null) { return; }
            if (isCustom)
            {
                __result = currentFirstAnim;
            }

        }

        static void ResetCustomStream()
        {
            currentFirstAnim = "";
            currentReactAnim = "";
            withEndScreen = true;
            isCustom = false;
            isDarkAngel = false;
            watchingNum = null;
            streamBG = StreamBackground.None;
            customStreamBG = null;
            currentCustomStream = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Live), "getReadAnimationFromHaishin")]
        static bool SetReadAnim(ref string __result)
        {
            if (isCustom)
            {
                if (currentReactAnim == "" || currentReactAnim == null) { return true; }
                __result = currentReactAnim;
                return false;
            }
            return true;

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(TenchanView), "OnEndStream")]
        static bool IsEndSplash()
        {
            if (!withEndScreen)
            {
                return false;
            }
            return true;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(App_BankView), "Awake")]
        static bool DarkBank(Animator ____anim)
        {
            if (!isDarkAngel)
            {
                return true;
            }
            switch (SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value)
            {
                case LanguageType.EN:
                    ____anim.Play("BankAnimation_Eng_B");
                    break;
                case LanguageType.KO:
                    ____anim.Play("BankAnimation_Kor_B");
                    break;
                case LanguageType.CN:
                case LanguageType.TW:
                    ____anim.Play("BankAnimation_Chn_B");
                    break;
                default:
                    ____anim.Play("BankAnimation_B");
                    break;
            }
            isDarkAngel = false;
            Yield();
            return false;

            async UniTask Yield()
            {
                await UniTask.Yield();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Live), "AddMob")]
        static bool AutoDeleteComments(string haisinPoint, List<LiveComment> ____selectableComments)
        {
            if (haisinPoint == "deleteAll")
            {

                for (int i = 0; i < ____selectableComments.Count; i++)
                {
                    if (____selectableComments[i].playing.color != SuperchatType.White || ____selectableComments[i].isHiroizumi) { continue; }
                    ____selectableComments[i].isHiroizumi = true;
                    AudioManager.Instance.PlaySeByType(SoundType.SE_pien, false);
                    ____selectableComments[i].honbunView.DOColor(new Color(0f, 0f, 0f, 0f), 0.4f).Play<TweenerCore<Color, Color, ColorOptions>>();
                    ____selectableComments[i].isDeleted = true;
                };

                return false;
            }
            if (haisinPoint == "delete")
            {

                if (____selectableComments[____selectableComments.Count - 1].playing.color != SuperchatType.White || ____selectableComments[____selectableComments.Count - 1].isHiroizumi) { return false; }
                ____selectableComments[____selectableComments.Count - 1].isHiroizumi = true;
                AudioManager.Instance.PlaySeByType(SoundType.SE_pien, false);
                ____selectableComments[____selectableComments.Count - 1].honbunView.DOColor(new Color(0f, 0f, 0f, 0f), 0.4f).Play<TweenerCore<Color, Color, ColorOptions>>();
                ____selectableComments[____selectableComments.Count - 1].isDeleted = true;


                return false;
            }
            return true;
        }

        //this is patched manually in the Initializer class, patching Live.SetScenario() as prefix
        static bool InitializeConditionalStream(Live __instance, ref LiveScenario __result)
        {
            if (currentCustomStream != null)
            {
                isCustom = true;
                SingletonMonoBehaviour<StatusManager>.Instance.isTodayHaishined = true;
                SingletonMonoBehaviour<StatusManager>.Instance.UpdateStatus(StatusType.RenzokuHaishinCount, 1);
                __result = __instance.SetScenario<TestScenario>();
                return false;
            }
            if (conditionalStreamList.Count == 0)
            {
                ResetCustomStream();
                return true;
            }
            for (int i = 0; i < conditionalStreamList.Count; i++)
            {
                bool condition = conditionalStreamList[i].SetCondition();
                if (condition)
                {
                    currentCustomStream = conditionalStreamList[i];
                    isCustom = true;
                    SingletonMonoBehaviour<StatusManager>.Instance.isTodayHaishined = true;
                    SingletonMonoBehaviour<StatusManager>.Instance.UpdateStatus(StatusType.RenzokuHaishinCount, 1);
                    __result = __instance.SetScenario<TestScenario>();
                    return false;
                }
            }
            ResetCustomStream();
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Action_HaishinStart), "startEvent", new Type[] { typeof(CancellationToken) })]
        [HarmonyPatch(typeof(Action_HaishinDark), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void IsConditionValid()
        {

            bool horror = SingletonMonoBehaviour<EventManager>.Instance.isHorror;
            EndingType end = SingletonMonoBehaviour<EventManager>.Instance.nowEnding;
            AlphaType alpha = SingletonMonoBehaviour<EventManager>.Instance.alpha;
            int alphalevel = SingletonMonoBehaviour<EventManager>.Instance.alphaLevel;
            if (currentCustomStream == null && !isCustom)
            {
                if (end == EndingType.Ending_None && !horror && actionStreamList.Count != 0)
                {
                    foreach (ExtActionNgoStream ext in actionStreamList)
                    {
                        string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{ext.ActionStreamId}Idea");
                        string streamed = SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.FirstOrDefault(x => x == $"_{ext.ActionStreamId}");
                        if (streamed != null)
                        {
                            continue;
                        }
                        if (discovered != null && alpha == ext.HintData.NetaType && alphalevel == ext.HintData.level)
                        {
                            isCustom = true;
                            currentCustomStream = ext;
                            currentFirstAnim = ext.StartingAnim;
                            actionStreamId = ext.ActionStreamId;
                            actionTweetType = ext.TweetResult;
                            if (ext.SearchResult != null)
                            {
                                try
                                {
                                    ExtTextManager.AddToExtList(new List<EgosaMaster.Param>() { ext.SearchResult });
                                }
                                catch
                                {
                                    Debug.LogError("Could not finish operation: TextExtender is not installed.");
                                }

                            }
                            return;
                        }
                    }
                }
                if (conditionalStreamList.Count == 0)
                {
                    return;
                }
                foreach (ExtNgoStream ext in conditionalStreamList)
                {
                    bool condition = ext.SetCondition();
                    if (condition)
                    {
                        isCustom = true;
                        currentCustomStream = ext;
                        currentFirstAnim = ext.StartingAnim;
                        return;
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Boot), "Awake")]
        static void ResetCustomStreamOnBoot()
        {
            ResetCustomStream();
        }


        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LiveScenario), "Awake")]
        static void Awake_Stub(LiveScenario instance)
        {
            throw new NotImplementedException();
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(LiveScenario), "StartScenario")]
        internal static async UniTask StartScenario_Stub(LiveScenario instance)
        {
            throw new NotImplementedException();
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Live), "UpdateDetail")]
        static void UpdateDetail_Stub(Live instance)
        {
            throw new NotImplementedException();
        }
    }

    [HarmonyPatch]
    public class ActionStreamExtender
    {
        /// <summary>
        /// Checks to see if an <c>ExtActionNgoStream</c> is discovered as an idea. (but not streamed)
        /// </summary>
        /// <param name="actionStream">The <c>ExtActionNgoStream</c> to check for.</param>
        /// <returns></returns>
        public static bool IsActionStreamDiscovered(ExtActionNgoStream actionStream)
        {
            List<string> eventList = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory;
            List<string> actionList = SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory;
            return eventList.Exists(x => x == actionStream.ActionStreamId + "Idea") && !actionList.Exists(x => x == "_" + actionStream.ActionStreamId);

        }

        /// <summary>
        /// Checks to see if an <c>ExtActionNgoStream</c> has been streamed already.
        /// </summary>
        /// <param name="actionStream">The <c>ExtActionNgoStream</c> to check for.</param>
        /// <returns></returns>
        public static bool IsActionStreamStreamed(ExtActionNgoStream actionStream)
        {
            List<string> actionList = SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory;
            return actionList.Exists(x => x == "_" + actionStream.ActionStreamId);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LoadNetaData), "ReadNetaContent")]
        static void SetCustomLabel(ref AlphaTypeToData __result, AlphaType NetaType, int level = 0)
        {
            AlphaLevel gotAlpha = SingletonMonoBehaviour<NetaManager>.Instance.GotAlpha.FirstOrDefault(al => al.alphaType == NetaType && al.level == level);
            AlphaLevel usedAlpha = SingletonMonoBehaviour<NetaManager>.Instance.usedAlpha.FirstOrDefault(al => al.alphaType == NetaType && al.level == level);
            if (StreamExtender.actionStreamList.Count == 0) { return; }
            List<ExtActionNgoStream> actionList = StreamExtender.actionStreamList.FindAll(n => n.HintData.NetaType == NetaType && n.HintData.level == level);
            if (actionList.Count == 0)
            {
                return;
            }
            foreach (ExtActionNgoStream action in actionList)
            {
                ExtActionNgoStream a = StreamExtender.actionStreamList.Find(ac => ac == action);
                string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.ActionStreamId}Idea");
                if (discovered != null)
                {
                    __result = action.HintData;
                    return;
                }
                if (action.SetCondition() && gotAlpha == null)
                {
                    a.isDiscovered = true;
                    __result = action.HintData;
                    return;
                }
                a.isDiscovered = false;

            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "ExecuteActionConfirmed")]
        static void ApplyDiscovered()
        {
            if (StreamExtender.actionStreamList.Count == 0)
            {
                return;
            }
            ExtActionNgoStream a = StreamExtender.actionStreamList.FirstOrDefault(n => n.isDiscovered == true);
            if (a == null)
            {
                return;
            }
            if (a.SetCondition())
            {
                a.isDiscovered = true;
                return;
            }
            a.isDiscovered = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "getHintedChip")]
        static void AddDiscoveredExtChip(ActionType a)
        {
            AlphaType alpha = SingletonMonoBehaviour<EventManager>.Instance.gotChipAlpha.alphaType;
            int alphalevel = SingletonMonoBehaviour<EventManager>.Instance.gotChipAlpha.level;
            if (StreamExtender.actionStreamList.Count == 0) { return; }
            ExtActionNgoStream data = StreamExtender.actionStreamList.FirstOrDefault(n => n.isDiscovered == true);
            if (data == null)
            {
                return;
            }
            if (alpha == data.HintData.NetaType && alphalevel == data.HintData.level && a == data.HintData.getJouken)
            {
                SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.Add($"{data.ActionStreamId}Idea");
                Debug.Log($"{data.ActionStreamId} discovered.");
                data.isDiscovered = false;
            }


        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NgoEvent), "tweetFromTip")]
        static bool AddCustomTweetResult()
        {
            if (StreamExtender.actionTweetType != null)
            {
                foreach (TweetType tweet in StreamExtender.actionTweetType)
                {
                    SingletonMonoBehaviour<PoketterManager>.Instance.AddQueueWithKusoreps(tweet);
                }
                StreamExtender.actionTweetType = null;
                return false;
            }
            StreamExtender.actionTweetType = null;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Action_HaishinEnd), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void AddActionExtToHistory()
        {
            AlphaType alpha = SingletonMonoBehaviour<EventManager>.Instance.alpha;
            int alphaLevel = SingletonMonoBehaviour<EventManager>.Instance.alphaLevel;
            if (StreamExtender.actionStreamId != null && !SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.Contains($"_{StreamExtender.actionStreamId}"))
            {
                SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.Remove($"{alpha}_{alphaLevel}");
                SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.Add($"_{StreamExtender.actionStreamId}");
                Debug.Log($"Added {StreamExtender.actionStreamId} to the Actions History.");
            }
            StreamExtender.actionStreamId = null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "CmdFromType")]
        static void ApplyCustomStreamCmd(ref CmdMaster.Param __result, CmdType type)
        {
            if (StreamExtender.actionStreamId == null) { return; }
            if (StreamExtender.actionStreamList.Count == 0) { return; }
            if ((int)type > 60) { return; }
            ExtActionNgoStream action = StreamExtender.actionStreamList.Find(n => n.ActionStreamId == StreamExtender.actionStreamId);
            string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.ActionStreamId}Idea");
            if (discovered != null && action.CommandResult != null)
            {
                __result = action.CommandResult;
                return;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CommandManager), "OnActionHovered", new Type[] { typeof(AlphaType), typeof(int) })]
        static bool HighlightCustomStreamCmd(AlphaType alpha, int level)
        {
            if (StreamExtender.actionStreamList.Count == 0) { return true; }
            List<ExtActionNgoStream> actionList = StreamExtender.actionStreamList.FindAll(n => n.HintData.NetaType == alpha && n.HintData.level == level);
            if (actionList.Count == 0)
            {
                return true;
            }
            foreach (ExtActionNgoStream action in actionList)
            {
                string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.ActionStreamId}Idea");
                if (discovered != null && action.CommandResult != null)
                {
                    SingletonMonoBehaviour<TooltipManager>.Instance.ShowAction(ActionType.Haishin, action.CommandResult);
                    return false;
                }

            }
            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(StatusManager), "AddDiffToDelta", new Type[] { typeof(StatusDiff), typeof(CmdMaster.Param) })]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static IEnumerable<CodeInstruction> ApplySymbolsAsArgs_Execute(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            Label gameLabel = generator.DefineLabel();
            Label yamiLabel = generator.DefineLabel();
            Label movieLabel = generator.DefineLabel();
            Label sexyLabel = generator.DefineLabel();
            int labelCount = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr)
                {
                    if ((string)codes[i].operand == "Game")
                    {
                        codes.InsertRange(i + 3, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_2),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "@"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, gameLabel)
                        });
                        labelCount++;
                        continue;
                    }
                    if ((string)codes[i].operand == "AnimeTalk")
                    {
                        codes.InsertRange(i + 3, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_2),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "#"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, movieLabel)
                        });
                        labelCount++;
                        continue;
                    }
                    if ((string)codes[i].operand == "Darkness")
                    {
                        codes.InsertRange(i + 3, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_2),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "!"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, yamiLabel)
                        });
                        labelCount++;
                        continue;
                    }
                    if ((string)codes[i].operand == "Hnahaisin")
                    {
                        codes.InsertRange(i + 3, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_2),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "$"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, sexyLabel)
                        });
                        labelCount++;
                        continue;
                    }
                }
                if (codes[i].opcode == OpCodes.Ldarg_0)
                {
                    switch (labelCount)
                    {
                        case 1:
                            codes[i].labels.Add(gameLabel);
                            break;
                        case 2:
                            codes[i].labels.Add(movieLabel);
                            break;
                        case 3:
                            codes[i].labels.Add(yamiLabel);
                            break;
                        case 4:
                            codes[i].labels.Add(sexyLabel);
                            labelCount++;
                            break;
                        default:
                            break;
                    }
                }
            }

            return codes.AsEnumerable();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(FollowerDiffComponent2D), "SetBonus")]
        [HarmonyPatch(typeof(FollowerDiffComponent), "SetBonus")]
        static IEnumerable<CodeInstruction> ApplySymbolsAsArgs_ShowTooltip(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            Label yamiLabel = generator.DefineLabel();
            Label movieLabel = generator.DefineLabel();
            Label sexyLabel = generator.DefineLabel();
            Label gameLabel = generator.DefineLabel();
            Label gameFalse = generator.DefineLabel();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr)
                {

                    if ((string)codes[i].operand == "AnimeTalk")
                    {
                        codes[i + 3].labels.Add(movieLabel);
                        codes.InsertRange(i - 2, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_1),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "#"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, movieLabel)
                        });

                        i += 6;
                        continue;
                    }
                    if ((string)codes[i].operand == "Darkness")
                    {
                        codes[i + 3].labels.Add(yamiLabel);
                        codes.InsertRange(i - 2, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_1),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param), nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "!"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, yamiLabel)
                        });
                        i += 6;
                        continue;
                    }
                    if ((string)codes[i].operand == "Hnahaisin")
                    {
                        codes[i + 3].labels.Add(sexyLabel);
                        codes.InsertRange(i - 2, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_1),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "$"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brtrue, sexyLabel)
                        });
                        break;
                    }
                    if ((string)codes[i].operand == "Game")
                    {
                        codes[i + 22].labels.Add(gameFalse);
                        codes[i + 3].labels.Add(gameLabel);
                        codes[i + 2].opcode = OpCodes.Brtrue;
                        codes[i + 2].operand = gameLabel;
                        codes.InsertRange(i + 3, new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_1),
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CmdMaster.Param),nameof(CmdMaster.Param.Id))),
                            new CodeInstruction(OpCodes.Ldstr, "@"),
                            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string),nameof(string.Contains),new Type[]{typeof(string)})),
                            new CodeInstruction(OpCodes.Brfalse, gameFalse)
                        });
                        i += 6;
                        continue;
                    }
                }

            }

            return codes.AsEnumerable();
        }
    }

    /// <summary>
    /// The blueprint used to load in a custom stream. <br/>Make sure your variable of this class isn't static, so that your stream dialogue and your comment's language can be updated if this stream is used more than once.
    /// </summary>
    public abstract class ExtNgoStream
    {
        /// <summary>
        /// The stream title.
        /// </summary>
        public virtual string StreamTitle { get; }
        /// <summary>
        /// The starting animation of a stream; only applies if KAngel is the first one talking.
        /// </summary>
        public virtual string StartingAnim { get => "stream_cho_akaruku"; }

        /// <summary>
        /// The playing list.
        /// </summary>
        public virtual List<Playing> NowPlaying { get; }
        /// <summary>
        ///  The condition for the event. 
        ///  <br/>Event will only start if the condition returns true. 
        /// </summary>
        /// <remarks><c>SetCondition</c> is not read if this ExtNgoStream is used with <c>StartCustomStream.</c></remarks>
        public abstract bool SetCondition();

        /// <summary>
        /// Sets any relevant attributes to the stream before it starts (music, effects, etc).
        /// <br/> You can set more attributes to the stream by changing fields from <c>Live</c>, using a Singleton to create an instance for <c>Live</c>.
        /// </summary>
        public abstract void SetStreamSettings();

        /// <summary>
        /// What happens after the stream script ends.
        /// </summary>
        public abstract UniTask AfterStream();


    }

    public abstract class ExtActionNgoStream : ExtNgoStream
    {
        protected internal bool isDiscovered = false;
        public abstract string ActionStreamId { get; }
        public abstract AlphaTypeToData HintData { get; }

        public abstract List<TweetType> TweetResult { get; }

        public virtual EgosaMaster.Param SearchResult { get; }

        public virtual CmdMaster.Param CommandResult { get => null; }

    }

}
