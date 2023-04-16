using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    public float MaxTargetDistance;

    [SerializeField] private float TargetDeadZone;

    [SerializeField] private GameObject DebugTargetDisplay;

    [SerializeField] private int ReactionTime = 1;

    [SerializeField] private bool DebugMode = false;

    [SerializeField] private float AttemptJumpRadius;

    [SerializeField] private float JumpTelegraph;

    public int HorizontalMoveCommand;

    private Vector2 target;

    private AngryRatComponent rat;

    private Transform playerTransform;

    private bool jumpLock = false;

    private float targetDistance => Vector2.Distance(transform.position, target);

    private MoveModes mode;

    private GroundCollisionComponent groundCollisionComponent;

    private void Awake()
    {
        rat = GetComponent<AngryRatComponent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        groundCollisionComponent = GetComponentInChildren<GroundCollisionComponent>();
        groundCollisionComponent.OnLand += EndJump;
        StartCoroutine(SearchForTarget());
    }

    IEnumerator SearchForTarget()
    {
        while (true)
        {
            if (!jumpLock)
                React();
            yield return new WaitForSeconds(ReactionTime);
        }
    }

    void SetTarget(Vector2 t)
    {
        target = t;
        if (DebugMode)
        {
            var debug = Instantiate(DebugTargetDisplay, new Vector3(t.x, t.y, 0), Quaternion.identity);
            GameObject.Destroy(debug, ReactionTime);
        }
    }

    void Update()
    {
        if (mode == MoveModes.Align)
        {
            Align();
        }
        else if (mode == MoveModes.Jump && !jumpLock)
        {
            Jump();
        }

        Debug.Log(mode);
    }

    void Align()
    {
        // Unlock jump once aligned again
        jumpLock = false;

        if (target.x > transform.position.x + TargetDeadZone)
            HorizontalMoveCommand = 1;
        else if (target.x < transform.position.x - TargetDeadZone)
        {
            HorizontalMoveCommand = - 1;
        }
        else
        {
            // React when the target is reached
            if (Vector2.Distance(target, transform.position) < TargetDeadZone)
            {
                React();
            }

            HorizontalMoveCommand = 0;
        }
    }

    async void Jump()
    {
        jumpLock = true;

        await Task.Delay((int)(JumpTelegraph * 100));

        if (target.x > transform.position.x)
            HorizontalMoveCommand = 1;
        else if (target.x < transform.position.x)
            HorizontalMoveCommand = -1;
        else
            HorizontalMoveCommand = 0;

        rat.AttempingJump = true;
    }

    void EndJump(object sender, EventArgs e)
    {
        if (jumpLock)
        {
            rat.AttempingJump = false;
            React();
        }
    }

    private void React()
    {
        if (rat.Aggravated)
        {
            SetTarget(playerTransform.position);
        }
        else
        {
            // TODO: Implement wandering
        }

        if (jumpLock || targetDistance > AttemptJumpRadius)
            mode = MoveModes.Align;
        else
            mode = MoveModes.Jump;
    }

    enum MoveModes
    {
        Align,
        Jump
    }
}
