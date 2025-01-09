using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Astar;

/// <summary>
/// script attached to the enemy
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    private BehaviourTree _tree;
    private NavMeshAgent _agent;

    public GameObject weapon;
    public GameObject player;

    public enum ActionState { IDLE, MOVING };
    ActionState state = ActionState.IDLE;

    //pseudo attack*
    private int _shots = 0;
    private float _timer = 5f;
    private float _currentTimer = 5f;


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        Node pickUpWeapon = new Node("pick_up_weapon");
        Leaf goToWeapon = new Leaf("go_to_weapon", GoToWeapon);
        Leaf attackPlayer = new Leaf("attack_player", AttackPlayer);

        pickUpWeapon.AddChild(goToWeapon);
        pickUpWeapon.AddChild(attackPlayer);

        _tree.AddChild(pickUpWeapon);

        _tree.DebugTree();

        _tree.Process();

    }

    //every method you give to a Node has to use the same format as the Tick() method
    public Node.Status GoToWeapon()
    {
        return GoToLocation(weapon.transform.position);
    }

    public Node.Status AttackPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        _currentTimer -= Time.deltaTime;

        if (distance <= 3)
        {
            GoToLocation(transform.position);
            if (_currentTimer < 0)
            {
                Debug.Log("Pew!");
                _shots++;
                if (_shots >= 3) return Node.Status.SUCCESS;
            }
        }
        else
        {
            GoToLocation(player.transform.position);
            _shots = 0;
            _currentTimer = _timer;
        }


        return Node.Status.RUNNING;
    }

    private Node.Status GoToLocation(Vector3 destination)
    {
        float distanceFromTarget = Vector3.Distance(transform.position, destination);
        if (state == ActionState.IDLE)
        {
            _agent.SetDestination(destination);
            state = ActionState.MOVING;
        }
        else if (Vector3.Distance(_agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILED;
        }
        else if (distanceFromTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }
}
