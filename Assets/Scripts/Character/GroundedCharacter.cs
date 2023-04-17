using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Windows;

public enum Slope { Down, Up, None };

public abstract class GroundedCharacter : MonoBehaviour
{
    [SerializeField] protected float terminalVelocity;
    [SerializeField] protected float gravAcceleration;
    [SerializeField] protected float horizontalSpeed;
    [SerializeField] protected float jumpVelocity;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float compensation;

    public bool isTouchingGround, isTouchingPlatform;
    public Slope slope = Slope.None;

    public Rigidbody2D RB { get; private set; }
    public CapsuleCollider2D CC { get; private set; }
    public Vector2 ColliderSize { get; private set; }

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
        get => isTouchingGround || isTouchingPlatform;
    }
    public bool IsTouchingSlope
    {
        get => slope != Slope.None;
    }

    protected void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        CC = GetComponent<CapsuleCollider2D>();
        ColliderSize = CC.size;
    }

    protected void FixedUpdate()
    {
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

    protected virtual void AddSlopeCompensation()
    {
        if (slope == Slope.Up)
            newVelocity.y = newVelocity.x;
        else if (slope == Slope.Down)
            newVelocity.y = -newVelocity.x;
    }

    protected void CheckGround()
    {
        Vector2 rayOrigin = transform.position - new Vector3(0, ColliderSize.y / 2 - compensation);
        SlopeCheck(rayOrigin);
        GroundCheck(rayOrigin);
    }
    void GroundCheck(Vector2 rayOrigin)
    {
        //TODO: Refactor grounded check by removing entire groundCollisionComponent and using raycast instead
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, 7);
        if (hit)
        {
            if (hit.normal.x == 0)
                slope = Slope.None;
        }
        else
            slope = Slope.None;
        Debug.DrawRay(rayOrigin, hit.normal, Color.red);
    }
    private void SlopeCheck(Vector2 rayOrigin)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(rayOrigin, Vector2.right, groundCheckDistance, 7);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(rayOrigin, Vector2.left, groundCheckDistance, 7);

        if (slopeHitFront)
            slope = Slope.Up;
        else if (slopeHitBack)
            slope = Slope.Down;
        else
            slope = Slope.None;
        Debug.DrawRay(rayOrigin, slopeHitBack.normal , Color.green);
        Debug.DrawRay(rayOrigin, slopeHitFront.normal, Color.blue);
    }
}
