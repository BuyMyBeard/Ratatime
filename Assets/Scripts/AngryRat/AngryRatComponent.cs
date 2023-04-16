using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static Game_Manager;

[RequireComponent(typeof(TargetingComponent))]
public class AngryRatComponent : GroundedCharacter
{
    public bool Aggravated { get; set; } = false;

    public bool AttempingJump { get; set; } = false;

    private TargetingComponent targetingComponent;



    new void Awake()
    {
        base.Awake();
        targetingComponent = GetComponent<TargetingComponent>();
        
    }

    private void Update()
    {
        Aggravated = Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < targetingComponent.MaxTargetDistance;
    }

    new void FixedUpdate()
    {
        newVelocity = Velocity;
        SetHorizontalMovement();
        AddGravity();
        CheckJump();
        LimitVelocity();
        Velocity = newVelocity;
    }

    void SetHorizontalMovement ()
    {
        newVelocity.x = targetingComponent.HorizontalMoveCommand * horizontalSpeed;
    }

    void CheckJump ()
    {
        if (AttempingJump && IsGrounded && !IsJumping)
        {
            newVelocity.y = jumpVelocity;
        }
    }
}
