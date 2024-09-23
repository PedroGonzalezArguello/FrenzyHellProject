using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 2f;
    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}
