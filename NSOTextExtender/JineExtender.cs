using Cysharp.Threading.Tasks;
using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class JineExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtJineParam_Num.json"));
        //public static List<LineMaster.Param> ExtList = JsonConvert.DeserializeObject<List<LineMaster.Param>>(json);
        public static List<LineMaster.Param> ExtList = new List<LineMaster.Param>();

        /// <summary>
        /// Create a custom user-made Jine message sent by Ame.
        /// </summary>
        /// <param name="id">The LineMaster.Param's Id used to load the message.</param>
        /// <exception cref="NullReferenceException"></exception>
        public static async UniTask StartExtAmeJine(string id)
        {

            LineMaster.Param exJineId = JineExtender.ExtList.Find((LineMaster.Param j) => j.Id == id);
            JineType jineData = ExtTextManager.GetUniqueIdNum<JineType>(exJineId.Id);
            await SingletonMonoBehaviour<JineManager>.Instance.AddJineHistory(jineData);


        }
        /// <summary>
        /// Create a Jine message sent by Ame.
        /// </summary>
        /// <param name="jineData">The JineType used to load the message. When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <exception cref="NullReferenceException"></exception>
        public static async UniTask StartAmeJine(JineType jineData)
        {
            await SingletonMonoBehaviour<JineManager>.Instance.AddJineHistory(jineData);
        }

        /// <summary>
        /// Creates a list of chooseable options on Jine. Use <c>SetExtPiOption</c> after based on the options listed.
        /// </summary>
        /// <param name="optionFiveId">The LineMaster.Param's Id used to load the fifth option. (optional)</param>
        /// <param name="optionFourId">The LineMaster.Param's Id used to load the fourth option. (optional)</param>
        /// <param name="optionThreeId">The LineMaster.Param's Id used to load the third option. (optional)</param>
        /// <param name="optionTwoId">The LineMaster.Param's Id used to load the second option. (optional)</param>
        /// <param name="optionOneId">The LineMaster.Param's Id used to load the first option.</param>
        public static void StartExtPiOptionList(string optionOneId, string optionTwoId = null, string optionThreeId = null, string optionFourId = null, string optionFiveId = null)
        {
            try
            {
                List<JineType> piList = new List<JineType>();
                AddToOptionList(optionOneId, piList);
                if (optionTwoId != null)
                {
                    AddToOptionList(optionTwoId, piList);
                }
                if (optionThreeId != null)
                {
                    AddToOptionList(optionThreeId, piList);
                }
                if (optionFourId != null)
                {
                    AddToOptionList(optionFourId, piList);
                }
                if (optionFiveId != null)
                {
                    AddToOptionList(optionFiveId, piList);
                }
                SingletonMonoBehaviour<JineManager>.Instance.StartOption(piList);
            }
            catch { Debug.LogError("Enumeration Test: One of these Jines do not exist!"); }

            void AddToOptionList(string s, List<JineType> list)
            {
                LineMaster.Param option = ExtList.Find((LineMaster.Param j) => j.Id == s);
                JineType jineData = ExtTextManager.GetUniqueIdNum<JineType>(option.Id);
                list.Add(jineData);
            }
        }

        /// <summary>
        /// Creates a list of chooseable options on Jine. Use <c>SetPiOption</c> after based on the options listed.
        /// </summary>
        /// <param name="optionFive">The JineType used to load the fifth option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionFour">The JineType used to load the fourth option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionThree">The JineType used to load the third option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionTwo"> The JineType used to load the second option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionOne"> The JineType used to load the first option. When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        public static void StartPiOptionList(JineType optionOne, JineType optionTwo = JineType.None, JineType optionThree = JineType.None, JineType optionFour = JineType.None, JineType optionFive = JineType.None)
        {
            try
            {
                List<JineType> piList = new List<JineType>();
                AddToOptionList(optionOne, piList);
                if (optionTwo != JineType.None)
                {
                    AddToOptionList(optionTwo, piList);
                }
                if (optionThree != JineType.None)
                {
                    AddToOptionList(optionThree, piList);
                }
                if (optionFour != JineType.None)
                {
                    AddToOptionList(optionFour, piList);
                }
                if (optionFive != JineType.None)
                {
                    AddToOptionList(optionFive, piList);
                }
                SingletonMonoBehaviour<JineManager>.Instance.StartOption(piList);
            }
            catch { Debug.LogError("Enumeration Test: One of these Jines do not exist!"); }

            void AddToOptionList(JineType jineData, List<JineType> list)
            {
                list.Add(jineData);
            }
        }

        /// <summary>
        /// Sends one custom user-made Jine message from P-chan based on the option chosen from <c>StartExtPiOptionList</c>. Using async methods for the <c>Action</c> parameters are recommended.
        /// </summary>
        /// <param name="id">The LineMaster.Param's Id used to load the message.</param>
        /// <param name="action">The <c>Action</c> method after this message is sent.</param>
        public static void StartExtPiOption(string id, Action action)
        {

            LineMaster.Param exJineId = ExtList.Find((LineMaster.Param j) => j.Id == id);
            JineType jineData = ExtTextManager.GetUniqueIdNum<JineType>(exJineId.Id);
            SingletonMonoBehaviour<JineManager>.Instance.OnChangeHistory.Where((CollectionAddEvent<JineData> x) => x.Value.id == jineData).Subscribe(async delegate (CollectionAddEvent<JineData> _)
            {
                action();
                ExtTextManager.ClearExDisposible();

            }).AddTo(ExtTextManager.CompositeDisposible);
        }

        /// <summary>
        /// Sends one custom user-made Jine message from P-chan based on the option chosen from <c>StartExtPiOptionList</c>. Using async methods for the <c>Action</c> parameters are recommended.
        /// </summary>
        /// <param name="jineData">The JineType used to load the message. When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="action">The <c>Action</c> method after this message is sent.</param>
        public static void StartPiOption(JineType jineData, Action action)
        {
            SingletonMonoBehaviour<JineManager>.Instance.OnChangeHistory.Where((CollectionAddEvent<JineData> x) => x.Value.id == jineData).Subscribe(async delegate (CollectionAddEvent<JineData> _)
            {
                action();
                ExtTextManager.ClearExDisposible();

            }).AddTo(ExtTextManager.CompositeDisposible);
        }

        /// <summary>
        /// Creates a list of chooseable options on Jine, then sends one custom user-made Jine message from P-chan based on the option chosen. Up to five options can be created. Using async methods for the <c>Action</c> parameters are recommended.
        /// </summary>
        /// <remarks> Note: If an optional option Id is not null, its related <c>Action</c> must be filled, or else the option won't appear on the list. (i.e. filling in the second option Id without including its related <c>Action</c>.)</remarks>
        /// <exception cref="NullReferenceException"></exception>
        /// <param name="actionFive">The <c>Action</c> method related to the fifth option. Required if the fifth option Id is not null or "".</param>
        /// <param name="actionFour">The <c>Action</c> method related to the fourth option. Required if the fourth option Id is not null or "".</param>
        /// <param name="actionThree">The <c>Action</c> method related to the third option. Required if the third option Id is not null or "".</param>
        /// <param name="actionTwo">The <c>Action</c> method related to the second option. Required if the second option Id is not null or "".</param>
        /// <param name="actionOne">The <c>Action</c> method related to the first option.</param>
        /// <param name="optionFiveId">The LineMaster.Param's Id used to load the fifth option. (optional)</param>
        /// <param name="optionFourId">The LineMaster.Param's Id used to load the fourth option. (optional)</param>
        /// <param name="optionThreeId">The LineMaster.Param's Id used to load the third option. (optional)</param>
        /// <param name="optionTwoId">The LineMaster.Param's Id used to load the second option. (optional)</param>
        /// <param name="optionOneId">The LineMaster.Param's Id used to load the first option.</param>
        public static void StartExtPiListAndSet(string optionOneId, Action actionOne, string optionTwoId = null, Action actionTwo = null, string optionThreeId = null, Action actionThree = null, string optionFourId = null, Action actionFour = null, string optionFiveId = null, Action actionFive = null)
        {
            if (optionTwoId == "") { optionTwoId = null; }
            if (optionThreeId == "") { optionThreeId = null; }
            if (optionFourId == "") { optionFourId = null; }
            if (optionFiveId == "") { optionFiveId = null; }
            StartExtPiOptionList(optionOneId, optionTwoId, optionThreeId, optionFourId, optionFiveId);
            StartExtPiOption(optionOneId, actionOne);
            if (optionTwoId != null && actionTwo != null)
            {
                StartExtPiOption(optionTwoId, actionTwo);
            }
            if (optionThreeId != null && actionThree != null)
            {
                StartExtPiOption(optionThreeId, actionThree);
            }
            if (optionFourId != null && actionFour != null)
            {
                StartExtPiOption(optionFourId, actionFour);
            }
            if (optionFiveId != null && actionFive != null)
            {
                StartExtPiOption(optionFiveId, actionFive);
            }
        }



        /// <summary>
        /// Creates a list of chooseable options on Jine, then sends one custom user-made Jine message from P-chan based on the option chosen. Up to five options can be created. Using async methods for the <c>Action</c> parameters are recommended.
        /// </summary>
        /// <remarks> Note: If an optional option Id is not null, its related <c>Action</c> must be filled, or else the option won't appear on the list. (i.e. filling in the second option Id without including its related <c>Action</c>.)</remarks>
        /// <exception cref="NullReferenceException"></exception>
        /// <param name="actionFive">The <c>Action</c> method related to the fifth option. Required if the fifth option Id is not null or "".</param>
        /// <param name="actionFour">The <c>Action</c> method related to the fourth option. Required if the fourth option Id is not null or "".</param>
        /// <param name="actionThree">The <c>Action</c> method related to the third option. Required if the third option Id is not null or "".</param>
        /// <param name="actionTwo">The <c>Action</c> method related to the second option. Required if the second option Id is not null or "".</param>
        /// <param name="actionOne">The <c>Action</c> method related to the first option.</param>
        /// <param name="optionFive">The JineType used to load the fifth option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionFour">The JineType used to load the fourth option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionThree">The JineType used to load the third option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionTwo"> The JineType used to load the second option. (optional) <br/> When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        /// <param name="optionOne"> The JineType used to load the first option. When loading in a custom message, either use <c>ExtTextExtender.GetUniqueIdNum()</c>, ot use your unique number at the end of your ID and add 10000 to it, then cast it as a JineType. </param>
        public static void StartPiListAndSet(JineType optionOne, Action actionOne, JineType optionTwo = JineType.None, Action actionTwo = null, JineType optionThree = JineType.None, Action actionThree = null, JineType optionFour = JineType.None, Action actionFour = null, JineType optionFive = JineType.None, Action actionFive = null)
        {
            StartPiOptionList(optionOne, optionTwo, optionThree, optionFour, optionFive);
            StartPiOption(optionOne, actionOne);
            if (optionTwo != JineType.None && actionTwo != null)
            {
                StartPiOption(optionTwo, actionTwo);
            }
            if (optionThree != JineType.None && actionThree != null)
            {
                StartPiOption(optionThree, actionThree);
            }
            if (optionFour != JineType.None && actionFour != null)
            {
                StartPiOption(optionFour, actionFour);
            }
            if (optionFive != JineType.None && actionFive != null)
            {
                StartPiOption(optionFive, actionFive);
            }
        }

        /// <summary>
        /// Loads a separate event where you have to choose the right trauma, if applicable.
        /// </summary>
        /// <remarks>Using async <c>Actions</c> are recommended here. </remarks>
        /// <param name="isRight">The <c>Action</c> that happens if you chose the right trauma. </param>
        /// <param name="isWrongOrNull">The <c>Action</c> that happens if you chose the wrong trauma, or if she didn't tell you any of her traumas on Day 15.</param>
        public static void StartTraumaList(Action isRight, Action isWrongOrNull)
        {
            JineType trauma = SingletonMonoBehaviour<EventManager>.Instance.Trauma;
            if (trauma == JineType.None) { isWrongOrNull(); }
            List<JineType> traumaList = new List<JineType>()
            {
                JineType.Ending_Normal_JINE004_Option001,
                JineType.Ending_Normal_JINE004_Option002,
                JineType.Ending_Normal_JINE004_Option003,
                JineType.Ending_Normal_JINE004_Option004,
                JineType.Ending_Normal_JINE004_Option005
            };
            int answer = new List<JineType>()
            {
                JineType.Event_LongLINE001,
                JineType.Event_LongLINE002,
                JineType.Event_LongLINE003,
                JineType.Event_LongLINE004,
                JineType.Event_LongLINE005
            }.IndexOf(trauma);
            SingletonMonoBehaviour<JineManager>.Instance.StartOption(traumaList);
            SingletonMonoBehaviour<JineManager>.Instance.OnChangeHistory.Subscribe(async delegate (CollectionAddEvent<JineData> piAnswer)
            {
                ExtTextManager.ClearExDisposible();
                if (traumaList.IndexOf(piAnswer.Value.id) == answer)
                {
                    isRight();
                }
                else { isWrongOrNull(); }

            }).AddTo(ExtTextManager.CompositeDisposible);
        }

        /// <summary>
        /// Starts a prompt that requires you to write a message to Ame on Jine.
        /// </summary>
        /// <param name="eventAfterMsg">The action that happens after the message has been sent.</param>
        public static void StartWrittenMsg(Action eventAfterMsg)
        {
            SingletonMonoBehaviour<JineManager>.Instance.StartMessage();
            SingletonMonoBehaviour<JineManager>.Instance.Message.Subscribe(async delegate (string m)
            {
                await SingletonMonoBehaviour<JineManager>.Instance.AddJineHistory(new JineData(JineUserType.pi, JineType.None, ResponseType.Freeform, StampType.None, m, 0));
                eventAfterMsg();

            }).AddTo(ExtTextManager.CompositeDisposible);
        }
        /// <summary>
        /// Starts a prompt that requires you to write a message to Ame on Jine. 
        /// </summary>
        /// <remarks>This version of the method checks the message using <c>checkMsgMatch</c>, where the remainder of the event changes depending on what <c>checkMsgMatch</c> returns as.</remarks>
        /// <param name="checkMsgMatch"> A <c>Func</c> bool that does something with the written message.</param>
        /// <param name="isRight">The <c>Action</c> that happens when <c>checkMsgMatch</c> returns true.</param>
        /// <param name="isWrong">The <c>Action</c> that happens when <c>checkMsgMatch</c> returns false.</param>
        public static void StartWrittenMsg(Func<string, bool> checkMsgMatch, Action isRight, Action isWrong)
        {
            SingletonMonoBehaviour<JineManager>.Instance.StartMessage();
            SingletonMonoBehaviour<JineManager>.Instance.Message.Subscribe(async delegate (string n)
            {
                SingletonMonoBehaviour<JineManager>.Instance.AddJineHistory(new JineData(JineUserType.pi, JineType.None, ResponseType.Freeform, StampType.None, n, 0)).Forget();
            }).AddTo(ExtTextManager.CompositeDisposible);
            SingletonMonoBehaviour<JineManager>.Instance.OnChangeHistory.Subscribe(async delegate (CollectionAddEvent<JineData> m)
            {
                ExtTextManager.ClearExDisposible();
                if (checkMsgMatch(m.Value.freeMessage)) { isRight(); }
                else { isWrong(); }
            }).AddTo(ExtTextManager.CompositeDisposible);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(JineDataConverter), "getJineFromTypeId")]
        static bool GetRawExtendedJineData(ref LineMaster.Param __result, JineType t)
        {
            if ((int)t >= 10000)
            {
                int jineIndex = (int)t - 10000;
                string jineFindIndex = $"_{jineIndex}";
                __result = JineExtender.ExtList.Find((LineMaster.Param j) => j.Id.EndsWith(jineFindIndex));
                return false;
            }
            return true;
        }

        /*public static void PiTest()
        {
            async void Test()
            {
                await UniTask.Delay(2000, false, PlayerLoopTiming.Update, default(CancellationToken));
                await AddExtJineHistory("Test_3");
                AudioManager.Instance.PlayBgmByType(SoundType.SE_Tetehen, true);
                SingletonMonoBehaviour<JineManager>.Instance.StartStamp();
            }
            Action a = Test;
            StartExtPiOptions("Test_10", a, "Test_2", a);
        }*/


        static List<JineType> _origNonNoonJines = new List<JineType>();
        static List<JineType> SetNotNoonJineList()
        {
            List<JineType> jineWeekDayList = new List<JineType>();
            if (_origNonNoonJines.Count == 0)
            {
                string[] listOne = Enum.GetNames(typeof(JineType));
                foreach (string str in listOne)
                {
                    if (str.StartsWith("LineWeekDay"))
                    {
                        JineType origJine = (JineType)Enum.Parse(typeof(JineType), str);
                        _origNonNoonJines.Add(origJine);
                    }
                }
            }
            //Debug.Log("All Original Line Count: " + _origNonNoonJines.Count);
            List<LineMaster.Param> extJineWeekDayList = JineExtender.ExtList.FindAll((LineMaster.Param p) => p.Id.StartsWith("JineWeekDay"));
            List<JineType> listTwo = new List<JineType>();
            foreach (LineMaster.Param param in extJineWeekDayList)
            {
                JineType jineData = ExtTextManager.GetUniqueIdNum<JineType>(param.Id);
                if ((int)jineData != 9999) { listTwo.Add((JineType)(jineData)); }
            }
            //Debug.Log("All Custom Jine Count: " + listTwo.Count);
            listTwo.AddRange(_origNonNoonJines);
            return listTwo;
        }

        static List<JineType> getJineHistory()
        {
            List<JineData> jineHistory = SingletonMonoBehaviour<JineManager>.Instance.history;
            List<JineType> jineTypeList = new List<JineType>();
            foreach (JineData data in jineHistory)
            {
                jineTypeList.Add(data.id);
            }
            //Debug.Log("Number of Original & Custom Non-Noon Jines: " + jineTypeList.FindAll((JineType t) => (int)t >= 10000 || t.ToString().Contains("LineWeekDay")).Count);
            return jineTypeList;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Event_Uzagarami_Dokuhaku), "startEvent", new Type[] { typeof(CancellationToken) })]
        static bool StartExtNonNoonJine(NgoEvent __instance, CancellationToken cancellationToken = default(CancellationToken))
        {
            startEvent(__instance, cancellationToken);
            IEnumerable<JineType> jineMainList = SetNotNoonJineList().Except(getJineHistory());
            JineType jine = jineMainList.ElementAt(UnityEngine.Random.Range(0, jineMainList.Count()));
            //Debug.Log("Available Non-Noon Jines: " + jineMainList.Count());
            //Debug.Log("Current Non-Noon Jine: " + (int)jine);
            if ((int)jine >= 10000)
            {
                jineExContinue(jine);
                return false;
            }
            return true;

            async void jineExContinue(JineType jt)
            {
                SingletonMonoBehaviour<JineManager>.Instance.addEventSeparator(JineType.Jine_Label_days);
                await SingletonMonoBehaviour<JineManager>.Instance.AddJineHistory(jt);
                await NgoEvent.DelaySkippable(Constants.MIDDLE);
                SingletonMonoBehaviour<JineManager>.Instance.StartStamp();
                endEvent(__instance);
            }

        }


        [HarmonyReversePatch]
        [HarmonyPatch(typeof(NgoEvent), "endEvent")]
        static void endEvent(NgoEvent instance)
        {

        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(NgoEvent), "startEvent", new Type[] { typeof(CancellationToken) })]
        static async UniTask startEvent(NgoEvent instance, CancellationToken cancellationToken = default(CancellationToken))
        {

        }

        static List<JineData> ExtJineOkReplies = new List<JineData>();
        static List<JineData> ExtJineOMGReplies = new List<JineData>();
        static List<JineData> ExtJineSadReplies = new List<JineData>();
        static List<JineData> ExtJineIDKReplies = new List<JineData>();
        static List<JineData> ExtJineSorryReplies = new List<JineData>();
        static List<JineData> ExtJineDeadReplies = new List<JineData>();
        static List<JineData> ExtJineLoveReplies = new List<JineData>();
        static List<JineData> ExtJineThisReplies = new List<JineData>();
        static List<JineData> ExtJineReplySpam = new List<JineData>();

        static List<JineData> OrigJineOkReplies = new List<JineData>();
        static List<JineData> OrigJineOMGReplies = new List<JineData>();
        static List<JineData> OrigJineSadReplies = new List<JineData>();
        static List<JineData> OrigJineIDKReplies = new List<JineData>();
        static List<JineData> OrigJineSorryReplies = new List<JineData>();
        static List<JineData> OrigJineDeadReplies = new List<JineData>();
        static List<JineData> OrigJineLoveReplies = new List<JineData>();
        static List<JineData> OrigJineThisReplies = new List<JineData>();
        static List<JineData> OrigJineReplySpam = new List<JineData>();

        /// <summary>
        /// Re-initializes custom Jine Replies. 
        /// </summary>
        public static void ResetExtReplyList()
        {
            ClearExtReplies();
            InitializeExtReplies();
        }

        static void ClearExtReplies()
        {
            ExtJineOkReplies.Clear();
            ExtJineOMGReplies.Clear();
            ExtJineSadReplies.Clear();
            ExtJineIDKReplies.Clear();
            ExtJineSorryReplies.Clear();
            ExtJineDeadReplies.Clear();
            ExtJineLoveReplies.Clear();
            ExtJineThisReplies.Clear();
            ExtJineReplySpam.Clear();
        }
        static void InitializeExtReplies()
        {
            List<LineMaster.Param> allJines = JineExtender.ExtList.FindAll((LineMaster.Param j) => j.Id.StartsWith("Reply"));
            foreach (LineMaster.Param param in allJines)
            {
                JineType jineType = ExtTextManager.GetUniqueIdNum<JineType>(param.Id);
                JineData jineData = new JineData(jineType, 0);
                if (param.Id.StartsWith("ReplyOk"))
                {
                    ExtJineOkReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplyOMG"))
                {
                    ExtJineOMGReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplySad"))
                {
                    ExtJineSadReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplyIDK"))
                {
                    ExtJineIDKReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplySorry"))
                {
                    ExtJineSorryReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplyDead"))
                {
                    ExtJineDeadReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplyLove"))
                {
                    ExtJineLoveReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplyThis"))
                {
                    ExtJineThisReplies.Add(jineData);
                }
                if (param.Id.StartsWith("ReplySpam"))
                {
                    ExtJineReplySpam.Add(jineData);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventTekitouhenji), "startEvent")]
        static void AddCustomReplies(ref List<JineData> ___ok, ref List<JineData> ___saikouka, ref List<JineData> ___pien, ref List<JineData> ___waritodoudemoii, ref List<JineData> ___gomen, ref List<JineData> ___bujisibou, ref List<JineData> ___love, ref List<JineData> ___sorena, ref List<JineData> ___rendasuruna)
        {
            if (OrigJineOkReplies.Count == 0)
            {
                OrigJineOkReplies.AddRange(___ok);
                OrigJineOMGReplies.AddRange(___saikouka);
                OrigJineSadReplies.AddRange(___pien);
                OrigJineIDKReplies.AddRange(___waritodoudemoii);
                OrigJineSorryReplies.AddRange(___gomen);
                OrigJineDeadReplies.AddRange(___bujisibou);
                OrigJineLoveReplies.AddRange(___love);
                OrigJineThisReplies.AddRange(___sorena);
                OrigJineReplySpam.AddRange(___rendasuruna);
            }

            List<JineData> okList = new List<JineData>();
            List<JineData> omgList = new List<JineData>();
            List<JineData> sadList = new List<JineData>();
            List<JineData> idkList = new List<JineData>();
            List<JineData> sorryList = new List<JineData>();
            List<JineData> deadList = new List<JineData>();
            List<JineData> loveList = new List<JineData>();
            List<JineData> thisList = new List<JineData>();
            List<JineData> spamList = new List<JineData>();

            okList.AddRange(OrigJineOkReplies);
            okList.AddRange(ExtJineOkReplies);

            omgList.AddRange(OrigJineOMGReplies);
            omgList.AddRange(ExtJineOMGReplies);

            sadList.AddRange(OrigJineSadReplies);
            sadList.AddRange(ExtJineSadReplies);

            idkList.AddRange(OrigJineIDKReplies);
            idkList.AddRange(ExtJineIDKReplies);

            sorryList.AddRange(OrigJineSorryReplies);
            sorryList.AddRange(ExtJineSorryReplies);

            deadList.AddRange(OrigJineDeadReplies);
            deadList.AddRange(ExtJineDeadReplies);

            loveList.AddRange(OrigJineLoveReplies);
            loveList.AddRange(ExtJineLoveReplies);

            thisList.AddRange(OrigJineThisReplies);
            thisList.AddRange(ExtJineThisReplies);

            spamList.AddRange(OrigJineReplySpam);
            spamList.AddRange(ExtJineReplySpam);

            ___ok = okList;
            ___saikouka = omgList;
            ___pien = sadList;
            ___waritodoudemoii = idkList;
            ___gomen = sorryList;
            ___bujisibou = deadList;
            ___love = loveList;
            ___sorena = thisList;
            ___rendasuruna = spamList;
        }

    }
}
