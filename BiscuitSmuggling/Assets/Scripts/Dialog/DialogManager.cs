using cherrydev;
using FMODUnity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public sealed class DialogManager : SingletonMonoBehaviour<DialogManager>
{
    public DialogBehaviour CurrentDialog { get; private set; }

    public string CurrentDialogName => CurrentDialog.CurrentSentenceNode.GetCharacterName();

    private DialogType _currentDialogType;

    [Serializable]
    private class DialogWinow
    {
        public DialogType[] type;
        public DialogBehaviour behavior;
    }

    [SerializeField] private CharacterToVoiceMap _characterToVoiceMap;
    [SerializeField] private DialogWinow[] _dialogWindows;

    // New events to notify when a dialog starts or ends
    public UnityEvent<DialogType> OnDialogStarted = new();
    public UnityEvent<DialogType> OnDialogEnded = new();

    protected override void Awake()
    {
        base.Awake();
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

                window.behavior.SentenceStarted += OnSentenceStarted;

                // Subscribe to OnDialogStarted and OnDialogFinished
                window.behavior.OnDialogStarted.AddListener(HandleDialogStarted);

                window.behavior.StartDialog(graph);
                behavior.Prepare(window.behavior);
                window.behavior.BindExternalFunction("Accept", behavior.Accept);
                window.behavior.BindExternalFunction("Reject", behavior.Reject);
                window.behavior.DialogEnded += HandleDialogEnded; // Subscribe to the DialogEnded event
                return;
            }
        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    private void HandleDialogEnded()
    {
        OnDialogEnded.Invoke(_currentDialogType);
        Debug.Log("Dialog ended");
        if (CurrentDialog != null)
        {
            CurrentDialog.gameObject.SetActive(false); // Deactivate the dialog window
            CurrentDialog.DialogEnded -= HandleDialogEnded; // Unsubscribe from the event
            CurrentDialog.SentenceStarted -= OnSentenceStarted;
            CurrentDialog = null; // Set CurrentDialog to null
        }
    }

    private void OnSentenceStarted()
    {
        if (!AudioVolumeController.Exists)
        {
            Debug.LogError("Dialog voices do not work. Add \"AudioController\" prefab to the scene.");
            return;
        }

        string name = CurrentDialog.CurrentSentenceNode.GetCharacterName().Trim().ToLower();
        var audioEvent = _characterToVoiceMap.NameToEvent(name);

        AudioVolumeController.Instance.DialogBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.PlayOneShot(audioEvent);
    }

    private void HandleDialogStarted()
    {
        OnDialogStarted.Invoke(_currentDialogType);
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
                    window.behavior.OnDialogStarted.AddListener(HandleDialogStarted);
                }

                window.behavior.StartDialog(graph);
                window.behavior.DialogEnded += HandleDialogEnded; // Subscribe to the DialogEnded event
                return;
            }
        }
        Debug.LogError($"No dialog window found for type: {type}");
    }

    public void HideInteractInfo()
    {
        if (CurrentDialog == null || _currentDialogType != DialogType.ShowInfo)
            return;
        CurrentDialog.gameObject.SetActive(false);
        CurrentDialog.DialogEnded -= HandleDialogEnded;
        CurrentDialog = null;
    }

}

public enum DialogType
{
    Interact,
    Talk,
    ShowInfo
}