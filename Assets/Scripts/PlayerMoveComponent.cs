using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    InputAction moveAction, jumpAction, dropAction;
    float movement = 0;


    private void Awake()
    {
        InputMap inputmap = new InputMap();
        moveAction = inputmap.FindAction("Move");
        jumpAction = inputmap.FindAction("Jump");
        dropAction = inputmap.FindAction("Drop");

    }
    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += (InputAction.CallbackContext ctx) =>
        {
            movement = ctx.ReadValue<float>();
            Debug.Log(movement);
        };
        moveAction.canceled += _ =>
        {
            movement = 0;
            Debug.Log(movement);
        };
    }
    private void OnDisable()
    {
        moveAction.performed -= (InputAction.CallbackContext ctx) =>
        {
            movement = ctx.ReadValue<float>();
        };
        moveAction.canceled -= _ =>
        {
            movement = 0;
        };
    }
    private void Update()
    {
        transform.Translate(movement * speed * Time.deltaTime, 0, 0);
    }
}
