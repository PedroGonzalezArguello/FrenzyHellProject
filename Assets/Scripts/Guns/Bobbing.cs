using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PositionFollower))]
public class Bobbing : MonoBehaviour
{
    [Header("On Movement Bobbing")]
    public float EffectIntensity;
    public float EffectIntensityX;
    public float EffectSpeed;

    [Header("On Idle Bobbing")]
    public float IdleEffectIntensity;
    public float IdleEffectIntensityX;
    public float IdleEffectSpeed;

    private PositionFollower FollowerInstnce;
    private Vector3 OriginalOffset;
    private float sinTime;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    public bool grounded;


    [Header("Referencias")]
    public DavesPM dpm;

    void Start()
    {
        FollowerInstnce = GetComponent<PositionFollower>();
        OriginalOffset = FollowerInstnce.Offset;
    }


    void Update()
    {

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
            if (inputVector.magnitude > 0f)
            {
                sinTime += Time.deltaTime * EffectSpeed;

            }
            else if (inputVector.magnitude == 0)
            {
                sinTime += Time.deltaTime * IdleEffectSpeed;
            }
            else
            {
                sinTime = 0f;
            }

            float sinAmountY = -Mathf.Abs(EffectIntensity * Mathf.Sin(sinTime));
            Vector3 sinAmountX = FollowerInstnce.transform.right * EffectIntensity * Mathf.Cos(sinTime) * EffectIntensityX;

            FollowerInstnce.Offset = new Vector3
            {
                x = OriginalOffset.x,
                y = OriginalOffset.y + sinAmountY,
                z = OriginalOffset.z
            };

            float IdlesinAmountY = -Mathf.Abs(IdleEffectIntensity * Mathf.Sin(sinTime));
            Vector3 IdlesinAmountX = FollowerInstnce.transform.right * IdleEffectIntensity * Mathf.Cos(sinTime) * IdleEffectIntensityX;

            FollowerInstnce.Offset = new Vector3
            {
                x = OriginalOffset.x,
                y = OriginalOffset.y + IdlesinAmountY,
                z = OriginalOffset.z
            };
        }

    }
}

    
