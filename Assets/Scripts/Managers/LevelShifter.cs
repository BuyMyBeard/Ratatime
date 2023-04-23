using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelShifter : MonoBehaviour, ITimeShifter
{
    GameObject pastLevel, futureLevel;

    // Start is called before the first frame update
    void Awake()
    {
        pastLevel = GameObject.FindGameObjectWithTag("Past");
        futureLevel = GameObject.FindGameObjectWithTag("Future");
    }
    public void ShiftToPast()
    {
        pastLevel.SetActive(true);
        futureLevel.SetActive(false);
    }
    public void ShiftToFuture()
    {
        pastLevel.SetActive(false);
        futureLevel.SetActive(true);
    }
}
