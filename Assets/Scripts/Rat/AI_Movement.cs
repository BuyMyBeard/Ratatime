using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    // Outputs
        // Horizontal Move Command
        // Jump Command
    public float HorizontalMoveCommand;
    public bool JumpCommand;

    // Objectives
        // Point A
        // Point B
        // Player
    private Vector2 objective;
    [SerializeField] GameObject PointA, PointB, Player;

    // Movements
        // Align
        // Jump
    private Movements currentMovement;

    enum Movements
    {
        Align,
        Jump
    }

    void Align()
    {
        if (objective != null && currentMovement == Movements.Align)
        {
            if (transform.position.x < objective.x)
            {
                HorizontalMoveCommand = 1;
            }
            else if (transform.position.x > objective.x)
            {
                HorizontalMoveCommand -= 1;
            }
            else
            {
                HorizontalMoveCommand = 0;
            }
        }
    }

    private void Update()
    {
        Align();   
    }
}
