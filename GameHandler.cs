using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private Terrain[] terrainArray;
    private void Awake()
    {
        CustomTickSystem.InitializeTickSystem();

        terrainArray = Terrain.activeTerrains;


        Debug.Log("Start creating terrain grids...");
        foreach (var terrain in terrainArray)
        {
            GenerateTerrainGrid terrainGridConstructor = terrain.gameObject.GetComponent<GenerateTerrainGrid>();
            terrainGridConstructor.CreateGrids();
        }
        Debug.Log("Terrain grids created !");
    }

    private void Start()
    {

    }
}
