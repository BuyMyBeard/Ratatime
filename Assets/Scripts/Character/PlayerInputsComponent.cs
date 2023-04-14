using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsComponent : MonoBehaviour
{
    InputAction moveAction, jumpAction, dropAction;
    public float HorizontalInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DropInput { get; private set; }
    private void Awake()
    {
        InputMap inputmap = new InputMap();
        moveAction = inputmap.FindAction("Move");
        jumpAction = inputmap.FindAction("Jump");
        dropAction = inputmap.FindAction("Drop");
    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += (InputAction.CallbackContext ctx) => HorizontalInput = ctx.ReadValue<float>();
        moveAction.canceled += _ => HorizontalInput = 0;

        jumpAction.Enable();
        jumpAction.performed += _ => JumpInput = true;
        jumpAction.canceled += _ => JumpInput = false;

        dropAction.Enable();
        dropAction.performed += _ => DropInput = true;
        dropAction.canceled += _ => DropInput = false;

    }
    private void OnDisable()
    {
        moveAction.performed -= (InputAction.CallbackContext ctx) => HorizontalInput = ctx.ReadValue<float>();
        moveAction.canceled -= _ => HorizontalInput = 0;

        jumpAction.performed -= _ => JumpInput = true;
        jumpAction.canceled -= _ => JumpInput = false;

        dropAction.performed -= _ => DropInput = true;
        dropAction.canceled -= _ => DropInput = false;
    }
}
