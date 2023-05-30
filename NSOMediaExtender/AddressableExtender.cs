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

        public static async UniTask<T> LoadAddressObj<T>(string address) where T : UnityEngine.Object
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

        public static void ReleaseAddressObj(string address) 
        {
            if (!addressHandles.TryGetValue(address, out var handle)) { return; }
            Addressables.Release(handle);
            addressHandles.Remove(address);
        }

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
