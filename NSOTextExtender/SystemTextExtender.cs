using HarmonyLib;
using ngov3;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class SysTxtExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtSystemText.json"));
        static List<SystemTextMaster.Param> ExtList = new List<SystemTextMaster.Param>();
        static List<SystemTextMaster.Param> originalList = new List<SystemTextMaster.Param>();
        static List<SystemTextMaster.Param> combinedList = new List<SystemTextMaster.Param>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getSystemTexts")]
        static void InitializeExtSysTxt(ref List<SystemTextMaster.Param> __result)
        {
            if (ExtList.Count != 0 && !__result.Exists(s => s.Id == ExtList[0].Id))
            {
                __result.AddRange(ExtList);
            }

        }

        static bool isCustomNote = false;
        static string customNoteId = "";

        /// <summary>
        /// Opens a Notepad window that contains custom text.
        /// </summary>
        /// <remarks>Note: This window won't open if "Ame's Notes" is open, as this uses "Ame's Notes" as a window base. "Ame's Notes" also won't open if this custom window is currently open. </remarks>
        /// <param name="contextId">The SystemTextMaster.Param's Id used to load the text. Can use either in-game or custom params.</param>
        /// <param name="nameId">The SystemTextMaster.Param's Id used to set the name of the window. Can use either in-game or custom params.</param>
        public static void OpenCustomNote(string contextId, string nameId)
        {
            isCustomNote = true;
            customNoteId = contextId;

            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.HintTextAfterEnding))
            {
                IWindow window = SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.HintTextAfterEnding, true);
                window.SetName(NgoEx.SystemTextFromTypeString(nameId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value));
            }
            isCustomNote = false;
            customNoteId = "";


        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(App_TextEditor), "Awake")]
        static void LoadCustomNoteText(TMP_Text ___text, ref Image ___picture)
        {
            if (isCustomNote)
            {
                ___text.text = NgoEx.SystemTextFromTypeString(customNoteId, SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value);
            }
        }
    }
}
