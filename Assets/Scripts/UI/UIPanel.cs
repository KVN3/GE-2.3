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
    public TextMeshProUGUI raceLapText;
    public TextMeshProUGUI raceTimesText;

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
        TimeSpan ts = PlayerShip.runData.raceTime;
        raceTimeText.text = ts.ToString(@"mm\:ss\.ff");

        raceLapText.text = $"Lap: {PlayerShip.runData.currentLap}/3";

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

            raceTimesText.text = builder.ToString();
        }
            
    }
}