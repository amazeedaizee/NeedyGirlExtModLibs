using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NSOMediaExtender
{
    public class AddressableExtender
    {
       internal static List<string> addressBundleList = new List<string>();
        internal static Dictionary<string, AsyncOperationHandle> addressHandles = new Dictionary<string, AsyncOperationHandle>();  
        /// <summary>
        /// Loads an external Addressable catalog file into the game. If you're relying on Addressables to load in external assets, this is required.
        /// </summary>
        /// <param name="path">The path to the external catalog.</param>
        /// <returns></returns>
        public static async UniTask AddExternalCatalog(string path)
        {
            AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(path, true);
            await UniTask.WaitUntil(() => handle.IsDone);
            Debug.Log("Catalog loaded.");
        }

        /// <summary>
        /// Loads an Asset Bundle related to an Addressable catalog.
        /// </summary>
        /// <remarks>This temporarily copies the Asset Bundle into the StreamingAssets folder of the game until the game closes. 
        /// <br/> Note: The Asset Bundle will not delete itself in the case of the game crashing or force quitted through Task Manager. (However, it will delete itself if the game is closed properly next time.)</remarks>
        /// <param name="path">The path to the Addressable Asset Bundle.</param>
        /// <returns></returns>
        public static async UniTask AddAddressBundle(string path)
        {
            string[] splitPath = path.Split('\\');
            string fileName = splitPath[splitPath.Length - 1];
            string targetStrAssetPath = Path.Combine(Addressables.RuntimePath, GetCurrentPlatform(), fileName);
            var bundleData = File.ReadAllBytes(path);
            using (FileStream targetStream = File.Create(targetStrAssetPath))
            {
                await targetStream.WriteAsync(bundleData, 0, bundleData.Length);
            }
            
            addressBundleList.Add(targetStrAssetPath);
        }

        /// <summary>
        /// Loads an asset from Addressables into the game.
        /// </summary>
        /// <remarks>For easy releasing, you can use <c>ReleaseAddressAsset</c> or <c>ReleaseAllHandles</c>.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async UniTask<T> LoadAddressAsset<T>(string address) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            await UniTask.WaitUntil(() => handle.IsDone);
            try 
            { 
                addressHandles.Add(address,handle);
                return handle.Result; 
            }
            catch { throw new Exception("An error occurred in loading the addressable."); };
        }

        /// <summary>
        /// Releases an object Addressable's handle loaded with <c>LoadAddressAsset()</c>.
        /// </summary>
        /// <param name="address"></param>
        public static void ReleaseAddressAsset(string address) 
        {
            if (!addressHandles.TryGetValue(address, out var handle)) { return; }
            Addressables.Release(handle);
            addressHandles.Remove(address);
        }

        /// <summary>
        /// Releases all handles loaded with <c>LoadAddressAsset()</c>.
        /// </summary>
        public static void ReleaseAllHandles()
        {
            if (addressHandles.Count == 0) { return; }
            foreach (var handle in addressHandles.Values) { Addressables.Release(handle); }
            addressHandles.Clear();
         }
       

        internal static string GetCurrentPlatform() 
        { 
            switch (Application.platform) 
            {
                case RuntimePlatform.WindowsPlayer:
                    return "StandaloneWindows64";
                case RuntimePlatform.OSXPlayer:
                    return "StandaloneOSX";
                case RuntimePlatform.LinuxPlayer:
                    return "StandaloneLinux64";            
            }
            return null;
        }
        internal static void DeleteAddressBundlesFromPath()
        {
            if (addressBundleList.Count == 0) { return; }
            foreach (string bundlePath in addressBundleList)
            {
                File.Delete(bundlePath);
            }
        }
    }
}
