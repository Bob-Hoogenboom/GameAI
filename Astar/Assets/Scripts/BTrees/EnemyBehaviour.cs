using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script attached to the enemy
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    private BehaviourTree _tree;

    private void Start()
    {
        _tree = new BehaviourTree();
        Node pickUpWeapon = new Node("pick_up_weapon");
        Node goToWeapon = new Node("go_to_weapon");
        Node attackPlayer = new Node("attack_player");

        pickUpWeapon.AddChild(goToWeapon);
        pickUpWeapon.AddChild(attackPlayer);

        _tree.AddChild(pickUpWeapon);

        _tree.DebugTree();
    }
}
