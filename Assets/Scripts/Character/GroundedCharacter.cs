using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Windows;
using Unity.Mathematics;
using UnityEditor.Build.Content;

public enum Slope { Down, Up, None };

public abstract class GroundedCharacter : MonoBehaviour
{
    [SerializeField] protected float terminalVelocity;
    [SerializeField] protected float gravAcceleration;
    [SerializeField] protected float horizontalSpeed;
    [SerializeField] protected float jumpVelocity;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float compensation;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask platformLayer;
    [SerializeField] protected PhysicsMaterial2D noFriction;
    [SerializeField] protected PhysicsMaterial2D highFriction;

    [SerializeField] protected bool wallOnLeft;
    [SerializeField] protected bool wallOnRight;
    [SerializeField] protected bool ceiling;

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
        get => !IsGrounded && Velocity.y < 0;
    }
    public bool IsJumping
    {
        get => !IsGrounded && Velocity.y > 0;
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

    protected virtual void FloorCheck()
    {
        Vector2 rayOrigin = transform.position - new Vector3(0, ColliderSize.y / 2);
        SlopeCheck(rayOrigin);
        GroundCheck(rayOrigin);
    }
    protected void GroundCheck(Vector2 rayOrigin)
    {
        bool wasGrounded = IsGrounded;
        RaycastHit2D groundHit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D platformHit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, platformLayer);
        isTouchingGround = !IsJumping && groundHit;
        isTouchingPlatform = !IsJumping && platformHit;

        if (!wasGrounded && IsGrounded)
        {
            if (isTouchingGround)
                transform.Translate(0, -groundHit.distance, 0);
            else
                transform.Translate(0, -platformHit.distance, 0);
        }
        if (!IsGrounded)
            slope = Slope.None;
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);
    }
    private void SlopeCheck(Vector2 rayOrigin)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(rayOrigin, Vector2.right, groundCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(rayOrigin, Vector2.left, groundCheckDistance, groundLayer);
        if (slopeHitBack && slopeHitFront)
        {
            slope = Slope.None;
            //fix by finding the closest point to the ground that
            //won't interact with front and back colliders and translate there in 1 instruction instead
            //doing so will make it smoother
            transform.Translate(Vector2.up * compensation);
        }
        else if (slopeHitFront)
            slope = Slope.Up;
        else if (slopeHitBack)
            slope = Slope.Down;
        else
            slope = Slope.None;
        Debug.DrawRay(rayOrigin,Vector2.right * groundCheckDistance, Color.green);
        Debug.DrawRay(rayOrigin, Vector2.left * groundCheckDistance, Color.blue);
    }
    protected void WallCheck()
    {
        float xOffset = ColliderSize.x / 2;
        float yOffset = ColliderSize.y / 2;
        wallOnLeft = Physics2D.BoxCast(
            new Vector2(transform.position.x - xOffset, transform.position.y),
            new Vector2(0.01f, ColliderSize.y),
            0, Vector2.left, groundCheckDistance, groundLayer);
        wallOnRight = Physics2D.BoxCast(
            new Vector2(transform.position.x + xOffset, transform.position.y),
            new Vector2(0.01f, ColliderSize.y),
            0, Vector2.right, groundCheckDistance, groundLayer);
        ceiling = Physics2D.BoxCast(
            new Vector2(transform.position.x, transform.position.y + yOffset),
            new Vector2(ColliderSize.x, 0.01f),
            0, Vector2.up, groundCheckDistance, groundLayer);
    }
}
