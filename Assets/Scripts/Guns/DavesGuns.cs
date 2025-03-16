using UnityEngine;
using TMPro;
using EZCameraShake;
using Unity.VisualScripting;
using static Unity.VisualScripting.Member;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening.Core.Easing;

public class GunSystem : MonoBehaviour
{
    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public float baseTimeBetweenShooting, baseReloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft, bulletsShot;

    [Header("Cam Effects")]
    public float magn;
    public float rough;
    public float fadeIn;
    public float fadeOut;

    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform muzzle;
    public RaycastHit rayEnemyHit;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public LayerMask shootable;
    public Animator gunAnim;
    public BulletTrailEffect BulletEffect;
    public LineRenderer lr;
    public ParticleSystem shootParticle;
    public WallRunning wr;
    SoundManager soundManager;
    public GameManager gameManager;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic, firePoint;
    public TextMeshProUGUI text;
    public List<GameObject> vfx = new List<GameObject>();
    private GameObject effectToSpawn;

    public Animation RecoilCam;

    // Fuerza de empuje
    public float pushForce = 500f;

    public bool pauseActive = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        reloadTime = baseReloadTime;
        timeBetweenShooting = baseTimeBetweenShooting;

        effectToSpawn = vfx[0];
    }

    private void Update()
    {
        if (pauseActive != true)
        {
            MyInput();

            //SetText
            text.SetText(bulletsLeft + " / " + magazineSize);

            //Recarga automática
            if (bulletsLeft == 0 && !reloading)
            {
                Reload();
                reloading = true;
            }
        }
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Recargar
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
            reloading = true;
        }
        // Recargar automáticamente cuando intentas disparar sin balas
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 || Input.GetKeyDown(KeyCode.R) && bulletsLeft > 5 && !reloading && readyToShoot && shooting)
        {
            Reload();
            reloading = true;
        }

        // Disparar
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            SpawnVFX();
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Reproducir sonido
        SoundManager.PlaySound(SoundType.GUN1, SoundManager.Instance.GetSFXVolume());

        // Reproducir animación
        gunAnim.SetTrigger("Recoil");

        if (!wr.wallLeft && !wr.wallRight)
        {
            RecoilCam.Play();
        }

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calcular dirección con Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);
        lr.SetPosition(0, fpsCam.transform.forward);


        float sphereRadius = 4;
        int layerMask = ~LayerMask.GetMask("projectileVFX");

        if (Physics.SphereCast(fpsCam.transform.position, sphereRadius, direction, out rayEnemyHit, range, whatIsEnemy & layerMask))
        {
            IDamageable damageable = rayEnemyHit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            EnemyTurret turret = rayEnemyHit.collider.GetComponent<EnemyTurret>();
            if (turret != null)
            {
                turret.TurretOffTime();
                SoundManager.PlaySound(SoundType.DRONEDEATH, SoundManager.Instance.GetSFXVolume());
            }

            Rigidbody rb = rayEnemyHit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-rayEnemyHit.normal * pushForce);
            }
        }

        Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, shootable);
        shootParticle.Play();

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        lr.enabled = false;
    }

    private void Reload()
    {
        reloading = true;
        gunAnim.SetTrigger("Reload");
        SoundManager.PlaySound(SoundType.GUNFLIP, SoundManager.Instance.GetSFXVolume());
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        gunAnim.SetTrigger("backtoiddle");
        bulletsLeft = magazineSize;
        reloading = false;
    }

    void SpawnVFX()
    {
        GameObject vfx;

        if (firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
            vfx.transform.localRotation = firePoint.transform.rotation;
            vfx.layer = LayerMask.NameToLayer("projectileVFX");
        }
    }
}
