﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MyMonoBehaviour, IObserver
{
    public PlayerShip playerShip { get; set; }

    Animator Anim;


    // Start is called before the first frame update
    void Awake() {
		Assert.IsNotNull(playerShip);

        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerShip.runData.raceFinished)
            Anim.SetTrigger("RaceFinished");
    }

    public void OnNotify(float score, float charges)
    {
        // Do Something
        
    }
}
