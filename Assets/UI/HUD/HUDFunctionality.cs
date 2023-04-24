using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUDFunctionality : MonoBehaviour
{
    [SerializeField] VisualTreeAsset HUD;
    [SerializeField] VisualTreeAsset PauseMenu;

    private UIDocument doc;
    private Label countdownTimer;
    private Button pauseButton;
    //private Button playButton;
    private Game_Manager manager;

    void Start()
    {
        doc = GetComponent<UIDocument>();
        Play();
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

    void Pause()
    {
        Time.timeScale = 0;
        doc.visualTreeAsset = PauseMenu;
        var playButton = doc.rootVisualElement.Q<Button>("PlayButton");
        playButton.clicked += Play;

        var restartButton = doc.rootVisualElement.Q<Button>("RestartButton");
        restartButton.clicked += Restart;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Play()
    {
        Time.timeScale = 1;
        doc.visualTreeAsset = HUD;

        countdownTimer = doc.rootVisualElement.Q<Label>("Timer");
        pauseButton = doc.rootVisualElement.Q<Button>("PauseButton");
        manager = FindObjectOfType<Game_Manager>();

        SetTimer(null, null);
        manager.TimeChanged += SetTimer;
        pauseButton.clicked += Pause;

        doc.rootVisualElement.RegisterCallback<NavigationSubmitEvent>((evt) =>
        {
            evt.StopPropagation();
        }, TrickleDown.TrickleDown);
    }
}
