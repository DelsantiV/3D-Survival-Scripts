using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class GridCell
    {
        public struct cellBounds
        {
            public Vector2 min;
            public Vector2 max;
        }

        public Vector3 worldPostion;
        public Vector2Int index;
        public cellBounds bounds;
        public float steepness;
        public bool isWalkable;
        public float temperature;
        public float humidity;
        public float walkSpeedMultiplier;
        public int detailLayer;


        public GridCell(Vector3 worldPostion, Vector2Int index, cellBounds bounds, float steepness)
        {
            this.worldPostion = worldPostion;
            this.index = index;
            this.bounds = bounds;
            this.steepness = steepness;
        }

        public GridCell(Vector3 worldPosition, int detailLayer)
        {
            this.worldPostion = worldPosition;
            this.detailLayer = detailLayer;
        }
    }
}
