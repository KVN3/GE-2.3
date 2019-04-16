using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Activate()
    {
        if (!particleSystem.isPlaying)
            particleSystem.Play();
    }

    public void Deactivate()
    {
        if (particleSystem.isPlaying)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
