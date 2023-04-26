using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] float speed = 1;

    public void RollCredits()
    {
        StartCoroutine(RaiseOverTime());
    }

    IEnumerator RaiseOverTime()
    {
        while (true)
        {
            transform.Translate(speed * Time.deltaTime * Vector2.up);
            yield return null;
        }

    }


}
