using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NinjaBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Transform> hidingSpots = new List<Transform>();
    public GameObject player;
    public EnemyBehaviour enemy;
    public NinjaDebugger debugger;
    public NavMeshAgent agent;
    private BehaviourTree _tree;

    [Header("Variables")]
    public float checkRange;
    private Transform _idealHidingSpot;

    //#TODO make a collective Enum for the ninja and Enemy
    public enum ActionState { IDLE, MOVING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    [Header("SmokeBomb Variables")]
    private float _timer = 3f;
    private float _currentTimer = 3f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        debugger.SetTree(_tree);



        //---
        Leaf checkEnemyAttacking = new Leaf("check_enemy_attacking", CheckEnemyAttack);
        Leaf getWaypoint = new Leaf("get_waypoint", GetNewWaypoint);
        Leaf goToWaypoint = new Leaf("go_to_waypoint", GoToWaypoint);
        Leaf confuseEnemy = new Leaf("confuse_enemy", ThrowSmokeBomb);

        //--
        Leaf followPlayer = new Leaf("follow_player", FollowPlayer);

        Sequence hide = new Sequence("hide");
        hide.AddChild(checkEnemyAttacking);
        hide.AddChild(getWaypoint);
        hide.AddChild(goToWaypoint);
        hide.AddChild(confuseEnemy);

        //-
        Selector ninjaAction = new Selector("ninja_action");
        ninjaAction.AddChild(followPlayer);
        ninjaAction.AddChild(hide);

        //~
        _tree.AddChild(ninjaAction);

        _tree.DebugTree();
    }

    public Node.Status CheckEnemyAttack()
    {
        if (enemy.isAttackingPlayer)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILED;
    }

    public Node.Status GetNewWaypoint()
    {
        float closestDistance = 1000f;
        foreach(Transform trans in hidingSpots)
        {
            var dist = Vector3.Distance(transform.position, trans.position);
            if(dist < closestDistance)
            {
                _idealHidingSpot = trans;
                closestDistance = dist;
            }
        }

        if (NavMesh.SamplePosition(_idealHidingSpot.position, out _, 1.0f, NavMesh.AllAreas)) return Node.Status.SUCCESS;

        return Node.Status.FAILED;
    }

    public Node.Status GoToWaypoint()
    {
        //check if player is in range when going to the waypoint
        if (CheckEnemyAttack() == Node.Status.SUCCESS) { return Node.Status.FAILED; }
        return GoToLocation(_idealHidingSpot.position);
    }

    private Node.Status GoToLocation(Vector3 destination)
    {
        float distanceFromTarget = Vector3.Distance(transform.position, destination);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.MOVING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 1)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILED;
        }
        else if (distanceFromTarget < 1)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    public Node.Status FollowPlayer()
    {
        if (CheckEnemyAttack() == Node.Status.SUCCESS) { return Node.Status.FAILED; }
        return GoToLocation(player.transform.position);
    }

    public Node.Status ThrowSmokeBomb()
    {

        _currentTimer -= Time.deltaTime;

        if (_currentTimer < 0)
        {
            _currentTimer = _timer;
            EnemyBehaviour targetEnemy = enemy.GetComponent<EnemyBehaviour>();
            targetEnemy.StunEnemy();
            return Node.Status.SUCCESS;
        }
        
        return Node.Status.RUNNING;
    }

    // Update is called once per frame
    private void Update()
    {
        treeStatus = _tree.Process();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRange);
    }
}
