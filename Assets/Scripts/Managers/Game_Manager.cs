using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public int Time;

    public bool IsFuture;

    public GameplayStates State;

    public event EventHandler TimeChanged;

    [SerializeField]
    private int SwitchInterval;

    private int timeUntilSwitch;

    private Camera cam;

    void Start()
    {
        // Assuming there is never more then one camera in the scene
        cam = FindObjectOfType<Camera>();

        StartCoroutine(TickDownTime());

        timeUntilSwitch = SwitchInterval;
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
        this.timeUntilSwitch = SwitchInterval;

        // TODO: Remove this code
        cam.backgroundColor = IsFuture ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.4f, 0.4f, 0.4f);
    }

    public enum GameplayStates
    {
        Playing,
        Lost,
        Won
    }
}

