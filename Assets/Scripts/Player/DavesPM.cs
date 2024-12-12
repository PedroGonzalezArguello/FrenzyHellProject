using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class DavesPM : MonoBehaviour
{
    [SerializeField]
    [Header("<color=blue>Movement</color>")]
    public float moveSpeed;
    public float extraMoveSpeed = 0;
    public float walkSpeed;
    //public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public bool activeGrapple;
    public float baseWalkSpeed;
    public float baseWallRunSpeed;
    public float baseSlideSpeed;
    public float ForceJumpX;
    public float velocityXZmultiplier;

    public MeshRenderer meshRenderer;


    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    //public int jumps;
    public int maxJumps = 2; // Establece el máximo número de saltos permitidos
    public int jumpsLeft;
    public float jumpDelay = 0.5f; // Delay entre saltos
    private float lastJumpTime;
    bool readyToJump;
    private bool canJump = true; // Bandera para permitir saltar


    [Header("GroundSlam")]
    [SerializeField] private float _groundSlamSpeed;
    private GroundSlam _groundSlam;

    [Header("Dash")]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;
    public bool dashing;
    private float speedChangeFactor;

    //[Header("Crouching")]
    //public float crouchSpeed;
    //public float crouchYScale;
    //private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    private KeyCode _slamKey = KeyCode.LeftControl;
    //public KeyCode sprintKey = KeyCode.LeftShift;
    //public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Effects")]
    public PlayerLook cam;
    public float grappleFOV;
    public float cameraTilt;
    public float HorizontalcameraTilt;

    [Header("Animaciones")]
    public Animator animatorUI;
    public Animator movementAnim;

    [Header("Referencias")]
    public Transform orientation;
    private WallRunning wr;
    public MeshRenderer mr;
    public PlayerLook camEffects;
    public GunSystem gunSystem;
    public TextMeshProUGUI textSpeed;
    public TextMeshProUGUI textFrenzy;
    public FrenzyManager fm;
    public ParticleSystem particles;
    SoundManager soundManager;
    Rigidbody rb;
    public GameManager gameManager;

    public ParticleSystem _linesParticles;
    public ParticleSystem _linesParticles2;
    public ParticleSystem _linesParticles3;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 playerVelocityMomentum;

    private MovementState lastState;
    private bool keepMomentum;


    public MovementState state;
    public enum MovementState
    {
        freeze,
        walking,
        //sprinting,
        //crouching,
        sliding,
        wallrunning,
        dashing,
        air
    }

    public bool sliding;
    public bool wallrunning;
    public bool freeze;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gameManager = FindObjectOfType<GameManager>();
        readyToJump = true;
        jumpsLeft = maxJumps; // Inicializa el contador de saltos
        cam.DoFov(95);
        _groundSlam = new GroundSlam(_groundSlamSpeed, this);

        //startYScale = transform.localScale.y;

        walkSpeed = baseWalkSpeed;
        wallRunSpeed = baseWallRunSpeed;
        slideSpeed = baseSlideSpeed;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        meshRenderer.enabled = false;

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (state == MovementState.walking)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (grounded)
        {
            jumpsLeft = maxJumps; // Restablece el contador de saltos al tocar el suelo
            movementAnim.SetBool("OnGround", true);
            //jumps = 0;
            cam.DoTilt(0);
        } else
        {
            movementAnim.SetBool("OnAir", false);
            movementAnim.SetBool("OnGround", false);
            if (Input.GetKeyDown(_slamKey)){
                StartCoroutine(_groundSlam.DoGroundSlam());
            }
        }


        //SetText
        textSpeed.SetText("RBVelocity: " + rb.velocity.magnitude + " moveSpeed: " + moveSpeed + "    desiredMoveSpeed " + desiredMoveSpeed);

        textFrenzy.SetText(" " + fm.currentFrenzy);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        if (!activeGrapple)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        //Reinicio Escena
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("Level1");
        }

        // Jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && jumpsLeft > 0 && Time.time - lastJumpTime > jumpDelay && canJump)
        {
            readyToJump = false;
            lastJumpTime = Time.time; // Actualiza el tiempo del último salto
            Jump();
            cam.DoFov(115);
            Invoke(nameof(ResetJump), jumpCooldown);

            
            _linesParticles3.Play();
        }
        
    }

    #region Estados
    private void StateHandler()
    {
        //Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        // Mode - Dashing
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;

            //pAra Activar particula de Rayo
            particles.Play();

            _linesParticles.Play();
            _linesParticles2.Play();
        }

        // Mode - Sliding
        else if (sliding)
        {
            //pAra Activar particula de lineas 
            _linesParticles.Play();
            _linesParticles2.Play();


            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = walkSpeed;



        }

        // Mode - Crouching
        //else if (Input.GetKey(crouchKey))
        //{
        //state = MovementState.crouching;
        //desiredMoveSpeed = crouchSpeed;
        //}

        // Mode - Sprinting
        //else if (grounded && Input.GetKey(sprintKey))
        //{
        //state = MovementState.sprinting;
        //desiredMoveSpeed = sprintSpeed;
        //}

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            if (horizontalInput == 0)
            {
                cam.DoTilt(0);
            }

            if (horizontalInput == 1)
            {
                cam.DoTilt(-HorizontalcameraTilt);
            }
            if (horizontalInput == -1)
            {
                cam.DoTilt(HorizontalcameraTilt);
            }

        }

        // Estado: WallRunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
            DisableJump();
            _linesParticles.Play();
            _linesParticles2.Play();
        }

        // Mode - Air
        else
        {
            EnableJump();
            state = MovementState.air;

        }

        // check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed2());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastState = state;

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, (time / difference)*1.5f);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed2()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, (time / difference) * 1.5f);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        //Vector3 characterVelocity = transform.right * horizontalInput * moveSpeed + transform.forward * verticalInput * moveSpeed;

        // apply momentum
        //moveSpeed += playerVelocityMomentum;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * (moveSpeed + extraMoveSpeed) * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }
    #endregion

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limit y vel
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

    private void Jump()
    {
        jumpsLeft--; // Decrementa el contador de saltos

        exitingSlope = true;

        movementAnim.SetBool("OnAir", true);

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        SoundManager.PlaySound(SoundType.JUMP, SoundManager.Instance.GetSFXVolume());
    }
    private void ResetJump()
    {
        readyToJump = true;
        cam.DoFov(95);
        exitingSlope = false;
    }
    public void RecoverJump()
    {
        if (jumpsLeft < maxJumps)
        {
            jumpsLeft++;
        }
    }
    public void DisableJump()
    {
        canJump = false;
    }
    private void EnableJump()
    {
        canJump = true;
    }

    public void JumpToPosition(Vector3 targetPosition)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, 100f);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;

        cam.DoFov(95);
        if (!wr) return;
        if (wr.wallRight)
        {
            cam.DoTilt(18);
        }
        else if (wr.wallLeft)
        {
            cam.DoTilt(-18);
        }

    }


    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }


    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = ((displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity)) * ForceJumpX));


        return velocityXZ + velocityY;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Spikes spikes = collision.gameObject.GetComponent<Spikes>();

        if (spikes != null)
        {
            SoundManager.PlaySound(SoundType.SPIKEDEATH, SoundManager.Instance.GetSFXVolume());

            //SceneManager.LoadScene("DefeatScene");
            gameManager.ShowDefeatScreen();
            //respawn.RespawnOnFall();    

            fm.TakeDamage(spikes.spikeDamage);

        }
        Space2Double SpaceTo = collision.gameObject.GetComponent<Space2Double>();

        if (SpaceTo != null)
        {
            SpaceTo.SpaceAnim();
        }

        PrensaDamage Prensa = collision.gameObject.GetComponent<PrensaDamage>();

        if (Prensa != null)
        {
            fm.TakeDamage(Prensa.prensaDamage);

        }


        if (collision.gameObject.CompareTag("VictoryCollider"))
        {
            gameManager.ShowVictoryScreen();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("SpaceToDouble"))
        {
            Space2Double SpaceTo = other.gameObject.GetComponent<Space2Double>();
            SpaceTo.SpaceAnim();
        }
    }

    public void ApplyImpulse(float upwardImpulseForce, float forwardImpulseForce)
    {
            // Calculate the direction
            Vector3 forwardDirection = transform.forward;
            Vector3 upwardDirection = Vector3.up;

            // Apply the forces
            Vector3 combinedForce = (upwardDirection * upwardImpulseForce) + (forwardDirection * forwardImpulseForce);
            rb.AddForce(combinedForce, ForceMode.Impulse);
    }

}