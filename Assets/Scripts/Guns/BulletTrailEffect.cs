using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailEffect : MonoBehaviour
{
    public GameObject bulletTrail;

    public void CreateBulletTrail(Transform spawnPoint, Vector3 hitPoint)
    {
        GameObject clone = Instantiate(bulletTrail, spawnPoint.position, bulletTrail.transform.rotation);
        //moveTrail trail = clone.GetComponent<moveTrail>();
        //trail.hitpoint = hitPoint;
    }
}
