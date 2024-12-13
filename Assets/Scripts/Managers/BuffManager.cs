using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public DavesPM Player;

    public static BuffManager Instance;

    private float[] tierSpeeds = { 50f, 100f, 200f, 300f }; // C, B, A, Super Frenzy

    [Header("VELOCIDAD EXTRA")]
    public float currentSpeedBuff; // Visible en el Inspector para depuración

    private void Start()
    {
        Instance = this;
    }
    public void Buff(int tierIndex)
    {
        Player.extraMoveSpeed += 100f;

        if (tierIndex >= 0 && tierIndex < tierSpeeds.Length)
        {
            Player.extraMoveSpeed = tierSpeeds[tierIndex];
            Debug.Log($"Buff aplicado: Tier {tierIndex}, Velocidad extra: {tierSpeeds[tierIndex]}");
            currentSpeedBuff = tierSpeeds[tierIndex]; // Actualizar la variable de depuración
        }
    }

    public void RemoveBuff()
    {
        Player.extraMoveSpeed = 0;
        currentSpeedBuff = 0;
    }
}
