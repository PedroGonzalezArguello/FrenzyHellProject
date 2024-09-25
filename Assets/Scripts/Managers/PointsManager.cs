using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [SerializeField] private int _totalPoints;
    private int _multiplyer = 1;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void AddPoints(int points)
    {
        _totalPoints += points * _multiplyer;
    }
}
