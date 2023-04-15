using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : GroundedCharacter
{
    [SerializeField] float ascendingDrag = 1;
    [SerializeField] float holdingJumpDrag = 1;
    [SerializeField] float coyoteTime = 0.2f;

    PlayerInputsComponent inputs;

    float coyoteTimeElapsed = 0;

    public bool IsCoyoteTime
    {
        get => coyoteTimeElapsed < coyoteTime;
    }

    new private void Awake()
    {
        base.Awake();
        inputs = GetComponent<PlayerInputsComponent>();
    }

    new private void FixedUpdate()
    {
        newVelocity = Velocity;
        SetHorizontalVelocity();
        AddGravity();
        AddDrag();
        CheckInputs();
        LimitVelocity();
        Velocity = newVelocity;

         // Debug.Log($"isGrounded: {IsGrounded}   velocity:({velocity.x},{velocity.y})");
    }

    private void SetHorizontalVelocity()
    {
        newVelocity.x = inputs.HorizontalInput * horizontalSpeed;
    }

    private void CheckInputs()
    {
        if (inputs.JumpPressInput && (IsGrounded || IsCoyoteTime) && !IsJumping)
            newVelocity.y = jumpVelocity;

        //can be optimized by exposing InputActions and tying events to them
        if (inputs.DropInput && IsTouchingPlatform)
        {
            IsTouchingPlatform = false;
        }
    }

    protected override void AddGravity()
    {
        if (IsGrounded)
            newVelocity.y = 0;
        else
        {
            newVelocity.y += gravAcceleration * Time.deltaTime;
            coyoteTimeElapsed += Time.deltaTime;
        }
    }

    private void AddDrag()
    {
        if (IsJumping)
        {
            newVelocity.y += ascendingDrag * Time.deltaTime;
            if (inputs.JumpHoldInput)
                newVelocity.y += holdingJumpDrag * Time.deltaTime;
        }
    }

    public void ResetCoyoteTime()
    {
        coyoteTimeElapsed = 0;
    }
}

