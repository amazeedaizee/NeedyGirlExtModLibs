using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace NSOMediaExtender
{
    public class AddressableExtender
    {
        public static async UniTask AddExternalCatalog(string path)
        {
            AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(path, true);
            await UniTask.WaitUntil(() => handle.IsDone);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("It worked!");
            }
        }
    }
}
