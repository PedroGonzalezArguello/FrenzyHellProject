using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public Text volumeText;

    private void Start()
    {
        // Cargar el volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        volumeSlider.value = savedVolume;
        MusicManager.Instance.SetVolume(savedVolume);

        // Actualizar el texto del slider
        volumeText.text = (savedVolume * 100).ToString("F0");

        // Añadir listener para el slider
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
    {
        MusicManager.Instance.SetVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        volumeText.text = (value * 100).ToString("F0");
    }
}
