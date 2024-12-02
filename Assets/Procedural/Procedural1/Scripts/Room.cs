using UnityEngine;

public class Room : MonoBehaviour
{
    public bool hasTopDoor, hasBottomDoor, hasLeftDoor, hasRightDoor;

    public bool HasTopDoor() { return hasTopDoor; }
    public bool HasBottomDoor() { return hasBottomDoor; }
    public bool HasLeftDoor() { return hasLeftDoor; }
    public bool HasRightDoor() { return hasRightDoor; }
}
