using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// script attached to the enemy
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public EnemyDebugger debugger;
    public NavMeshAgent agent;
    private BehaviourTree _tree;

    public bool isAttackingPlayer = false;
    public float checkRange = 5f;
    public Vector3 patrolOrigin = new Vector3(0, 0, 0);

    //#TODO make a collective Enum for the ninja and Enemy
    public enum ActionState { IDLE, MOVING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;


    [Header("Stun Variables")]
    public bool isStunned = false;
    public float stunDuration = 3f;
    private float stunTimer = 3f;

    [Header("Patrol Variables")]    
    public Vector3 waypoint;
    public float waypointRange = 7f;


    [Header("Attack Variables")]
    public GameObject weaponA;
    public GameObject weaponB;

    private float _timer = 3f;
    private float _currentTimer = 3f;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        debugger.SetTree(_tree);



        // ----
        Leaf goToWeaponA = new Leaf("go_to_weapon_A", GoToWeaponA);
        Leaf goToWeaponB = new Leaf("go_to_weapon_B", GoToWeaponB);

        // ---
        Leaf getWaypoint = new Leaf("get_waypoint", GetNewWaypoint);
        Leaf goToWaypoint = new Leaf("go_to_waypoint", GoToWaypoint);
        Leaf idle = new Leaf("idle", Idle);
        Leaf returnToOrigin = new Leaf("return_to_origin", ReturnToOrigin);

        Leaf checkPlayerRange = new Leaf("check_player_range", CheckPlayerRange);
        Selector pickWeapon = new Selector("pick_Weapon");
        pickWeapon.AddChild(goToWeaponA);
        pickWeapon.AddChild(goToWeaponB);
        Leaf attackPlayer = new Leaf("attack_player", AttackPlayer);

        // --
        Sequence patrol = new Sequence("patrol");
        patrol.AddChild(getWaypoint);
        patrol.AddChild(goToWaypoint);
        patrol.AddChild(idle);
        patrol.AddChild(returnToOrigin);

        Sequence attack = new Sequence("attack");
        attack.AddChild(checkPlayerRange);
        attack.AddChild(pickWeapon);
        attack.AddChild(attackPlayer);

        // -
        Selector enemyAction = new Selector("enemy_action");
        enemyAction.AddChild(attack);
        enemyAction.AddChild(patrol);
        
        //~
        _tree.AddChild(enemyAction);
        
        _tree.DebugTree();
    }

    public Node.Status CheckPlayerRange()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist < checkRange)
        {
            isAttackingPlayer = true;
            return Node.Status.SUCCESS;
        }
        isAttackingPlayer = false;
        return Node.Status.FAILED;
    }

    //every method you give to a Node has to use the same format as the Tick() method

    public Node.Status Idle()
    {
        if (CheckPlayerRange() == Node.Status.SUCCESS) { return Node.Status.FAILED; }

        _currentTimer -= Time.deltaTime;

        if (_currentTimer < 0)
        {
            _currentTimer = _timer;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    public Node.Status GetNewWaypoint()
    {
        float randomZ = Random.Range(-waypointRange, waypointRange);
        float randomX = Random.Range(-waypointRange, waypointRange);

        waypoint = new Vector3(this.transform.position.x + randomX, this.transform.position.y, this.transform.position.z + randomZ);

        //checks if the waypoint is on the NavMesh 
        if (NavMesh.SamplePosition(waypoint, out _, 1.0f, NavMesh.AllAreas)) return Node.Status.SUCCESS;

        return Node.Status.FAILED;
    }

    public Node.Status GoToWaypoint()
    {
        //check if player is in range when going to the waypoint
        if (CheckPlayerRange() == Node.Status.SUCCESS) { return Node.Status.FAILED; }
        return GoToLocation(waypoint);
    }

    public Node.Status GoToWeaponA()
    {
        return GoToWeapon(weaponA);
    }

    public Node.Status GoToWeaponB()
    {
        return GoToWeapon(weaponB);
    }

    public Node.Status ReturnToOrigin()
    {
        return GoToLocation(patrolOrigin);
    }

    public Node.Status AttackPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        _currentTimer -= Time.deltaTime;

        if (distance <= checkRange)
        {
            PlayerMovement playerMove = player.GetComponent<PlayerMovement>();
            GoToLocation(transform.position);
            if (_currentTimer < 0)
            {
                _currentTimer = _timer;
                Debug.Log("Pew!");
                playerMove.health--;

                if (playerMove.health <= 0)
                {
                    return Node.Status.SUCCESS;
                }
            }
        }
        else
        {
            if(distance >= waypointRange)
            {
                Debug.Log("failed");
                return Node.Status.FAILED;
            }

            GoToLocation(player.transform.position);
            _currentTimer = _timer;
        }


        return Node.Status.RUNNING;
    }

    public Node.Status GoToWeapon(GameObject weapon)
    {
        Node.Status s = GoToLocation(weapon.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!weapon.GetComponent<Gun>().empty) //check if gun is not empty = SUCCES
            {
                weapon.transform.parent = this.gameObject.transform;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILED; //gun is empty = FAILED, check next gun
        }
        else
        {
            return s;
        }
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

    //while all actions have failed or are running we keep on updating
    //we want to stop as soon as the player has died*
    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                isAttackingPlayer = false;
                stunTimer = stunDuration;
            }
            return; 
        }

        treeStatus = _tree.Process();
    }

    public void StunEnemy()
    {
        isStunned = true;
        agent.ResetPath();
        state = ActionState.IDLE;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(waypoint, 0.3f);
        Gizmos.DrawWireSphere(transform.position, checkRange);
        Gizmos.DrawWireSphere(transform.position, waypointRange);
    }
}
