using UnityEngine;
using UnityEngine.UI;

public class Parry : MonoBehaviour
{
    private float _parryTime = 0.1f;
    [SerializeField] private float _timer;
    [SerializeField] private bool _inParry;
    [SerializeField] private GameObject _parryCube;

    private void Awake()
    {
        _parryCube.SetActive(false);
    }
    private void Update()
    {
        if((Input.GetButton("ParryKey")))
        {
            _timer = _parryTime;
            _parryCube.SetActive(true);
        }
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _inParry = false;
            _parryCube.SetActive(false);
        }
        else
        {
            _inParry = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        IParryable parryable = other.GetComponent<IParryable>();

        if (parryable != null && _inParry)
        {
            StartCoroutine(parryable.Parry());
            _parryCube.SetActive(true);
        }
    }
}
