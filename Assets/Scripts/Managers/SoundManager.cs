using UnityEngine;
using System;
using System.Collections.Generic;

public enum SoundType
{
    WALLRUN,
    DASH,
    SLIDE,
    GRAPPLE,
    LAND,
    HURT,
    JUMP,
    GUN1,
    GUN2,
    GUNFLIP,
    WALK,
    MUSIC,
    DRONEALIVE,
    ONDRONEDETECT,
    DRONEDEATH,
    STARTWALLRUN,
    DRONECOLLISION,
    DRONECHARGE,
    DRONEATTACK,
    SPIKEDEATH,
    RELOAD,
    HITSFX,
    PRUEBA,
    METALHIT,
    TrapExplode,
    TurretAttack,
    ImpulseTrap,
    DRONEXPLODE,
    PRENSAULTRA,
    HURTAHSFX,
    OPENDOORSFX,
    GRAPPLEON,
    FREEZETIME,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;
    private Dictionary<SoundType, AudioSource> loopingSources; // Para manejar los sonidos en bucle

    private float sfxVolume = 1f;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("SoundManager");
                    instance = obj.AddComponent<SoundManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        loopingSources = new Dictionary<SoundType, AudioSource>();
        

        // Cargar el volumen guardado al iniciar
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if(SFXDONTDESTROY.instance != null)
        {
            sfxVolume = SFXDONTDESTROY.instance.GetSFXValue;
        }
        

    }

    public static void PlaySound(SoundType sound, float volume = -1f)
    {
        volume = (volume == -1f) ? instance.sfxVolume : volume;
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static void PlayLoopingSound(SoundType sound, float volume = -1f)
    {
        volume = (volume == -1f) ? instance.sfxVolume : volume;

        if (instance.loopingSources.ContainsKey(sound))
        {
            // Ya se está reproduciendo este sonido
            return;
        }

        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        AudioSource newSource = instance.gameObject.AddComponent<AudioSource>();
        newSource.clip = randomClip;
        newSource.volume = volume;
        newSource.loop = true;
        newSource.Play();

        instance.loopingSources[sound] = newSource;
    }

    public static void StopLoopingSound(SoundType sound)
    {
        if (instance.loopingSources.ContainsKey(sound))
        {
            AudioSource source = instance.loopingSources[sound];
            source.Stop();
            Destroy(source);
            instance.loopingSources.Remove(sound);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Actualizar volumen de los sonidos en bucle
        foreach (var source in loopingSources.Values)
        {
            source.volume = volume;
        }
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
            soundList[i].name = names[i];
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}