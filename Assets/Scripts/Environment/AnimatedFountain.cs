using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedFountain : MonoBehaviour, ITimeShifter
{
    [SerializeField] bool animatedInFuture = true;
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void ShiftToFuture()
    {
        if (animatedInFuture)
            animator.Play("Future1");
        else
            animator.Play("Future2");
    }

    public void ShiftToPast()
    {
        animator.Play("Past");
    }
}
