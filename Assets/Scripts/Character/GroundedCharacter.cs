using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Windows;

public abstract class GroundedCharacter : MonoBehaviour
{
    [SerializeField] protected float terminalVelocity;
    [SerializeField] protected float gravAcceleration;
    [SerializeField] protected float horizontalSpeed;
    [SerializeField] protected float jumpVelocity;
    public bool IsTouchingGround { get; set; }
    public bool IsTouchingPlatform { get; set; }

    public Rigidbody2D RB { get; private set; }

    public Vector2 Velocity
    {
        get => RB.velocity;
        protected set => RB.velocity = value;
    }

    protected Vector2 newVelocity;

    public bool IsFalling
    {
        get => Velocity.y < 0;
    }
    public bool IsJumping
    {
        get => Velocity.y > 0;
    }
    public bool IsGrounded
    {
        get => IsTouchingGround || IsTouchingPlatform;
    }

    protected void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    protected void FixedUpdate()
    {
        newVelocity = Velocity;
        AddGravity();
        LimitVelocity();
        Velocity = newVelocity;
    }

    protected virtual void LimitVelocity()
    {
        if (newVelocity.y < terminalVelocity)
            newVelocity.y = terminalVelocity;
    }

    protected virtual void AddGravity()
    {
        if (IsGrounded)
            newVelocity.y = 0;
        else
        {
            newVelocity.y += gravAcceleration * Time.deltaTime;
        }
    }
}
