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

    public bool IsTouchingGround
    {
        set
        {
            IsGrounded = value;
        }
    }
    public bool IsTouchingPlatform
    {
        set
        {
            IsGrounded = value && !inputs.DropInput;
        }
    }
    public bool IsGrounded { get; private set; } = false;

    [SerializeField] float gravitationalAcceleration = -1;
    [SerializeField] float initialJumpVerticalVelocity = 1;
    [SerializeField] float terminalFallingSpeed = -1;
    [SerializeField] float horizontalSpeed = 1;

    Vector2 velocity = Vector2.zero;

    private void Update()
    {

        float horizontalVelocity = inputs.HorizontalInput * horizontalSpeed;

        float verticalVelocity = 0;
        if (!IsGrounded)
            verticalVelocity = velocity.y + gravitationalAcceleration * Time.deltaTime;
        if (inputs.JumpInput && IsGrounded)
            verticalVelocity = initialJumpVerticalVelocity;

        if (verticalVelocity < terminalFallingSpeed)
            verticalVelocity = terminalFallingSpeed;

        velocity = new Vector2(horizontalVelocity, verticalVelocity);
        transform.Translate(Time.deltaTime * velocity);
        //Debug.Log($"isGrounded: {IsGrounded}   velocity:({velocity.x},{velocity.y})   jumping:{jumpInput}");
        
    }
}

