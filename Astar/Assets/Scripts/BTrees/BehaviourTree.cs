using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basicly a root node, it holds all the node underneath and is the start of the 'Tree' 
/// source<https://learn.unity.com/tutorial/nodes-jxw?uv=2021.3&courseId=5dd851beedbc2a1bf7b72bed&projectId=60645258edbc2a001f5585aa#63238ddfedbc2a438158ed6a>
/// </summary>
public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        name = "Tree";
    }

    public BehaviourTree(string n)
    {
        name = n;
    }

    public void DebugTree()
    {
        //without recursion
        string treePrintOut = "N/A";

        Stack<Node> nodeStack = new Stack<Node>();
        Node currentNode = this;
        nodeStack.Push(currentNode); //adds the root of the node in the stack

        while (nodeStack.Count != 0)
        {
            Node nextNode = nodeStack.Pop();
            treePrintOut += nextNode.name + "\n";

            //print the tree in reverse (reverse from stack.Pop())
            for (int i = nextNode.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(nextNode.children[i]);
            }
        }
        Debug.Log(treePrintOut);
        
    }
}
