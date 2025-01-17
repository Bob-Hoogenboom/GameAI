using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NinjaBehaviour : MonoBehaviour
{
    [Header("References")]
    private BehaviourTree _tree;
    private NavMeshAgent _agent;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
