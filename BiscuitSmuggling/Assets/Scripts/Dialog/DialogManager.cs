using cherrydev;
using System;
using System.Linq;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private static DialogManager _instance;

    public static DialogManager Instance => _instance;

    public DialogBehaviour CurrentDialog { get; private set; }

    [Serializable]
    private class DialogWinow
    {
        public DialogType[] type;
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
            if (window.type.Contains(type))
            {
                CurrentDialog = window.behavior;
                window.behavior.gameObject.SetActive(true);
                window.behavior.StartDialog(graph);
                window.behavior.BindExternalFunction("Accept", behavior.Accept);
                window.behavior.BindExternalFunction("Reject", behavior.Reject);
                window.behavior.DialogEnded += OnDialogEnded; // Subscribe to the DialogEnded event
                return;
            }
            else
            {
                window.behavior.gameObject.SetActive(false);
            }

        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    private void OnDialogEnded()
    {
        if (CurrentDialog != null)
        {
            CurrentDialog.gameObject.SetActive(false); // Deactivate the dialog window
            CurrentDialog.DialogEnded -= OnDialogEnded; // Unsubscribe from the event
            CurrentDialog = null; // Set CurrentDialog to null
        }
    }
}

public enum DialogType
{
    Interact,
    Talk
}