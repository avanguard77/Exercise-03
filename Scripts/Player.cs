using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = UnityEngine.CharacterController;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public event EventHandler<WakingToRunningEventArges> WakingToRunning;
    public event EventHandler SwitchRunnnigToFlying;

    public class WakingToRunningEventArges : EventArgs
    {
        public float playerSpeed;
    }

    [Header("REFERENCES")] private CharacterController controller;

    [Header("Movement Settings")] [SerializeField]
    private GameInput gameInput;

    [SerializeField] private float movementSpeed = 0f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] float increaseRunningSpeed = 8;
    [SerializeField] float increaseWalkingSpeed = 6;
    [SerializeField] float decreaseSpeed = 4;
    
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
        controller = GetComponent<CharacterController>();
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
        SwitchRunnnigToFlying?.Invoke(this, EventArgs.Empty);

        Vector2 inputVector2 = gameInput.GetInputGroundVector2Normalized();

        Vector3 movementDir = gameInput.CameraRotationRightXZ() * inputVector2.x +
                              gameInput.CameraRotationForwardXZ() * inputVector2.y;

        float movementDistance = movementSpeed * Time.deltaTime;

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
        controller.Move(movementDir * movementDistance);
        // transform.position += movementDir * movementDistance;

        transform.forward = Vector3.Slerp(transform.forward, movementDir, rotateSpeed * Time.deltaTime);
    }
}