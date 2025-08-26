using cherrydev;
using UnityEngine;

public class TestDialogInit : MonoBehaviour
{
    [SerializeField] private DialogBehaviour _behavior;
    [SerializeField] private DialogNodeGraph _graph;

    private void Start()
    {
        _behavior.StartDialog(_graph);

    }
}
