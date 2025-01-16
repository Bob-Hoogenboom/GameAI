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

    public GameObject weaponA;
    public GameObject weaponB;
    public GameObject player;

    public enum ActionState { IDLE, MOVING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    //pseudo attack*
    private int _shots = 0;
    private float _timer = 5f;
    private float _currentTimer = 5f;


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        Sequence attack = new Sequence("attack");
        Leaf goToWeaponA = new Leaf("go_to_weapon_A", GoToWeaponA);
        Leaf goToWeaponB = new Leaf("go_to_weapon_B", GoToWeaponB);
        Leaf attackPlayer = new Leaf("attack_player", AttackPlayer);
        Selector PickWeapon = new Selector("Pick_Weapon");

        PickWeapon.AddChild(goToWeaponA);
        PickWeapon.AddChild(goToWeaponB);

        attack.AddChild(PickWeapon);
        attack.AddChild(attackPlayer);
        _tree.AddChild(attack);

        _tree.DebugTree();
    }

    //every method you give to a No de has to use the same format as the Tick() method
    public Node.Status GoToWeaponA()
    {
        return GoToWeapon(weaponA);
    }

    public Node.Status GoToWeaponB()
    {
        return GoToWeapon(weaponB);
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
                _currentTimer = _timer;
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
            _agent.SetDestination(destination);
            state = ActionState.MOVING;
        }
        else if (Vector3.Distance(_agent.pathEndPosition, destination) >= 1)
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

    void Update()
    {
        if(treeStatus == Node.Status.RUNNING)
        {
            treeStatus = _tree.Process();
        }
    }
}
