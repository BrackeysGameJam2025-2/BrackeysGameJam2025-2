using cherrydev;
using UnityEngine;

public class TestDialogInit : MonoBehaviour
{
    [SerializeField] private DialogBehaviour _behavior;
    [SerializeField] private DialogNodeGraph _graph;
    [SerializeField] private AudioSource _audioSource;

    PlayerInputActions _inputActions;
    private void Start()
    {
        _behavior.StartDialog(_graph);
        _behavior.SetVariableValue("Reputation", 10);
    }

    private void Update()
    {
        if (_behavior.IsTypingActive)
        {
            if (_audioSource != null && !_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            if (_audioSource != null && _audioSource.isPlaying)
                _audioSource.Stop();
        }
    }
}
