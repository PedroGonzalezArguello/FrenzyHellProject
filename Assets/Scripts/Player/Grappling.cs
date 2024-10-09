using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public DavesPM dpm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    SoundManager soundManager;
    public Image Crossair;
    public Image _Crossair;

    [Header("Camera Effects")]
    public PlayerLook camEffects;
    public float grappleFOV;
    public float cameraTilt;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;
    private bool CanGrapple;

    private void Start()
    {
        dpm = GetComponent<DavesPM>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) && !grappling)
        {
            _Crossair.enabled = true;
            Crossair.color = Color.red;
            SoundManager.PlayLoopingSound(SoundType.GRAPPLEON, SoundManager.Instance.GetSFXVolume());

        }
        else
        {
            _Crossair.enabled = false;
            Crossair.color = Color.blue;
            SoundManager.StopLoopingSound(SoundType.GRAPPLEON);
        }

        if (Input.GetKeyDown(grappleKey) && !grappling)
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        _Crossair.enabled = false;
        Crossair.color = Color.blue;

        if (grapplingCdTimer > 0) return;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grappling = true;
            dpm.freeze = true;
            grapplePoint = hit.point;



            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }

        else
        {
            grapplePoint = Vector3.zero;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private void ExecuteGrapple()
    {
        if (grapplePoint == Vector3.zero) return;

        camEffects.DoFov(grappleFOV);

        SoundManager.PlaySound(SoundType.GRAPPLE, SoundManager.Instance.GetSFXVolume());
        dpm.freeze = false;
        /*
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        float grappleSpeed = 5f;

        dpm.JumpToPosition(grapplePoint * grappleSpeed * Time.deltaTime);

        Invoke(nameof(StopGrapple), 1f);*/


        //Que hace esto? -- Drito
        float hookshootSpeedMin = 40f;
        float hookshootSpeedMax = 45f;
        float hookshootSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grapplePoint), hookshootSpeedMin, hookshootSpeedMax);
        float speed = 80f;
        StartCoroutine(MoveToGrapplePoint(grapplePoint, speed));


    }
    private IEnumerator MoveToGrapplePoint(Vector3 targetPosition, float speed)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / speed;

        /*
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        */

        while (Vector3.Distance(transform.position, targetPosition) > 1.6f) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
            print(Vector3.Distance(transform.position, targetPosition));
        }

        //transform.position = targetPosition;
        StopGrapple();
    }

    public void StopGrapple()
    {
        camEffects.DoFov(95);

        dpm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;

    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}

