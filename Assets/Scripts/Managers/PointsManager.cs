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
    [SerializeField] private int _pointsInCooldown;
    private int _multiplier = 1;
    private float _maxTimer = 3f;
    private float _actualTimer = 0f;
    public float timeModifier;

    public delegate void EventPointsCooldown();
    public static event EventPointsCooldown OnPointsCooldown;

    public int PointsInCooldown
    {
        get { return _pointsInCooldown; }
    }
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        OnPointsCooldown += ResetActualPoints;
    }

    private void Update()
    {
        _actualTimer -= Time.deltaTime / timeModifier;
        if (_actualTimer <= 0f)
        {
            OnPointsCooldown?.Invoke();
        }

        if (_slider != null)
        {
            _slider.value = _actualTimer / _maxTimer;
        }

        if (_pointsText != null)
        {
            _pointsText.text = "Total Points: " + _totalPoints.ToString();
        }
    }

    public void AddPoints(int points)
    {
        if (_actualTimer > 0 && _multiplier < _maxMultiplier)
        {
            _multiplier++;
        }
        _actualTimer = _maxTimer;
        _pointsInCooldown += points * _multiplier;

        // Notificar al TierManager para revisar el Tier
        TierManager tierManager = FindObjectOfType<TierManager>();
        if (tierManager != null)
        {
            tierManager.CheckActualTier();
        }
    }

    private void ResetActualPoints()
    {
        _totalPoints += _pointsInCooldown;
        _pointsInCooldown = 0;
        _multiplier = 1;
    }
}
