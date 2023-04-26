using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : GroundedCharacter
{
    enum Animations { Stunned, Idle, Running, Jumping, Launch, Raising, Falling, Land };
    [SerializeField] float ascendingDrag = 1;
    [SerializeField] float holdingJumpDrag = 1;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float stunnedGravityScale = 1;
    [SerializeField] PhysicsMaterial2D ragdollPhysics;
    AudioManagerComponent sfx;
    AudioSource audioSource;
    Animations currentAnimation = Animations.Idle;
    readonly Dictionary<Animations, string> dictAnimations = new Dictionary<Animations, string>() 
    {
        { Animations.Stunned, "MCStunned" },
        { Animations.Idle, "MCIdle" },
        { Animations.Running, "MCRun" },
        { Animations.Launch, "MCLaunch" },
        { Animations.Raising, "MCRaising" },
        { Animations.Falling, "MCFalling" },
        { Animations.Land, "MCLand" },
    };

    PlayerInputsComponent inputs;
    public bool inDeathPit = false;
    bool isDroppingPlatform = false, stunned = false;
    bool ended = false;
    float coyoteTimeElapsed = 0;

    public bool IsCoyoteTime
    {
        get => coyoteTimeElapsed < coyoteTime;
    }

    new private void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        inputs = GetComponent<PlayerInputsComponent>();
        sfx = GetComponent<AudioManagerComponent>();
    }

    new private void FixedUpdate()
    {
        if (stunned || ended)
            return;
        
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

    private void SetAnimation(Animations animation)
    {
        if (currentAnimation != animation)
        {
            if (animation == Animations.Running)
                audioSource.Play();
            else
                audioSource.Stop();
                
            animator.Play(dictAnimations[animation]);
            currentAnimation = animation;
        }
    }


    private void SetHorizontalVelocity()
    {
        if (inDeathPit)
            newVelocity.x = 0;
        else
        {
            newVelocity.x = inputs.HorizontalInput * horizontalSpeed;
            if (inputs.HorizontalInput == 0)
            {
                return;
            }
            else if (inputs.HorizontalInput > 0)
                Sprite.flipX = false;
            else
                Sprite.flipX = true;             
        }
    }

    private void Update()
    {
        if (stunned || ended)
            return;
        if (IsGrounded)
        {
            if (Velocity.x != 0)
                SetAnimation(Animations.Running);
            else 
                SetAnimation(Animations.Idle);
        }
        else
        {
            if (currentAnimation == Animations.Launch)
                return;
            if (Velocity.y <= 0)
                SetAnimation(Animations.Falling);
            else
                SetAnimation(Animations.Raising);
        }
    }

    private void CheckInputs()
    {
        if (inputs.JumpPressInput && (IsGrounded || IsCoyoteTime) && !IsJumping && !inDeathPit)
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
    public void TakeKnockBack(Vector2 knockback)
    {
        SetAnimation(Animations.Stunned);
        currentAnimation = Animations.Stunned;
        RB.velocity = knockback;
        RB.gravityScale = stunnedGravityScale;
        stunned = true;
        RB.sharedMaterial = ragdollPhysics;
        sfx.PlaySFX(0);
    }
    public void Recover()
    {
        currentAnimation = Animations.Idle;
        SetAnimation(Animations.Idle);
        RB.gravityScale = 0;
        stunned = false;
    }
    public void End()
    {
        SetAnimation(Animations.Idle);
        Velocity = Vector2.zero;
        ended = true;
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(1);
        float a = 1;
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            a -= 0.10f;
            Color color = Sprite.color;
            color.a = a;
            Sprite.color = color;
        }
    }
}

