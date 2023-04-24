using System;
using System.Collections;
using UnityEngine;

public enum GroundedCharacterAnimations { Idle, Walking, Jumping, Raising, Falling, Landing }
public enum Slope { Down, Up, None };

public abstract class GroundedCharacter : MonoBehaviour
{
    [SerializeField] protected float terminalVelocity = -6;
    [SerializeField] protected float gravAcceleration = -15;
    [SerializeField] public float horizontalSpeed = 7;
    [SerializeField] protected float jumpVelocity = 12;
    [SerializeField] protected float groundCheckDistance = 0.1f;
    [SerializeField] protected float compensation;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask platformLayer;
    [SerializeField] protected PhysicsMaterial2D noFriction;
    [SerializeField] protected PhysicsMaterial2D highFriction;
    [SerializeField] protected float SlopeUpCompensation = 0.05f;
    AudioManagerComponent audioManager;

    protected bool isTouchingGround, isTouchingPlatform;
    protected Animator animator;
    public Slope slope = Slope.None;
    protected SpriteRenderer Sprite { get; private set; }
    public Rigidbody2D RB { get; protected set; }
    public CapsuleCollider2D CC { get; protected set; }
    public Vector2 ColliderSize { get; protected set; }

    public event EventHandler OnLand;

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
        Sprite = GetComponent<SpriteRenderer>();
        ColliderSize = CC.size * transform.localScale;
        audioManager = GetComponent<AudioManagerComponent>();
        animator = GetComponent<Animator>();
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
        {
            newVelocity.y = 0;
            RB.sharedMaterial = highFriction;
        }
        else
        {
            newVelocity.y += gravAcceleration * Time.deltaTime;
            RB.sharedMaterial = noFriction;
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
        Vector2 rayOrigin = transform.position - new Vector3(0, ColliderSize.y / 2) + (Vector3)(CC.offset * transform.localScale);
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
            InvokeOnLand();
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
        //if (this is AngryRatComponent)
        //{
        //    Debug.Log($"({rayOrigin.x}, {rayOrigin.y}), {groundCheckDistance}");
        //}
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

    void InvokeOnLand()
    {
        if (OnLand != null)
            OnLand.Invoke(this, null);
    }
}
