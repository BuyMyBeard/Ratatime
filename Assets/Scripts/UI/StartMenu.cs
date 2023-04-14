using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    public VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        Button button = root.Q<Button>("StartButton");

        button.clicked += Button_clicked;
    }

    private void Button_clicked()
    {
        FindObjectOfType<Game_Manager>().State = Game_Manager.GameplayStates.Playing;
    }
}
