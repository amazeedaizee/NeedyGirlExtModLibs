using BepInEx;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NGOTxtExtender
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("Windose.exe")]
    public class BepInStart: BaseUnityPlugin
    {
        public const string pluginGuid = "needy.girl.txt_extender";
        public const string pluginName = "Text Extender";
        public const string pluginVersion = "1.0.0.0";

        public static PluginInfo PInfo { get; private set; }
        public void Awake()
        {
            PInfo = Info;

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void Start()
        {
            KRepExtender.ResetExtKReps();
            JineExtender.ResetExtReplyList();
        }
    }

















}


