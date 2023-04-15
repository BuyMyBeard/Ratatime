using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Future : MonoBehaviour
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
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
