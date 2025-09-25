using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Vector2Int startCords;
    public Vector2Int StartCords { get { return startCords; } }

    [SerializeField] Vector2Int targetCords;
    public Vector2Int TargetCords { get { return targetCords; } }

    Node startNode;
    Node targetNode;
    public Node currentNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    Vector2Int[] searchOrder = {Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down};


    private void Awake()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        if(gridManager != null)
        {
            grid = gridManager.Grid;
        }
    }
    // public List<Node> GetNewPath()
    // {
    //     return GetNewPath(startCords);
    // }

public List<Node> GetNewPath(Vector2Int startCoordinates, Vector2Int targetCoordinates)
{
    startNode = grid[startCoordinates];
    targetNode = grid[targetCoordinates];

    gridManager.ResetNodes();

    frontier.Clear();
    reached.Clear();

    frontier.Enqueue(startNode);
    reached.Add(startCoordinates, startNode);

    bool isRunning = true;
    while (frontier.Count > 0 && isRunning)
    {
        currentNode = frontier.Dequeue();
        currentNode.explored = true;
        ExploreNeighbors();
        if (currentNode.cords == targetCoordinates)
        {
            isRunning = false;
            //currentNode.walkable = false;
        }
    }

    return BuildPathFrom(targetNode);
}

List<Node> BuildPathFrom(Node endNode)
{
    List<Node> path = new List<Node>();
    Node currentNode = endNode;

    if (currentNode == null)
        return path;

    path.Add(currentNode);
    currentNode.path = true;

    while (currentNode.connectTo != null)
    {
        currentNode = currentNode.connectTo;
        path.Add(currentNode);
        currentNode.path = true;
    }

    path.Reverse();
    return path;
}


    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector2Int direction in searchOrder)
        {
            Vector2Int neighborCords = currentNode.cords + direction;

            if (grid.ContainsKey(neighborCords))
            {
                neighbors.Add(grid[neighborCords]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.cords) && neighbor.walkable)
            {
                neighbor.connectTo = currentNode;
                reached.Add(neighbor.cords, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        path.Add(currentNode);
        currentNode.path = true;

        while (currentNode.connectTo != null)
        {
            currentNode = currentNode.connectTo;
            path.Add(currentNode);
            currentNode.path = true;
        }

        path.Reverse();
        return path;
    }


    public void NotifyReceievers()
    {
        BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
    }

    public void SetNewDestination(Vector2Int startCoordinates, Vector2Int targetCoordinates)
    {
        startCords = startCoordinates;
        targetCords = targetCoordinates;
        startNode = grid[this.startCords];
        targetNode = grid[this.targetCords];
     //   GetNewPath();
    }
}
