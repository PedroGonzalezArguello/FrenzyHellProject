using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float recoilDistance = 0.5f;
    [SerializeField] private float recoilReturnSpeed = 1f;
    [SerializeField] private float patrolTime = 3f; // Tiempo que rota en una dirección
    [SerializeField] private Animation turretOffAnim;
    [SerializeField] private ParticleSystem particleOff;
    [SerializeField] private ParticleSystem particleOff1;

    public GameObject bulletPrefab;
    public Transform shootPoint;
    SoundManager soundManager;

    private float timeSinceLastShot;
    private Vector3 originalPosition;
    private bool isRecoiling = false;

    // Variables para patrullaje
    private float patrolTimer;
    private bool rotateClockwise = true;
    private bool isDetectingPlayer = false;

    void Start()
    {
        originalPosition = transform.position;
        patrolTimer = patrolTime;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        DetectAndShoot();

        if (!isDetectingPlayer)
        {
            Patrol();
        }

        if (isRecoiling)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * recoilReturnSpeed);
            if (Vector3.Distance(transform.position, originalPosition) < 0.01f)
            {
                transform.position = originalPosition;
                isRecoiling = false;
            }
        }
    }

    void DetectAndShoot()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        isDetectingPlayer = players.Length > 0;

        if (isDetectingPlayer)
        {
            Transform target = players[0].transform;
            RotateTowardsTarget(target);

            if (timeSinceLastShot >= timeBetweenShots)
            {
                Shoot(target);
                timeSinceLastShot = 0f;
            }
        }
    }

    void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot(Transform target)
    {
        SoundManager.PlaySound(SoundType.TurretAttack, SoundManager.Instance.GetSFXVolume());
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (target.position - shootPoint.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

        // Apply recoil effect
        Vector3 recoilPosition = transform.position - transform.forward * recoilDistance;
        StartCoroutine(Recoil(recoilPosition));
    }

    System.Collections.IEnumerator Recoil(Vector3 recoilPosition)
    {
        isRecoiling = true;
        transform.position = recoilPosition;
        yield return null; // Wait for the next frame before starting to return to the original position
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            rotateClockwise = !rotateClockwise;
            patrolTimer = patrolTime;
        }

        float rotationDirection = rotateClockwise ? 1f : -1f;
        transform.Rotate(Vector3.up, rotationSpeed * rotationDirection * Time.deltaTime);
    }
    
    public void TurretOffTime()
    {
        turretOffAnim.Play();
        particleOff.Play();
        particleOff1.Play();
        SoundManager.PlaySound(SoundType.DRONEDEATH, SoundManager.Instance.GetSFXVolume());
        StartCoroutine(TurretOffCoroutine());
    }
    
    private System.Collections.IEnumerator TurretOffCoroutine()
    {
        Time.timeScale = 0f;
        SoundManager.PlaySound(SoundType.FREEZETIME, SoundManager.Instance.GetSFXVolume());
        yield return new WaitForSecondsRealtime(0.2f);
        

        Time.timeScale = 1f;

        this.enabled = false;

        yield return new WaitForSeconds(5f);
        turretOffAnim.Stop();

        this.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
