using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public void SpawnDrone()
    {
            Instantiate(prefab, transform.position, transform.rotation); 
    }
}
