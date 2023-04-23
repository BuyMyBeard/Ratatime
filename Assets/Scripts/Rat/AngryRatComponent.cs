using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static Game_Manager;

public class AngryRatComponent : GroundedCharacter
{
    public bool JumpCommand;
    public int HorizontalMoveCommand;

    public float ActualHorizontalSpeed;

    new void Awake()
    {
        base.Awake();
        ActualHorizontalSpeed = horizontalSpeed;
    }

    new void FixedUpdate()
    {
        newVelocity = Velocity;
        SetHorizontalMovement();
        FloorCheck();
        AddGravity();
        CheckJump();
        AddSlopeCompensation();
        LimitVelocity();
        Velocity = newVelocity;
    }

    void SetHorizontalMovement()
    {
        newVelocity.x = HorizontalMoveCommand * ActualHorizontalSpeed;
    }

    void CheckJump()
    {
        if (JumpCommand && IsGrounded && !IsJumping)
        {
            newVelocity.y = jumpVelocity;
        }
    }
}
