using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    public Transform TargetTransform;
    public Vector3 Offset;

    private void Update()
    {
        transform.position = TargetTransform.position + Offset;
    }
}
