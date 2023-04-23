using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatTimeShiftComponent : MonoBehaviour, ITimeShifter
{
    GameObject pastRat, futureRat;


    private void Awake()
    {
        pastRat = GetComponentInChildren<FriendlyRatComponent>().gameObject;
        futureRat = GetComponentInChildren<AngryRatComponent>().gameObject;
    }
    public void ShiftToPast()
    {
        pastRat.SetActive(true);
        pastRat.transform.position = futureRat.transform.position;
        futureRat.SetActive(false);
    }
    public void ShiftToFuture()
    {
        pastRat.SetActive(false);
        futureRat.transform.position = pastRat.transform.position;
        futureRat.SetActive(true);
    }
}
