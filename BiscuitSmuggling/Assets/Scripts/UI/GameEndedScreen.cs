using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndedScreen : MonoBehaviour
{
    private Bus _bus;

    [SerializeField]
    private EventReference m_Music;
    [SerializeField]
    private EventReference m_Stinger;

    private void Awake()
    {
        _bus = RuntimeManager.GetBus("bus:/");
    }

    private void OnEnable()
    {
        PauseMenu.Instance.SetPaused();
        _bus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        RuntimeManager.PlayOneShot(m_Stinger);
        RuntimeManager.PlayOneShot(m_Music);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        _bus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
