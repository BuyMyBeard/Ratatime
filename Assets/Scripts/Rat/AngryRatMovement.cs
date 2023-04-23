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
    #region Fields
    [SerializeField]
    bool isPatroling = true;

    [SerializeField]
    bool isJumpLocked, isWaitLocked;

    [SerializeField]
    Movements currentMovement;

    [SerializeField]
    AngryRatComponent angryRatComponent;

    [SerializeField]
    private GameObject objective, Player;
    #endregion

    #region Engine Methods
    private void Update()
    {
        angryRatComponent.ActualHorizontalSpeed = objective == Player ? angryRatComponent.horizontalSpeed * AgroSpeedMultiplier : angryRatComponent.horizontalSpeed;
        currentMovement = CheckJump();
        Align();
        Jump();
    }

    private void Awake()
    {
        angryRatComponent = GetComponent<AngryRatComponent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        angryRatComponent.OnLand += EndJump;
        objective = PointA;
        

        StartCoroutine("SearchForObjective");
    }

    private void OnEnable()
    {        
        StartCoroutine("SearchForObjective");
        isJumpLocked = false;
        isWaitLocked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(PointA.transform.position, AlignmentDeadzone);
        Gizmos.DrawWireSphere(PointB.transform.position, AlignmentDeadzone);
        Gizmos.DrawLine(PointA.transform.position, PointB.transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LeftBound.transform.position, AlignmentDeadzone);
        Gizmos.DrawLine(LeftBound.transform.position + (Vector3.down * 10), LeftBound.transform.position + (Vector3.up * 10));
        Gizmos.DrawWireSphere(RightBound.transform.position, AlignmentDeadzone);
        Gizmos.DrawLine(RightBound.transform.position + (Vector3.down * 10), RightBound.transform.position + (Vector3.up * 10));
    }
    #endregion

    #region Serialized Fields
    [Tooltip("The rat will patrol to this point")]
    [SerializeField] GameObject PointA, PointB;

    [Tooltip("The rat will jump if the player is within the horizontal offset, and above the vertical offset.")]
    [SerializeField] float HorizontalJumpOffset, VerticalJumpOffset;

    [Tooltip("Keep this a small value")]
    [SerializeField] float AlignmentDeadzone;

    //Outerbounds
    [Tooltip("If the player goes inside the bounds, the rat will agro")]
    [SerializeField] GameObject LeftBound, RightBound;

    [Tooltip("How often in seconds the rat checks for the player while patroling")]
    [SerializeField] float ObjectiveSearchTime;

    [Tooltip("How long in seconds the rat waits at a point while patroling")]
    [SerializeField] float PointWaitTime;

    [Tooltip("How long in seconds the rat waits before jumping")]
    [SerializeField] float JumpTelegraph;

    [Tooltip("How much faster the rat runs when attacking")]
    [SerializeField] float AgroSpeedMultiplier;
    #endregion

    #region Private Methods

    void Align()
    {

        if (objective != null && currentMovement == Movements.Align)
        {
            if (transform.position.x < objective.transform.position.x - AlignmentDeadzone)
            {
                angryRatComponent.HorizontalMoveCommand = 1;
            }
            else if (transform.position.x > objective.transform.position.x + AlignmentDeadzone)
            {
                angryRatComponent.HorizontalMoveCommand = -1;
            }
            else
            {
                angryRatComponent.HorizontalMoveCommand = 0;
                if (isPatroling)
                {
                    PatrolPointReached();
                }
            }
        }
    }

    void Jump()
    {
        if (currentMovement == Movements.Jump)
        {
            if (!isJumpLocked)
            {
                isJumpLocked = true;

                // Stop the rat while they wait
                angryRatComponent.HorizontalMoveCommand = 0;

                StartCoroutine("WaitForJump");                
            }
        }
    }

    void EndJump(object sender, EventArgs e)
    {
        if (isJumpLocked)
        {
            isJumpLocked = false;
            angryRatComponent.JumpCommand = false;
        }
    }

    Movements CheckJump()
    {
        if (objective.transform.position.y > transform.position.y + VerticalJumpOffset)
        {
            var jumpBoundLeft = transform.position.x - HorizontalJumpOffset;
            var jumpBoundRight = transform.position.x + HorizontalJumpOffset;
            var objectivex = objective.transform.position.x;

            if (objectivex > jumpBoundLeft && objectivex < jumpBoundRight)
            {
                return Movements.Jump;
            }
        }

        if (isJumpLocked)
        {
            return Movements.Jump;
        }

        return Movements.Align;
    }

    void GetObjective()
    {
        if (Player.transform.position.x > LeftBound.transform.position.x && Player.transform.position.x < RightBound.transform.position.x)
        {
            isPatroling = false;
            objective = Player;
        }
        else
        {
            if (objective == Player)
            {
                isPatroling = true;
                objective = PointA;
            }
        }
    }

    void PatrolPointReached()
    {
        if (!isWaitLocked)
        {
            isWaitLocked = true;
            StartCoroutine("WaitAtPoint");
        }
    }
    #endregion

    #region Coroutines
    IEnumerator SearchForObjective()
    {
        while (true)
        {

            GetObjective();
            
            yield return new WaitForSeconds(ObjectiveSearchTime);
        }
    }

    IEnumerator WaitForJump()
    {
        yield return new WaitForSeconds(JumpTelegraph);

        angryRatComponent.JumpCommand = true;

        if (objective.transform.position.x > transform.position.x)
            angryRatComponent.HorizontalMoveCommand = 1;
        else if (objective.transform.position.x < transform.position.x)
            angryRatComponent.HorizontalMoveCommand = -1;
        else
            angryRatComponent.HorizontalMoveCommand = 0;
    }

    IEnumerator WaitAtPoint()
    {
        yield return new WaitForSeconds(PointWaitTime);

        if (objective == PointA)
        {
            objective = PointB;
        }
        else if (objective == PointB)
        {
            objective = PointA;
        }
        isWaitLocked = false;
    }
    #endregion

    #region Enums
    enum Movements
    {
        Align,
        Jump
    }
    #endregion
}
