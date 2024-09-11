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
    EnemiesScript EnemiesScript;
    EnemyDrone EnemyDrone;
    CoinBullet coinBullet;
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

        GameObject enemyAiTutorialInstance = GameObject.FindGameObjectWithTag("Enemy");
        if (enemyAiTutorialInstance != null)
        {
            EnemiesScript = enemyAiTutorialInstance.GetComponent<EnemiesScript>();
        }

        GameObject enemyDroneInstance = GameObject.FindGameObjectWithTag("Enemy");
        if (enemyDroneInstance != null)
        {
            EnemyDrone = enemyDroneInstance.GetComponent<EnemyDrone>();
        }

        BulletEffect = GetComponent<BulletTrailEffect>();
    }

    private void Start()
    {
        reloadTime = baseReloadTime;
        timeBetweenShooting = baseTimeBetweenShooting;

        effectToSpawn = vfx[0];
    }

    private void Update()
    {
        if(pauseActive != true)
        {

            MyInput();

            //SetText
            text.SetText(bulletsLeft + " / " + magazineSize);

            //Recarga Auto
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

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
            reloading = true;
        }
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 || Input.GetKeyDown(KeyCode.R) && bulletsLeft > 5 && !reloading && readyToShoot && shooting)
        {
            Reload();
            reloading = true;
        }

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            SpawnVFX();
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        /*
        if(Input.GetKey(KeyCode.Space))
        {
            gunAnim.SetBool("OnJump" , true);
        }
        else
        {
            gunAnim.SetBool("OnJump", false);
        }*/

    }

    private void Shoot()
    {
        readyToShoot = false;
        //Play Sound
        SoundManager.PlaySound(SoundType.GUN1, SoundManager.Instance.GetSFXVolume());

        //Play Anim
        gunAnim.SetTrigger("Recoil");

        if (!wr.wallLeft && !wr.wallRight)
        {
            RecoilCam.Play();
        }


        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);
        lr.SetPosition(0, fpsCam.transform.forward);

        //ShakeCamera
        //CameraShaker.Instance.ShakeOnce(magn, rough, fadeIn, fadeOut);

        //RayCast
        int layerMask = ~LayerMask.GetMask("projectileVFX");
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayEnemyHit, range, whatIsEnemy & layerMask))
        {
            

            EnemiesScript enemy = rayEnemyHit.collider.GetComponent<EnemiesScript>();
            if (enemy != null)
            {
                // Apply damage to the enemy
                enemy.TakeDamage(damage);
            }

            EnemyDrone enemyDrone = rayEnemyHit.collider.GetComponent<EnemyDrone>();
            if (enemyDrone != null)
            {
                // Apply damage to the enemy
                enemyDrone.TakeDamage(damage);
                SoundManager.PlaySound(SoundType.METALHIT, SoundManager.Instance.GetSFXVolume());
            }

            KamikazeDrone kamikazeDrone = rayEnemyHit.collider.GetComponent<KamikazeDrone>();
            if (kamikazeDrone != null)
            {
                // Apply damage to the enemy
                kamikazeDrone.TakeDamage(damage);
                SoundManager.PlaySound(SoundType.METALHIT, SoundManager.Instance.GetSFXVolume());
            }

            CoinBullet coin = rayEnemyHit.collider.GetComponent<CoinBullet>();
            if (coin != null)
            {
                coin.Explode();
            }

            /*
            MiniBoss bossWin = rayEnemyHit.collider.GetComponent<MiniBoss>();
            if (bossWin != null)
            {
                // Apply damage to the enemy
                bossWin.TakeDamage(damage);
                SoundManager.PlaySound(SoundType.HITSFX, SoundManager.Instance.GetSFXVolume());
            }*/

            BossHead firstBoss = rayEnemyHit.collider.GetComponent<BossHead>();
            if (firstBoss != null)
            {
                // Apply damage to the FirsBoss
                firstBoss.TakeDamage(damage);
                SoundManager.PlaySound(SoundType.HITSFX, SoundManager.Instance.GetSFXVolume());
            }

            ExplosiveTrap explosiveTrap = rayEnemyHit.collider.GetComponent<ExplosiveTrap>();
            if (explosiveTrap != null)
            {
                explosiveTrap.Explode1();
            }

            EnemyTurret turretHead = rayEnemyHit.collider.GetComponent<EnemyTurret>();
            if (turretHead != null)
            {
                
                turretHead.TurretOffTime();
            }
            

            ButtonDoor button = rayEnemyHit.collider.GetComponent<ButtonDoor>();
            if (button != null)
            {
                button.Open();
                SoundManager.PlaySound(SoundType.HITSFX, SoundManager.Instance.GetSFXVolume());
            }

            // Empujar el objeto golpeado si tiene un Rigidbody
            Rigidbody rb = rayEnemyHit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-rayEnemyHit.normal * pushForce);
            }
            

            //BulletEffect.CreateBulletTrail(muzzle, hit.point);
            SoundManager.PlaySound(SoundType.GUN1, SoundManager.Instance.GetSFXVolume());
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

        if(firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);

            vfx.transform.localRotation = firePoint.transform.rotation;

            vfx.layer = LayerMask.NameToLayer("projectileVFX");
        }
    }
}
