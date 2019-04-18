﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipComponents
{
    public ShipMovement movement;
    public ShipEngines engines;
    public ShipGun gun;
    public ShipSystem system;
}

public class Ship : MonoBehaviour
{
    public ShipComponents components;
    public ShipSoundManager shipSoundManagerClass;
    public LevelSoundManager levelSoundManagerClass;

    private List<ShipComponent> componentsList;
    private ShipSoundManager shipSoundManager;
    private LevelSoundManager levelSoundManager;

    // Collectables
    private Collectable collectableItemClass;
    private int itemAmount;

    public virtual void Awake()
    {
        shipSoundManager = Instantiate(shipSoundManagerClass, transform.localPosition, transform.localRotation, this.transform);

        componentsList = new List<ShipComponent>();

        componentsList.Add(components.movement);
        componentsList.Add(components.engines);
        componentsList.Add(components.gun);
        componentsList.Add(components.system);

        foreach (ShipComponent component in componentsList)
        {
            component.SetParentShip(this);
            component.SetShipSoundManager(shipSoundManager);
        }

        //components.shipSoundManager.InitializeComponent();
        //components.levelSoundManager.InitializeComponent();
    }

    public virtual void Start()
    {

    }

    public void UseItem()
    {
        if (itemAmount > 0)
        {
            if (collectableItemClass is JammerProjectile)
            {
                components.gun.Shoot((JammerProjectile)collectableItemClass);
                itemAmount--;
            }
        }
    }

    public void GetHitByEmp(int duration)
    {
        if (!components.system.IsSystemDown())
        {
            components.system.ShutDown();
            components.engines.RestoreSystem();
        }
    }

    public void SetItem(Collectable item, int amount)
    {
        this.collectableItemClass = item;
        itemAmount = amount;
    }

    public ShipSoundManager GetShipSoundManager()
    {
        return shipSoundManager;
    }
}

// TO DO: camera straight while turning
// TO DO: temporary increase of air drag while turning (big question mark)