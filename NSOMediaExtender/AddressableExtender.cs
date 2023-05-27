using Cysharp.Threading.Tasks;
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
