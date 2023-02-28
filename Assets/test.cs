using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour
{
    public AnimationClip CountdownClip;
    public GameObject CountDown;
    // Start is called before the first frame update
    void Start()
    {
        CountDown.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // when press space bar, play animation clip "Countdown" on the object "Countdown"
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("AA key was pressed.");
            CountDown.SetActive(true);
            CountDown.GetComponent<Animator>().Play("Countdown");
            StartCoroutine(DisableCountdown());
        }
        
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(CountdownClip.length-0.2f);
        CountDown.SetActive(false);
    }
}
