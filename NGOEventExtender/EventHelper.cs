using HarmonyLib;
using ngov3;
using ngov3.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NGOEventExtender
{
    [HarmonyPatch]
    public class EventHelper
    {
        private static ReactiveProperty<Sprite> jineIcon = new ReactiveProperty<Sprite>(null);
        private static ReactiveProperty<Sprite> angelTweetIcon = new ReactiveProperty<Sprite>(null);
        private static ReactiveProperty<Sprite> ameTweetIcon = new ReactiveProperty<Sprite>(null);
        private static Sprite origJineIcon;
        private static Sprite origAngelTweetIcon;
        private static Sprite origAmeTweetIcon;

        static GameObject DayPassingCover;

        /// <summary>
        /// Loads in the graphic where a number of days pass (found in Painful Future, Labor Is Evil, etc.)
        /// </summary>
        public static void StartYearsPass()
        {
            DayPassingCover.GetComponent<IDayPassing>().yearsPass();
        }

        /// <summary>
        /// Sets the interactivity of the shortcuts and the taskbar.
        /// </summary>
        /// <param name="isActive"> Can you interact with the shortcuts and the taskbar? </param>
        /// <param name="alphaIfInactive"> The alpha / opaqueness of the shortcuts if it can't be interactable. </param>
        public static void SetShortcutTaskbarState(bool isActive, float alphaIfInactive = 0.4f)
        {
            SingletonMonoBehaviour<EventManager>.Instance.SetShortcutState(isActive, alphaIfInactive);
            SingletonMonoBehaviour<TaskbarManager>.Instance.SetTaskbarInteractive(isActive);
        }

        /// <summary>
        /// Spawns in a notification that does an action you choose after. Meant for any endings on the last day.
        /// </summary>
        /// <param name="action"> The <c>Action</c> to play when the notiifcation is pressed. </param>
        /// <param name="disposables"> The <c>CompositeDisposable</c> to use to dispose the subscribed notification event.</param>
        public static void SetLastDayNotif(Action action, CompositeDisposable disposables)
        {
            if (SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.DayIndex) < 30) { return; }
            SingletonMonoBehaviour<NotificationManager>.Instance.AddMaturoNotifier();
            (from v in SingletonMonoBehaviour<StatusManager>.Instance.GetStatusObservable(StatusType.DayPart)
             where v == 3
             select v).Subscribe(async delegate (int _)
             {
                 action();
             }).AddTo(disposables);
        }


        /// <summary>
        /// Changes Ame's JINE icon to a new Sprite. 
        /// </summary>
        /// <param name="sprite"> The Sprite to change Ame's icon too.</param>
        public static void ChangeJineAmeIcon(Sprite sprite)
        {
            jineIcon.Value = sprite;
        }


        /// <summary>
        /// Changes KAngel's Tweeter/Poketter icon to a new Sprite. 
        /// </summary>
        /// <param name="sprite"> The Sprite to change Ame's icon too.</param>
        public static void ChangeTweetKAngelIcon(Sprite sprite) { angelTweetIcon.Value = sprite; }


        /// <summary>
        /// Changes Ame's Tweeter/Poketter icon to a new Sprite.  
        /// </summary>
        /// <param name="sprite"> The Sprite to change Ame's icon too.</param>
        public static void ChangeTweetAmeIcon(Sprite sprite) { ameTweetIcon.Value = sprite; }

        /// <summary>
        /// Resets all changed Icons to its default.
        /// </summary>
        /// <param name="resetJineIcon">Reset Ame's Jine icon? Default is <c>true.</c></param>
        /// <param name="resetAngelTweetIcon">Reset KAngel's Tweeter/Poketter icon? Default is <c>true.</c></param>
        /// <param name="resetAmeTweetIcon">Reset Ame's Tweeter/Poketter  icon? Default is <c>true.</c></param>
        public static void ResetIcons(bool resetJineIcon = true, bool resetAngelTweetIcon = true, bool resetAmeTweetIcon = true)
        {
            if (resetJineIcon) { jineIcon.Value = null; }
            if (resetAngelTweetIcon) { angelTweetIcon.Value = null; }
            if (resetAmeTweetIcon) { ameTweetIcon.Value = null; }

        }

        private static void ChangeAllJineIcons(List<JineCell2D> list)
        {
            if (origJineIcon == null)
            {

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].gameObject.name.Contains("App_Jine_Ame_2D"))
                    {
                        origJineIcon = list[i].transform.GetChild(0).GetComponent<SpriteStencilSetter>()._rend.sprite;
                    }
                }
                return;
            }

            if (jineIcon.Value != null)
            {

                foreach (JineCell2D jine in list)
                {
                    if (jine.gameObject.name.Contains("App_Jine_Ame_2D"))
                    {
                        jine.transform.GetChild(0).GetComponent<SpriteStencilSetter>()._rend.sprite = jineIcon.Value;
                    }

                }
            }
            if (jineIcon.Value == null)
            {

                foreach (JineCell2D jine in list)
                {
                    if (jine.gameObject.name.Contains("App_Jine_Ame_2D"))
                    {
                        jine.transform.GetChild(0).GetComponent<SpriteStencilSetter>()._rend.sprite = origJineIcon;
                    }
                }
            }
        }

        private static void ChangeAllTweetIcons(List<PoketterCell2D> list)
        {
            if (origAngelTweetIcon == null && origAmeTweetIcon == null)
            {
                origAngelTweetIcon = list[0]._omoteIcon;
                origAmeTweetIcon = list[0]._uraIcon;
                return;
            }
            if (angelTweetIcon.Value == null && ameTweetIcon.Value == null)
            {
                foreach (PoketterCell2D tweet in list)
                {
                    if (angelTweetIcon.Value == null) { tweet._omoteIcon = origAngelTweetIcon; }
                    if (ameTweetIcon.Value == null) { tweet._uraIcon = origAmeTweetIcon; }
                    tweet.SetUserIcon();
                }
                return;
            }
            if (angelTweetIcon.Value != null || ameTweetIcon.Value != null)
            {
                foreach (PoketterCell2D tweet in list)
                {
                    if (angelTweetIcon.Value != null) { tweet._omoteIcon = angelTweetIcon.Value; }
                    if (ameTweetIcon.Value != null) { tweet._uraIcon = ameTweetIcon.Value; }
                    if (angelTweetIcon.Value == null) { tweet._omoteIcon = origAngelTweetIcon; }
                    if (ameTweetIcon.Value == null) { tweet._uraIcon = origAmeTweetIcon; }
                    tweet.SetUserIcon();
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventManager), "Awake")]
        static void GetObjects()
        {
            if (DayPassingCover != null)
            {
                return;
            }
            DayPassingCover = GameObject.Find("DayPassingCover");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpriteStencilSetter), "Awake")]
        static void SetSpriteJineIcon(ref SpriteRenderer ____rend)
        {
            if (jineIcon.Value != null && ____rend.transform.name == "Icon2D")
            {
                ____rend.sprite = jineIcon.Value;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PoketterCell2D), "Awake")]
        static void SetSpriteTweetIcons(ref Sprite ____omoteIcon, ref Sprite ____uraIcon)
        {
            if (angelTweetIcon.Value != null) { ____omoteIcon = angelTweetIcon.Value; }
            if (ameTweetIcon.Value != null) { ____uraIcon = ameTweetIcon.Value; }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(JineView2D), "Awake")]
        static void SubToJineIconChange(List<JineCell2D> ____jineCells, Button2D ___ToBottom)
        {
            GameObject obj = ___ToBottom.transform.parent.gameObject;
            jineIcon.Subscribe((Sprite _) =>
            {
                ChangeAllJineIcons(____jineCells);

            }).AddTo(obj);
            //  if (jineIcon.Value == null) { return; }
            ChangeAllJineIcons(____jineCells);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PoketterView2D), "Start")]
        static void SubToTweetIconChange(List<PoketterCell2D> ____tweetCells, ScrollRect ____scrollRect)
        {
            GameObject obj = ____scrollRect.transform.parent.transform.parent.gameObject;
            angelTweetIcon.Subscribe((Sprite _) =>
            {
                ChangeAllTweetIcons(____tweetCells);

            }).AddTo(obj);
            ameTweetIcon.Subscribe((Sprite _) =>
            {
                ChangeAllTweetIcons(____tweetCells);

            }).AddTo(obj);
            // if (angelTweetIcon.Value == null && ameTweetIcon.Value == null) { return; }
            ChangeAllTweetIcons(____tweetCells);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Boot), "Awake")]
        [HarmonyPatch(typeof(EventManager), "Load")]
        [HarmonyPatch(typeof(EventManager), "StartOver")]
        [HarmonyPatch(typeof(ngov3.DayPassing), "startEvent", new Type[] { typeof(CancellationToken) })]
        static void ResetOnBootAndLoad()
        {
            ResetIcons();
        }
    }
}
