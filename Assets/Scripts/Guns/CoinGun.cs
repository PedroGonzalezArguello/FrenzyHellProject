using EZCameraShake;
using UnityEngine;
using TMPro;
using static Unity.VisualScripting.Member;


public class CoinGun : MonoBehaviour
{
    //bullet 
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting;

    //Recoil
    public Rigidbody playerRb;

    //bools
    bool shooting, readyToShoot;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    SoundManager soundManager;

    public bool allowInvoke = true;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        shooting = Input.GetKeyDown(KeyCode.Q);

        //Shooting
        if (readyToShoot && shooting)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithoutSpread.normalized;

        //Add forces to bullet
        
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Play Sound
        SoundManager.PlaySound(SoundType.GUN1, SoundManager.Instance.GetSFXVolume());

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }
}
