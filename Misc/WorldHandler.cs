using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldHandler : MonoBehaviour
{
    public static UnityEvent worldReady;
    private void Awake()
    {
        worldReady = new UnityEvent();
        worldReady.AddListener(OnWorldReady);
        CustomTickSystem.InitializeTickSystem();
        StartCoroutine(CreateTerrainGrids());
    }

    private void Start()
    {

    }

    private void FireTerrainReady(string terrainName)
    {
        Debug.Log("Successfully generated Terrain grids for terrain" + terrainName + "!");
    }

    private void OnWorldReady()
    {
        GameManager.SpawnPlayer();
    }

    public IEnumerator CreateTerrainGrids()
    {
        Terrain[] terrainArray = Terrain.activeTerrains;


        Debug.Log("Start creating Terrain grids...");
        foreach (var terrain in terrainArray)
        {
            GenerateTerrainGrid terrainGridConstructor = terrain.gameObject.GetComponent<GenerateTerrainGrid>();
            //terrainGridConstructor.OnTerrainReady.AddListener(FireTerrainReady);
            StartCoroutine(terrainGridConstructor.CreateGrids());
        }
        yield return null;
        worldReady.Invoke();
    }

}
