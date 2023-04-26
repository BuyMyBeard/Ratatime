using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public int Time;

    public bool IsFuture = false;

    public GameplayStates State;

    public event EventHandler TimeChanged;

    [SerializeField]
    private int SwitchInterval;

    [SerializeField]
    Color PastSkyBox, FutureSkyBox;

    private int timeUntilSwitch;

    private Camera cam;

    List<ITimeShifter> timeShifters = new List<ITimeShifter>();

    void Start()
    {
        timeShifters = FindObjectsOfType<MonoBehaviour>().OfType<ITimeShifter>().ToList();
        timeShifters.ForEach(t => t.ShiftToPast());
        // Assuming there is never more then one camera in the scene
        cam = FindObjectOfType<Camera>();

        if (IsFuture)
            timeShifters.ForEach(t => t.ShiftToFuture());
        else
            timeShifters.ForEach(t => t.ShiftToPast());

        timeUntilSwitch = SwitchInterval;
        StartCoroutine(TickDownTime());
        cam.backgroundColor = IsFuture ? FutureSkyBox : PastSkyBox;
    }

    IEnumerator TickDownTime()
    {
        while (true)
        {
            // Tick the timers
            if (this.State == GameplayStates.Playing)
            {
                Time--;
                timeUntilSwitch--;
                if (TimeChanged != null)
                {
                    TimeChanged.Invoke(IsFuture, null);
                }
            }

            // Check to see if the state has changed
            if (Time <= 0)
                this.State = GameplayStates.Lost;

            if (timeUntilSwitch <= 0)
                Switch();

            yield return new WaitForSeconds(1);
        }
    }
   
    void Switch()
    {
        this.IsFuture = !this.IsFuture;
        if (IsFuture)
            timeShifters.ForEach(t => t.ShiftToFuture());
        else 
            timeShifters.ForEach(t => t.ShiftToPast());
        this.timeUntilSwitch = SwitchInterval;

        cam.backgroundColor = IsFuture ? FutureSkyBox : PastSkyBox;
    }

    public enum GameplayStates
    {
        Playing,
        Lost,
        Won
    }

    public void AddTime(int count)
    {
        Time += count;
    }
}

