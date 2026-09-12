using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider ambienceVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;

    [Header("Volume Labels")]
    [SerializeField] private TextMeshProUGUI masterVolumeLabel;
    [SerializeField] private TextMeshProUGUI voiceVolumeLabel;
    [SerializeField] private TextMeshProUGUI ambienceVolumeLabel;
    [SerializeField] private TextMeshProUGUI effectsVolumeLabel;

    private const string MasterVolumeKey = "master_volume";
    private const string VoiceVolumeKey = "voice_volume";
    private const string AmbienceVolumeKey = "ambience_volume";
    private const string EffectsVolumeKey = "effects_volume";

    private const float MinimumVolume = 0.0001f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadSettings();
    }

    private void Start()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);
        }

        if (ambienceVolumeSlider != null)
        {
            ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);
        }

        if (effectsVolumeSlider != null)
        {
            effectsVolumeSlider.onValueChanged.AddListener(SetEffectsVolume);
        }

        UpdateMixerVolumes();
        UpdateVolumeLabels();
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();

        UpdateVolumeLabel(masterVolumeLabel, value);
    }

    public void SetVoiceVolume(float value)
    {
        SetMixerVolume("VoiceVolume", value);
        PlayerPrefs.SetFloat(VoiceVolumeKey, value);
        PlayerPrefs.Save();

        UpdateVolumeLabel(voiceVolumeLabel, value);
    }

    public void SetAmbienceVolume(float value)
    {
        SetMixerVolume("AmbienceVolume", value);
        PlayerPrefs.SetFloat(AmbienceVolumeKey, value);
        PlayerPrefs.Save();

        UpdateVolumeLabel(ambienceVolumeLabel, value);
    }

    public void SetEffectsVolume(float value)
    {
        SetMixerVolume("EffectsVolume", value);
        PlayerPrefs.SetFloat(EffectsVolumeKey, value);
        PlayerPrefs.Save();

        UpdateVolumeLabel(effectsVolumeLabel, value);
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
        {
            return;
        }

        float volume = Mathf.Clamp(value, MinimumVolume, 1f);
        float decibels = Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat(parameterName, decibels);
    }

    private void LoadSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float voiceVolume = PlayerPrefs.GetFloat(VoiceVolumeKey, 1f);
        float ambienceVolume = PlayerPrefs.GetFloat(AmbienceVolumeKey, 1f);
        float effectsVolume = PlayerPrefs.GetFloat(EffectsVolumeKey, 1f);

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
        }

        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.value = voiceVolume;
        }

        if (ambienceVolumeSlider != null)
        {
            ambienceVolumeSlider.value = ambienceVolume;
        }

        if (effectsVolumeSlider != null)
        {
            effectsVolumeSlider.value = effectsVolume;
        }
    }

    private void UpdateMixerVolumes()
    {
        if (masterVolumeSlider != null)
        {
            SetMixerVolume("MasterVolume", masterVolumeSlider.value);
        }

        if (voiceVolumeSlider != null)
        {
            SetMixerVolume("VoiceVolume", voiceVolumeSlider.value);
        }

        if (ambienceVolumeSlider != null)
        {
            SetMixerVolume("AmbienceVolume", ambienceVolumeSlider.value);
        }

        if (effectsVolumeSlider != null)
        {
            SetMixerVolume("EffectsVolume", effectsVolumeSlider.value);
        }
    }

    private void UpdateVolumeLabels()
    {
        if (masterVolumeSlider != null)
        {
            UpdateVolumeLabel(masterVolumeLabel, masterVolumeSlider.value);
        }

        if (voiceVolumeSlider != null)
        {
            UpdateVolumeLabel(voiceVolumeLabel, voiceVolumeSlider.value);
        }

        if (ambienceVolumeSlider != null)
        {
            UpdateVolumeLabel(ambienceVolumeLabel, ambienceVolumeSlider.value);
        }

        if (effectsVolumeSlider != null)
        {
            UpdateVolumeLabel(effectsVolumeLabel, effectsVolumeSlider.value);
        }
    }

    private void UpdateVolumeLabel(TextMeshProUGUI label, float value)
    {
        if (label == null)
        {
            return;
        }

        int percentage = Mathf.RoundToInt(value * 100f);
        label.text = $"{percentage}%";
    }
}