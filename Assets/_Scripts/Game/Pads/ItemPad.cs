using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPad : MonoBehaviour
{
    public Collectable[] itemClasses;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();

            playerShip.PlaySound(SoundType.PICKUP);
            playerShip.collectableItemClass = itemClasses[Random.Range(0, itemClasses.Length)];
            playerShip.itemAmount = 1;
        }
    }

}
