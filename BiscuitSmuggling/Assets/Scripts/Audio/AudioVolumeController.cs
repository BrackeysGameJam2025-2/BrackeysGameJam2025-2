using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public sealed class AudioVolumeController : SingletonMonoBehaviour<AudioVolumeController>
{
    private float _masterVolume = 1f;
    private float _sfxVolume = 1f;
    private float _soundtrackVolume = 0.8f;
    private float _dialogVolume = 0.8f;

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
            UpdateMixer();
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
            UpdateMixer();
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
            UpdateMixer();
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
            UpdateMixer();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _masterBus = RuntimeManager.GetBus("bus:/");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _soundtrackBus = RuntimeManager.GetBus("bus:/Soundtrack");
        _dialogBus = RuntimeManager.GetBus("bus:/Dialog");

        UpdateMixer();
    }

    private void Start()
    {
        UpdateMixer();
    }

    private void UpdateMixer()
    {
        _masterBus.setVolume(_masterVolume);
        _sfxBus.setVolume(_sfxVolume);
        _soundtrackBus.setVolume(_soundtrackVolume);
        _dialogBus.setVolume(_dialogVolume);
    }
}
