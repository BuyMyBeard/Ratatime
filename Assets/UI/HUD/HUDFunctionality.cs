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
    [SerializeField] VisualTreeAsset GameOver;

    private UIDocument doc;
    private Label countdownTimer;
    private Label cheeseCount;
    private Button pauseButton;
    //private Button playButton;
    private Game_Manager manager;


    void Start()
    {
        doc = GetComponent<UIDocument>();
        Play();
        SetCheeseCount(0, null);
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

        var displayString = $"0{minutes}:{seconds}";
        return displayString;
    }

    void SetTimer(object sender, EventArgs e)
    {
        countdownTimer.text = GetDisplayTime(manager.Time);

        if (manager.Time <= 0)
        {
            EndGame();
        }
    }

    void EndGame ()
    {
        Time.timeScale = 0;
        doc.visualTreeAsset = GameOver;

        var restartButton = doc.rootVisualElement.Q<Button>("PlayButton");
        var quitButton = doc.rootVisualElement.Q<Button>("QuitButton");
        restartButton.clicked += Restart;
        quitButton.clicked += QuitButton_clicked;
        
    }

    private void QuitButton_clicked()
    {
        SceneManager.LoadScene(0);
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

    void SetCheeseCount(object sender, EventArgs e)
    {
        cheeseCount.text = sender.ToString();
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
        cheeseCount = doc.rootVisualElement.Q<Label>("CheeseCount");

        manager = FindObjectOfType<Game_Manager>();
        var player = GameObject.FindGameObjectWithTag("Player");
        var tradeComponent = player.GetComponent<PlayerTradeComponent>();

        tradeComponent.CheeseCountChanged += SetCheeseCount;
        manager.TimeChanged += SetTimer;
        pauseButton.clicked += Pause;

        SetTimer(null, null);        

        doc.rootVisualElement.RegisterCallback<NavigationSubmitEvent>((evt) =>
        {
            evt.StopPropagation();
        }, TrickleDown.TrickleDown);
    }
}
