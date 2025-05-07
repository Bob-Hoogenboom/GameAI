using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basicly a root node, it holds all the node underneath and is the start of the 'Tree' 
/// source<https://learn.unity.com/tutorial/nodes-jxw?uv=2021.3&courseId=5dd851beedbc2a1bf7b72bed&projectId=60645258edbc2a001f5585aa#63238ddfedbc2a438158ed6a>
/// </summary>
public class BehaviourTree : Node
{
    public Node activeNode { get; private set; }

    public BehaviourTree()
    {
        name = "Tree";
    }

    public BehaviourTree(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        activeNode = children[currentChild];  
        return activeNode.Process();
    }

    struct NodeLevel 
    {
        public int level;
        public Node node;
    }

    public void DebugTree()
    {
        //without recursion
        string treePrintOut = "~";

        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode }); //adds the root of the node in the stack

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintOut += new string ('-', nextNode.level) + nextNode.node.name + "\n"; //the amount of '-'s defines what level the node is in the tree

            //print the tree in reverse (reverse from stack.Pop())
            for (int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }
        Debug.Log(treePrintOut);
    }
}
