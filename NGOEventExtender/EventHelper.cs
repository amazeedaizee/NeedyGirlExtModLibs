using Cysharp.Threading.Tasks;
using HarmonyLib;
using NGO;
using ngov3;
using ngov3.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NGOEventExtender
{
    [HarmonyPatch]
    public class EventHelper
    {
        private static ReactiveProperty<Sprite> jineIcon;
        private static ReactiveProperty<Sprite> angelTweetIcon;
        private static ReactiveProperty<Sprite> ameTweetIcon;
        private static Sprite origJineIcon;
        private static Sprite origAngelTweetIcon;
        private static Sprite origAmeTweetIcon;

        public static GameObject DayPassingCover;

        public static void StartYearsPass()
        {
            DayPassingCover.GetComponent<IDayPassing>().yearsPass();
        }

        public static void SetShortcutTaskbarState(bool isActive, float alphaIfInactive = 0.4f)
        {
            SingletonMonoBehaviour<EventManager>.Instance.SetShortcutState(isActive, alphaIfInactive);
            SingletonMonoBehaviour<TaskbarManager>.Instance.SetTaskbarInteractive(isActive);
        }

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

        public static void ChangeJineAmeIcon(Sprite sprite)
        {
            jineIcon.Value = sprite;
        }

        public static void ChangeTweetKAngelIcon(Sprite sprite) { angelTweetIcon.Value = sprite; }

        public static void ChangeTweetAmeIcon(Sprite sprite) { ameTweetIcon.Value = sprite; }

        public static void ResetIcons(bool resetJineIcon = true, bool resetAngelTweetIcon = true, bool resetAmeTweetIcon=true)
        { 
            if (resetJineIcon) { jineIcon.Value = null; }
            if (resetAngelTweetIcon) { angelTweetIcon.Value = null; }
            if (resetAmeTweetIcon) { ameTweetIcon.Value = null; }

        }

        private static void ChangeAllJineIcons(List<JineCell2D> list)
        {
            if (jineIcon.Value != null)
            {
                foreach (JineCell2D jine in list)
                {
                    jine.GetComponent<SpriteStencilSetter>()._rend.sprite = jineIcon.Value;
                }
            }
            if (jineIcon.Value == null)
            {
                foreach (JineCell2D jine in list)
                {
                    jine.GetComponent<SpriteStencilSetter>()._rend.sprite = jineIcon.Value;
                }
            }
        }

        private static void ChangeAllTweetIcons(List<PoketterCell2D> list)
        {
            if (angelTweetIcon.Value != null || ameTweetIcon.Value != null)
            {
                foreach (PoketterCell2D tweet in list)
                {
                    if (angelTweetIcon.Value != null) { tweet._omoteIcon = angelTweetIcon.Value; }
                    if (ameTweetIcon.Value != null) { tweet._uraIcon = ameTweetIcon.Value; }
                    if (angelTweetIcon.Value == null) { tweet._omoteIcon = origAngelTweetIcon; }
                    if (ameTweetIcon.Value == null) { tweet._uraIcon = origAmeTweetIcon; }
                }
            }
            if (angelTweetIcon.Value == null && ameTweetIcon.Value == null)
            {
                foreach (PoketterCell2D tweet in list)
                {
                    if (angelTweetIcon.Value == null) { tweet._omoteIcon = origAngelTweetIcon; }
                    if (ameTweetIcon.Value == null) { tweet._uraIcon = origAmeTweetIcon; }
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
            if (origJineIcon == null && ____rend.sprite.name == "icon_jine_ame") { origJineIcon = ____rend.sprite; }
            if ( jineIcon != null && ____rend.sprite.name == "icon_jine_ame")
            {             
                ____rend.sprite = jineIcon.Value;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PoketterCell2D), "Awake")]
        static void SetSpriteTweetIcons(ref Sprite ____omoteIcon, ref Sprite ____uraIcon)
        {
            if (origAngelTweetIcon ==null) { origAngelTweetIcon = ____omoteIcon; }
            if (origAmeTweetIcon ==null) { origAmeTweetIcon = ____uraIcon; }
            if (angelTweetIcon != null) { ____omoteIcon = angelTweetIcon.Value; }
            if (ameTweetIcon != null) { ____uraIcon = ameTweetIcon.Value; }
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
            if (jineIcon.Value == null) { return; }
            ChangeAllJineIcons(____jineCells);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(JineView2D), "Awake")]
        static void SubToTweetIconChange(List<PoketterCell2D> ____tweetCells, ScrollRect ____scrollRect)
        {
            GameObject obj = ____scrollRect.transform.parent.transform.parent.gameObject;
            jineIcon.Subscribe((Sprite _) =>
            {
                ChangeAllTweetIcons(____tweetCells);

            }).AddTo(obj);
            if (angelTweetIcon.Value == null && ameTweetIcon.Value == null) { return; }
            ChangeAllTweetIcons(____tweetCells);
        }
    }
}
