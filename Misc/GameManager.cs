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

    private static AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle;

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadMainScene()
    {
        Debug.Log("Loading scene Main Scene...");
        m_SceneLoadOpHandle = Addressables.LoadSceneAsync(mainSceneToLoad, activateOnLoad: true);
        m_SceneLoadOpHandle.Completed += OnSceneLoaded;
    }

    private static void OnSceneLoaded(AsyncOperationHandle<SceneInstance> m_SceneLoadOpHandle)
    {

    }

    public static void SpawnPlayer()
    {
        PlayerLoader playerLoader = new PlayerLoader(new Vector3(10,3,10));
        playerLoader.LoadPlayer();
    }
}
