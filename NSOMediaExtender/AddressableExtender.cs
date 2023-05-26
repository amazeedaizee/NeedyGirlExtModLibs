using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NSOMediaExtender
{
    public class AddressableExtender
    {
        /// <summary>
        /// Loads an external Addressable catalog file into the game. If you're relying on Addressables to load in external assets, this is required.
        /// </summary>
        /// <param name="path">The path to the external catalog.</param>
        /// <returns></returns>
        public static async UniTask AddExternalCatalog(string path)
        {
            AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(path, true);
            await UniTask.WaitUntil(() => handle.IsDone);
        }
    }
}
