using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour, IDamageable
{
    private DoorScript door;
    private PalancaScript palanca;

    private void Awake()
    {
        door = GetComponentInChildren<DoorScript>();
        palanca = GetComponentInChildren<PalancaScript>();
    }

    public void TakeDamage(int doors)
    {
        door.OpenDoor();
        palanca.DoAnim();
    }


}
