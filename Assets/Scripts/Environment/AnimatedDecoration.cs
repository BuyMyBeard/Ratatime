using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimatedDecoration : MonoBehaviour, ITimeShifter
{
    enum Type { Leak1, Leak2, Leak3, Bubble1, Bubble2, Froth1, Froth2 }
    [SerializeField] Type decorationType = Type.Leak1;
    [SerializeField] bool random;
    [SerializeField] float minCooldown;
    [SerializeField] float maxCooldown;
    [SerializeField] bool appearInPast = false;
    [SerializeField] Color32 colorInPast;

    SpriteRenderer sprite;
    Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        if (random)
            StartCoroutine(PlayRandomly());
        else
            StartCoroutine(PlayAtInterval());
    }

    public void ShiftToPast()
    {
        if (appearInPast)
            sprite.color = colorInPast;
        else
            sprite.enabled = false;
    }

    public void ShiftToFuture()
    {
        if (appearInPast)
            sprite.color = new Color(255, 255, 255, 255);
        else
            sprite.enabled = true;
    }
    IEnumerator PlayRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minCooldown, maxCooldown));
            animator.Play(decorationType.ToString(), -1, 0f);
        }
    }

    IEnumerator PlayAtInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(minCooldown);
            animator.Play(decorationType.ToString(), -1, 0f);
        }
    }
}