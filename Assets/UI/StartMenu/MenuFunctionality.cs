using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuFunctionality : MonoBehaviour
{
    private VisualElement root;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        var playButton = root.Q<Button>("Play_Button");

        playButton.clicked += PlayButton_Clicked;
    }

    private void PlayButton_Clicked()
    {
        // TODO: Set the real play scene
        SceneManager.LoadScene(2);        
    }
}
