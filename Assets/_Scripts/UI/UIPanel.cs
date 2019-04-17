using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPanel : UIBehaviour
{
    public TextMeshProUGUI raceTimeText;
    public TextMeshProUGUI bestRaceTimeText;

    public TextMeshProUGUI raceLapText;
    public TextMeshProUGUI playerSpeedText;

    public Slider chargeBar;

    public TextMeshProUGUI raceTimesText;

    public Ship ship;

    public PlayerShip PlayerShip
    {
        get
        {
            HUD Hud = GetComponentInParent<HUD>();

            Assert.IsNotNull(Hud, "Bla bla je hebt het verkeerd ingesteld");
            Assert.IsNotNull(Hud.playerShip, "Je hebt geen playerShip in je hud");

            return Hud.playerShip;
        }
    }

    protected override void Start()
    {
        base.Start();

        // Default text
        raceLapText.text = $"Lap: 0/3";
    }

    void Update()
    {
        #region In-GameUI

        // Laps
        raceLapText.text = $"Lap: {PlayerShip.runData.currentLap}/{PlayerShip.runData.maxLaps}";

        // Race Time
        raceTimeText.text = "CURR - " + PlayerShip.runData.raceTime.ToString(@"mm\:ss\.ff");

        // Best race time
        if (PlayerShip.runData.bestRaceTime == TimeSpan.Parse("00:00:00.000"))
            bestRaceTimeText.text = "--.---";
        else
            bestRaceTimeText.text = PlayerShip.runData.bestRaceTime.ToString(@"mm\:ss\.ff");

        // Speed (Get and convert speed from rb)

        //Rigidbody rb = target.GetComponent<Rigidbody>();
        //var localVelocity = transform.InverseTransformVector(rb.velocity);
        //var forwardSpeed = Mathf.Abs(localVelocity.z);
        //playerSpeedText.text = forwardSpeed.ToString("0") + " KM/H";
        float currSpeed = ship.components.movement.GetCurrentSpeed();
        playerSpeedText.text = currSpeed.ToString("0") + " KM/H";

        // Charges
        chargeBar.value = PlayerShip.runData.charges;

        #endregion

        #region Race Finished Screen
        // Only show racetimes when finished and display them using a stringbuilder (for lines)
        if (PlayerShip.runData.raceFinished)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < PlayerShip.runData.raceTimes.Count; i++)
            {
                string lapTime = PlayerShip.runData.raceTimes[i].ToString(@"mm\:ss\.ff");
                string lapCount = (i + 1).ToString(); // Arrays start at 0 but laps start at 1

                builder.Append("Lap ").Append(lapCount).Append(": ").Append(lapTime).AppendLine();
            }
            builder.Append("Best Lap: ").Append(PlayerShip.runData.bestRaceTime);

            raceTimesText.text = builder.ToString();
        }
        #endregion
    }
}