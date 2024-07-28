using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private void Awake()
    {
        CustomTickSystem.InitializeTickSystem();

        CreateTerrainGrids();

        LoadItemsFromMemory();
    }

    private void Start()
    {

    }

    private void FireTerrainReady()
    {
        Debug.Log("Successfully generated Terrain grids !");
    }

    private void CreateTerrainGrids()
    {
        Terrain[]  terrainArray = Terrain.activeTerrains;


        Debug.Log("Start creating Terrain grids...");
        foreach (var terrain in terrainArray)
        {
            GenerateTerrainGrid terrainGridConstructor = terrain.gameObject.GetComponent<GenerateTerrainGrid>();
            terrainGridConstructor.TerrainReady.AddListener(FireTerrainReady);
            StartCoroutine(terrainGridConstructor.CreateGrids());
        }
    }

    private void LoadItemsFromMemory()
    {
        ItemLoader itemLoader = new ItemLoader();
        StartCoroutine(itemLoader.LoadItemsJSONFromMemory());
    }
}
