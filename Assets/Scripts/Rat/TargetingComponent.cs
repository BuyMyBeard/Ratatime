using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    #region Public Fields
    public float MaxTargetDistance;

    public int HorizontalMoveCommand;
    #endregion

    #region Serialized Fields
    [SerializeField] private float TargetDeadZone;

    [SerializeField] private GameObject DebugTargetDisplay;

    [SerializeField] private int ReactionTime = 1;

    [SerializeField] private bool DebugMode = false;

    [SerializeField] private float AttemptJumpRadius;

    [SerializeField] private float JumpTelegraph;
    #endregion

    #region Private Fields
    public Vector2 target;

    private AngryRatComponent rat;

    private Transform playerTransform;

    private bool jumpLock = false;

    private float targetDistance => Vector2.Distance(transform.position, target);

    public MoveModes mode;

    public event EventHandler ReachedTarget;

    Pathfinder pathfinder;
    #endregion

    #region Unity Methods

    void Awake()
    {
        rat = GetComponent<AngryRatComponent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        var groundCollisionComponent = GetComponent<GroundedCharacter>();
        groundCollisionComponent.OnLand += EndJump;
        pathfinder = GetComponent<Pathfinder>();
        StartCoroutine(SearchForTarget());
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
    #endregion

    #region Coroutines
    IEnumerator SearchForTarget()
    {
        while (true)
        {
            if (!jumpLock)
                React();
            yield return new WaitForSeconds(ReactionTime);
        }
    }
    #endregion

    #region Private Methods
    public void SetTarget(Vector2 t)
    {
        target = t;
        if (DebugMode)
        {
            var debug = Instantiate(DebugTargetDisplay, new Vector3(t.x, t.y, 0), Quaternion.identity);
            GameObject.Destroy(debug, ReactionTime);
        }
    }

    public void InvokeTargetReached ()
    {
        if (ReachedTarget != null)
            ReachedTarget.Invoke(this, null);
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
            if (Vector2.Distance(target, transform.position) < TargetDeadZone)
            {
                
                React();
            }
            InvokeTargetReached();
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

    void React()
    {

        if (rat.Aggravated)
        {
            pathfinder.FindPath();
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

    #endregion

    #region Enums
    public enum MoveModes
    {
        Align,
        Jump
    }
    #endregion
}
