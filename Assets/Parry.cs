using UnityEngine;
using UnityEngine.UI;

public class Parry : MonoBehaviour
{
    private float _parryTime = 0.3f;
    [SerializeField] private float _timer;
    [SerializeField] private bool _inParry;


    private void Update()
    {
        if((Input.GetButton("ParryKey")))
        {
            _timer = _parryTime;
        }
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _inParry = false;
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
        }
    }
}
