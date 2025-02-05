using HarmonyLib;
using NGO;
using ngov3;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace NGOTxtExtender
{
    [HarmonyPatch]
    public class ExtTextManager
    {
        internal static CompositeDisposable CompositeDisposible = new CompositeDisposable();


        /// <summary>
        /// Clears the Extend Manager's CompositeDisposable.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NgoEvent), "endEvent")]
        public static void ClearExDisposible()
        {
            CompositeDisposible.Clear();
        }

        ///<summary>
        ///   Gets the unique Id Number at the end of a param's Id, and converts it to a supported enumtype.
        ///<list type="bullet">
        ///<item>
        ///<description> <c>JineType</c></description>
        ///</item>
        ///<item>
        ///<description> <c>KusoRepType</c></description>
        ///</item>
        ///<item>
        ///<description> <c>TooltipType</c></description>
        ///</item>
        ///<item>
        ///<description> <c>TweetType</c></description>
        ///</item>
        ///</list>
        ///</summary>
        ///<exception cref="ArgumentException"> 
        /// Thrown if the type is either not supported or is not an enumtype.
        ///</exception>
        ///<exception cref="InvalidOperationException"> 
        /// Thrown if the unique Id Number failed to parse.
        ///</exception>

        ///<typeparam name="T">The enumtype to convert to.</typeparam>
        ///<param name="id">The param's Id to be used for conversion.</param>
        public static T GetUniqueIdNum<T>(string id) where T : Enum
        {
            Type type = typeof(T);

            if (!(type == typeof(JineType) || type == typeof(TweetType) || type == typeof(KusoRepType) || type == typeof(TooltipType)))
            {
                if (type == typeof(Enum))
                {
                    throw new ArgumentException($"This method doesn't support this enumtype: {type}!");
                }
                throw new ArgumentException($"{type} is not a valid enumtype.");
            }
            string[] titleSplit = id.Split('_');
            bool parseWorked = int.TryParse(titleSplit[titleSplit.Count() - 1], out int index);
            int newInt;
            if (parseWorked) { newInt = index + 10000; }
            else
            {
                throw new InvalidOperationException("Result is not a number. End of Id must be formatted like this: \n\"_#\" \nwhere # is a unique number.");
            }
            return (T)(object)newInt;
        }

        /// <summary>
        /// Adds a specific type of list to the corresponding <c>ExtList</c>.
        /// Type must be a valid Master.Param.
        /// </summary>
        /// <remarks> This method is required to load custom text into the game.</remarks>
        /// <typeparam name="T">The type of list to add. Must be a valid Master.Param </typeparam>
        /// <param name="list">The list to be added to the corresponding <c>ExtList</c>.</param>
        public static void AddToExtList<T>(List<T> list) where T : class
        {
            Type type = typeof(T);
            object obj = Activator.CreateInstance(type);
            switch (obj)
            {
                case KituneMaster.Param _:
                    KitsuneExtender.ExtList.AddRange((IEnumerable<KituneMaster.Param>)list);
                    break;
                case KituneSuretaiMaster.Param _:
                    KitsuneExtender.ExtList_Title.AddRange((IEnumerable<KituneSuretaiMaster.Param>)list);
                    break;
                case KRepMaster.Param _:
                    KRepExtender.ExtList.AddRange((IEnumerable<KRepMaster.Param>)list);
                    KRepExtender.ResetExtKReps();
                    break;
                case TweetMaster.Param _:
                    TweetExtender.ExtList.AddRange((IEnumerable<TweetMaster.Param>)list);
                    break;
                case LineMaster.Param _:
                    JineExtender.ExtList.AddRange((IEnumerable<LineMaster.Param>)list);
                    JineExtender.ResetExtReplyList();
                    break;
                case EgosaMaster.Param _:
                    EgosaExtender.ExtList.AddRange((IEnumerable<EgosaMaster.Param>)list);
                    break;
                case MobCommentMaster.Param _:
                    MobComExtender.ExtList.AddRange((IEnumerable<MobCommentMaster.Param>)list);
                    break;
                case TenCommentMaster.Param _:
                    TenComExtender.ExtList.AddRange((IEnumerable<TenCommentMaster.Param>)list);
                    break;
                case KusoCommentMaster.Param _:
                    KusoComExtender.ExtList.AddRange((IEnumerable<KusoCommentMaster.Param>)list);
                    break;
                case SystemTextMaster.Param _:
                    SysTxtExtender.ExtList.AddRange((IEnumerable<SystemTextMaster.Param>)list);
                    break;
                case TooltipMaster.Param _:
                    TooltxtExtender.ExtList.AddRange((IEnumerable<TooltipMaster.Param>)list);
                    break;
                default:
                    Debug.LogError($"{type} is either not a Master.Param or is not supported.");
                    break;
            }
        }
    }
}
