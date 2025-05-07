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

        Debug.Log(_behaviourTree.activeNode);
        debugText.text = $"Active Node: {_behaviourTree.activeNode.name}";
    }

    public void SetTree(BehaviourTree tree)
    {
        _behaviourTree = tree;
    }
}