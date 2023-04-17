using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
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
        FloorCheck();
        //WallCheck();
        SetHorizontalVelocity();
        AddGravity();
        AddSlopeCompensation();
        CheckInputs();
        AddDrag();
        LimitVelocity();
        Velocity = newVelocity;
        if (IsGrounded)
            ResetCoyoteTime();
         // Debug.Log($"isGrounded: {IsGrounded}   velocity:({velocity.x},{velocity.y})");
    }

    private void SetHorizontalVelocity()
    {
        newVelocity.x = inputs.HorizontalInput * horizontalSpeed;
        if (newVelocity.x == 0)
            RB.sharedMaterial = highFriction;
        else
            RB.sharedMaterial = noFriction;
    }

    private void CheckInputs()
    {
        if (inputs.JumpPressInput && (IsGrounded || IsCoyoteTime) && !IsJumping)
            newVelocity.y = jumpVelocity;
    }

    protected override void AddGravity()
    {
        if (inputs.DropInput && isTouchingPlatform)
        {
            isTouchingPlatform = false;
        }
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

