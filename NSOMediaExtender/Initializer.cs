using BepInEx;
using HarmonyLib;
using UnityEngine;

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
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Start()
        {
            ///Add some code or whatever
        }

        private void OnApplicationQuit()
        {
            AddressableExtender.DeleteAddressBundlesFromPath();
        }
    }

}


