using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class PlayerInputsComponent : MonoBehaviour
{
    [SerializeField] float jumpInputBuffer = 0.1f;

    InputAction moveAction, jumpAction, dropAction, interactAction;
    public float HorizontalInput { get; private set; }
    public bool JumpPressInput { get; private set; }
    public bool JumpHoldInput { get; private set; }
    public bool DropInput { get; private set; }
    public bool InteractInput { get; private set; }

    private void Awake()
    {

        InputMap inputmap = new InputMap();
        moveAction = inputmap.FindAction("Move");
        jumpAction = inputmap.FindAction("Jump");
        dropAction = inputmap.FindAction("Drop");
        interactAction = inputmap.FindAction("Interact");
    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += (InputAction.CallbackContext ctx) => HorizontalInput = ctx.ReadValue<float>();
        moveAction.canceled += _ => HorizontalInput = 0;

        jumpAction.Enable();
        jumpAction.started += _ => 
        { 
            if (this != null)
            StartCoroutine(BufferJump()); 
        };
        jumpAction.performed += _ => JumpHoldInput = true;
        jumpAction.canceled += _ => JumpHoldInput = false;

        dropAction.Enable();
        dropAction.performed += _ => DropInput = true;
        dropAction.canceled += _ => DropInput = false;

        interactAction.Enable();
        interactAction.performed += _ => InteractInput = true;
        interactAction.canceled += _ => InteractInput = false;
    }
    private void OnDisable()
    {
        moveAction.performed -= (InputAction.CallbackContext ctx) => HorizontalInput = ctx.ReadValue<float>();
        moveAction.canceled -= _ => HorizontalInput = 0;

        jumpAction.started -= _ => StartCoroutine(BufferJump());
        jumpAction.performed -= _ => JumpHoldInput = true;
        jumpAction.canceled -= _ => JumpHoldInput = false;

        dropAction.performed -= _ => DropInput = true;
        dropAction.canceled -= _ => DropInput = false;

        interactAction.performed -= _ => InteractInput = true;
        interactAction.canceled -= _ => InteractInput = false;
    }


    IEnumerator BufferJump()
    {
        JumpPressInput = true;
        yield return new WaitForSeconds(jumpInputBuffer);
        JumpPressInput = false;
    }
}
