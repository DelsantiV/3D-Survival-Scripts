using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
    private Slider m_LoadingSlider;

    private void Awake()
    {
        m_LoadingSlider = FindAnyObjectByType<Slider>();
        GameManager.assetsReady.AddListener(OnAssetsReady);
    }

    private IEnumerator loadNextLevel(string level)
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync(level, activateOnLoad: false);

        while (!m_SceneLoadOpHandle.IsDone)
        {
            m_LoadingSlider.value = m_SceneLoadOpHandle.PercentComplete;
            yield return null;
        }

        WorldHandler newSceneWorldHandler = FindAnyObjectByType<WorldHandler>();
        Task terrainHandling = new Task(newSceneWorldHandler.CreateTerrainGrids());
        terrainHandling.Finished += delegate { OnTerrainReady(); };
        yield return null;
        Debug.Log($"Loaded Level {level}");
    }

    private void OnAssetsReady()
    {
        StartCoroutine(loadNextLevel("MainScene"));
    }


    private void OnTerrainReady()
    {
        Debug.Log("Terrain ready !");
    }
}
