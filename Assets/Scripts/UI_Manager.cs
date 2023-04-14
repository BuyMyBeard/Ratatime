using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    private Game_Manager game_manager;

    [SerializeField]
    private UIDocument startMenu;

    [SerializeField]
    private UIDocument gameplayUI;

    private void Start()
    {
        game_manager = FindAnyObjectByType<Game_Manager>();
    }

    private void Update()
    {
        if (game_manager.State == Game_Manager.GameplayStates.AboutToPlay)
        {
            startMenu.enabled = true;
            gameplayUI.enabled = false;
        }
        else if (game_manager.State == Game_Manager.GameplayStates.Playing)
        {
            startMenu.enabled = false;
            gameplayUI.enabled = true;
        }
    }
}
