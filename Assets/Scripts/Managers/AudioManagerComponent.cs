using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerComponent : MonoBehaviour
{
    [SerializeField] AudioClip[] sfx;
    AudioSource sfxSource;
    private void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
    }
    public void PlaySFX(int id)
    {
        sfxSource.PlayOneShot(sfx[id]);
    }
}
