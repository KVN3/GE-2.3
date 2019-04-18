using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public ParticleSystem particleSystem;
    private AudioSource audioSource;
    private bool engineOn;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;

        Deactivate();
    }

    public void Activate()
    {
        if (!particleSystem.isPlaying)
        {
            particleSystem.Play();
            engineOn = true;
            audioSource.enabled = true;
        }
            
    }

    public void Deactivate()
    {
        if (particleSystem.isPlaying)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            engineOn = false;
            audioSource.enabled = false;
        }
    }

    public void SetStartSpeed(float speed)
    {
        ParticleSystem.MainModule pMain = particleSystem.main;
        pMain.startSpeed = speed;
    }

    public void SetLifeTime(float lifeTime)
    {
        ParticleSystem.MainModule pMain = particleSystem.main;
        pMain.startLifetime = lifeTime;
    }



}
