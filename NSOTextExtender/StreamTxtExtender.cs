using HarmonyLib;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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
            if (originalMobs.Count == 0)
            {
                originalMobs = __result.param;
            }
            MobCommentMaster newMobs = ScriptableObject.CreateInstance<MobCommentMaster>();
            newMobs.param.AddRange(originalMobs);
            newMobs.param.AddRange(ExtList);
            __result = newMobs;

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
            if (originalTenCom.Count == 0)
            {
                originalTenCom = __result;
            }
            List<TenCommentMaster.Param> newMobs = new List<TenCommentMaster.Param>();
            newMobs.AddRange(originalTenCom);
            newMobs.AddRange(ExtList);
            __result = newMobs;

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
            if (originalKusoCom.Count == 0)
            {
                originalKusoCom = __result;
            }
            List<KusoCommentMaster.Param> newMobs = new List<KusoCommentMaster.Param>();
            newMobs.AddRange(originalKusoCom);
            newMobs.AddRange(ExtList);
            __result = newMobs;

        }
    }
}
