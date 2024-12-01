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
            StartCoroutine(LoadItemsFromMemory());
            gameManager = GameManager.Instance;

        }


        private void OnAssetsReady()
        {
            Debug.Log("Assets ready !");
            gameManager.LoadMainScene();
        }


        public IEnumerator LoadItemsFromMemory()
        {
            Debug.Log("Loading items !");
            ItemLoader itemLoader = new ItemLoader();
            ItemLoader.Ready.AddListener(OnAssetsReady);
            Task itemLoading = new Task(itemLoader.LoadItems());
            yield return itemLoading;
        }
    }
}
