using System;
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
    public bool inDeathPit = false;
    bool isDroppingPlatform = false;

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
        SetHorizontalVelocity();
        AddGravity();
        AddSlopeCompensation();
        CheckInputs();
        AddDrag();
        LimitVelocity();
        Velocity = newVelocity;
        if (IsGrounded)
            ResetCoyoteTime();
    }

    private void SetHorizontalVelocity()
    {
        if (inDeathPit)
            newVelocity.x = 0;
        else
        {
            newVelocity.x = inputs.HorizontalInput * horizontalSpeed;
            if (inputs.HorizontalInput > 0)
                Sprite.flipX = false;
            else if (inputs.HorizontalInput < 0)
                Sprite.flipX = true;
        }
    }

    private void CheckInputs()
    {
        if (inputs.JumpPressInput && (IsGrounded || IsCoyoteTime) && !IsJumping)
            newVelocity.y = jumpVelocity;
    }

    protected override void AddGravity()
    {
        if (inputs.DropInput && isTouchingPlatform && !isDroppingPlatform)
        {
            StartCoroutine(DropPlatform());
        }
        if (isDroppingPlatform)
            isTouchingPlatform = false;
        if (IsGrounded)
        {
            newVelocity.y = 0;
            RB.sharedMaterial = highFriction;
        }
        else
        {
            newVelocity.y += gravAcceleration * Time.deltaTime;
            coyoteTimeElapsed += Time.deltaTime;
            RB.sharedMaterial = noFriction;
        }
    }

    IEnumerator DropPlatform()
    {
        coyoteTimeElapsed += coyoteTime;
        newVelocity.y = 0;
        isDroppingPlatform = true;
        yield return new WaitForSeconds(0.1f);
        isDroppingPlatform = false;
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

