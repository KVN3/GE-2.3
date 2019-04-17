using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    protected AudioSource audioSource;
    protected AudioClip audioClip;

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void PlaySound(SoundType soundType)
    {

    }
}
