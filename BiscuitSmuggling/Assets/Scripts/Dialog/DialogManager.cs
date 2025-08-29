using cherrydev;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    // New events to notify when a dialog starts or ends
    public UnityEvent<DialogType> OnDialogStarted = new UnityEvent<DialogType>();
    public UnityEvent<DialogType> OnDialogEnded = new UnityEvent<DialogType>();

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

                // Notify listeners that a dialog has started
                OnDialogStarted.Invoke(type);

                // Subscribe to OnDialogStarted and OnDialogFinished
                window.behavior.OnDialogStarted.AddListener(() => HandleDialogStarted(type));

                window.behavior.StartDialog(graph);
                window.behavior.BindExternalFunction("Accept", behavior.Accept);
                window.behavior.BindExternalFunction("Reject", behavior.Reject);
                window.behavior.DialogEnded += () => HandleDialogEnded(type); // Subscribe to the DialogEnded event
                return;
            }
            else
            {
                window.behavior.gameObject.SetActive(false);
            }

        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    private void HandleDialogEnded(DialogType type)
    {
        if (CurrentDialog != null)
        {
            CurrentDialog.gameObject.SetActive(false); // Deactivate the dialog window
            CurrentDialog.DialogEnded -= () => HandleDialogEnded(type); // Unsubscribe from the event
            CurrentDialog = null; // Set CurrentDialog to null

            // Notify listeners that a dialog has ended
            OnDialogEnded.Invoke(type);
        }
    }

    private void HandleDialogStarted(DialogType type)
    {
        OnDialogStarted.Invoke(type);
    }
}

public enum DialogType
{
    Interact,
    Talk
}