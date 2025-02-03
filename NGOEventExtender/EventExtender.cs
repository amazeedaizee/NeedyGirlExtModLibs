using Cysharp.Threading.Tasks;
using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NGOEventExtender
{
    [HarmonyPatch]
    public class EventExtender
    {
        public static NgoEvent ngoEvent = new NgoEvent();
        public static bool isEventing = false;
        public static bool resetDayCustomEvent = false;

        static bool isHeadPat = true;

        public static NgoEvent currentExtDayEvent;

        public static List<NgoExtEvent> randomDayExtEvents = new List<NgoExtEvent>();
        static List<string> origDayEvent = new List<string>();
        static List<string> origFHEvent = new List<string>();
        static List<string> origFLEvent = new List<string>();
        static List<string> origDHEvent = new List<string>();
        static List<string> origDLEvent = new List<string>();
        static List<string> origLHEvent = new List<string>();
        static List<string> origLLEvent = new List<string>();
        static List<string> origSHEvent = new List<string>();
        static List<string> origSLEvent = new List<string>();

        static List<NgoExtEvent> specialDayExtEvents = new List<NgoExtEvent>();
        static List<NgoExtEvent> specialNightExtEvents = new List<NgoExtEvent>();
        static List<NgoExtEvent> specialMidnightExtEvents = new List<NgoExtEvent>();

        static Dictionary<string, List<NgoExtEvent>> hijackingEventList = new Dictionary<string, List<NgoExtEvent>>();


        /// <summary>
        /// Hijacks a queued original event and loads a custom event instead. 
        /// <br/>If the custom event has a condition, if will only replace the original if the condition is met.
        /// </summary>
        ///  <remarks>
        /// This method adds the custom event to a list, where it will track when its related original event is de-queued. 
        /// <br/> Therefore, it's not required to call the same method with the same arguments more than once.
        /// </remarks>
        /// <typeparam name="T">The event to replace.</typeparam>
        /// <typeparam name="ExT">The event that will be loaded instead.</typeparam>
        public static void HijackOriginalEvent<T, ExT>() where T : NgoEvent where ExT : NgoExtEvent
        {
            Type type = typeof(T);
            Type typeTwo = typeof(ExT);
            NgoEvent original = Activator.CreateInstance(type) as NgoEvent;
            NgoExtEvent replacement = Activator.CreateInstance(typeTwo) as NgoExtEvent;
            HijackOriginalEvent(original, replacement);

        }


        /// <summary>
        /// Hijacks a queued original event and loads a custom event instead. 
        /// <br/>If the custom event has a condition, if will only replace the original if the condition is met.
        /// </summary>
        ///  <remarks>
        /// This method adds the custom event to a list, where it will track when its related original event is de-queued. 
        /// <br/> Therefore, it's not required to call the same method with the same arguments more than once.
        /// </remarks>
        /// <typeparam name="T">The event to replace.</typeparam>
        /// <param name="replacement">The event that will be loaded instead.</param>
        public static void HijackOriginalEvent<T>(NgoExtEvent replacement) where T : NgoEvent
        {
            Type type = typeof(T);
            NgoEvent original = Activator.CreateInstance(type) as NgoEvent;
            HijackOriginalEvent(original, replacement);

        }

        /// <summary>
        /// Hijacks a queued original event and loads a custom event instead. 
        /// <br/>If the custom event has a condition, if will only replace the original if the condition is met.
        /// </summary>
        ///  <remarks>
        /// This method adds the custom event to a list, where it will track when its related original event is de-queued. 
        /// <br/> Therefore, it's not required to call the same method with the same arguments more than once.
        /// </remarks>
        /// <param name="original">The event to replace.</param>
        /// <param name="replacement">The event that will be loaded instead.</param>
        public static void HijackOriginalEvent(NgoEvent original, NgoExtEvent replacement)
        {
            if (hijackingEventList.ContainsKey(original.ToString()))
            {
                hijackingEventList[original.ToString()].Add(replacement);
                return;
            }

            hijackingEventList.Add(original.ToString(), new List<NgoExtEvent>() { replacement });

        }

        /// <summary>
        /// Hijacks a queued original event and loads a custom event instead. 
        /// <br/>If the custom event has a condition, if will only replace the original if the condition is met.
        /// </summary>
        /// <remarks>
        /// This method adds the custom events to a list, where it will track when its related original event is de-queued. <br/>
        /// Therefore, it's not required to call the same method with the same arguments more than once.
        /// </remarks>
        /// <typeparam name="T">The event to replace.</typeparam>
        /// <param name="replacements">The list of events that will be loaded instead.</param>
        public static void HijackOriginalEvent<T>(List<NgoExtEvent> replacements) where T : NgoEvent
        {
            Type type = typeof(T);
            if (type == typeof(Event_ImageTest))
            {
                Debug.LogError("You can't replace this event.");
                return;
            }
            NgoEvent original = Activator.CreateInstance(type) as NgoEvent;
            HijackOriginalEvent(original, replacements);

        }

        /// <summary>
        /// Hijacks a queued original event and loads a custom event instead. 
        /// <br/>If the custom event has a condition, if will only replace the original if the condition is met.
        /// </summary>
        /// <remarks>
        /// This method adds the custom events to a list, where it will track when its related original event is de-queued. <br/>
        /// Therefore, it's not required to call the same method with the same arguments more than once.
        /// </remarks>
        /// <param name="original">The event to replace.</param>
        /// <param name="replacements">The list of events that will be loaded instead.</param>
        public static void HijackOriginalEvent(NgoEvent original, List<NgoExtEvent> replacements)
        {

            if (hijackingEventList.ContainsKey(original.ToString()))
            {
                hijackingEventList[original.ToString()].AddRange(replacements);
                return;
            }

            hijackingEventList.Add(original.ToString(), replacements);

        }

        /// <summary>
        /// Enables or disables the ability to headpat Ame's head. Useful if you don't want Ame's head to be interactable during events.
        /// </summary>
        /// <remarks> Ability to pat will be reset to true during the boot screen, reloading a day or going to the next day.</remarks>
        /// <param name="isPat">Can Ame's head be patted or not?</param>
        public static void CanHeadpat(bool isPat)
        {
            isHeadPat = isPat;
        }

        /// <summary>
        /// Starts a custom event.
        /// </summary>
        /// <param name="ngoEvent"> The event that will be loaded.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StartEvent(NgoEvent ngoEvent)
        {
            if (isEventing)
            {
                return;
            }
            AddEvent(ngoEvent);

        }

        /// <summary>
        /// Starts a custom event.
        /// </summary>
        ///  <typeparam name="T">The event that will be loaded.</typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void StartEvent<T>() where T : NgoEvent, new()
        {
            if (isEventing)
            {
                return;
            }
            SingletonMonoBehaviour<EventManager>.Instance.AddEvent<T>();
        }


        /// <summary>
        /// Sets custom Random Day Events. How these events are loaded are based on the event's title.
        /// </summary>
        /// <param name="extEventList">The list of custom random day events to load.</param>
        public static void SetRandomDayExtEvents(List<NgoExtEvent> extEventList)
        {
            randomDayExtEvents.AddRange(extEventList);
        }
        /// <summary>
        /// Sets custom Special Day Events. 
        /// <br/>In-game Special Day Events (fixed/milestones) take priority over your events, so your conditions have to be unique.
        /// </summary>
        /// <param name="extEventList">The list of custom special day events to load.</param>
        public static void SetSpecialDayExtEvent(List<NgoExtEvent> extEventList)
        {
            specialDayExtEvents.AddRange(extEventList);
        }
        /// <summary>
        /// Sets custom Special Night Events. 
        /// <br/>In-game Special Night Events take priority over your events, so your conditions have to be unique.
        /// </summary>
        /// <param name="extEventList">The list of custom special day events to load.</param>
        public static void SetSpecialNightExtEvent(List<NgoExtEvent> extEventList)
        {
            specialNightExtEvents.AddRange(extEventList);
        }
        /// <summary>
        /// Sets custom Special Midnight Events. (the time after Sleep To Tomorrow is pressed, but before the day transitions to the next day.) 
        /// <br/>In-game Special Midnight Events take priority over your events, so your conditions have to be unique.
        /// </summary>
        /// <param name="extEventList">The list of custom special day events to load.</param>
        public static void SetSpecialMidnightExtEvent(List<NgoExtEvent> extEventList)
        {
            specialMidnightExtEvents.AddRange(extEventList);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "FetchDialog")]
        static bool InitializeSpecialDayExts()
        {
            if (specialDayExtEvents.Count == 0) { return true; }
            for (int i = 0; i < specialDayExtEvents.Count; i++)
            {
                bool condition = specialDayExtEvents[i].SetCondition();
                if (condition)
                {
                    AddEvent(specialDayExtEvents[i]);
                    return false;
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "FetchUzagarami")]
        static bool InitializeSpecialNightExts()
        {
            if (specialNightExtEvents.Count == 0) { return true; }
            if (SingletonMonoBehaviour<EventManager>.Instance.isGedatsu)
            {
                return true;
            }
            if (SingletonMonoBehaviour<EventManager>.Instance.nowEnding == EndingType.Ending_Completed)
            {
                return true;
            }
            for (int i = 0; i < specialNightExtEvents.Count; i++)
            {
                bool condition = specialNightExtEvents[i].SetCondition();
                if (condition)
                {
                    AddEvent(specialNightExtEvents[i]);
                    return false;
                }
            }
            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "FetchScenarioEvent")]
        static void InitializeSpecialMidnightExts(ref bool __result)
        {
            if (specialMidnightExtEvents.Count == 0) { return; }
            if (__result == true) { return; }
            if (SingletonMonoBehaviour<EventManager>.Instance.isHorror)
            {
                __result = false;
                return;
            }
            if (SingletonMonoBehaviour<EventManager>.Instance.nowEnding == EndingType.Ending_Completed)
            {
                __result = false;
                return;
            }

            for (int i = 0; i < specialMidnightExtEvents.Count; i++)
            {
                bool condition = specialMidnightExtEvents[i].SetCondition();
                if (condition)
                {
                    AddEvent(specialMidnightExtEvents[i]);
                    __result = true;
                    return;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Event_Uzagarami_Kaiwa), "getUniqueUzagaramiId")]
        static void InitializeRandomDayExts(ref List<string> ___FollowerHigh, ref List<string> ___FollowerLow, ref List<string> ___heijouEvents, ref List<string> ___LoveHigh, ref List<string> ___LoveLow, ref List<string> ___StressHigh, ref List<string> ___StressLow, ref List<string> ___YamiHigh, ref List<string> ___YamiLow)
        {
            if (origDayEvent.Count == 0)
            {
                origDayEvent = ___heijouEvents;
                origFHEvent = ___FollowerHigh;
                origFLEvent = ___FollowerLow;
                origDHEvent = ___YamiHigh;
                origDLEvent = ___YamiLow;
                origLLEvent = ___LoveLow;
                origLHEvent = ___LoveHigh;
                origSHEvent = ___StressHigh;
                origSLEvent = ___StressLow;
            }
            if (resetDayCustomEvent)
            {
                ___heijouEvents = origDayEvent;
                ___FollowerHigh = origFHEvent;
                ___FollowerLow = origFLEvent;
                ___YamiHigh = origDHEvent;
                ___YamiLow = origDLEvent;
                ___LoveHigh = origLHEvent;
                ___LoveLow = origLLEvent;
                ___StressHigh = origSHEvent;
                ___StressLow = origSLEvent;
                resetDayCustomEvent = false;
            }
            else
            {
                foreach (NgoExtEvent extEvent in randomDayExtEvents)
                {
                    if (extEvent.Id == null || extEvent.Id == "")
                    {
                        Debug.LogError("This event must have an Id!");
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_FH"))
                    {
                        ___FollowerHigh.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_FL"))
                    {
                        ___FollowerLow.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_DH"))
                    {
                        ___YamiHigh.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_DL"))
                    {
                        ___YamiLow.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_LH"))
                    {
                        ___LoveHigh.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_LL"))
                    {
                        ___LoveLow.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_SH"))
                    {
                        ___StressHigh.Add(extEvent.Id);
                        continue;
                    }
                    if (extEvent.Id.EndsWith("_SL"))
                    {
                        ___StressLow.Add(extEvent.Id);
                        continue;
                    }
                    ___heijouEvents.Add(extEvent.Id);
                }
            }

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Event_Uzagarami_Kaiwa), "getUniqueUzagaramiId")]
        static void StartRandomDayExt(ref string __result)
        {
            string eventName = __result;
            if (randomDayExtEvents.Exists((NgoExtEvent ext) => ext.Id == eventName))
            {
                NgoExtEvent extDayEvent = randomDayExtEvents.Find((NgoExtEvent ext) => ext.Id == eventName);


                bool condition = extDayEvent.SetCondition();
                if (condition == false)
                {
                    resetDayCustomEvent = true;
                    __result = getUniqueUzagaramiId(ngoEvent);
                    return;
                }
                currentExtDayEvent = extDayEvent;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "StartEvent")]
        static void EventReplacer()
        {
            string eventOne;
            try { eventOne = SingletonMonoBehaviour<EventManager>.Instance.eventQueue.Peek().ToString(); }
            catch { return; };
            if (!hijackingEventList.ContainsKey(eventOne))
            {
                return;
            }
            hijackingEventList.TryGetValue(eventOne, out List<NgoExtEvent> extEvents);
            if (extEvents.Count == 0) { return; }
            for (int i = 0; i < extEvents.Count; i++)
            {

                if (i == extEvents.Count - 1)
                {
                    bool finalCondition = extEvents[i].SetCondition();
                    if (finalCondition == false)
                    {
                        return;
                    }

                }


                bool condition = extEvents[i].SetCondition();
                if (condition == false)
                {
                    continue;
                }

                SingletonMonoBehaviour<EventManager>.Instance.eventQueue.Clear();
                AddEvent(extEvents[i]);
                return;
            }
        }

        static async void AddEvent(NgoEvent ngoEvent)
        {
            SingletonMonoBehaviour<EventManager>.Instance.eventQueue.Enqueue(ngoEvent);
            await UniTask.Delay(1, false, PlayerLoopTiming.Update, default(CancellationToken));
            await SingletonMonoBehaviour<EventManager>.Instance.StartEvent();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(App_Webcam), nameof(App_Webcam.Nade))]
        static bool ConfirmHeadpat()
        {
            return isHeadPat;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Boot), "Awake")]
        [HarmonyPatch(typeof(EventManager), "Load")]
        [HarmonyPatch(typeof(DayPassing), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void ResetHeadPat()
        {
            isHeadPat = true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(EventManager), "AddEventQueue", new Type[] { typeof(string) })]
        static bool SetCustomDayEvent()
        {
            if (currentExtDayEvent != null)
            {
                SingletonMonoBehaviour<EventManager>.Instance.eventQueue.Enqueue(currentExtDayEvent);
                currentExtDayEvent = null;
                return false;
            }
            return true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Event_Uzagarami_Kaiwa), "getUniqueUzagaramiId")]
        static string getUniqueUzagaramiId(NgoEvent instance)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// An NgoEvent with a condition attached to it.
    /// </summary>
    public abstract class NgoExtEvent : NgoEvent
    {
        public virtual string Id => null;
        /// <summary>
        ///  The condition for the event. 
        ///  <br/>Event will only start if the condition returns true. 
        /// </summary>
        /// <remarks><c>SetCondition</c> is not read if this NgoExtEvent is used with <c>StartCustomEvent.</c></remarks>
        public abstract bool SetCondition();
    }

}
