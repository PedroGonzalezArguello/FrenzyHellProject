using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    public Slider sfxVolumeSlider;
    public Text sfxVolumeText;

    private void Start()
    {
        // Asegúrate de que el SoundManager esté inicializado
        SoundManager soundManager = SoundManager.Instance;
        if (soundManager == null)
        {
            Debug.LogError("SoundManager instance is null in SFXSlider Start!");
            return;
        }

        // Cargar el volumen guardado
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxVolumeSlider.value = savedVolume;
        SoundManager.Instance.SetSFXVolume(savedVolume);

        // Actualizar el texto del slider
        sfxVolumeText.text = (savedVolume * 100).ToString("F0");

        // Añadir listener para el slider
        sfxVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        Debug.Log("SFXSlider initialized with volume: " + savedVolume);
    }

    public void OnVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        sfxVolumeText.text = (value * 100).ToString("F0");
        SFXDONTDESTROY.instance.GetSFXValue = value;

        Debug.Log("Volume changed to: " + value);
    }
}