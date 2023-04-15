using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Past : MonoBehaviour
{
    void Awake()
    {
        Game_Manager mgr = GameObject.FindObjectOfType<Game_Manager>();
        mgr.TimeChanged += TimeChanged;
    }

    void TimeChanged(object sender, EventArgs e)
    {
        if ((sender as bool?).Value)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
