using HarmonyLib;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class TooltxtExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtTooltip.json"));
        public static List<TooltipMaster.Param> ExtList = new List<TooltipMaster.Param>();


        /// <summary>
        /// Creates a Tooltip with custom text.
        /// </summary>
        /// <param name="isCat"> Shows the Cat Tooltip if true. Otherwise, shows the normal speech bubble tooltip.</param>
        /// <param name="id"> The TooltipMaster.Param's Id used to load the tooltip text.</param>
        public static void ShowCustomTooltip(bool isCat, string id)
        {
            TooltipMaster.Param exTooltxtId = ExtList.Find((TooltipMaster.Param j) => j.Id == id);
            TooltipType tooltxtData = ExtTextManager.GetUniqueIdNum<TooltipType>(exTooltxtId.Id);
            if (isCat)
            {
                SingletonMonoBehaviour<TooltipManager>.Instance.ShowTutorial(tooltxtData);
                return;
            }
            SingletonMonoBehaviour<TooltipManager>.Instance.Show(tooltxtData); ;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NgoEx), "getToolTip")]
        static bool GetRawExtendedToolTxtData(ref TooltipMaster.Param __result, TooltipType type)
        {
            if ((int)type >= 10000)
            {
                int toolIndex = (int)type - 10000;
                string toolFindIndex = $"_{toolIndex}";
                __result = ExtList.Find((TooltipMaster.Param j) => j.Id.EndsWith(toolFindIndex));
                return false;
            }
            return true;
        }
    }

}
