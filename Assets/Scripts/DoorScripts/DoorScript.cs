using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Transform targetTransform;
    public float speed;
    public float waitTime;
    private Vector3 initialPosition;
    private bool isMoving = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void OpenDoor()
    {
        if (!isMoving && targetTransform != null) 
        {
            SoundManager.PlaySound(SoundType.OPENDOORSFX, SoundManager.Instance.GetSFXVolume());
            StartCoroutine(OpenAndCloseDoor());
        }
    }

    private IEnumerator OpenAndCloseDoor()
    {
        yield return MoveDoor(targetTransform.position); // Mover hacia la posición del transform objetivo
        yield return new WaitForSeconds(waitTime);
        SoundManager.PlaySound(SoundType.OPENDOORSFX, SoundManager.Instance.GetSFXVolume());
        yield return MoveDoor(initialPosition); // Volver a la posición inicial
    }

    private IEnumerator MoveDoor(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target; 
        isMoving = false;
    }
}
