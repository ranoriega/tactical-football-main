using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] Vector2Int startCords;
    [SerializeField] Vector2Int targetCords;

    public Vector2Int StartCords { get { return startCords; } }

  
    public Vector2Int TargetCords { get { return targetCords; } }

    Node startNode;
    Node targetNode;
    public Node currentNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    Vector2Int[] searchOrder = {Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down};
    public static Pathfinding Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (gridManager != null)
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

   public List<Node> GetReachableArea(Vector2Int startCoordinates, int range)
{
    Node startNode = grid[startCoordinates];

    Queue<(Node node, int dist)> frontier = new Queue<(Node, int)>();
    HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
    List<Node> result = new List<Node>();

    frontier.Enqueue((startNode, 0));
    visited.Add(startCoordinates);

    while (frontier.Count > 0)
    {
        var current = frontier.Dequeue();
        Node node = current.node;
        int dist = current.dist;

        // no incluir el nodo inicial
        if (dist > 0)
        {
            result.Add(node);
            node.inRange = true; // <- para pintar en azul
        }

        if (dist >= range)
            continue;

        foreach (Vector2Int dir in searchOrder)
        {
            Vector2Int nextPos = node.cords + dir;

            if (!grid.ContainsKey(nextPos))
                continue;

            if (visited.Contains(nextPos))
                continue;

            Node nextNode = grid[nextPos];

            if (!nextNode.walkable)
                continue;

            visited.Add(nextPos);
            frontier.Enqueue((nextNode, dist + 1));
        }
    }

    return result;
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
