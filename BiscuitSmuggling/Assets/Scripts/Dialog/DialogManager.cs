using System;
using cherrydev;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private static DialogManager _instance;

    public static DialogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("DialogManager");
                    _instance = singletonObject.AddComponent<DialogManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

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

    public void ShowDialog(DialogType type, DialogNodeGraph graph)
    {
        foreach (var window in _dialogWindows)
        {
            if (window.type == type)
            {
                window.behavior.StartDialog(graph);
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