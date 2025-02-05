using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;

namespace NGOExtraSettings
{

    /// <exclude />
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("Windose.exe")]


    public class MyPatches : BaseUnityPlugin
    {
        public const string pluginGuid = "needy.girl.extra_settings";
        public const string pluginName = "Extra Settings";
        public const string pluginVersion = "1.0.0.0";

        public static PluginInfo PInfo { get; private set; }
        public void Awake()
        {
            PInfo = Info;

            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Start()
        {
            ///Add some code or whatever
        }
    }

    public class ExtraSettings
    {
        public static T LoadFromExtSettings<T>(string key)
        {
            string text = "ExtraSettings.es3";
            T value = ES3.Load<T>(key, text);
            return (T)(object)value;
        }
        public static void SaveToExtSettings<T>(string key, T data)
        {
            string text = "ExtraSettings.es3";
            Type type = typeof(T);
            object obj = Activator.CreateInstance(type);
            ES3.Save(key, (T)(object)data, text);
        }
    }
}


