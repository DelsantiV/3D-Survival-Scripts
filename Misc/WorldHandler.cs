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
        worldReady.AddListener(FireTerrainReady);
        CustomTickSystem.InitializeTickSystem();
        StartCoroutine(CreateTerrainGrids());
    }

    private void Start()
    {

    }

    private void FireTerrainReady()
    {
        Debug.Log("Successfully generated Terrain grids !");
        GameManager.SpawnPlayer();
    }

    public IEnumerator CreateTerrainGrids()
    {
        Terrain[] terrainArray = Terrain.activeTerrains;


        Debug.Log("Start creating Terrain grids...");
        foreach (var terrain in terrainArray)
        {
            GenerateTerrainGrid terrainGridConstructor = terrain.gameObject.GetComponent<GenerateTerrainGrid>();
            terrainGridConstructor.TerrainReady.AddListener(FireTerrainReady);
            StartCoroutine(terrainGridConstructor.CreateGrids());
        }
        yield return null;
        worldReady.Invoke();
    }

}
