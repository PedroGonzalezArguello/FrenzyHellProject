using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask isWall;
    public LayerMask isGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpForwardForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Wall Climb")]
    public float maxWallClimbTime = 2.0f; // Tiempo máximo de escalada
    private float wallClimbTimer;
    private bool isClimbingWall;

    [Header("SpeedClimb")]
    public float speedClimbForce = 10f; // Fuerza de escalada rápida
    public float maxSpeedClimbTime = 2.0f; // Tiempo máximo para la escalada rápida
    private float speedClimbTimer;
    private bool isSpeedClimbing;

    [Header("Wall Climb Jump")]
    public float climbJumpUpForce = 5f; // Fuerza del salto hacia arriba al terminar de trepar
    public float climbJumpBackForce = 5f; // Fuerza del salto hacia atrás al terminar de trepar

    [Header("Input")]
    public KeyCode climbKey = KeyCode.W; // Tecla para escalar y correr hacia arriba
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance = 0.5f;
    public float frontWallCheckDistance = 1.5f; // Distancia para detectar la pared frente a ti
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private RaycastHit frontWallHit;
    public bool wallLeft;
    public bool wallRight;
    public bool frontWall; // Nuevo booleano para la detección de la pared frontal

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Effects")]
    public float normalFOV;
    public float fovEffect;
    public float cameraTilt;

    [Header("References")]
    private Rigidbody rb;
    private DavesPM dpm;
    public Transform orientation;
    public PlayerLook cam;
    private SoundManager soundManager;
    public Dashing dash;

    private bool isWallRunningSoundPlaying = false; // Bandera para controlar el sonido de wallrun

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dpm = GetComponent<DavesPM>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
        CheckForWall();
        HandleWallClimbing();
        HandleSpeedClimbing(); // Manejo de SpeedClimb
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (dpm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        // Detección de pared lateral para wallrunning
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, isWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, isWall);

        // Detección de pared frontal para wall climbing y SpeedClimbing
        frontWall = Physics.Raycast(transform.position, orientation.forward, out frontWallHit, frontWallCheckDistance, isWall);

        if (frontWall && Input.GetKeyDown(climbKey))
        {
            StartWallClimb();
        }
    }

    private void HandleWallClimbing()
    {
        if (isClimbingWall)
        {
            ClimbWall();

            if (Input.GetKeyDown(jumpKey) || wallClimbTimer <= 0)
            {
                PerformBackJump();
            }
        }
    }

    private void StartWallClimb()
    {
        isClimbingWall = true;
        wallClimbTimer = maxWallClimbTime;
        rb.useGravity = false; // Desactiva la gravedad mientras escalas
    }

    private void ClimbWall()
    {
        if (wallClimbTimer > 0)
        {
            // Movimiento de escalada
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
            wallClimbTimer -= Time.deltaTime;
        }
        else
        {
            StopWallClimb();
        }
    }

    private void PerformBackJump()
    {
        // Salto hacia atrás
        StopWallClimb();

        Vector3 backJumpDirection = -orientation.forward + Vector3.up; // Hacia atrás y arriba
        rb.AddForce(backJumpDirection * wallJumpForwardForce, ForceMode.Impulse);

        cam.DoFov(normalFOV); // Restablecer FOV de la cámara si es necesario
    }

    private void StopWallClimb()
    {
        isClimbingWall = false;
        rb.useGravity = true; // Reactiva la gravedad
    }

    private void HandleSpeedClimbing()
    {
        if (isSpeedClimbing)
        {
            PerformSpeedClimb();

            if (speedClimbTimer <= 0 || !Input.GetKey(climbKey))
            {
                StopSpeedClimb();
            }
        }
        else if (frontWall && Input.GetKey(climbKey) && !isClimbingWall) // Comienza SpeedClimb
        {
            StartSpeedClimb();
        }
    }

    private void StartSpeedClimb()
    {
        isSpeedClimbing = true;
        speedClimbTimer = maxSpeedClimbTime;
        rb.useGravity = false; // Desactiva la gravedad mientras escalas rápidamente
    }

    private void PerformSpeedClimb()
    {
        if (speedClimbTimer > 0)
        {
            // Movimiento de escalada rápida
            rb.velocity = new Vector3(rb.velocity.x, speedClimbForce, rb.velocity.z);
            speedClimbTimer -= Time.deltaTime;
        }
        else
        {
            StopSpeedClimb();
        }
    }

    private void StopSpeedClimb()
    {
        if (isSpeedClimbing)
        {
            PerformClimbJump(); // Realiza el salto al terminar la escalada
        }

        isSpeedClimbing = false;
        rb.useGravity = true; // Reactiva la gravedad
    }

    private void PerformClimbJump()
    {
        // Vector de dirección para el salto hacia arriba y atrás
        Vector3 climbJumpDirection = Vector3.up * climbJumpUpForce - orientation.forward * climbJumpBackForce;

        // Aplicar la fuerza del salto
        rb.AddForce(climbJumpDirection, ForceMode.Impulse);

        // Restablecer el FOV de la cámara si es necesario
        cam.DoFov(normalFOV);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Detener la escalada si se deja de colisionar con la pared
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            StopWallClimb();
            StopSpeedClimb();
        }
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(climbKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && !exitingWall)
        {
            if (!dpm.wallrunning)
            {
                StartWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && dpm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(jumpKey) || Input.GetKeyDown(dash.dashKey))
            {
                WallJump();
                cam.DoFov(normalFOV);
            }
        }
        else if (exitingWall)
        {
            if (dpm.wallrunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (dpm.wallrunning)
            {
                StopWallRun();
            }
        }

        // Lógica para SpeedClimbing
        if (frontWall && verticalInput > 0 && !exitingWall && !dpm.wallrunning && !isClimbingWall)
        {
            if (!isSpeedClimbing)
            {
                StartSpeedClimb();
            }
        }
        else
        {
            StopSpeedClimb();
        }
    }

    private void StartWallRun()
    {
        dpm.wallrunning = true;

        SoundManager.PlaySound(SoundType.STARTWALLRUN, SoundManager.Instance.GetSFXVolume());

        wallRunTimer = maxWallRunTime;

        dpm.RecoverJump();

        cam.DoFov(fovEffect);
        if (wallLeft)
        {
            cam.DoTilt(-cameraTilt);
        }
        if (wallRight)
        {
            cam.DoTilt(cameraTilt);
        }

        if (!isWallRunningSoundPlaying)
        {
            SoundManager.PlayLoopingSound(SoundType.WALLRUN, SoundManager.Instance.GetSFXVolume());
            isWallRunningSoundPlaying = true;
        }
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }

        if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRun()
    {
        dpm.wallrunning = false;

        cam.DoFov(normalFOV);
        cam.DoTilt(0);

        if (isWallRunningSoundPlaying)
        {
            SoundManager.StopLoopingSound(SoundType.WALLRUN);
            isWallRunningSoundPlaying = false;
        }
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;
        cam.DoFov(normalFOV);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        Vector3 forceToJump = transform.forward * wallJumpForwardForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        rb.AddForce(forceToJump, ForceMode.Force);

        SoundManager.PlaySound(SoundType.JUMP, SoundManager.Instance.GetSFXVolume());
    }
}
