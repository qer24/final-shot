using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public EntityStats playerStats;

    [Space]

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public Transform crouchPivot;
    public float normalHeight = 2f, crouchHeight = 1.5f;
    public float crouchSlowdown = 0.6f;
    public Vector3 crouchOffset = new Vector3(0f, -0.5f, 0f);

    public float slideTimeAfterRun = 0.5f;
    public float slideSpeedUp;
    public float slideSlowdown;
    public float slideCooldown = 2f;
    public ParticleSystem slideParticles;
    public ParticleSystem slideSpeedlines;

    [SerializeField] private float stepInterval = 5f;
    [SerializeField] [Range(0f, 1f)] private float runstepLenghten = 0.7f;
    [SerializeField] private bool useHeadBob = true;
    [SerializeField] private CurveControlledBob headBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob jumpBob = new LerpControlledBob();
    [SerializeField] private Transform hand = null;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    bool isRunning = false;
    private Vector3 originalCameraPosition;
    bool previouslyGrounded;

    float stepCycle;
    float nextStep;

    public bool isCrouching;
    public bool isSliding;
    bool slideFovBoost;
    float slideTimer;
    float currentSlideSpeed;
    float slideCooldownTimer;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalCameraPosition = playerCamera.transform.localPosition;
        headBob.Setup(playerCamera, stepInterval);

        stepCycle = 0f;
        nextStep = stepCycle / 2f;

        isSliding = false;
        slideTimer = 0;
        currentSlideSpeed = runningSpeed;
        slideFovBoost = false;
    }

    void Update()
    {
        if (GameManager.isPaused || RewardsManager.isChoosingReward) return;

        if(transform.position.y < -10)
        {
            transform.position = Vector3.up;
        }

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isRunning)
        {
            if (slideTimer <= slideTimeAfterRun)
            {
                slideTimer += Time.deltaTime;
            }
        }
        else
        {
            if (slideTimer >= 0)
            {
                slideTimer -= Time.deltaTime;
            }
        }

        if (!isSliding && isCrouching && slideTimer > 0 && slideCooldownTimer <= 0)
        {
            slideTimer = 0;
            StartCoroutine(nameof(Slide));
        }
        if (isSliding && !isCrouching && characterController.isGrounded)
        {
            CancelSlide();
        }
        slideCooldownTimer -= Time.deltaTime;

        Crouch();

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        var fov = slideFovBoost ? 105 : isRunning ? 90 : 75;
        if (input.magnitude < 0.1f) fov = 75;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, Time.deltaTime * 8f);

        float speed = isSliding ? currentSlideSpeed : canMove ? (isRunning ? runningSpeed : walkingSpeed) : 0;
        speed = PlayerRewind.isRewinding ? 0 : speed;
        speed *= playerStats.speedMultiplier;

        UpdateCameraRotation(speed);
        UpdateCameraPosition(speed);
        ProgressStepCycle(speed);

        if (PlayerRewind.isRewinding) return;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = speed * input.y;
        float curSpeedY = speed * input.x;

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection = Vector3.ClampMagnitude(moveDirection, speed);

        if (moveDirection.magnitude < 0.1f)
        {
            CancelSlide();
            slideTimer = 0;
        }

        if (Input.GetButtonDown("Jump") && canMove && characterController.isGrounded)
        {
            AudioManager.Play("jump");
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }


        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        //jump bob
        if (!previouslyGrounded && characterController.isGrounded)
        {
            StartCoroutine(jumpBob.DoBobCycle());
            PlayLandingSound();
        }

        previouslyGrounded = characterController.isGrounded;
    }

    private void CancelSlide()
    {
        slideFovBoost = false;
        StopCoroutine(nameof(Slide));
        isSliding = false;
        currentSlideSpeed = runningSpeed;

        var emision = slideParticles.emission;
        emision.rateOverDistance = 0f;
        emision = slideSpeedlines.emission;
        emision.rateOverDistance = 0f;
    }

    void Crouch()
    {
        var pos = isCrouching || isSliding ? crouchOffset : Vector3.zero;
        crouchPivot.localPosition = Vector3.Lerp(crouchPivot.localPosition, pos, Time.deltaTime * 8f);
        var colliderHeight = isCrouching || isSliding ? crouchHeight : normalHeight;
        characterController.height = Mathf.Lerp(characterController.height, colliderHeight, Time.deltaTime * 8f);
    }

    IEnumerator Slide()
    {
        AudioManager.Play("slide");

        var emision = slideParticles.emission;
        emision.rateOverDistance = 20f;
        emision = slideSpeedlines.emission;
        emision.rateOverDistance = 10f;

        isSliding = true;
        slideCooldownTimer = slideCooldown;

        currentSlideSpeed = runningSpeed;
        float slideTargetSpeed = slideSpeedUp;

        slideFovBoost = true;

        while (Mathf.Abs(currentSlideSpeed - slideTargetSpeed) > 0.1f)
        {
            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, slideTargetSpeed, Time.deltaTime * 9f);
            yield return null;
        }

        emision = slideParticles.emission;
        emision.rateOverDistance = 0f;
        emision = slideSpeedlines.emission;
        emision.rateOverDistance = 0f;

        if (characterController.isGrounded)
        {

            slideTargetSpeed = slideSlowdown;

            slideFovBoost = false;

            while (Mathf.Abs(currentSlideSpeed - slideTargetSpeed) > 0.1f)
            {
                currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, slideTargetSpeed, Time.deltaTime * 5f);
                yield return null;
            }
        }else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isSliding = false;
    }

    private void UpdateCameraRotation(float speed)
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed * SensitivtySettings.sensitivityMultiplier;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed * SensitivtySettings.sensitivityMultiplier, 0);
    }

    private void ProgressStepCycle(float speed)
    {
        if (characterController.velocity.sqrMagnitude > 0 && speed != 0)
        {
            stepCycle += (characterController.velocity.magnitude + (speed * (!isRunning ? 1f : runstepLenghten))) * Time.deltaTime;
        }

        if (!(stepCycle > nextStep))
        {
            return;
        }

        nextStep = stepCycle + stepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!characterController.isGrounded || isSliding)
        {
            return;
        }
        AudioManager.Play("footstep");
    }

    private void PlayLandingSound()
    {
        AudioManager.Play("land");
    }

    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!useHeadBob || Mathf.Approximately(speed, 0) || isSliding)
        {
            return;
        }
        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            playerCamera.transform.localPosition =
                headBob.DoHeadBob(characterController.velocity.magnitude +
                                  (speed * (!isRunning ? 1f : runstepLenghten)));
            newCameraPosition = playerCamera.transform.localPosition;
            newCameraPosition.y = playerCamera.transform.localPosition.y - jumpBob.Offset();
        }
        else
        {
            newCameraPosition = playerCamera.transform.localPosition;
            newCameraPosition.y = originalCameraPosition.y - jumpBob.Offset();
        }

        hand.transform.localPosition = newCameraPosition - originalCameraPosition;

        newCameraPosition.x = 0;
        playerCamera.transform.localPosition = newCameraPosition;
    }
}
