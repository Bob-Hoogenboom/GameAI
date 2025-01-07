using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// source<https://learn.unity.com/tutorial/nodes-jxw?uv=2021.3&courseId=5dd851beedbc2a1bf7b72bed&projectId=60645258edbc2a001f5585aa#63238ddfedbc2a438158ed6a>
/// </summary>
public class Node 
{
    public enum Status { SUCCES, RUNNING, FAILED};
    public Status status;

    public List<Node> children = new List<Node> ();
    public int currentChild = 0;

    public string name;

    public Node() { }
    public Node(string n)
    {
        name = n;
    }

    public void AddChild(Node n)
    {
        children.Add(n);
    }
}
