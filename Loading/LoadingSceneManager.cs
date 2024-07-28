using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;
    private Slider m_LoadingSlider;
    private GameManager gameManager;

    private void Awake()
    {
        m_LoadingSlider = FindAnyObjectByType<Slider>();
        StartCoroutine(LoadMainScene());
        gameManager = GameManager.Instance;
        
    }


    private void OnAssetsReady()
    {
        Debug.Log("Assests ready !");
        gameManager.LoadMainScene();
    }


    public IEnumerator LoadMainScene()
    {
        Debug.Log("Loading Main Scene !"); 
        StartCoroutine(LoadItemsFromMemory());
        yield return null;
        /*
        WorldHandler newSceneWorldHandler = FindAnyObjectByType<WorldHandler>(); // Replace be something that can find WorldHandler in inactive scene. Or make scene actove but handle world loading apart.
        Task terrainHandling = new Task(newSceneWorldHandler.CreateTerrainGrids());
        terrainHandling.Finished += delegate { OnTerrainReady(); };
        */
    }


    public IEnumerator LoadItemsFromMemory()
    {
        Debug.Log("Loading items !");
        ItemLoader itemLoader = new ItemLoader();
        ItemLoader.Ready.AddListener(OnAssetsReady);
        Task itemLoading = new Task(itemLoader.LoadItemsJSONFromMemory());
        yield return itemLoading;
    }

    private void OnTerrainReady()
    {
        Debug.Log("Terrain ready !");
    }
}
