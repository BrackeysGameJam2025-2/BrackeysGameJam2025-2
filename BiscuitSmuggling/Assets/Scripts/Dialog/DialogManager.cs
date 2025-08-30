using cherrydev;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class DialogManager : MonoBehaviour
{
    private static DialogManager _instance;

    public static DialogManager Instance => _instance;

    public DialogBehaviour CurrentDialog { get; private set; }

    public string CurrentDialogName => CurrentDialog.CurrentSentenceNode.GetCharacterName();

    private DialogType _currentDialogType;

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
        // Deactivate all dialog windows before activating the desired one
        foreach (var window in _dialogWindows)
        {
            window.behavior.gameObject.SetActive(false);
        }

        foreach (var window in _dialogWindows)
        {
            if (window.type.Contains(type))
            {
                CurrentDialog = window.behavior;
                _currentDialogType = type;
                window.behavior.gameObject.SetActive(true);

                // Notify listeners that a dialog has started
                OnDialogStarted.Invoke(type);

                // Subscribe to OnDialogStarted and OnDialogFinished
                window.behavior.OnDialogStarted.AddListener(() => HandleDialogStarted(type));

                window.behavior.StartDialog(graph);
                behavior.Prepare(window.behavior);
                window.behavior.BindExternalFunction("Accept", behavior.Accept);
                window.behavior.BindExternalFunction("Reject", behavior.Reject);
                window.behavior.DialogEnded += () => HandleDialogEnded(type); // Subscribe to the DialogEnded event
                return;
            }
        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    private void HandleDialogEnded(DialogType type)
    {
        OnDialogEnded.Invoke(type);
        Debug.Log("Dialog ended");
        if (CurrentDialog != null)
        {
            CurrentDialog.gameObject.SetActive(false); // Deactivate the dialog window
            CurrentDialog.DialogEnded -= () => HandleDialogEnded(type); // Unsubscribe from the event
            CurrentDialog = null; // Set CurrentDialog to null
        }
    }

    private void HandleDialogStarted(DialogType type)
    {
        OnDialogStarted.Invoke(type);
    }

    public void ShowInteractInfo(DialogNodeGraph graph)
    {
        // Deactivate all dialog windows before activating the desired one
        foreach (var window in _dialogWindows)
        {
            window.behavior.gameObject.SetActive(false);
        }
        var type = DialogType.ShowInfo;
        foreach (var window in _dialogWindows)
        {
            if (window.type.Contains(type))
            {
                CurrentDialog = window.behavior;
                _currentDialogType = type;
                window.behavior.gameObject.SetActive(true);

                if (type != DialogType.ShowInfo)
                {
                    // Notify listeners that a dialog has started
                    OnDialogStarted.Invoke(type);

                    // Subscribe to OnDialogStarted and OnDialogFinished
                    window.behavior.OnDialogStarted.AddListener(() => HandleDialogStarted(type));
                }

                window.behavior.StartDialog(graph);
                window.behavior.DialogEnded += () => HandleDialogEnded(type); // Subscribe to the DialogEnded event
                return;
            }
        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    public void HideInteractInfo()
    {
        if (CurrentDialog == null || _currentDialogType != DialogType.ShowInfo)
            return;
        var type = DialogType.ShowInfo;
        CurrentDialog.gameObject.SetActive(false);
        CurrentDialog.DialogEnded -= () => HandleDialogEnded(type);
        CurrentDialog = null;
    }

}

public enum DialogType
{
    Interact,
    Talk,
    ShowInfo
}