using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class AnimatedDoorComponent : MonoBehaviour, ITimeShifter
{
    Animator animator;
    bool future;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void ShiftToFuture()
    {
        future = true;
        animator.Play("Future");
    }

    public void ShiftToPast()
    {
        future = false;
        animator.Play("Past");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (future)
            animator.Play("FutureDoor");
        else
            animator.Play("PastDoor");
        //TODO: trigger end game
        Debug.Log("YOU WON");
    }


}
