using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library

public class PathfindingTest : MonoBehaviour {

    Pathfinder<Vector3> pathfinder;
    List<Vector3> path = new List<Vector3>();
    [SerializeField] LayerMask obstacles;
    [SerializeField] GameObject Target;
    [SerializeField] float gridSize;
    [SerializeField] int radius;
    private void Start()
    {
        Vector3 targetPosition = Target.transform.position;
        pathfinder = new Pathfinder<Vector3>(GetDistance, GetNeighbourNodes);
        Debug.Log(pathfinder.GenerateAstarPath(transform.position, targetPosition, out path));
        foreach(Vector3 node in path) { Debug.Log(node); }
    }
    /*
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //check for a new target
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var _path = new Vector3[0];
            if (pathfinder.GenerateAstarPath(transform.position, target, out path)) { path = new List<Vector3>(_path); }
            //if there is a path from current position to target position reassign path.
        }
        transform.position = path[0]; //go to next node
        path.RemoveAt(0); //remove the node from path
    }
    */

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { NavigateToTarget(Target.transform.position); }
    }

    private void NavigateToTarget(Vector3 targetPosition)
    {
        if (pathfinder.GenerateAstarPath(transform.position, targetPosition, out path))
        {
            if (path.Count > 0)
            {
                transform.position = path[0];
                path.RemoveAt(0);
            }
        }
    }

    float GetDistance(Vector3 A, Vector3 B)
    {
        return (A - B).sqrMagnitude;
    }

    Vector3 GetClosestNode(Vector3 target)
    {
        return new Vector3(Mathf.Round(target.x / gridSize) * gridSize, 0, Mathf.Round(target.z / gridSize) * gridSize);
    }
    Dictionary<Vector3, float> GetNeighbourNodes(Vector3 pos)
    {
        Dictionary<Vector3, float> neighbours = new Dictionary<Vector3, float>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            { 
                if (i == 0 && j == 0) continue;
                Vector3 dir = new Vector3(i, 0, j).normalized*gridSize;
                if (!Physics.Linecast(pos, pos + dir, obstacles))
                {
                    Debug.Log(dir + "  |  " + (pos + dir) + "  |  " + new Vector2 (i,j));
                    neighbours.Add((pos + dir), dir.magnitude);
                }
                
            }
        }
        return neighbours;
    }
}
