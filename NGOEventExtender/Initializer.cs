using BepInEx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using HarmonyLib;
using Newtonsoft.Json;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NGOEventExtender
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency("needy.girl.txt_extender", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("Windose.exe")]
    public class BepInStart : BaseUnityPlugin
    {
        public const string pluginGuid = "needy.girl.event_extender";
        public const string pluginName = "Event Extender";
        public const string pluginVersion = "1.0.0.0";

        public void Awake()
        {
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
            MethodInfo original = AccessTools.FirstMethod(typeof(Live), x => x.Name.Contains("SetScenario"));
            MethodInfo patch = AccessTools.Method(typeof(StreamExtender), "InitializeConditionalStream");
            harmony.Patch(original, new HarmonyMethod(patch));

        }

        public void Start()
        {
        }
    }





}
