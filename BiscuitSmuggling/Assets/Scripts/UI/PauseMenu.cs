using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PauseMenu : SingletonMonoBehaviour<PauseMenu>
{
    private bool _isPaused = false;

    private Bus _oneTimeSfxBus;
    private Bus _continuousSfxBus;
    private Bus _dialogBus;

    [SerializeField]
    private GameObject m_Overlay;
    [SerializeField]
    private GameObject m_SettingsMenu;

    [SerializeField]
    private EventReference m_PauseEvent;
    [SerializeField]
    private EventReference m_UnpauseEvent;

    public bool IsPaused => _isPaused;

    protected override void Awake()
    {
        base.Awake();

        _oneTimeSfxBus = RuntimeManager.GetBus("bus:/SFX/OneTime");
        _continuousSfxBus = RuntimeManager.GetBus("bus:/SFX/Continuous");
        _dialogBus = RuntimeManager.GetBus("bus:/Dialog");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _oneTimeSfxBus.setPaused(false);
        _continuousSfxBus.setPaused(false);
        _dialogBus.setPaused(false);
        RuntimeManager.StudioSystem.setParameterByName("PauseBlend", 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused && m_Overlay.activeInHierarchy) Unpause();
            if (!IsPaused) Pause();
        }
    }

    public void Pause()
    {
        if (_isPaused || DialogManager.Instance.CurrentDialog != null) return;

        SetPaused();

        m_Overlay.SetActive(true);

        RuntimeManager.PlayOneShot(m_PauseEvent);
        RuntimeManager.StudioSystem.setParameterByName("PauseBlend", 1f);
    }

    public void Unpause()
    {
        if (!_isPaused) return;

        if (m_SettingsMenu.activeInHierarchy)
        {
            m_SettingsMenu.SetActive(false);
            return;
        }

        SetUnpaused();

        m_Overlay.SetActive(false);

        RuntimeManager.PlayOneShot(m_UnpauseEvent);
        RuntimeManager.StudioSystem.setParameterByName("PauseBlend", 0f);
    }

    public void SetPaused()
    {
        if (_isPaused) return;

        _isPaused = true;
        Time.timeScale = 0f;

        _oneTimeSfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _oneTimeSfxBus.setPaused(true);
        _continuousSfxBus.setPaused(true);
        _dialogBus.setPaused(true);
    }

    public void SetUnpaused()
    {
        if (!_isPaused) return;

        _isPaused = false;
        Time.timeScale = 1f;

        _oneTimeSfxBus.setPaused(false);
        _continuousSfxBus.setPaused(false);
        _dialogBus.setPaused(false);
    }

    public void OpenSettingsMenu()
    {
        m_SettingsMenu.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game (don't work in Editor, will work in build)");
        Application.Quit();
    }
}
