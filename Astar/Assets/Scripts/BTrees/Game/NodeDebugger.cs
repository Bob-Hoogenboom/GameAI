using TMPro;
using UnityEngine;

public class NodeDebugger : MonoBehaviour
{
    public EnemyBehaviour enemy;
    public TMP_Text debugText;

    private BehaviourTree _behaviourTree;
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(_cam.transform);

        if (Node.currentActiveNode != null)
        {
            debugText.text = $"Active Node: \n{Node.currentActiveNode.name}";
        }
        else
        {
            debugText.text = "Active Node: None";
        }
    }

    public void SetTree(BehaviourTree tree)
    {
        _behaviourTree = tree;
    }
}