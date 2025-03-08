using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoTF.GameLoading;
using GoTF.Utilities;

namespace GoTF.World
{
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

        private void FireTerrainReadyEvent(string terrainName)
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
                TerrainGrid terrainGridConstructor = terrain.gameObject.GetComponent<TerrainGrid>();
                //terrainGridConstructor.OnTerrainReady.AddListener(FireTerrainReady);
                StartCoroutine(terrainGridConstructor.CreateGrids());
            }
            yield return null;
            worldReady.Invoke();
        }

    }
}
