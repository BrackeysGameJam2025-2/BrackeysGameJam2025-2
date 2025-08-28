using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField]
    private Slider m_MasterVolume;
    [SerializeField]
    private Slider m_SfxVolume;
    [SerializeField]
    private Slider m_DialogVolume;
    [SerializeField]
    private Slider m_SoundtrackVolume;

    private void OnEnable()
    {
        m_MasterVolume.value = AudioVolumeController.Instance.MasterVolume;
        m_SfxVolume.value = AudioVolumeController.Instance.SfxVolume;
        m_DialogVolume.value = AudioVolumeController.Instance.DialogVolume;
        m_SoundtrackVolume.value = AudioVolumeController.Instance.SoundtrackVolume;

        m_MasterVolume.onValueChanged.AddListener(OnVolumeChanged);
        m_SfxVolume.onValueChanged.AddListener(OnVolumeChanged);
        m_DialogVolume.onValueChanged.AddListener(OnVolumeChanged);
        m_SoundtrackVolume.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnDisable()
    {
        m_MasterVolume.onValueChanged.RemoveListener(OnVolumeChanged);
        m_SfxVolume.onValueChanged.RemoveListener(OnVolumeChanged);
        m_DialogVolume.onValueChanged.RemoveListener(OnVolumeChanged);
        m_SoundtrackVolume.onValueChanged.RemoveListener(OnVolumeChanged);

        AudioVolumeController.Instance.Save();
    }

    private void OnVolumeChanged(float volume)
    {
        AudioVolumeController.Instance.MasterVolume = m_MasterVolume.value;
        AudioVolumeController.Instance.SfxVolume = m_SfxVolume.value;
        AudioVolumeController.Instance.DialogVolume = m_DialogVolume.value;
        AudioVolumeController.Instance.SoundtrackVolume = m_SoundtrackVolume.value;
    }
}
