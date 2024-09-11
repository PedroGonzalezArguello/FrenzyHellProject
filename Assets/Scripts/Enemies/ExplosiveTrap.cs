using UnityEngine;

public class ExplosiveTrap : MonoBehaviour
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionDamage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private FrenzyManager frenzyManager;
    CameraShake cameraShake; 
    SoundManager soundManager;

     void Start()
     {
        soundManager = GetComponent<SoundManager>();

        frenzyManager = FrenzyManager.Instance;
        
     }

    void OnCollisionEnter(Collision collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Explode(collision.gameObject);
        }
    }


    public void Explode(GameObject player)
    {
        // Show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            SoundManager.PlaySound(SoundType.TrapExplode, SoundManager.Instance.GetSFXVolume());
        }
        

        // Apply damage and force to the player
        ApplyExplosionEffects(player);

        // Destroy the explosive object
        Destroy(gameObject);
    }

    public void Explode1()
    {
        // Show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            SoundManager.PlaySound(SoundType.TrapExplode, SoundManager.Instance.GetSFXVolume());
        }

        // Destroy the explosive object
        Destroy(gameObject);
    }

    void ApplyExplosionEffects(GameObject player)
    {
        // Apply damage
        if(frenzyManager != null) 
        {
            frenzyManager.TakeDamage(explosionDamage);
        }

        // Apply force
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            Vector3 explosionDirection = player.transform.position - transform.position;
            playerRigidbody.AddForce(explosionDirection.normalized * explosionForce, ForceMode.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
