using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerLoader 
{
    private string playerPrefabAddress = "Prefabs/Player/Player.Prefab";
    private Vector3 spawnPoint = Vector3.zero;
    private AsyncOperationHandle<GameObject> m_PlayerLoadOpHandle;

    public PlayerLoader(Vector3 SpawnPoint)
    {
        this.spawnPoint = SpawnPoint;
    }

    public void LoadPlayer()
    {
        m_PlayerLoadOpHandle = Addressables.LoadAssetAsync<GameObject>(playerPrefabAddress);
        m_PlayerLoadOpHandle.Completed += OnPlayerLoaded;
    }

    private void OnPlayerLoaded(AsyncOperationHandle<GameObject> m_PlayerLoadOpHandle)
    {
        if (m_PlayerLoadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Spawning Player...");
            SpawnPlayer(m_PlayerLoadOpHandle.Result);
            LoadPlayerData();
        }
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        GameObject.Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
    }

    private void LoadPlayerData()
    {

    }
}
