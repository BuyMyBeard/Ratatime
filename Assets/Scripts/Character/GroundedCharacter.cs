using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public abstract class GroundedCharacter : MonoBehaviour
{
    protected Vector2 velocity;
    [SerializeField] protected float terminalVelocity;
    [SerializeField] protected float gravAcceleration;
    [SerializeField] protected float horizontalSpeed;
    [SerializeField] protected float jumpVelocity;
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


    protected virtual void LimitVelocity()
    {
        if (velocity.y < terminalVelocity)
            velocity.y = terminalVelocity;
    }

    protected virtual void AddGravity()
    {
        if (IsGrounded)
            velocity.y = 0;
        else
        {
            velocity.y += gravAcceleration * Time.deltaTime;
        }
    }
}
