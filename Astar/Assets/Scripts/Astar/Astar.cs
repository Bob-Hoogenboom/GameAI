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
        //OutOfGridBounds check
        if (!IsWithinGrid(endPos, grid))
        {
            Debug.LogWarning("EndPosition is out of bounds, try another position");
            return new List<Vector2Int>();
        }

        List<Node> toSearch = new List<Node>();
        List<Node> visited = new List<Node>();

        Node startNode = new Node(startPos,null, 0, CalculateHeuristic(startPos, endPos));
        Node endNode = new Node(endPos, null, 0, 0);

        toSearch.Add(startNode);

        while (toSearch.Any())
        {
            if (!toSearch.Any())
            {
                Debug.LogWarning("No path found to target.");
                return null;
            }

            Node currentNode = toSearch.OrderBy(node => node.FScore).First(); //orders the list by Lowest FScore

            if (currentNode.position == endNode.position)
            {
                return ReconstructPath(currentNode);
            }

            toSearch.Remove(currentNode);
            visited.Add(currentNode);

            Cell currentCell = GetCellByPos(currentNode.position, grid);

            List<Cell> neighbourCells = currentCell.GetNeighbours(grid);

            foreach (Cell cell in neighbourCells)
            {
                //if in open update || if not then make
                Node neighbourNode = new Node(cell.gridPosition, currentNode, currentNode.GScore, CalculateHeuristic(cell.gridPosition, endPos));

                if (visited.Contains(neighbourNode)) // We are assuming everything is traversable so only check contain*
                { 
                    continue; 
                }

                if (!toSearch.Contains(neighbourNode))
                {
                    neighbourNode.GScore = neighbourNode.parent.GScore +1;
                    neighbourNode.parent = currentNode;

                    toSearch.Add(neighbourNode);
                }

            }

        }
        return null;
    }

    private bool IsWithinGrid(Vector2Int position, Cell[,] grid)
    {
        return position.x >= 0 && position.y >= 0 && position.x < grid.GetLength(0) && position.y < grid.GetLength(1);
    }

    //finds Cell based on GridPosition
    private Cell GetCellByPos(Vector2Int value, Cell[,] grid)
    {
        return grid[value.x, value.y];
    }

    //for calculating Hscore
    private int CalculateHeuristic(Vector2Int current, Vector2Int goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    private List<Vector2Int> ReconstructPath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();
        return path;
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
        public int GScore; //Current Travelled Distance
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
