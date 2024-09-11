using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public DavesPM pm;
    public WallRunning wr;

    public GameObject _DashFX;
    public GameObject _SlideFX;
    public GameObject _LWallFX;
    public GameObject _RWallFX;
    public Transform FXPosition;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            pm = playerObject.GetComponent<DavesPM>();
            wr = playerObject.GetComponentInParent<WallRunning>();

        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player' que contenga el componente DavesPM.");
        }
    }

    void Update()
    {
        if (pm != null && pm.dashing) DashFX();
        if (pm != null && pm.sliding) SlideFX();
        if (wr != null && wr.wallLeft) LWallFX();
        if (wr != null && wr.wallRight) RWallFX();
    }

    public void DashFX ()
    {
        if (_DashFX!= null)
        {
            GameObject DashFXInst = Instantiate(_DashFX, FXPosition.position, Quaternion.identity);
            //Destroy(DashFXInst, 1f);
        }
    }

    public void SlideFX ()
    {
        if (_SlideFX != null)
        {
            GameObject SlideFXInst = Instantiate(_SlideFX, FXPosition.position, Quaternion.identity);
            //Destroy(SlideFXInst, 1f);
        }
    }

    public void LWallFX ()
    {
        if (_LWallFX != null)
        {
            GameObject LWallFXInst = Instantiate(_LWallFX, FXPosition.position, Quaternion.identity);
            //Destroy(LWallFXInst, 1f);
        }
    }

    public void RWallFX ()
    {
        if (_RWallFX != null)
        {
            GameObject RWallFXInst = Instantiate(_RWallFX, FXPosition.position, Quaternion.identity);
            //Destroy(RWallFXInst, 1f);
        }
    }
}
