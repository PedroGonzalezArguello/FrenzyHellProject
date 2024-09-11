using UnityEngine;

public class ImpulsePlatform : MonoBehaviour
{
    [SerializeField] private float upwardImpulseForce;
    [SerializeField] private float forwardImpulseForce;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject explosionEffect;
    DavesPM davesPM;

    private void OnCollisionEnter(Collision collision)
    {
        davesPM = collision.gameObject.GetComponent<DavesPM>();

        if(davesPM != null )
        {
            Explode();
        }
    }
    void Explode()
    {
        // Show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Apply Impulse and force to the player
        ApplyImpulseEffects();
    }

    void ApplyImpulseEffects()
    {
        SoundManager.PlaySound(SoundType.ImpulseTrap, SoundManager.Instance.GetSFXVolume());

        // Apply force
        davesPM.ApplyImpulse(upwardImpulseForce, forwardImpulseForce);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, upwardImpulseForce);
    }
}
