using Cysharp.Threading.Tasks;
using ngov3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NSOMediaExtender
{
    public class AssetBundleLoader
    {
        /// <summary>
        /// Loads specific assets from an Asset Bundle, then unloads the rest of the assets. 
        /// <br/>
        /// <br/> These specific assets include:
        /// <br/><c>AnimationClip</c> assets that starts with "stream_cho_" or "stream_ame_".
        /// <br/><c>AudioClip</c> assets that starts with "BGM_" or "SE_".
        /// <br/><c>Sprite</c> assets that starts with "MyPic_".
        /// </summary>
        /// <param name="path"> The path to the asset bundle.</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        public static async UniTask LoadCustomAssets(string path)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            await UniTask.WaitUntil(() => bundleRequest.isDone);
            AssetBundle bundle = bundleRequest.assetBundle;
            var assetRequest = bundle.LoadAllAssetsAsync();
            await UniTask.WaitUntil(() => assetRequest.isDone);
            foreach (var asset in assetRequest.allAssets)
            {
                if (asset.GetType() == typeof(AnimationClip) && (asset.name.StartsWith("stream_cho_") || asset.name.StartsWith("stream_ame_")))
                {
                    WebCamExtender.animationClipExtList.Add((AnimationClip)asset);
                    continue;
                }
                if (asset.GetType() == typeof(AudioClip) && (asset.name.StartsWith("BGM_") || asset.name.StartsWith("SE_")))
                {
                    Sound sound = AudioExtender.CreateSound(asset.name, (AudioClip)asset);
                    if (sound != null)
                    {
                        AudioExtender.customSoundList.Add(sound);
                    }
                    continue;
                }
                if (asset.GetType() == typeof(Sprite) && asset.name.StartsWith("MyPic_"))
                {
                    PictureExtender.SpritePicExtList.Add((Sprite)asset);
                    continue;
                }
            }
            var bundleUnload = bundle.UnloadAsync(false);
            await UniTask.WaitUntil(() => bundleUnload.isDone);
        }

    }
}
