using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public DavesPM Player;

    public static BuffManager Instance;

    private void Start()
    {
        Instance = this;
    }
    public void Buff()
    {
        Player.extraMoveSpeed += 100f;
    }

    public void RemoveBuff()
    {
        Player.extraMoveSpeed = 0;
    }


}
