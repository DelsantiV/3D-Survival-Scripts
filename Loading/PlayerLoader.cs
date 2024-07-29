using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerLoader
{
    private string playerPrefabAddress = "Prefabs/Player/Player.Prefab";
    private string canvasPrefabAddress = "Prefabs/Player/Canvas.Prefab";
    private Vector3 spawnPoint = Vector3.zero;
    private AsyncOperationHandle<GameObject> m_PlayerLoadOpHandle;
    private AsyncOperationHandle<GameObject> m_CanvasLoadOpHandle;

    public PlayerLoader(Vector3 SpawnPoint)
    {
        this.spawnPoint = SpawnPoint;
    }

    public void LoadPlayer()
    {
        m_PlayerLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(playerPrefabAddress);
        m_CanvasLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(canvasPrefabAddress);
        m_PlayerLoadOpHandle.Completed += OnPlayerLoaded;
    }

    private void OnPlayerLoaded(AsyncOperationHandle<GameObject> m_PlayerLoadOpHandle)
    {
        if (m_PlayerLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            m_CanvasLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(canvasPrefabAddress);
            m_CanvasLoadOpHandle.Completed += delegate { OnCanvasLoaded(m_CanvasLoadOpHandle, m_PlayerLoadOpHandle); };
        }
    }

    private void OnCanvasLoaded(AsyncOperationHandle<GameObject> m_CanvasLoadOpHandle, AsyncOperationHandle<GameObject> m_PlayerLoadOpHandle)
    {
        if (m_CanvasLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Spawning Player...");
            SpawnPlayer(m_PlayerLoadOpHandle.Result, m_CanvasLoadOpHandle.Result);
            LoadPlayerData();
        }
    }

    private void SpawnPlayer(GameObject playerPrefab, GameObject canvasPrefab)
    {
        GameObject player = Object.Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        GameObject canvas = Object.Instantiate(canvasPrefab);
        Camera.main.GetComponent<vThirdPersonCamera>().target = player.transform;
    }

    private void LoadPlayerData()
    {

    }
}
