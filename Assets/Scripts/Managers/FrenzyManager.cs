using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FrenzyManager : MonoBehaviour
{
    [Header("Referencias")]
    public Image frenzyMeterIMG;
    public DavesPM dpm;
    public Respawn respawn;
    public GunSystem guns;
    public CustomBullet custombullet;
    public CameraShake cameraShake;
    public Animator HpAnimator;
    public GameManager gameManager;

    private static FrenzyManager instance;

    [Header("Valores")]
    public float currentFrenzy;
    public float startingFrenzy;
    public float frenzy1Points;
    public float frenzy1Increase;
    public float frenzy2Points;
    public float frenzy2Increase;
    public float frenzy3Points;
    public float frenzy3Increase;
    public float frenzy4Points;
    public float frenzy4Increase;
    public float frenzy5Points;
    public float frenzy5Increase;
    public float safeGuardTime;
    public Color frenzyBarOG;
    public bool frenzy2Applied;
    public bool frenzy1Applied;
    public bool frenzy3Applied;
    public bool frenzy4Applied;
    public bool safeGuard;
    public bool frenzy5Applied;

    public static FrenzyManager Instance {  get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        frenzyBarOG = frenzyMeterIMG.color;
        gameManager = FindObjectOfType<GameManager>();
        currentFrenzy = startingFrenzy;
        frenzyMeterIMG.fillAmount = currentFrenzy / 100f;
        safeGuard = true;
        Invoke("ChangeSafeGuard", safeGuardTime);
        
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //FrenzyEffects();

        if (currentFrenzy >= 0 && !safeGuard)
            currentFrenzy -= Time.fixedDeltaTime * 0.1f;
        
        frenzyMeterIMG.fillAmount = currentFrenzy / 100f;

        if (currentFrenzy <= 0)
        {
            gameManager.ShowDefeatScreen();
            //SceneManager.LoadScene("DefeatScene");
            //respawn.RespawnOnFall();    
        }
            
            
    }

    private void ChangeSafeGuard()
    {
        safeGuard = false;
    }

    public void TakeDamage(float damage)
    {
        frenzyMeterIMG.color = Color.red;
        //Animacion de recibir daño
        HpAnimator.SetTrigger("OnTakeDamage");
        cameraShake.StartAttackShake(1.3f);
        SoundManager.PlaySound(SoundType.HURTAHSFX, SoundManager.Instance.GetSFXVolume());
        currentFrenzy -= damage;
        
        frenzyMeterIMG.fillAmount = currentFrenzy / 100f;
        Invoke("BlueColor", 0.2f);

    }    
    private void BlueColor()
    {
        frenzyMeterIMG.color = frenzyBarOG;
    }    

    public void AddPoints(float points)
    {
        currentFrenzy += points;
        currentFrenzy = Mathf.Clamp(currentFrenzy, 0, 100);

        frenzyMeterIMG.fillAmount = currentFrenzy / 100f;

        //FrenzyEffects();
    }
/*
    public void FrenzyEffects()
    {
        // Nivel 0 (Sin efectos adicionales)
        if (currentFrenzy < frenzy1Points)
        {
            if (frenzy1Applied)
            {
                ResetSpeeds();
                frenzy1Applied = false;
                frenzy2Applied = false;
                frenzy3Applied = false;
                frenzy4Applied = false;
                frenzy5Applied = false;
            }
        }

        // Nivel 1
        if (currentFrenzy >= frenzy1Points && currentFrenzy < frenzy2Points)
        {
            if (!frenzy1Applied)
            {
                ResetSpeeds(); // Restablecer a las velocidades base
                IncreaseSpeeds(frenzy1Increase); // Aplicar aumento de velocidad nivel 1
                frenzy1Applied = true;
                frenzy2Applied = false;
                frenzy3Applied = false;
                frenzy4Applied = false;
                frenzy5Applied = false;

                //frenzyXSpeedApplied = false; // Asegurarse de que el nivel X no esté aplicado
            }
        }

        // Nivel 2
        if (currentFrenzy >= frenzy2Points)
        {
            if (!frenzy2Applied)
            {
                ResetSpeeds(); // Restablecer a las velocidades base
                IncreaseSpeeds(frenzy2Increase); // Aplicar aumento de velocidad nivel 2
                frenzy1Applied = false;
                frenzy2Applied = true;
                frenzy3Applied = false;
                frenzy4Applied = false;
                frenzy5Applied = false;
            }
        }

        // Nivel 3
        if (currentFrenzy >= frenzy3Points)
        {
            if (!frenzy3Applied)
            {
                ResetSpeeds(); // Restablecer a las velocidades base
                IncreaseSpeeds(frenzy3Increase); // Aplicar aumento de velocidad nivel 2
                frenzy1Applied = false;
                frenzy2Applied = false;
                frenzy3Applied = true;
                frenzy4Applied = false;
                frenzy5Applied = false;
            }
        }

        // Nivel 4
        if (currentFrenzy >= frenzy4Points)
        {
            if (!frenzy4Applied)
            {
                ResetSpeeds(); // Restablecer a las velocidades base
                IncreaseSpeeds(frenzy4Increase); // Aplicar aumento de velocidad nivel 2
                frenzy1Applied = false;
                frenzy2Applied = false;
                frenzy3Applied = false;
                frenzy4Applied = true;
                frenzy5Applied = false;
            }
        }

        // Nivel 5
        if (currentFrenzy >= frenzy5Points)
        {
            if (!frenzy5Applied)
            {
                ResetSpeeds(); // Restablecer a las velocidades base
                IncreaseSpeeds(frenzy5Increase); // Aplicar aumento de velocidad nivel 2
                frenzy1Applied = false;
                frenzy2Applied = false;
                frenzy3Applied = false;
                frenzy4Applied = false;
                frenzy5Applied = true;
            }
        }

    }

    public void ResetSpeeds()
    {
        dpm.walkSpeed = dpm.baseWalkSpeed;
        dpm.wallRunSpeed = dpm.baseWallRunSpeed;
        dpm.slideSpeed = dpm.baseSlideSpeed;
        guns.timeBetweenShooting = guns.baseTimeBetweenShooting;
        guns.reloadTime = guns.baseReloadTime;
    }

    public void IncreaseSpeeds(float speedincrese)
    {
        dpm.walkSpeed *= speedincrese;
        dpm.wallRunSpeed *= speedincrese;
        dpm.slideSpeed *= speedincrese;
        guns.timeBetweenShooting /= speedincrese;
        guns.reloadTime /= speedincrese;
    }
*/
}
