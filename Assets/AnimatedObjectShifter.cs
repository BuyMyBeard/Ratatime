using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObjectShifter : MonoBehaviour, ITimeShifter
{
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void ShiftToFuture()
    {
        animator.Play("Future");
    }

    public void ShiftToPast()
    {
        animator.Play("Past");
    }


   
}
