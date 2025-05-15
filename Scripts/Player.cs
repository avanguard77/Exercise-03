using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = UnityEngine.CharacterController;

public class Player : MonoBehaviour
{
    public static Player Instance;
    
    public event EventHandler<WakingToRunningEventArges> WakingToRunning;
    public event EventHandler Jumping;
    public event EventHandler JumpingToHanging;
    public event EventHandler HangingToLanding;

    public class WakingToRunningEventArges : EventArgs
    {
        public float playerSpeed;
    }

    [Header("REFERENCES")] private CharacterController controller;

    [Header("Movement Settings")] [SerializeField]
    private GameInput gameInput;

    [SerializeField] private float movementSpeed = 0f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float increaseRunningSpeed = 8;
    [SerializeField] private float increaseWalkingSpeed = 6;
    [SerializeField] private float decreaseSpeed = 4;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 2f;

    
     private bool wasAirborne = false;
    private float verticalVelocity = 0f;

    private bool isJumping ;
    private bool isWalking;
    private bool isShiftHold;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.ShiftSpeedStart += InstanceOnShiftSpeedStart;
        GameInput.Instance.ShiftSpeedEnd += InstanceOnShiftSpeedEnd;

        GameInput.Instance.IsJumpingPressed_Performed += InstanceOnIsJumpingPressed_Performed;

        controller = GetComponent<CharacterController>();
    }


    private void InstanceOnIsJumpingPressed_Performed(object sender, EventArgs e)
    {
        isJumping = true;
    }


    private void InstanceOnShiftSpeedEnd(object sender, EventArgs e)
    {
        isShiftHold = false;
    }

    private void InstanceOnShiftSpeedStart(object sender, EventArgs e)
    {
        isShiftHold = true;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        HandleGroundMovement();
    }

    private void HandleGroundMovement()
    {
        Vector2 inputVector2 = gameInput.GetInputGroundVector2Normalized();

        Vector3 movementDir = gameInput.CameraRotationRightXZ() * inputVector2.x +
                              gameInput.CameraRotationForwardXZ() * inputVector2.y;

        float movementDistance = movementSpeed * Time.deltaTime;
        verticalVelocity = VerticalForceCalculation();

        Vector3 finalMovement = movementDir * movementDistance + new Vector3(0, verticalVelocity * Time.deltaTime, 0);

        isWalking = movementDir != Vector3.zero;
        if (isWalking)
        {
            if (!isShiftHold)
            {
                float maxWalkingSpeed = 2;
                if (movementSpeed < maxWalkingSpeed)
                {
                    movementSpeed += Time.deltaTime * increaseWalkingSpeed;
                }
                else if (movementSpeed >= maxWalkingSpeed)
                {
                    movementSpeed -= Time.deltaTime * decreaseSpeed;
                }
            }
            else
            {
                float maxRunningSpeed = 6;

                if (movementSpeed <= maxRunningSpeed)
                {
                    movementSpeed += Time.deltaTime;
                }
            }
        }
        else
        {
            if (movementSpeed >= 0)
            {
                movementSpeed -= Time.deltaTime * decreaseSpeed;
            }
        }

        WakingToRunning?.Invoke(this, new WakingToRunningEventArges { playerSpeed = movementSpeed });

        controller.Move(finalMovement);

        // transform.position += movementDir * movementDistance;

        transform.forward = Vector3.Slerp(transform.forward, movementDir, rotateSpeed * Time.deltaTime);
        
        
    }

    // Track previous state

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            if (wasAirborne) // Ensure event fires only once upon landing
            {
                HangingToLanding?.Invoke(this, EventArgs.Empty);
            }

            wasAirborne = false; // Reset state when grounded
            verticalVelocity = 0f;

            if (isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * gravity * 2);
                Jumping?.Invoke(this, EventArgs.Empty);
                isJumping = false;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;

            if (!wasAirborne) // If transitioning from jumping to hanging
            {
                JumpingToHanging?.Invoke(this, EventArgs.Empty);
            }

            wasAirborne = true; // Mark player as airborne
            isJumping = false; // Ensure jumping state resets correctly
        }
        return verticalVelocity;
    }
}