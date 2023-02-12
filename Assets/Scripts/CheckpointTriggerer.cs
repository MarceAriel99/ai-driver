using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;


public class CheckpointTriggerer : MonoBehaviour
{
    // points counter variable
    public int points = 0;

    public DriveAgent driveAgent;

    public GameObject[] checkpoints;

    void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
    }

    private void OnTriggerEnter(Collider other)
    {      
        // sums points only if the object that enters the trigger is a checkpoint and it accords to the points counter
        if (other.gameObject.tag == "Checkpoint" && other.gameObject == checkpoints[points])
        {
            // add reward to the agent
            points++;
            if (points == checkpoints.Length)
            {
                points = 0;
                driveAgent.LapCompleted();
            }
            driveAgent.onCheckpointReached();
        }
        
    }
}
