using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;
    private AudioSource audioSource;

    public static MusicManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }


        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();

        // Cargar el volumen guardado al iniciar
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SetVolume(savedVolume);
    }

    public void PlayMusic()
    {
        GetComponent<AudioSource>().Play();
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void RestartMusic()
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
