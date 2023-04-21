using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageComponent : MonoBehaviour
{
    SpriteRenderer sprite;
    [SerializeField] float iFramesLength;
    bool isInvulnerable;
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvulnerable && collision.gameObject.layer == 3)
        {
            //StartCoroutine(TakeKnockback());
            StartCoroutine(BecomeInvincible());
        }
    }

    //IEnumerator TakeKnockback()
    //{
    //    return null;
    //}

    public IEnumerator BecomeInvincible()
    {
        isInvulnerable = true;
        Color color = sprite.color;
        color.a = 0.5f;
        sprite.color = color;
        yield return new WaitForSeconds(iFramesLength);
        color.a = 1;
        sprite.color = color;
        isInvulnerable = false;
    }

}
