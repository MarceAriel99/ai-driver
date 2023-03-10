using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GUI : MonoBehaviour
{
    GameObject player;
    DriveAgentInterface agent;
    CarController controller;
    private int gearst = 0;
    private float needleAngle = -150;

    GameObject needle;
    TextMeshProUGUI speedText;
    TextMeshProUGUI gearText;
    TextMeshProUGUI lapText;
    CheckpointTriggerer checkpointTriggerer;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerCar");
        agent = player.GetComponent<PlayerManager>();
        controller = player.GetComponent<CarController>();
        needle = GameObject.Find("Needle");
        speedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        gearText = GameObject.Find("GearText").GetComponent<TextMeshProUGUI>();
        lapText = GameObject.Find("LapText").GetComponent<TextMeshProUGUI>();
        checkpointTriggerer = player.GetComponent<CheckpointTriggerer>(); // to check current and total laps
    }

    void FixedUpdate()
    {
        ShowCarUI();
        UpdatePosition();
        // Debug.Log("GEAR: " + controller.CurrentGear);
        // Debug.Log("RPM: " + controller.EngineRPM);
    }

    public void ShowCarUI()
    {
        gearst = controller.CurrentGear;
        lapText.text = "Lap " + checkpointTriggerer.currentLap + "/" + checkpointTriggerer.raceLaps;

        speedText.text = controller.SpeedInHour.ToString("000");
        if (gearst > 0 && controller.CurrentSpeed > 1)
        {
            gearText.color = Color.green;
            gearText.text = gearst.ToString();
        }
        else if (controller.CurrentSpeed > 1)
        {
            gearText.color = Color.red;
            gearText.text = "R";
        }
        else
        {
            gearText.color = Color.white;
            gearText.text = "N";
        }

        needleAngle = (controller.EngineRPM / 33.33f) - 180;
        needle.GetComponent<Image>().rectTransform.rotation = Quaternion.Euler(0, 0, -needleAngle);
    }

    public void UpdatePosition()
    {   
        if(agent.raceTrainManager.carsPositions.Count == 0)
        {
            return;
        }
        TextMeshProUGUI positionText = GameObject.Find("PositionInRaceText").GetComponent<TextMeshProUGUI>();
        int position = agent.raceTrainManager.GetCarPosition(player);
        string position_text = " Position: " + position;
        switch (position)
        {
            case 1:
                positionText.text = position_text + "st";
                break;
            case 2:
                positionText.text = position_text + "nd";
                break;
            case 3:
                positionText.text = position_text + "rd";
                break;
            default:
                positionText.text = position_text + "th";
                break;
        }
    }
}
