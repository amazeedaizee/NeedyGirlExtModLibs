using Cysharp.Threading.Tasks;
using HarmonyLib;
using NGO;
using ngov3;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NSOMediaExtender
{
    [HarmonyPatch]
    public class PictureExtender
    {
        internal static List<Sprite> SpritePicExtList = new List<Sprite>();
        static List<ResourceLocal> customMyPictureList = new List<ResourceLocal>();
        static List<ResourceLocal> originalMyPictureList = new List<ResourceLocal>();

        /// <summary>
        /// Adds a <c>Sprite</c> to the <c>Sprite</c> Extlist.
        /// </summary>
        /// <param name="sprite">The sprite to load.</param>
        public static void AddExtSpriteForMyPics(Sprite sprite)
        {
            SpritePicExtList.Add(sprite);
        }

        /// <summary>
        /// Loads a custom image file as a Sprite, and adds it to the <c>Sprite</c> Extlist. 
        /// </summary>
        /// <remarks>Supported image types are jpg, and png.</remarks>
        /// <param name="path">The path to the image file.</param>
        public static void LoadImgFileForMyPic(string path)
        {
            string[] getFileName = path.Split('\\');
            string fileName = getFileName[getFileName.Count() - 1];
            byte[] file = File.ReadAllBytes(path);

            if
            (
                //JPEG
                ((file[0] == 0xFF && file[1] == 0xD8) && (file[file.LongLength - 2] == 0xFF && file[file.LongLength - 1] == 0xD9))
                //PNG
                || (file[0] == 0x89 && file[1] == 0x50 && file[2] == 0x4E && file[3] == 0x47 && file[4] == 0x0D && file[5] == 0x0A && file[6] == 0x1A && file[7] == 0x0A)
            )
            {
                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, file, false);
                tex.filterMode = FilterMode.Point;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                sprite.name = fileName;
                SpritePicExtList.Add(sprite);
            }
        }


        /// <summary>
        /// Creates a ResourceLocal object, the reference the game uses to load an image from Tweeter/Poketter, Jine, and MyPictures.
        /// <br/> Only use this if you're relying on Addressables to load images.
        /// </summary>
        /// <param name="address">The image's addressable Id, which the game will use to load the image.</param>
        public static void CreateExtResWithAddress(string address)
        {
            int index = customMyPictureList.Count;
            ResourceLocal resource = new ResourceLocal
            {
                FileName = address,
                Path = ""
            };

            string s;
            if (index < 10)
            {
                s = $"00{index}";
            }
            else if (index < 100)
            {
                s = $"0{index}";
            }
            else
            {
                s = index.ToString();
            }
            resource.Id = $"EXT{s}";
            customMyPictureList.Add(resource);
        }

        /// <summary>
        /// Uses the <c>Sprite</c> ExtList to create related ResourceLocal objects, the reference the game uses to load an image from Tweeter/Poketter, Jine, and MyPictures.
        /// </summary>
        public static void ExtSpriteToResLocal()
        {
            int index = customMyPictureList.Count;
            for (int i = index; i < (index + SpritePicExtList.Count); i++)
            {
                string s;
                if (i < 10)
                {
                    s = $"00{i}";
                }
                else if (i < 100)
                {
                    s = $"0{i}";
                }
                else
                {
                    s = i.ToString();
                }
                ResourceLocal resource = new ResourceLocal
                {
                    Id = $"EXT{s}",
                    FileName = SpritePicExtList[i].name,
                    Path = ""
                };
                customMyPictureList.Add(resource);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ImageViewerHelper), "LoadResourcesList")]
        static void InitializeExtMyPictures(ref List<ResourceLocal> __result)
        {
            if (originalMyPictureList.Count == 0)
            {
                originalMyPictureList.AddRange(__result);
            }
            List<ResourceLocal> newList = new List<ResourceLocal>();
            newList.AddRange(originalMyPictureList);
            newList.InsertRange(128, customMyPictureList);
            __result = newList;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LoadPictures), "LoadPictureAsync")]
        static async UniTask<Sprite> LoadCustomMyPictures(UniTask<Sprite> value, string address)
        {
            if (SpritePicExtList.Count == 0)
            {
                return await value;
            }
            try
            {
                Sprite customSprite = SpritePicExtList.Find(s => s.name == address);
                if (customSprite == null || customSprite.ToString() == "")
                {
                    return await value;
                }
                return customSprite;
            }
            catch
            {
            }
            return await value;
        }
    }
}
