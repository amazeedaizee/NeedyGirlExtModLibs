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

namespace NSOMediaExtender
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("Windose.exe")]
    public class MyPatches : BaseUnityPlugin
    {
        public const string pluginGuid = "needy.girl.media_extender";
        public const string pluginName = "Media Extender";
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
            ///Add some code or whatever
        }
    }

}


