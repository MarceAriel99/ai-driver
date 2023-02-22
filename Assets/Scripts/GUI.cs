using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GUI : MonoBehaviour
{
    GameObject player;
    DriveAgentM2 agent;
    CarController controller;
    private int gearst = 0;
    private float needleAngle = -150;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerCar");
        agent = player.GetComponent<DriveAgentM2>();
        controller = player.GetComponent<CarController>();
        Debug.Log("players: " + agent.raceTrainManager.ToString());
    }

    void FixedUpdate()
    {
        ShowCarUI();
        UpdatePosition();
    }

    public void ShowCarUI()
    {
        GameObject needle = GameObject.Find("Needle");
        TextMeshProUGUI speedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI gearText = GameObject.Find("GearText").GetComponent<TextMeshProUGUI>();
        
        
        gearst = controller.CurrentGear;

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

        needleAngle = (controller.EngineRPM / 20) - 175;
        needleAngle = Mathf.Clamp(needleAngle, -180, 90);
        needle.GetComponent<Image>().rectTransform.rotation = Quaternion.Euler(0, 0, -needleAngle);
    }

    public void UpdatePosition()
    {
        TextMeshProUGUI positionText = GameObject.Find("PositionInRaceText").GetComponent<TextMeshProUGUI>();
        int position = agent.raceTrainManager.GetCarPosition(player);
        switch (position)
        {
            case 1:
                positionText.text = "1st Position";
                break;
            case 2:
                positionText.text = "2nd Position";
                break;
            case 3:
                positionText.text = "3rd Position";
                break;
            default:
                positionText.text = position + "th Position";
                break;
        }
    }
}
