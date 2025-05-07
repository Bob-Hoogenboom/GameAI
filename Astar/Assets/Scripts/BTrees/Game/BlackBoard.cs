using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public Transform target;
    public Vector3 lastKnownPosition;
    public GameObject self;

    // You can use a dictionary for general purpose key-value storage
    public Dictionary<string, object> data = new Dictionary<string, object>();

    public T Get<T>(string key)
    {
        return data.TryGetValue(key, out var value) ? (T)value : default;
    }

    public void Set(string key, object value)
    {
        data[key] = value;
    }
}
