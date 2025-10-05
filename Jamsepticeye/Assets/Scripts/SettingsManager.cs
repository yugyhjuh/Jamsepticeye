using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;      // Assign your AudioMixer
    public Slider volumeSlider;        // Assign from UI

    [Header("Display Settings")]
    public Toggle fullscreenToggle;    // Assign from UI

    private const string MASTER_VOL = "MasterVol";

    void Start()
    {
        // Load saved settings or defaults
        float savedVol = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        volumeSlider.value = savedVol;
        fullscreenToggle.isOn = savedFullscreen;

        ApplyVolume(savedVol);
        ApplyFullscreen(savedFullscreen);

        // Hook events
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
        fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);
    }

    private void ApplyVolume(float value)
    {
        // Convert linear 0–1 to dB scale (-80 to 0)
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(MASTER_VOL, dB);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void ApplyFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }
}
