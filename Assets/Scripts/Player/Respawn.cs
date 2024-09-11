using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float minYThreshold = -15f;
    public DavesPM dpm;
    public FrenzyManager frenzyManager;
    public Rigidbody rb;
    public KeyCode reloadButton = KeyCode.Y;
    public GameManager gameManager;

    private void Update()
    {
        if (transform.position.y < minYThreshold || Input.GetKey(reloadButton))
        {
            gameManager.ShowDefeatScreen();
        }
    }

    public void RespawnOnFall()
    {
        frenzyManager.currentFrenzy = frenzyManager.startingFrenzy;

        dpm.enabled = false;

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        rb.velocity = new Vector3(0,0,0);


        dpm.enabled = true;
    }
}
