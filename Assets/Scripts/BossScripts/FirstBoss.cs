using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    [SerializeField] private float BossHealth;
    [SerializeField] private bool isBossAlive;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameManager gameManager;


    private void Start()
    {
        isBossAlive = true;
        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (transform.position.y < -300)
        {
            gameManager.ShowVictoryScreen();
        }
    }

    public void TakeDamage(int damage)
    {
        BossHealth -= damage;
        string DamagePopUp = "X";
        PopUpManager._current.PopUp(transform.position, DamagePopUp, Color.red);
        SoundManager.PlaySound(SoundType.METALHIT, SoundManager.Instance.GetSFXVolume());

        if (BossHealth <= 0 && isBossAlive)
        {
            isBossAlive = false;
            rb.useGravity = true;
            
        }

    }
}
