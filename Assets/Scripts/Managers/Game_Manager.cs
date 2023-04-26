using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Game_Manager : MonoBehaviour
{
    [SerializeField]
    VideoClip GlitchClip;

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

    private VideoPlayer player;

    List<ITimeShifter> timeShifters = new List<ITimeShifter>();

    void Start()
    {

        player = GetComponent<VideoPlayer>();
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

        this.player.loopPointReached += Player_loopPointReached;
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
        this.player.clip = GlitchClip;
        this.player.targetCamera = cam;
        this.player.Play();

        this.IsFuture = !this.IsFuture;
        if (IsFuture)
            timeShifters.ForEach(t => t.ShiftToFuture());
        else 
            timeShifters.ForEach(t => t.ShiftToPast());
        this.timeUntilSwitch = SwitchInterval;

        cam.backgroundColor = IsFuture ? FutureSkyBox : PastSkyBox;
    }

    private void Player_loopPointReached(VideoPlayer source)
    {
        source.Stop();
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

    public void End()
    {
        StopAllCoroutines();
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2);
        timeShifters.ForEach(t => t.ShiftToPast());

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(5);
            timeShifters.ForEach(t => t.ShiftToFuture());
            yield return new WaitForSeconds(0.5f);
            timeShifters.ForEach(t => t.ShiftToPast());
        }
    }
    
}

