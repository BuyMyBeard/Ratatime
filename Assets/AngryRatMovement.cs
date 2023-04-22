using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

[RequireComponent(typeof (AngryRatComponent))]
public class AngryRatMovement : MonoBehaviour
{
    public float HorizontalMoveCommand;

    [SerializeField] float PlayerAgroDistance, TargetDeadZone, IdealJumpDistance, PatrolPauseTime, FOV, AgroSpeedMultiplier, JumpTelegraph, Persistance;
    [SerializeField] GameObject PatrolPointA, PatrolPointB;
    

    event EventHandler TargetReached;
    AngryRatComponent rat;
    GameObject player;

    [SerializeField]
    bool isPatroling;
    bool isAggravated => Vector2.Distance(player.transform.position, transform.position) < PlayerAgroDistance;

    
    Vector2 target, launchPoint, landPoint;

    [SerializeField]
    MoveModes mode;
    bool jumpLock;

    private float targetDistance => Vector2.Distance(transform.position, target);

    private void Awake()
    {
        rat = GetComponent<AngryRatComponent>();
        var groundCollisionComponent = GetComponent<GroundedCharacter>();
        player = GameObject.FindGameObjectWithTag("Player");

        transform.position = PatrolPointA.transform.position;
        target = PatrolPointB.transform.position;


        TargetReached += OnTargetReached;
        groundCollisionComponent.OnLand += EndJump;

        StartCoroutine(GiveUp());
    }

    private void Update()
    {
        if (mode == MoveModes.Align)
        {
            Align();
        }
        else if (mode == MoveModes.Jump && !jumpLock)
        {
           Jump();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PatrolPointA.transform.position, TargetDeadZone);
        Gizmos.DrawWireSphere(PatrolPointB.transform.position, TargetDeadZone);
        Gizmos.DrawLine(PatrolPointA.transform.position, PatrolPointB.transform.position);
    }

    public void SetTarget(Vector2 newTarget)
    {
        target = newTarget;
    }
    
    private void OnTargetReached(object sender, EventArgs e)
    {
        Debug.Log("Target Reached");
        if (!isAggravated)
        {
            StartCoroutine(PausePatrol());
        }
        else
        {
            if (target == launchPoint)
            {
                SetTarget(landPoint);
                mode = MoveModes.Jump;
            }
            else
            {
                React();
            }
        }
    }

    private void FindPath()
    {
        var endpoint = FindEndpoint();

        var platform = FindPlatforms(endpoint);

        if (platform == null)
            landPoint = endpoint;
        else
        {
            SetLandPosition(platform);
        }

        SetLaunchPosition();

        SetTarget(launchPoint);
    }

    Vector2 FindEndpoint()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    Collider2D FindPlatforms(Vector2 endpoint)
    {
        LayerMask mask = LayerMask.GetMask("Platform");

        //if (IsDebug)
        //{
        //    Debug.DrawRay(transform.position, (endpoint - new Vector2(transform.position.x, transform.position.y)), Color.green, 10);
        //}

        var hit = Physics2D.CircleCast(transform.position, FOV, endpoint - new Vector2(transform.position.x, transform.position.y), Mathf.Infinity, mask);
        return hit.collider == null ? null : hit.collider;
    }

    void SetLandPosition(Collider2D platform)
    {
        var platformWidth = platform.bounds.size.x;
        var platformHeight = platform.bounds.size.y;
        var center = platform.bounds.center;

        var surface = center.y - platformHeight / 2;

        var leftEdge = new Vector2(center.x - platformWidth / 2, surface);
        var rightEdge = new Vector2(center.x + platformWidth / 2, surface);

        if (Vector2.Distance(transform.position, leftEdge) >= Vector2.Distance(transform.position, rightEdge))
            landPoint = rightEdge;
        else
            landPoint = leftEdge;
        //if (IsDebug)
        //{
        //    var instance = Instantiate(landPointVisualizer, new Vector3(landPoint.x, landPoint.y, 0), Quaternion.identity);
        //    Destroy(instance, 10);
        //}

    }

    void SetLaunchPosition()
    {
        LayerMask mask = LayerMask.GetMask("Platform");
        var currentPlatform = Physics2D.CircleCast(transform.position, 1f, Vector2.down, Mathf.Infinity, mask).collider;

        if (currentPlatform == null)
        {
            mask = LayerMask.GetMask("Ground");
            currentPlatform = Physics2D.CircleCast(transform.position, 1f, Vector2.down, Mathf.Infinity, mask).collider;
        }

        var closestPoint = currentPlatform.bounds.ClosestPoint(landPoint);

        var launchAdjustment = 0f;

        if (closestPoint.x == landPoint.x)
        {
            launchAdjustment = transform.position.x <= closestPoint.x ? -IdealJumpDistance : IdealJumpDistance;
        }
        else
        {
            //TODO: Handle this
        }

        closestPoint.x += launchAdjustment;

        launchPoint = closestPoint;

        //if (IsDebug)
        //{
        //    var instance = Instantiate(launchPointVisualizer, new Vector3(launchPoint.x, launchPoint.y, 0), Quaternion.identity);
        //    Destroy(instance, 10);
        //}

    }

    private void Align()
    {
        jumpLock = false;
        if (target.x > transform.position.x + TargetDeadZone)
            HorizontalMoveCommand = 1;
        else if (target.x < transform.position.x - TargetDeadZone)
        {
            HorizontalMoveCommand = -1;
        }
        else
        {
            HorizontalMoveCommand = 0;
            InvokeTargetReached();
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

    private void InvokeTargetReached()
    {
        if (TargetReached != null)
        {
            TargetReached.Invoke(this, null);
        }
    }

    void React()
    {
        rat.ActualHorizontalSpeed = isAggravated ? rat.horizontalSpeed * AgroSpeedMultiplier : rat.horizontalSpeed;
        if (isAggravated)
        {
            isPatroling = false;
            Debug.Log("Finding Path");
            FindPath();
        }
        else
        {
            isPatroling = true;
            var patrolPA = new Vector2(PatrolPointA.transform.position.x, PatrolPointA.transform.position.y);
            var patrolPB = new Vector2(PatrolPointB.transform.position.x, PatrolPointB.transform.position.y);
            if (target == patrolPA)
            {
                Debug.Log("Patroling to point b");
                target = patrolPB;
            }
            else
            {
                Debug.Log("Patroling to point a");
                target = patrolPA;
            }
        }

        if (jumpLock || targetDistance > IdealJumpDistance)
            mode = MoveModes.Align;
        else
            mode = MoveModes.Jump;

        
    }

    IEnumerator PausePatrol()
    {
        bool waiting = true;
        while (waiting)
        { 
            yield return new WaitForSeconds(PatrolPauseTime);
            waiting = false;
            React();
        }
    }

    IEnumerator GiveUp()
    {
        while (true)
        {
            if (!isPatroling && !jumpLock)
            {
                React();
            }

            yield return new WaitForSeconds(Persistance);
        }
    }

    public enum MoveModes
    {
        Align,
        Jump,
        Patrol
    }
}
