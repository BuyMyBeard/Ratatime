using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : MonoBehaviour
{
    PlayerInputsComponent inputs;

    private void Awake()
    {
        inputs = GetComponent<PlayerInputsComponent>();
    }
    public bool IsTouchingGround { get; set; }
    public bool IsTouchingPlatform { get; set; }

    public bool IsFalling
    {
        get
        {
            return velocity.y < 0;
        }
    }
    public bool IsJumping
    {
        get
        {
            return velocity.y > 0;
        }
    }
    public bool IsGrounded
    {
        get => IsTouchingGround || IsTouchingPlatform;
    }

    [SerializeField] float gravAcceleration = -5;
    [SerializeField] float ascendingDrag = 1;
    [SerializeField] float holdingJumpDrag = 1;
    [SerializeField] float initialJumpVerticalVelocity = 1;
    [SerializeField] float terminalFallingSpeed = -1;
    [SerializeField] float horizontalSpeed = 1;

    Vector2 velocity = Vector2.zero;

    private void Update()
    {
        velocity.x = inputs.HorizontalInput * horizontalSpeed;

        if (IsGrounded)
            velocity.y = 0;
        else
            velocity.y += gravAcceleration * Time.deltaTime;
        
        
        if (IsJumping)
        {
            velocity.y += ascendingDrag * Time.deltaTime;
            if (inputs.JumpInput)
                velocity.y += holdingJumpDrag * Time.deltaTime;
        }       


        if (inputs.JumpInput && IsGrounded)
            velocity.y = initialJumpVerticalVelocity;
        if (velocity.y < terminalFallingSpeed)
            velocity.y = terminalFallingSpeed;


        //can be optimized by exposing InputActions and tying events to them
        if (inputs.DropInput && IsTouchingPlatform)
        {
            IsTouchingPlatform = false;
        }
        transform.Translate(Time.deltaTime * velocity);
        Debug.Log($"isGrounded: {IsGrounded}   velocity:({velocity.x},{velocity.y})   jumping:{inputs.JumpInput}");
        
    }
}

