using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using GoTF.Utilities;

namespace GoTF.GameLoading
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
        private Slider m_LoadingSlider;
        private GameManager gameManager;

        private void Awake()
        {
            m_LoadingSlider = FindAnyObjectByType<Slider>();
            StartCoroutine(LoadAssets());
            gameManager = GameManager.Instance;
        }


        private void OnAssetsReady()
        {
            Debug.Log("Assets ready !");
            gameManager.LoadMainScene();
        }


        public IEnumerator LoadAssets()
        {
            Debug.Log("Loading assets !");
            ItemLoader itemLoader = new ItemLoader();
            CraftingRecipesLoader recipesLoader = new CraftingRecipesLoader();
            yield return itemLoader.LoadItems();
            yield return recipesLoader.LoadRecipes();
            OnAssetsReady();
        }
    }
}
