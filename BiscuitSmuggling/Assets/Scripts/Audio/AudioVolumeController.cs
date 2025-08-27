using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeController : SingletonMonoBehaviour<AudioVolumeController>
{
    private float _masterVolume = 1f;
    private float _sfxVolume = 1f;
    private float _soundtrackVolume = 0.8f;

    [SerializeField]
    private AudioMixer m_Mixer;

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
    /// Gets/sets soundtrack volume  in a range from 0 to 1.
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

    private void Start()
    {
        if (!m_Mixer)
        {
            Debug.LogError("Mixer is uninitialized");
        }
        UpdateMixer();
    }

    private static float GetDbVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float db = 20f * Mathf.Log10(volume);
        if (float.IsInfinity(db)) return -80f;
        return db;
    }

    private void UpdateMixer()
    {
        if (!m_Mixer) return;

        m_Mixer.SetFloat("Master_Volume", GetDbVolume(_masterVolume));
        m_Mixer.SetFloat("SFX_Volume", GetDbVolume(_sfxVolume));
        m_Mixer.SetFloat("Soundtrack_Volume", GetDbVolume(_soundtrackVolume));
    }
}
