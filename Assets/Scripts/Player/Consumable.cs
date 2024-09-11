using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public DavesPM dpm;

    [SerializeField]private Transform _target;

    public float moreSpeed = 100;

    void Start()
    {
        dpm = FindObjectOfType<DavesPM>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DavesPM pm = collision.gameObject.GetComponent<DavesPM>();

        if (pm != null)
        {
            pm = GetComponent<DavesPM>();

            //pm.GetSpeed(moreSpeed);
            
        }
        Destroy(gameObject);


    }

    public void GetPlayerTransform(Transform target)
    {
        _target = target;
    }



}
