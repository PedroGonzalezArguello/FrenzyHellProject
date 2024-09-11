using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("Reference")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private Camera cam;
    public PlayerLook camEffects;
    public WallRunning wr;
    public DavesPM dpm;
    SoundManager soundManager;
    public Animator animator;


    [Header("Sliding")]
    [SerializeField] private float slideCooldown;
    private bool canSlide = true;
    public float mazSlideTime;
    public float slideForce;
    private float slideTimer;

    //Bool para evitar Bug de Sonido
    private bool OnSlide;


    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey;
    private float horizontalInput;
    private float verticalInput;

    [Header("Effects")]
    public float fovEffect;
    public float cameraTilt;

    public Animator slidingAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dpm = GetComponent<DavesPM>();

        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("SlideKey"))
        {
           
            if ((horizontalInput != 0 || verticalInput != 0) && dpm.grounded && canSlide && !OnSlide)
            {
                //Bool para evitar Bug de Sonido
                OnSlide = true;
                StartSlide();
                SoundManager.PlaySound(SoundType.SLIDE, SoundManager.Instance.GetSFXVolume());
            }
            
        }

        if (Input.GetButtonUp("SlideKey") && dpm.sliding)
        {
            StopSlide();
            
            //Bool para evitar Bug de Sonido
            OnSlide = false;
        }
    }

    private void FixedUpdate()
    {
        if (dpm.sliding)
            SlidingMovement();
    }

    private void StartSlide () 
    {
        int RandomAnim = Random.Range(0, 101);
        if(RandomAnim < 100)
        {
            animator.SetBool("Sliding", true);
        }
        
        
        dpm.sliding = true;


        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);

        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = mazSlideTime;

        // Aplicar efectos de cámara
        camEffects.DoFov(fovEffect);
        camEffects.DoTilt(cameraTilt);
    }

    private void SlidingMovement() 
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        

        // Deslizamiento normal
        if (!dpm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Impulse);

            slideTimer -= Time.deltaTime;
        }

        // Deslizamiento en pendiente
        else 
        {
            rb.AddForce(dpm.GetSlopeMoveDirection(inputDirection).normalized * slideForce, ForceMode.Impulse);
        }


        

        if (slideTimer <= 0)
            StopSlide();
        

    }

    private void StopSlide()
    {
        animator.SetBool("Sliding", false);

        dpm.sliding = false;

        OnSlide = false;


        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

        StartCoroutine(SlideCooldown());

        // Resete la cámara
        camEffects.DoFov(wr.normalFOV);
        camEffects.DoTilt(0);
    }

    IEnumerator SlideCooldown()
    {
        // Entra en cooldown 
        canSlide = false;

        // Espera los segundos indicados por escena
        yield return new WaitForSeconds(slideCooldown);

        // Termina el cooldown
        canSlide = true;
    }
}
