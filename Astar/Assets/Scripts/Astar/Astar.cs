using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private List<Node> _nodeBase = new List<Node>();
    private Node _currentNode;
    private Node _goalNode;

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
        //makes the NodeGrid once except if its count changes then it regenerates
        if (_nodeBase.Count != grid.Length) 
        {
            Debug.Log("You are missing some Nodes, Let me help you! NodeCount: " + _nodeBase.Count() + " GridLength: " + grid.Length);
            _nodeBase.Clear();
            _nodeBase = InstantiateNodes(grid); 
        }

        //Make 2 Lists:
        List<Node> toSearch = new List<Node>();
        List<Node> visited = new List<Node>();

        //Define start en goal pos:
        _currentNode = GetNodeByGridPos(startPos);
        _goalNode = GetNodeByGridPos(endPos);


        //continue aStar as normal but check in one of the while loops for walls#




        return null;
    }

    //instantiates node grid by cell grid
    private List<Node> InstantiateNodes(Cell[,] grid)
    {
        List<Node> nodes = new List<Node>();
        foreach (Cell cell in grid)
        {
            Node node = new Node();
            node.position = cell.gridPosition;

            nodes.Add(node); 
        }
        return nodes;
    }

    //finds Node based on GridPosition
    private Node GetNodeByGridPos(Vector2Int value)
    {
        foreach (Node node in _nodeBase) 
        {
            if (node.position == value) return node;
        }

        return null;
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
