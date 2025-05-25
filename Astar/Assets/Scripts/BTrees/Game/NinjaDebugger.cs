using TMPro;
using UnityEngine;

public class NinjaDebugger: MonoBehaviour
{
    public NinjaBehaviour ninja;
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

        if (_behaviourTree != null && _behaviourTree.activeNode != null)
        {
            debugText.text = $"Active Node: \n{_behaviourTree.activeNode.name}";
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

    public BehaviourTree GetTree()
    {
        return _behaviourTree;
    }
}