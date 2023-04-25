using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShifter : MonoBehaviour, ITimeShifter
{
    AudioSource pastMusic, futureMusic;

    void Awake()
    {
        var audioSources = GetComponents<AudioSource>();
        pastMusic = audioSources[0];
        futureMusic = audioSources[1];

    }
    public void ShiftToFuture()
    {
        pastMusic.mute = true;
        futureMusic.mute = false;
    }

    public void ShiftToPast()
    {
        pastMusic.mute = false;
        futureMusic.mute = true;
    }
}
