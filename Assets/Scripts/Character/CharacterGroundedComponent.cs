using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGroundedComponent : MonoBehaviour
{
    PlayerMoveComponent playerMove;
    float characterHeight;
    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMoveComponent>();
        characterHeight = transform.parent.GetComponentInChildren<SpriteRenderer>().bounds.size.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        //transform into switch case if there are more cases to manage
        if (collision.gameObject.layer == 6)                                            
            playerMove.IsTouchingPlatform = true;
        else if (collision.gameObject.layer == 7)
            playerMove.IsTouchingGround = true;
        playerMove.transform.position = collision.ClosestPoint(playerMove.transform.position) + new Vector2(0, characterHeight / 2);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
            playerMove.IsTouchingPlatform = false;
        else if (collision.gameObject.layer == 7)
            playerMove.IsTouchingGround = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
            playerMove.IsTouchingPlatform = true;
    }
}
