using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tierText;
    [SerializeField] private TierStruct[] _tiers;
    [SerializeField] private int _actualTier = 0;

    private void Start()
    {
        PointsManager.OnPointsCooldown += ResetTier;
    }

    private void Update()
    {
        if (PointsManager.Instance != null && PointsManager.Instance.PointsInCooldown > 0)
        {
            _tierText.gameObject.SetActive(true);
            _tierText.text = "Tier: " + _tiers[_actualTier].name + " - ("+ PointsManager.Instance.PointsInCooldown.ToString() + " Points)";
            CheckActualTier();
        }
    }

    public void ResetTier()
    {
        _actualTier = 0;
        if (_tierText != null)
        {
            _tierText.gameObject.SetActive(false);
            BuffManager.Instance.RemoveBuff();
        }
    }

    public void CheckActualTier()
    {
        if (PointsManager.Instance.PointsInCooldown > _tiers[_actualTier].points && _actualTier < _tiers.Length - 1)
        {
            _actualTier++;
            BuffManager.Instance.Buff(_actualTier); // Pasar el índice actual al BuffManager
            Debug.Log($"Nuevo Tier: {_tiers[_actualTier].name} - Velocidad ajustada.");
        }
    }
}
