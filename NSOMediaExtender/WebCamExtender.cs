using Cysharp.Threading.Tasks;
using HarmonyLib;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace NSOMediaExtender
{
    /// <summary>
    /// The idle animations Ame does based on her Task Manager Stats, with the exception of <c>Horror</c>.
    /// </summary>
    public enum WebcamType
    {
        Normal, Stressed, Dark, Darker, StressDark, StressDarker, Like, Love, StressLike, StressLove, DarkerLove, StressDarkerLove, Horror
    }

    [HarmonyPatch]
    public class WebCamExtender
    {
        internal static List<AnimationClip> animationClipExtList = new List<AnimationClip>();
        internal static bool originalAnim = true;

        static List<string> addToNormalAnim = new List<string>();
        static List<string> addToStressedAnim = new List<string>();
        static List<string> addToDarkAnim = new List<string>();
        static List<string> addToDarkerAnim = new List<string>();
        static List<string> addToStressDarkAnim = new List<string>();
        static List<string> addToStressDarkerAnim = new List<string>();
        static List<string> addToLikeAnim = new List<string>();
        static List<string> addToLoveAnim = new List<string>();
        static List<string> addToStressLikeAnim = new List<string>();
        static List<string> addToStressLoveAnim = new List<string>();
        static List<string> addToDarkerLoveAnim = new List<string>();
        static List<string> addToStressDarkerLoveAnim = new List<string>();
        static List<string> addToHorrorAnim = new List<string>();

        static List<string> replaceNormalAnim = new List<string>();
        static List<string> replaceStressedAnim = new List<string>();
        static List<string> replaceDarkAnim = new List<string>();
        static List<string> replaceDarkerAnim = new List<string>();
        static List<string> replaceStressDarkAnim = new List<string>();
        static List<string> replaceStressDarkerAnim = new List<string>();
        static List<string> replaceLikeAnim = new List<string>();
        static List<string> replaceLoveAnim = new List<string>();
        static List<string> replaceStressLikeAnim = new List<string>();
        static List<string> replaceStressLoveAnim = new List<string>();
        static List<string> replaceDarkerLoveAnim = new List<string>();
        static List<string> replaceStressDarkerLoveAnim = new List<string>();
        static List<string> replaceHorrorAnim = new List<string>();

        static List<string> originalNormalAnim = new List<string>();
        static List<string> originalStressedAnim = new List<string>();
        static List<string> originalDarkAnim = new List<string>();
        static List<string> originalDarkerAnim = new List<string>();
        static List<string> originalStressDarkAnim = new List<string>();
        static List<string> originalStressDarkerAnim = new List<string>();
        static List<string> originalLikeAnim = new List<string>();
        static List<string> originalLoveAnim = new List<string>();
        static List<string> originalStressLikeAnim = new List<string>();
        static List<string> originalStressLoveAnim = new List<string>();
        static List<string> originalDarkerLoveAnim = new List<string>();
        static List<string> originalStressDarkerLoveAnim = new List<string>();
        static List<string> originalHorrorAnim = new List<string>();


        /// <summary>
        /// Adds an <c>AnimationClip</c> to the <c>AnimationClip</c> ExtList.
        /// </summary>
        /// <param name="clip">The <c>AnimationClip</c> to add to the list. </param>
        public static void AddCustomAnimClip(AnimationClip clip)
        {
            animationClipExtList.Add(clip);
        }

        /// <summary>
        /// Adds new idle animations to Ame.
        /// </summary>
        /// <param name="extAnims">The list of animations to add, based on their <c>AnimationClip</c> name.</param>
        /// <param name="type">The idle animation list to add to.</param>
        public static void AddCustomIdleAnims(List<string> extAnims, WebcamType type)
        {
            switch (type)
            {
                case WebcamType.Stressed:
                    addToStressedAnim.AddRange(extAnims);
                    break;
                case WebcamType.Dark:
                    addToDarkAnim.AddRange(extAnims);
                    break;
                case WebcamType.Darker:
                    addToDarkerAnim.AddRange(extAnims);
                    break;
                case WebcamType.StressDark:
                    addToStressDarkAnim.AddRange(extAnims);
                    break;
                case WebcamType.StressDarker:
                    addToStressDarkerAnim.AddRange(extAnims);
                    break;
                case WebcamType.Like:
                    addToLikeAnim.AddRange(extAnims);
                    break;
                case WebcamType.Love:
                    addToLoveAnim.AddRange(extAnims);
                    break;
                case WebcamType.StressLike:
                    addToStressLikeAnim.AddRange(extAnims);
                    break;
                case WebcamType.StressLove:
                    addToStressLoveAnim.AddRange(extAnims);
                    break;
                case WebcamType.DarkerLove:
                    addToDarkerLoveAnim.AddRange(extAnims);
                    break;
                case WebcamType.StressDarkerLove:
                    addToStressDarkerLoveAnim.AddRange(extAnims);
                    break;
                case WebcamType.Horror:
                    addToHorrorAnim.AddRange(extAnims);
                    break;
                default:
                    addToNormalAnim.AddRange(extAnims);
                    break;
            }
        }

        /// <summary>
        /// Replaces default idle animations of Ame to custom ones.
        /// </summary>
        /// <remarks>Note: If two or more different mods use this method, the mod loaded last will take priority on what animations to replace.</remarks>
        /// <param name="extAnims">The list of animations to replace with, based on their <c>AnimationClip</c> name.</param>
        /// <param name="type">The idle animation list to replace.</param>
        public static void ReplaceCustomIdleAnims(List<string> extAnims, WebcamType type)
        {
            switch (type)
            {
                case WebcamType.Stressed:
                    replaceStressedAnim = extAnims;
                    break;
                case WebcamType.Dark:
                    replaceDarkAnim = extAnims;
                    break;
                case WebcamType.Darker:
                    replaceDarkerAnim = extAnims;
                    break;
                case WebcamType.StressDark:
                    replaceStressDarkAnim = extAnims;
                    break;
                case WebcamType.StressDarker:
                    replaceStressDarkerAnim = extAnims;
                    break;
                case WebcamType.Like:
                    replaceLikeAnim = extAnims;
                    break;
                case WebcamType.Love:
                    replaceLoveAnim = extAnims;
                    break;
                case WebcamType.StressLike:
                    replaceStressLikeAnim = extAnims;
                    break;
                case WebcamType.StressLove:
                    replaceStressLoveAnim = extAnims;
                    break;
                case WebcamType.DarkerLove:
                    replaceDarkerLoveAnim = extAnims;
                    break;
                case WebcamType.StressDarkerLove:
                    replaceStressDarkerLoveAnim = extAnims;
                    break;
                case WebcamType.Horror:
                    replaceHorrorAnim = extAnims;
                    break;
                default:
                    replaceNormalAnim = extAnims;
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WebCamManager), "GetStatusFace")]
        static void SetCustomIdleAnim(ref string[] ___horror, ref string[] ___normal, ref string[] ___normalStress, ref string[] ___suki4, ref string[] ___suki4Stress, ref string[] ___suki5, ref string[] ___suki5Stress, ref string[] ___yami4, ref string[] ___yami4Stress, ref string[] ___yami5, ref string[] ___yami5Stress, ref string[] ___yamisuki5, ref string[] ___yamisuki5Stress)
        {
            if (originalNormalAnim.Count == 0)
            {
                originalNormalAnim.AddRange(___normal.ToList());
                originalStressedAnim.AddRange(___normalStress.ToList());
                originalDarkAnim.AddRange(___yami4.ToList());
                originalDarkerAnim.AddRange(___yami5.ToList());
                originalStressDarkAnim.AddRange(___yami4Stress.ToList());
                originalStressDarkerAnim.AddRange(___yami5Stress.ToList());
                originalLikeAnim.AddRange(___suki4.ToList());
                originalLoveAnim.AddRange(___suki5.ToList());
                originalStressLikeAnim.AddRange(___suki4Stress.ToList());
                originalStressLoveAnim.AddRange(___suki5Stress.ToList());
                originalDarkerLoveAnim.AddRange(___yamisuki5.ToList());
                originalStressDarkerLoveAnim.AddRange(___yamisuki5Stress.ToList());
                originalHorrorAnim.AddRange(___horror.ToList());
            }


            ___horror = AddOrReplaceIdleAnims(originalHorrorAnim, replaceHorrorAnim, addToHorrorAnim);
            ___normal = AddOrReplaceIdleAnims(originalNormalAnim, replaceNormalAnim, addToNormalAnim);
            ___normalStress = AddOrReplaceIdleAnims(originalStressedAnim, replaceStressedAnim, addToStressedAnim);
            ___yami4 = AddOrReplaceIdleAnims(originalDarkAnim, replaceDarkAnim, addToDarkAnim);
            ___yami5 = AddOrReplaceIdleAnims(originalDarkerAnim, replaceDarkerAnim, addToDarkerAnim);
            ___yami4Stress = AddOrReplaceIdleAnims(originalStressDarkAnim, replaceStressDarkAnim, addToStressDarkAnim);
            ___yami5Stress = AddOrReplaceIdleAnims(originalStressDarkerAnim, replaceStressDarkerAnim, addToStressDarkerAnim);
            ___suki4 = AddOrReplaceIdleAnims(originalLikeAnim, replaceLikeAnim, addToLikeAnim);
            ___suki5 = AddOrReplaceIdleAnims(originalLoveAnim, replaceLoveAnim, addToLoveAnim);
            ___suki4Stress = AddOrReplaceIdleAnims(originalStressLikeAnim, replaceStressLikeAnim, addToStressLikeAnim);
            ___suki5Stress = AddOrReplaceIdleAnims(originalStressLoveAnim, replaceStressLoveAnim, addToStressLoveAnim);
            ___yamisuki5 = AddOrReplaceIdleAnims(originalDarkerLoveAnim, replaceDarkerLoveAnim, addToDarkerLoveAnim);
            ___yamisuki5Stress = AddOrReplaceIdleAnims(originalStressDarkerLoveAnim, replaceStressDarkerLoveAnim, addToStressDarkerLoveAnim);

        }

        private static string[] AddOrReplaceIdleAnims(List<string> original, List<string> replace, List<string> add)
        {
            if (replace.Count != 0)
            {
                return replace.ToArray();
            }
            else if (add.Count != 0)
            {
                return original.Concat(add).ToArray();
            }
            return original.ToArray();
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LoadWebcamData), "LoadAnimation")]
        [HarmonyPatch(typeof(LoadLiveViewData), "LoadAnimation")]
        static async UniTask<AnimationClip> SetCustomAnim(UniTask<AnimationClip> value, string address)
        {
            string customId = address.Remove(address.Length - 5, 5);
            if (animationClipExtList.Count == 0)
            {
                originalAnim = false;
                return await value;
            }
            try
            {
                AnimationClip customClip = animationClipExtList.FirstOrDefault(x => x.name == customId);
                if (customClip == null || customClip.ToString() == "")
                {
                    originalAnim = false;
                    return await value;
                }
                if (ThatOneLongListOfAnimationsOriginallyInTheGame.list.Exists(x => x == customClip.name))
                {
                    originalAnim = false;
                    return await value;
                }
                return customClip;
            }
            catch
            {
            }
            originalAnim = false;
            return await value;
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(LoadWebcamData), "LoadAnimation")]
        [HarmonyPatch(typeof(LoadLiveViewData), "LoadAnimation")]
        static InvalidKeyException WhatInvalidKey()
        { if (originalAnim) { return new InvalidKeyException(); }
            originalAnim = true;
            return null;
        }
    }
}
