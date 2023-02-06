using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointTriggerer : MonoBehaviour
{
    // points counter variable
    public int points = 0;

    public DriveAgent driveAgent;

    private void OnTriggerEnter(Collider other)
    {
        // get list of all checkpoints
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        //Debug.Log(other.gameObject.name);
        
        // sums points only if the object that enters the trigger is a checkpoint and it accords to the points counter
        if (other.gameObject.tag == "Checkpoint" && other.gameObject == checkpoints[points])
        {
            // add reward to the agent
            points++;
            driveAgent.onCheckpointReached();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
