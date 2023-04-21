using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

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
