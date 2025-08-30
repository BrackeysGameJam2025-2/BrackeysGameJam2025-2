using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public sealed class AudioVolumeController : SingletonMonoBehaviour<AudioVolumeController>
{
    private float _masterVolume;
    private float _sfxVolume;
    private float _soundtrackVolume;
    private float _dialogVolume;

    private Bus _masterBus;
    private Bus _sfxBus;
    private Bus _soundtrackBus;
    private Bus _dialogBus;

    /// <summary>
    /// Gets/sets a volume of all audio sources (master volume) in a range from 0 to 1.
    /// </summary>
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            UpdateVolumes();
        }
    }

    /// <summary>
    /// Gets/sets sound effects volume in a range from 0 to 1.
    /// </summary>
    public float SfxVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            UpdateVolumes();
        }
    }

    /// <summary>
    /// Gets/sets soundtrack volume in a range from 0 to 1.
    /// </summary>
    public float SoundtrackVolume
    {
        get => _soundtrackVolume;
        set
        {
            _soundtrackVolume = Mathf.Clamp01(value);
            UpdateVolumes();
        }
    }

    /// <summary>
    /// Gets/sets dialog volume in a range from 0 to 1.
    /// </summary>
    public float DialogVolume
    {
        get => _dialogVolume;
        set
        {
            _dialogVolume = Mathf.Clamp01(value);
            UpdateVolumes();
        }
    }

    public Bus MasterBus => _masterBus;
    public Bus SfxBus => _sfxBus;
    public Bus SoundtrackBus => _soundtrackBus;
    public Bus DialogBus => _dialogBus;

    protected override void Awake()
    {
        base.Awake();

        _masterBus = RuntimeManager.GetBus("bus:/");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _soundtrackBus = RuntimeManager.GetBus("bus:/Soundtrack");
        _dialogBus = RuntimeManager.GetBus("bus:/Dialog");

        UpdateVolumes();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void Start()
    {
        Load();
    }

    private void UpdateVolumes()
    {
        _masterBus.setVolume(_masterVolume);
        _sfxBus.setVolume(_sfxVolume);
        _soundtrackBus.setVolume(_soundtrackVolume);
        _dialogBus.setVolume(_dialogVolume);
    }

    public void Load()
    {
        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        _soundtrackVolume = PlayerPrefs.GetFloat("SoundtrackVolume", 0.8f);
        _dialogVolume = PlayerPrefs.GetFloat("DialogVolume", 1f);

        UpdateVolumes();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        PlayerPrefs.SetFloat("SfxVolume", _sfxVolume);
        PlayerPrefs.SetFloat("SoundtrackVolume", _soundtrackVolume);
        PlayerPrefs.SetFloat("DialogVolume", _dialogVolume);
        PlayerPrefs.Save();
    }
}
