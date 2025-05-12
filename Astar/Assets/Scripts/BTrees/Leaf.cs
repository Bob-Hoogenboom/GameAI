using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    //delegate
    public delegate Status Tick(); //tick is like an update frame
    public Tick ProcessMethod;

    public Leaf() { }
    public Leaf(string n, Tick pm)
    {
        name = n;
        ProcessMethod = pm;
    }

    public override Status Process()
    {
        SetActive();
        if (ProcessMethod != null) return ProcessMethod();
        return Status.FAILED;
    }
}
