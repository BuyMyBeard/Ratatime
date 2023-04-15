using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDFunctionality : MonoBehaviour
{
    private VisualElement root;
    private Label countdownTimer;
    private Game_Manager manager;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        countdownTimer = root.Q<Label>("Timer");
        manager = GameObject.FindObjectOfType<Game_Manager>();
        SetTimer(null, null);
        manager.TimeChanged += SetTimer;
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

    void SetTimer(object sender, EventArgs e)
    {
        countdownTimer.text = GetDisplayTime(manager.Time);
    }
}
