using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [SerializeField] private int _totalPoints;
    [SerializeField] private TextMeshProUGUI _pointsText;
    [SerializeField] private Slider _slider;
    [SerializeField] private int _maxMultiplier = 3;
    private int _multiplier = 1;
    private float _maxTimer = 3f;
    private float _actualTimer = 0f;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Update()
    {
        _actualTimer -= Time.deltaTime;
        _slider.value = _actualTimer / _maxTimer;
        _pointsText.text = "Points: " + _totalPoints.ToString();
    }

    public void AddPoints(int points)
    {
        if (_actualTimer > 0 && _multiplier < _maxMultiplier)
        {
            _multiplier++;
        }
        _actualTimer = _maxTimer;
        _totalPoints += points * _multiplier;
    }
}
