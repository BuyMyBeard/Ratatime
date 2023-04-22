using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageComponent : MonoBehaviour
{
    SpriteRenderer sprite;
    [SerializeField] float stunnedIFramesTime = 3;
    [SerializeField] float stunnedTime = 2;
    [SerializeField] Vector2 launchDirection;
    [SerializeField] float launchSpeed;
    PlayerMoveComponent playerMove;

    bool isInvulnerable;
    private void Awake()
    {
        launchDirection = launchDirection.normalized;
        playerMove = GetComponent<PlayerMoveComponent>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvulnerable && collision.gameObject.layer == 3)
        {
            StartCoroutine(TakeKnockback(collision));
            StartCoroutine(BecomeInvincible(stunnedIFramesTime));
        }
    }

    IEnumerator TakeKnockback(Collider2D collision)
    {
        float deltaX = transform.position.x - collision.transform.position.x;
        Vector2 launchVector = launchDirection * launchSpeed;
        if (deltaX >= 0)
            launchVector.x = -launchVector.x;
        playerMove.TakeKnockBack(launchVector);
        yield return new WaitForSeconds(stunnedTime);
        playerMove.Recover();
    }

    public IEnumerator BecomeInvincible(float time)
    {
        Physics.IgnoreLayerCollision(9, 3, true);
        isInvulnerable = true;
        Color color = sprite.color;
        color.a = 0.5f;
        sprite.color = color;
        yield return new WaitForSeconds(time);
        color.a = 1;
        sprite.color = color;
        isInvulnerable = false;
        Physics.IgnoreLayerCollision(9, 3, false);
    }
    public void TriggerInvincibility(float time)
    {
        StartCoroutine(BecomeInvincible(time));
    }

}
