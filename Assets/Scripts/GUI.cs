using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GUI : MonoBehaviour
{
    private int gearst = 0;
    private float needleAngle = -150;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        ShowCarUI();
    }

    public void ShowCarUI()
    {
        GameObject needle = GameObject.Find("Needle");
        TextMeshProUGUI speedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI gearText = GameObject.Find("GearText").GetComponent<TextMeshProUGUI>();
        
        GameObject player = GameObject.Find("Car (Model 2) (2)");
        CarController controller = player.GetComponent<CarController>();
        
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
}
