using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public event EventHandler IsJumpingPressed_Performed;
    public event EventHandler ShiftSpeedStart;
    public event EventHandler ShiftSpeedEnd;


    private InputSystem playerInputAction;

    [SerializeField] private Camera playerCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerInputAction = new InputSystem();
        playerInputAction.Enable();

        playerInputAction.Player.ShiftSpeed.performed += ShiftSpeedStart_Onperformed;
        playerInputAction.Player.ShiftSpeed.canceled += ShiftSpeedEnd_Oncanceled;

        playerInputAction.Player.Jumping.performed += JumpingOnperformed;

    }

    private void JumpingOnperformed(InputAction.CallbackContext obj)
    {
        IsJumpingPressed_Performed?.Invoke(this, EventArgs.Empty);
    }

    private void ShiftSpeedEnd_Oncanceled(InputAction.CallbackContext obj)
    {
        ShiftSpeedEnd?.Invoke(this, EventArgs.Empty);
    }

    private void ShiftSpeedStart_Onperformed(InputAction.CallbackContext obj)
    {
        ShiftSpeedStart?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputGroundVector2Normalized()
    {
        Vector2 input = playerInputAction.Player.Move.ReadValue<Vector2>();
        input.Normalize();
        return input;
    }

    public Vector3 CameraRotationForwardXZ()
    {
        Vector3 camForwardXZ = new Vector3(playerCam.transform.forward.x, 0, playerCam.transform.forward.z);
        return camForwardXZ.normalized;
    }

    public Vector3 CameraRotationRightXZ()
    {
        Vector3 camForwardXZ = new Vector3(playerCam.transform.right.x, 0, playerCam.transform.right.z);
        return camForwardXZ.normalized;
    }
}