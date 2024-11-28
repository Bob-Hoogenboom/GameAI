using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        //failsafe
        if(!IsWithinBounds(endPos, grid))
        {
            Debug.LogWarning("EndPosition is out of bounds, try another position");
            return new List<Vector2Int>();
        }

        // 2 lists to keep track of nodes we need to search and nodes we have searched
        List<Node> toSearch = new List<Node>();
        List<Node> searched = new List<Node>();

        // Create start and goal nodes
        Node startNode = new Node(startPos, null, 0, CalculateHeuristic(startPos, endPos));
        Node goalNode = new Node(endPos, null, 0, 0);

        toSearch.Add(startNode);

        while (toSearch.Count > 0)
        {
            Node currentNode = toSearch.OrderBy(node => node.FScore).First();

            if (currentNode.position == goalNode.position)
                return ReconstructPath(currentNode);

            toSearch.Remove(currentNode);
            searched.Add(currentNode);

            // Get valid neighbors of the current node
            foreach (Node neighbor in GetNeighbors(currentNode, grid, endPos))
            {
                if (searched.Contains(neighbor))
                    continue;

                float nodeCost = currentNode.GScore + 1; 

                if (!toSearch.Contains(neighbor) || nodeCost < neighbor.GScore)
                {
                    neighbor.parent = currentNode;
                    neighbor.GScore = nodeCost;
                    neighbor.HScore = CalculateHeuristic(neighbor.position, endPos);

                    if (!toSearch.Contains(neighbor))
                        toSearch.Add(neighbor);
                }
            }
        }

        // no path was found
        return new List<Vector2Int>();
    }

    // Checks if the grid position is within bounds
    private bool IsWithinBounds(Vector2Int position, Cell[,] grid)
    {
        return position.x >= 0 && position.y >= 0 &&
               position.x < grid.GetLength(0) && position.y < grid.GetLength(1);
    }

    // We need the Heuristic value to determain a path
    private int CalculateHeuristic(Vector2Int current, Vector2Int goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    // Function to return the path by returning the parenmts
    private List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.position);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    // Function to get valid neighbours
    private List<Node> GetNeighbors(Node node, Cell[,] grid, Vector2Int goal)
    {
        List<Node> neighbors = new List<Node>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Wall[] correspondingWalls = { Wall.UP, Wall.DOWN, Wall.LEFT, Wall.RIGHT };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int neighborPos = node.position + directions[i];

            // Check bounds
            if (!IsWithinBounds(neighborPos, grid)) continue;

            Cell currentCell = grid[node.position.x, node.position.y];
            Cell neighborCell = grid[neighborPos.x, neighborPos.y];

            // Check walls
            if (currentCell.HasWall(correspondingWalls[i])) continue;
            Wall oppositeWall = GetOppositeWall(correspondingWalls[i]);
            if (neighborCell.HasWall(oppositeWall)) continue;

            // Ensure neighbor is unique by position
            if (neighbors.Any(n => n.position == neighborPos)) continue;

            neighbors.Add(new Node(neighborPos, node, 0, 0));
        }

        return neighbors;
    }

    // Helper function to get the opposite wall
    private Wall GetOppositeWall(Wall wall)
    {
        switch (wall)
        {
            case Wall.UP: return Wall.DOWN;
            case Wall.DOWN: return Wall.UP;
            case Wall.LEFT: return Wall.RIGHT;
            case Wall.RIGHT: return Wall.LEFT;
            default: return 0;
        }
    }


    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
