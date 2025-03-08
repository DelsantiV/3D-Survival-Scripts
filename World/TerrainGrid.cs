using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;

namespace GoTF.World
{
    public class TerrainGrid : MonoBehaviour
    {
        public class TerrainReady : UnityEvent<string> { }
        public new string name { get; private set; }

        private Terrain terrain;

        [SerializeField] private int smallGridSize = 2;
        [SerializeField] private int largeGridSize = 128;
        public GridCell[,] smallGrid;
        public GridCell[,] largeGrid;
        public Vector3 gridOrigin;


        private int length;
        private int width;
        private float conversionFactorX;
        private float conversionFactorZ;

        public TerrainReady OnTerrainReady;

        public void InitializeConstructor()
        {
            terrain = GetComponent<Terrain>();
            int[,] detailLayer = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 1);
            conversionFactorX = terrain.terrainData.detailWidth / terrain.terrainData.bounds.max.x;
            conversionFactorZ = terrain.terrainData.detailHeight / terrain.terrainData.bounds.max.z;
        }
        public GridCell[,] ConstructGrid(int gridSize, Color debugLinesColor)
        {
            gridOrigin = terrain.GetPosition();
            name = "center : " + gridOrigin.ToString();
            length = Mathf.FloorToInt(terrain.terrainData.detailWidth / gridSize);
            width = Mathf.FloorToInt(terrain.terrainData.detailHeight / gridSize);
            GridCell[,] grid = new GridCell[length, width];

            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    Vector3 gridCenterWorldPosition = GetGridCellCenterWorldPosition(x, z, gridSize);

                    grid[x, z] = new GridCell(
                        gridCenterWorldPosition,
                        new Vector2Int(x, z), GetCellBounds(x, z, gridSize),
                        terrain.terrainData.GetSteepness(gridCenterWorldPosition.x,
                        gridCenterWorldPosition.z)
                        );

                    //Debug.DrawLine(GetWorldPosition(x, z, gridSize), GetWorldPosition(x, z + 1, gridSize), debugLinesColor, 100f);
                    //Debug.DrawLine(GetWorldPosition(x, z, gridSize), GetWorldPosition(x + 1, z, gridSize), debugLinesColor, 100f);
                }
            }
            //Debug.DrawLine(GetWorldPosition(0, width, gridSize), GetWorldPosition(length, width, gridSize), debugLinesColor, 100f);
            //Debug.DrawLine(GetWorldPosition(length, 0, gridSize), GetWorldPosition(length, width, gridSize), debugLinesColor, 100f);

            return grid;
        }

        public IEnumerator CreateGrids()
        {
            InitializeConstructor();
            smallGrid = ConstructGrid(smallGridSize, Color.blue);
            //largeGrid = ConstructGrid(largeGridSize, Color.red);
            yield return smallGrid;
            //OnTerrainReady.Invoke(name);
        }

        private Vector3 GetWorldPosition(int x, int z, int gridSize)
        {
            Vector3 worldPosition = new Vector3(x / conversionFactorX, 0, z / conversionFactorZ) * gridSize; ;
            worldPosition.y = terrain.SampleHeight(worldPosition);
            return worldPosition;
        }

        public GridCell GetGridCell(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - terrain.GetPosition().x) * conversionFactorX / smallGridSize);
            int z = Mathf.FloorToInt((worldPosition.z - terrain.GetPosition().z) * conversionFactorZ / smallGridSize);
            return smallGrid[x, z];
        }

        public GridCell GetGridCell(int x, int z)
        {
            return smallGrid[x, z];
        }

        private Vector3 GetGridCellCenterWorldPosition(int x, int z, int gridSize)
        {
            Vector3 worldPosition = new Vector3((x + 0.5f) / conversionFactorX, 0, (z + 0.5f) / conversionFactorZ) * gridSize;
            worldPosition.y = terrain.SampleHeight(worldPosition);
            return worldPosition;
        }

        private Vector3[,] GetEdgesPositions(int x, int z, int gridSize)
        {
            Vector3[,] edges = new Vector3[2, 2];
            edges[0, 0] = GetWorldPosition(x, z, gridSize);
            edges[0, 1] = GetWorldPosition(x, z + 1, gridSize);
            edges[1, 0] = GetWorldPosition(x + 1, z, gridSize);
            edges[1, 1] = GetWorldPosition(x + 1, z + 1, gridSize);
            return edges;
        }

        private Vector2 GetCellPositionBoundLow(int x, int z, int gridSize)
        {
            return new Vector2(x / conversionFactorX, z / conversionFactorZ) * gridSize;
        }

        private GridCell.cellBounds GetCellBounds(int x, int z, int gridSize)
        {
            GridCell.cellBounds bounds = new GridCell.cellBounds();
            bounds.min = GetCellPositionBoundLow(x, z, gridSize);
            bounds.max = GetCellPositionBoundLow(x + 1, z + 1, gridSize);
            return bounds;
        }

        public void SetDetailLayerForCell(GridCell cell, int layer, int value)
        {
            int[,] detailLayer = terrain.terrainData.GetDetailLayer(cell.index.x * smallGridSize, cell.index.y * smallGridSize, smallGridSize, smallGridSize, layer);
            for (int i = 0; i < detailLayer.GetLength(0); i++)
            {
                for (int j = 0; j < detailLayer.GetLength(1); j++)
                {
                    detailLayer.SetValue(value, i, j);
                }
            }
            terrain.terrainData.SetDetailLayer(cell.index.x * smallGridSize, cell.index.y * smallGridSize, layer, detailLayer);
        }

        public bool IsInsideBounds(Vector3 position)
        {
            if (position.x > terrain.terrainData.bounds.max.x) { return false; }
            if (position.x < terrain.terrainData.bounds.min.x) { return false; }
            if (position.z > terrain.terrainData.bounds.max.z) { return false; }
            if (position.z < terrain.terrainData.bounds.min.z) { return false; }

            return true;
        }
    }
}
