using HarmonyLib;
using ngov3;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class MobComExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtMobComParam_Pure.json"));
        public static List<MobCommentMaster.Param> ExtList = new List<MobCommentMaster.Param>();
        static List<MobCommentMaster.Param> originalMobs = new List<MobCommentMaster.Param>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getMobs")]
        static void InitializeExtMobs(ref MobCommentMaster __result)
        {
            if (ExtList.Count != 0 && !__result.param.Exists(m => m.Id == ExtList[0].Id))
            {
               __result.param.AddRange(ExtList);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Live), "FetchComment")]
        static IEnumerable<CodeInstruction> FetchComment_MaxCountFix(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_1)
                {
                    codes[i].opcode = OpCodes.Nop;
                    continue;
                }
                if (codes[i].opcode == OpCodes.Sub)
                {
                    codes[i].opcode = OpCodes.Nop;
                    break;
                }
            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch]
    public class TenComExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtTenComment.json"));
        public static List<TenCommentMaster.Param> ExtList = new List<TenCommentMaster.Param>();
        static List<TenCommentMaster.Param> originalTenCom = new List<TenCommentMaster.Param>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getTenComments")]
        static void InitializeExtTenCom(ref List<TenCommentMaster.Param> __result)
        {
            if (ExtList.Count != 0 && !__result.Exists(t => t.Id == ExtList[0].Id))
            {
                __result.AddRange(ExtList);
            }
        }
    }

    [HarmonyPatch]
    public class KusoComExtender
    {
        //public static string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(MyPatches.PInfo.Location), "ExtKusoComment.json"));
        public static List<KusoCommentMaster.Param> ExtList = new List<KusoCommentMaster.Param>();
        static List<KusoCommentMaster.Param> originalKusoCom = new List<KusoCommentMaster.Param>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEx), "getKusoComments")]
        static void InitializeExtKusoCom(ref List<KusoCommentMaster.Param> __result)
        {
            if (ExtList.Count != 0 && !__result.Exists(k => k.Id == ExtList[0].Id))
            {
                __result.AddRange(ExtList);
            }
        }
    }
}
