using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public float moveDistance;
    public float speed;
    public float waitTime;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(moveDistance, 0, 0);
    }

    public void OpenDoor()
    {
        if (!isMoving)
        {
            SoundManager.PlaySound(SoundType.OPENDOORSFX, SoundManager.Instance.GetSFXVolume());
            StartCoroutine(OpenAndCloseDoor());
        }
    }

    private IEnumerator OpenAndCloseDoor()
    {
        yield return MoveDoor(targetPosition); // Move to open position
        yield return new WaitForSeconds(waitTime); // Wait for the specified time
        SoundManager.PlaySound(SoundType.OPENDOORSFX, SoundManager.Instance.GetSFXVolume());
        yield return MoveDoor(initialPosition); // Move back to initial position
    }

    private IEnumerator MoveDoor(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target; // Ensure the final position is exact
        isMoving = false;
    }
}
