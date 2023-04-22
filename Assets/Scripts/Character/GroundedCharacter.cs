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
public enum GroundedCharacterAnimations { Idle, Walking, Jumping, Raising, Falling, Landing }
public enum Slope { Down, Up, None };

public abstract class GroundedCharacter : MonoBehaviour
{
    [SerializeField] protected float terminalVelocity;
    [SerializeField] protected float gravAcceleration;
    [SerializeField] protected float horizontalSpeed;
    [SerializeField] protected float jumpVelocity;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected float compensation;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask platformLayer;
    [SerializeField] protected PhysicsMaterial2D noFriction;
    [SerializeField] protected PhysicsMaterial2D highFriction;
    [SerializeField] protected float SlopeUpCompensation = 0.05f;
    AudioManagerComponent audioManager;

    protected bool isTouchingGround, isTouchingPlatform;
    protected Animator animator;
    private Slope slope = Slope.None;
    protected SpriteRenderer Sprite { get; private set; }
    public Rigidbody2D RB { get; protected set; }
    public CapsuleCollider2D CC { get; protected set; }
    public Vector2 ColliderSize { get; protected set; }


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

    public bool IsWalking
    {
        get => IsGrounded && Velocity.x != 0;
    }

    protected void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        CC = GetComponent<CapsuleCollider2D>();
        Sprite = GetComponentInChildren<SpriteRenderer>();
        ColliderSize = CC.size;
        audioManager = GetComponent<AudioManagerComponent>();
        //animator = GetComponent<Animator>();
    }

    protected void FixedUpdate()
    {
        newVelocity = Velocity;
        FloorCheck();
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
        else if (!IsGrounded)
            slope = Slope.None;
        //supposed to compensate for moving up slope, but breaks a bunch of stuff
        //else if (isTouchingGround && slope == Slope.None && groundHit.distance != 0)
        //    transform.Translate(0, -groundHit.distance, 0);

        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, Color.red);
    }
    private void SlopeCheck(Vector2 rayOrigin)
    {
        Slope oldSlope = slope;
        RaycastHit2D slopeHitRight = Physics2D.Raycast(rayOrigin, Vector2.right, groundCheckDistance, groundLayer);
        RaycastHit2D slopeHitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, groundCheckDistance, groundLayer);
        if (slopeHitLeft == slopeHitRight) //XNOR
            slope = Slope.None;
        else if (slopeHitRight)
            slope = Slope.Up;
        else //slopeHitBack
            slope = Slope.Down;

        if (oldSlope != Slope.None && slope == Slope.None && IsGrounded)
            StartCoroutine(CompensateForSlopeUp());
        Debug.DrawRay(rayOrigin,Vector2.right * groundCheckDistance, Color.green);
        Debug.DrawRay(rayOrigin, Vector2.left * groundCheckDistance, Color.blue);
    }

    IEnumerator CompensateForSlopeUp()
    {
        yield return new WaitForSeconds(SlopeUpCompensation);
        Vector2 rayOrigin = transform.position - new Vector3(0, ColliderSize.y / 2);
        RaycastHit2D groundHit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);
        if (groundHit && IsGrounded && slope == Slope.None)
        {
            transform.Translate(0, -groundHit.distance, 0);
        }
    }
}
