using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : MonoBehaviour
{
    [SerializeField] float gravAcceleration = -5;
    [SerializeField] float ascendingDrag = 1;
    [SerializeField] float holdingJumpDrag = 1;
    [SerializeField] float jumpVelocity = 1;
    [SerializeField] float terminalVelocity = -1;
    [SerializeField] float horizontalSpeed = 1;
    [SerializeField] float coyoteTime = 0.2f;

    PlayerInputsComponent inputs;
    Vector2 velocity = Vector2.zero;
    float coyoteTimeElapsed = 0;

    public bool IsTouchingGround { get; set; }
    public bool IsTouchingPlatform { get; set; }

    public bool IsFalling
    {
        get => velocity.y < 0;
    }
    public bool IsJumping
    {
        get => velocity.y > 0;
    }
    public bool IsGrounded
    {
        get => IsTouchingGround || IsTouchingPlatform;
    }
    public bool IsCoyoteTime
    {
        get => coyoteTimeElapsed < coyoteTime; 
    }

    private void Awake()
    {
        inputs = GetComponent<PlayerInputsComponent>();
    }

    private void Update()
    {
        SetHorizontalVelocity();
        AddGravity();
        AddDrag();
        CheckInputs();
        LimitVelocity();
        transform.Translate(Time.deltaTime * velocity);
        //Debug.Log($"isGrounded: {IsGrounded}   velocity:({velocity.x},{velocity.y})   jumping:{inputs.JumpInput}");
    }

    private void SetHorizontalVelocity()
    {
        velocity.x = inputs.HorizontalInput * horizontalSpeed;
    }

    private void CheckInputs()
    {
        if (inputs.JumpPressInput && (IsGrounded || IsCoyoteTime) && !IsJumping)
            velocity.y = jumpVelocity;

        //can be optimized by exposing InputActions and tying events to them
        if (inputs.DropInput && IsTouchingPlatform)
        {
            IsTouchingPlatform = false;
        }
    }

    private void LimitVelocity()
    {
        if (velocity.y < terminalVelocity)
            velocity.y = terminalVelocity;
    }

    private void AddGravity()
    {
        if (IsGrounded)
            velocity.y = 0;
        else
        {
            velocity.y += gravAcceleration * Time.deltaTime;
            coyoteTimeElapsed += Time.deltaTime;
        }
    }

    private void AddDrag()
    {
        if (IsJumping)
        {
            velocity.y += ascendingDrag * Time.deltaTime;
            if (inputs.JumpHoldInput)
                velocity.y += holdingJumpDrag * Time.deltaTime;
        }
    }

    public void ResetCoyoteTime()
    {
        coyoteTimeElapsed = 0;
    }
}

