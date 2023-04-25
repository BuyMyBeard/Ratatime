using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShifter : MonoBehaviour, ITimeShifter
{
    [SerializeField] Sprite pastSprite, futureSprite;
    SpriteRenderer sprite;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void ShiftToFuture()
    {
        sprite.sprite = futureSprite;
    }

    public void ShiftToPast()
    {
        sprite.sprite = pastSprite;
    }


    
}
