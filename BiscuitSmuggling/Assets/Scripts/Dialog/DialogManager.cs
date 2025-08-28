using cherrydev;
using System;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private static DialogManager _instance;

    public static DialogManager Instance => _instance;

    [Serializable]
    private class DialogWinow
    {
        public DialogType type;
        public DialogBehaviour behavior;
    }

    [SerializeField] private DialogWinow[] _dialogWindows;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowDialog(DialogType type, DialogNodeGraph graph, InteractiveObjectBehavior behavior)
    {
        foreach (var window in _dialogWindows)
        {
            if (window.type == type)
            {
                window.behavior.StartDialog(graph);
                window.behavior.BindExternalFunction("Accept", behavior.Accept);
                window.behavior.BindExternalFunction("Reject", behavior.Reject);
                return;
            }
        }
        Debug.LogError($"No dialog window found for type: {type}");
    }
}

public enum DialogType
{
    Interact,
    Talk
}