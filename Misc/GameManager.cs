using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private string mainSceneToLoad;

    [SerializeField] private string loadingScene;

    public static UnityEvent assetsReady;

    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;

    public void Awake()
    {
        assetsReady = new UnityEvent();
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadSceneFromAddressables(loadingScene);
        LoadItemsFromMemory(); 
    }

    private void LoadSceneFromAddressables(string sceneName)
    {
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync(sceneName, activateOnLoad: true);
    }


    public IEnumerator LoadItemsFromMemory()
    {
        ItemLoader itemLoader = new ItemLoader();
        StartCoroutine(itemLoader.LoadItemsJSONFromMemory());
        yield return null;
        assetsReady.Invoke();
    }
}
