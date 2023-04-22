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
using System.Threading;
using TMPro;
using UnityEngine;

namespace NGOEventExtender
{
    public class PlayingList
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
        /// A Super Chat comment, which KAngel can read near the end of the stream. Gives out two responses to the same comment.
        /// </summary>
        /// <param name="kChatId">The KusoComment.Param's Id used to load the comment.</param>
        /// <param name="aChatId">The TenComment.Param's Id used to load her first response.</param>
        /// <param name="aChatTwoId">The TenComment.Param's Id used to load her second response.</param>
        /// <param name="aAnimId">The animation that KAngel uses when she speaks her first response.</param>
        /// <param name="aAnimTwoId">The animation that KAngel uses when she speaks her second response.</param>
        /// <returns></returns>
        public static Playing SuperKTalk_RepTwice(string kChatId, string aChatId, string aChatTwoId, string aAnimId = "", string aAnimTwoId = "")
        {
            string kChat = kChatId == "" || kChatId == null ? "" : NgoEx.TenTalk(kChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            string aChat = aChatId == "" || aChatId == null ? "" : NgoEx.TenTalk(aChatId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            string aChatTwo = aChatTwoId == "" || aChatTwoId == null ? "" : NgoEx.TenTalk(aChatTwoId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            return new Playing(false, kChat, StatusType.Tension, 0, 10, aChat + "___" + aChatTwo, $"{aAnimId}___{aAnimTwoId}", "", false, SuperchatType.White, false, "");
        }

        /// <summary>
        ///  A Super Chat comment, which KAngel can read near the end of the stream. Gives out three or more responses to the same comment.
        /// </summary>
        /// <param name="kChatId">The KusoComment.Param's Id used to load the comment.</param>
        /// <param name="aChatId">The <c>List</c> of TenComment.Param Ids used to load her responses.</param>
        /// <param name="aAnimId">The <c>List</c> of animation Ids that KAngel does when she speaks, per response.</param>
        /// <returns></returns>
        public static Playing SuperKTalk_RepConvo(string kChatId, List<string> aChatId, List<string> aAnimId)
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
        /// <br/>However this can only be used if the stream type is set to Uncontrollable.</remarks>
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
        internal static TweetType actionTweetType = TweetType.None;

        static ExtStream currentCustomStream;
        internal static List<ExtActionStream> actionStreamList = new List<ExtActionStream>();
        static List<ExtStream> conditionalStreamList = new List<ExtStream>();

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
            await HaishinFirstAnimation.LoadHaishinFirstAnimation();
        }
        /// <summary>
        /// Starts a custom stream, based on the contents from your <c>ExtStream.</c> <br/>Make sure the <c>ExtStream</c> you use isn't static.
        /// </summary>
        /// <typeparam name="T">The <c>ExtStream</c> data used to load your custom stream.</typeparam>
        /// <param name="isDarkUI"> If true, loads in the Dark UI for the stream, otherwise uses the default stream UI.</param>
        /// <param name="isDarkAnim">If true, uses the Dark Angel transition before a stream, otherwise uses the default KAngel transition.</param>
        public static void StartCustomStream<T>(bool isDarkUI = false, bool isDarkAnim = false)
        {
            Type type = typeof(T);
            ExtStream stream = Activator.CreateInstance(type) as ExtStream;

            StartCustomStream(stream, isDarkUI, isDarkAnim);

        }

        /// <summary>
        /// Starts a custom stream, based on the contents from your <c>ExtStream.</c> <br/>Make sure the <c>ExtStream</c> you use isn't static.
        /// </summary>
        /// <param name="stream">The <c>ExtStream</c> data used to load your custom stream. Make sure the <c>ExtStream</c> you use isn't static.</param>
        /// <param name="isDarkUI"> If true, loads in the Dark UI for the stream, otherwise uses the default stream UI.</param>
        /// <param name="isDarkAnim">If true, uses the Dark Angel transition before a stream, otherwise uses the default KAngel transition.</param>
        public static void StartCustomStream(ExtStream stream, bool isDarkUI = false, bool isDarkAnim = false)
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
        public static void AddExtActionStream<T>() where T : ExtActionStream
        {
            Type type = typeof(T);
            ExtActionStream stream = Activator.CreateInstance(type) as ExtActionStream;
            actionStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom normal stream to the game, where it will only play if its conditions are met.
        /// <br/> (Normal: the streams you can choose during a night.)
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <param name="stream">The custom stream to add.</param>
        public static void AddExtActionStream(ExtActionStream stream)
        {
            actionStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom stream to the game, where it will only play if its conditions are met.
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <typeparam name="T">The custom stream to add.</typeparam>
        public static void AddConditionalStream<T>() where T : ExtStream
        {
            Type type = typeof(T);
            ExtStream stream = Activator.CreateInstance(type) as ExtStream;
            conditionalStreamList.Add(stream);
        }

        /// <summary>
        /// Adds a custom stream to the game, where it will only play if its conditions are met.
        /// <br/> Useful if you want to replace a queued stream with your own.
        /// </summary>
        /// <remarks>Your streams take priority over others, so make sure your conditions are specific.</remarks>
        /// <param name="stream">The custom stream to add.</param>
        public static void AddConditionalStream(ExtStream stream)
        {
            conditionalStreamList.Add(stream);

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
                if (SingletonMonoBehaviour<Live>.Instance.isUncontrollable)
                {
                    for (int i = 0; i < ____selectableComments.Count; i++)
                    {
                        AudioManager.Instance.PlaySeByType(SoundType.SE_pien, false);
                        ____selectableComments[i].honbunView.DOColor(new Color(0f, 0f, 0f, 0f), 0.4f).Play<TweenerCore<Color, Color, ColorOptions>>();
                    };
                }
                return false;
            }
            if (haisinPoint == "delete")
            {
                if (SingletonMonoBehaviour<Live>.Instance.isUncontrollable)
                {
                    AudioManager.Instance.PlaySeByType(SoundType.SE_pien, false);
                    ____selectableComments[____selectableComments.Count - 1].honbunView.DOColor(new Color(0f, 0f, 0f, 0f), 0.4f).Play<TweenerCore<Color, Color, ColorOptions>>();

                }
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
                    foreach (ExtActionStream ext in actionStreamList)
                    {
                        string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{ext.Id}Idea");
                        string streamed = SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.FirstOrDefault(x => x == $"_{ext.Id}");
                        if (streamed != null)
                        {
                            continue;
                        }
                        if (discovered != null && alpha == ext.LabelData.NetaType && alphalevel == ext.LabelData.level)
                        {
                            isCustom = true;
                            currentCustomStream = ext;
                            currentFirstAnim = ext.StartingAnim;
                            actionStreamId = ext.Id;
                            actionTweetType = ext.TweetId;
                            if (ext.SearchResult != null)
                            {
                                try
                                {
                                    ExtTextManager.AddToExtList<EgosaMaster.Param>(new List<EgosaMaster.Param>() { ext.SearchResult });
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
                foreach (ExtStream ext in conditionalStreamList)
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
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LoadNetaData), "ReadNetaContent")]
        static void SetCustomLabel(ref AlphaTypeToData __result, AlphaType NetaType, int level = 0)
        {
            AlphaLevel gotAlpha = SingletonMonoBehaviour<NetaManager>.Instance.GotAlpha.FirstOrDefault(al => al.alphaType == NetaType && al.level == level);
            AlphaLevel usedAlpha = SingletonMonoBehaviour<NetaManager>.Instance.usedAlpha.FirstOrDefault(al => al.alphaType == NetaType && al.level == level);
            if (StreamExtender.actionStreamList.Count == 0) { return; }
            List<ExtActionStream> actionList = StreamExtender.actionStreamList.FindAll(n => n.LabelData.NetaType == NetaType && n.LabelData.level == level);
            if (actionList.Count == 0)
            {

                return;
            }
            foreach (ExtActionStream action in actionList)
            {
                ExtActionStream a = StreamExtender.actionStreamList.Find(ac => ac == action);
                string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.Id}Idea");
                if (discovered != null)
                {
                    a.isDiscovered = false;
                    __result = action.LabelData;
                    return;
                }
                if (action.SetCondition() && gotAlpha == null)
                {
                    a.isDiscovered = true;
                    __result = action.LabelData;
                    return;
                }
                a.isDiscovered = false;

            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "getHintedChip")]
        static void AddDiscoveredExtChip(ActionType a)
        {
            AlphaType alpha = SingletonMonoBehaviour<EventManager>.Instance.gotChipAlpha.alphaType;
            int alphalevel = SingletonMonoBehaviour<EventManager>.Instance.gotChipAlpha.level;
            if (StreamExtender.actionStreamList.Count == 0) { return; }
            ExtActionStream data = StreamExtender.actionStreamList.FirstOrDefault(n => n.isDiscovered == true);
            if (data == null)
            {
                return;
            }
            if (alpha == data.LabelData.NetaType && alphalevel == data.LabelData.level && a == data.LabelData.getJouken)
            {
                SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.Add($"{data.Id}Idea");
                Debug.Log($"{data.Id} discovered.");
            }


        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NgoEvent), "tweetFromTip")]
        static bool AddCustomTweetResult()
        {
            if (StreamExtender.actionTweetType != TweetType.None)
            {
                SingletonMonoBehaviour<PoketterManager>.Instance.AddQueueWithKusoreps(StreamExtender.actionTweetType);
                StreamExtender.actionTweetType = TweetType.None;
                return false;
            }
            StreamExtender.actionTweetType = TweetType.None;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Action_HaishinEnd), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void AddActionExtToHistory()
        {
            if (StreamExtender.actionStreamId != null && !SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.Contains($"_{StreamExtender.actionStreamId}"))
            {
                SingletonMonoBehaviour<EventManager>.Instance.dayActionHistory.Add($"_{StreamExtender.actionStreamId}");
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
            ExtActionStream action = StreamExtender.actionStreamList.Find(n => n.Id == StreamExtender.actionStreamId);
            string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.Id}Idea");
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
            List<ExtActionStream> actionList = StreamExtender.actionStreamList.FindAll(n => n.LabelData.NetaType == alpha && n.LabelData.level == level);
            if (actionList.Count == 0)
            {
                return true;
            }
            foreach (ExtActionStream action in actionList)
            {
                string discovered = SingletonMonoBehaviour<EventManager>.Instance.eventsHistory.FirstOrDefault(x => x == $"{action.Id}Idea");
                if (discovered != null && action.CommandResult != null)
                {
                    SingletonMonoBehaviour<TooltipManager>.Instance.ShowAction(ActionType.Haishin, action.CommandResult);
                    return false;
                }

            }
            return true;
        }
    }

    /// <summary>
    /// The blueprint used to load in a custom stream. <br/>Make sure your variable of this class isn't static, so that your stream dialogue and your comment's language can be updated if this stream is used more than once.
    /// </summary>
    public abstract class ExtStream
    {

        /// <summary>
        /// The custom stream's title. For localization purposes, use <c>"NgoEx.TenTalk(stringId, LanguageType)"</c>
        /// </summary>
        public abstract string StreamTitle { get; }
        /// <summary>
        /// The starting animation of a stream; only applies if KAngel is the first one talking.
        /// </summary>
        public abstract string StartingAnim { get; }
        /// <summary>
        /// The list that sets up the script of your stream.
        /// </summary>
        /// <remarks> Add dialogue and chat comments in the list by using methods from the <c>StreamList</c> class, or by making new <c>Playing</c> objects.</remarks>
        public abstract List<Playing> NowPlaying { get; }
        /// <summary>
        ///  The condition for the event. 
        ///  <br/>Event will only start if the condition returns true. 
        /// </summary>
        /// <remarks><c>SetCondition</c> is not read if this ExtStream is used with <c>StartCustomStream.</c></remarks>
        public abstract bool SetCondition();

        /// <summary>
        /// Sets any relevant attributes to the stream before it starts (music, effects, etc).
        /// <br/> You can set more attributes to the stream by changing fields from <c>Live</c>, using a Singleton to create an instance for <c>Live</c>.
        /// </summary>
        public abstract void SetStreamSettings();

        /// <summary>
        /// What happens after the stream script ends.
        /// </summary>
        /// <returns></returns>
        public abstract UniTask AfterStream();

    }

    public abstract class ExtActionStream : ExtStream
    {
        protected internal bool isDiscovered = false;
        public abstract string Id { get; }

        public abstract TweetType TweetId { get; }
        public abstract AlphaTypeToData LabelData { get; }

        public virtual EgosaMaster.Param SearchResult { get => null; }

        public virtual CmdMaster.Param CommandResult { get => null; }
    }
}
