using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static Game_Manager;

public class AngryRatComponent : GroundedCharacter
{
    public bool AttempingJump { get; set; } = false;

    public float ActualHorizontalSpeed;
    private AngryRatMovement movementComponent;



    new void Awake()
    {
        base.Awake();
        ActualHorizontalSpeed = horizontalSpeed;
        movementComponent = GetComponent<AngryRatMovement>();
    }

    new void FixedUpdate()
    {
        newVelocity = Velocity;
        SetHorizontalMovement();
        FloorCheck();
        AddGravity();
        AddSlopeCompensation();
        CheckJump();
        LimitVelocity();
        Velocity = newVelocity;
    }

    void SetHorizontalMovement ()
    {
        newVelocity.x = movementComponent.HorizontalMoveCommand * ActualHorizontalSpeed;
    }

    void CheckJump ()
    {
        if (AttempingJump && IsGrounded && !IsJumping)
        {
            newVelocity.y = jumpVelocity;
        }
    }
}
