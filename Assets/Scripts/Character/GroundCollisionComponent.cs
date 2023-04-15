using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollisionComponent : MonoBehaviour
{
    GroundedCharacter characterMovement;
    float characterHeight;
    private void Awake()
    {
        characterMovement = GetComponentInParent<PlayerMoveComponent>();
        characterHeight = transform.parent.GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        //transform into switch case if there are more cases to manage
        if (collision.gameObject.layer == 6 && characterMovement.IsFalling)
        {
            characterMovement.IsTouchingPlatform = true;
            characterMovement.transform.position = collision.ClosestPoint(characterMovement.transform.position) + new Vector2(0, characterHeight / 2);
        }                                            
        else if (collision.gameObject.layer == 7 && characterMovement.IsFalling)
        {
            characterMovement.IsTouchingGround = true;
            characterMovement.transform.position = collision.ClosestPoint(characterMovement.transform.position) + new Vector2(0, characterHeight / 2);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            characterMovement.IsTouchingPlatform = false;
            if (characterMovement is PlayerMoveComponent)
            (characterMovement as PlayerMoveComponent).ResetCoyoteTime();
        }
        else if (collision.gameObject.layer == 7)
        {
            characterMovement.IsTouchingGround = false;
            if (characterMovement is PlayerMoveComponent)
                (characterMovement as PlayerMoveComponent).ResetCoyoteTime();
        }
    }
}
