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
    private float _timer = 3f;
    private float _currentTimer = 3f;


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _tree = new BehaviourTree();
        Selector enemy = new Selector("enemy_selector");
        Sequence attack = new Sequence("attack");
        Leaf checkPlayerRange = new Leaf("check_player_range", CheckPlayerRange);
        Leaf goToWeaponA = new Leaf("go_to_weapon_A", GoToWeaponA);
        Leaf goToWeaponB = new Leaf("go_to_weapon_B", GoToWeaponB);
        Leaf attackPlayer = new Leaf("attack_player", AttackPlayer);
        Selector PickWeapon = new Selector("Pick_Weapon");
       


        PickWeapon.AddChild(goToWeaponA);
        PickWeapon.AddChild(goToWeaponB);

        attack.AddChild(checkPlayerRange);
        attack.AddChild(PickWeapon);
        attack.AddChild(attackPlayer);
        _tree.AddChild(attack);

        _tree.DebugTree();
    }

    public Node.Status CheckPlayerRange()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILED;
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
            if(distance >= 10)
            {
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

    //while all actions have failed or are running we keep on updating
    //we want to stop as soon as the player has died*
    void Update()
    {
        if(treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = _tree.Process();
        }
    }
}
