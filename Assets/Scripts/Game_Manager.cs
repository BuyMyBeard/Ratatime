using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public int Time;

    public bool IsFuture;

    public GameplayStates State = GameplayStates.Playing;

    public Text CountDownTimer;

    [SerializeField]
    private int SwitchInterval;

    private int timeUntilSwitch;

    private Camera cam;

    public enum GameplayStates
    {
        AboutToPlay,
        Playing,
        Lost,
        Won
    }

    void Start()
    {
        // Assuming there is never more then one camera in the scene
        cam = GameObject.FindObjectOfType<Camera>();

        StartCoroutine(TickDownTime());
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

                // Update the UI
                CountDownTimer.text = GetDisplayTime(Time);
            }

            // Check to see if the state has changed
            if (Time <= 0)
                this.State = GameplayStates.Lost;

            if (timeUntilSwitch <= 0)
                Switch();

            yield return new WaitForSeconds(1);
        }
    }

    string GetDisplayTime(int time)
    {
        var seconds = time;
        var minutes = 0;

        while (seconds > 60)
        {
            minutes++;
            seconds -= 60;
        }

        var displayString = $"{minutes} : {seconds}";
        return displayString;
    }

    void Switch()
    {
        this.IsFuture = !this.IsFuture;
        this.timeUntilSwitch = SwitchInterval;

        // TODO: Remove this code
        cam.backgroundColor = IsFuture ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.6f, 0.6f, 0.6f);
    }
}

