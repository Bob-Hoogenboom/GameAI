using UnityEngine;
using UnityEngine.AI;

public class NinjaBehaviour : MonoBehaviour
{
    [Header("References")]
    private BehaviourTree _tree;
    private NavMeshAgent _agent;
    public GameObject player;
    public NodeDebugger debugger;

    //#TODO make a collective Enum for the ninja and Enemy
    public enum ActionState { IDLE, MOVING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        debugger.SetTree(_tree);


        _tree.DebugTree();
    }

    // Update is called once per frame
    private void Update()
    {
        treeStatus = _tree.Process();
    }
}
