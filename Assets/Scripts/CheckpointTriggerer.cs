using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;


public class CheckpointTriggerer : MonoBehaviour
{
    // points counter variable
    public int points = 0;

    public DriveAgent driveAgent;

    public GameObject[] checkpoints;

    public float distanceToNextCheckpoint;

    public int finishLineCheckpointIndex;

    public int episodeStep = 50;

    void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        driveAgent = GetComponent<DriveAgent>(); // FIXME: esto esta bien o deberia ser M2?
        //finishLineCheckpoint = GetFinishLineCheckpointIndex(driveAgent.CompletedEpisodes);
        //finishLineCheckpoint = 0;
    }

    /*
    private int GetFinishLineCheckpointIndex(int completedEpisodes)
    {
        // there is one finish line every 5 checkpoints and move finish line forwards when they complete the race 'episodeStep' times

        int finishLineCheckpointIndex = ((int)math.floor(completedEpisodes / episodeStep)) * 1;

        if (finishLineCheckpointIndex > checkpoints.Length)
        {
            finishLineCheckpointIndex = checkpoints.Length;
        }

        return finishLineCheckpointIndex;
    }
    */

    void FixedUpdate()
    {
        // distance to next checkpoint
        distanceToNextCheckpoint = Vector3.Distance(transform.position, checkpoints[points].transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {      
        // sums points only if the object that enters the trigger is a checkpoint and it accords to the points counter
        if (other.gameObject.tag == "Checkpoint" && other.gameObject == checkpoints[points])
        {
            // add reward to the agent
            if (points == finishLineCheckpointIndex)
            {
                points = 0;
                // FIXME hay codigo repetido pero hasta que no funcionen los puntos queda asi
                driveAgent.LapCompleted();
                //finishLineCheckpoint = GetFinishLineCheckpointIndex(driveAgent.CompletedEpisodes);
            } else {
                points++;
                driveAgent.OnCheckpointReached();
            }
        }
    }

    public GameObject GetNextCheckpointPlusOffset(int offset)
    {
        int nextCheckpointIndex = points + offset;

        if (nextCheckpointIndex >= checkpoints.Length)
        {
            nextCheckpointIndex = nextCheckpointIndex - checkpoints.Length;
        }

        return checkpoints[nextCheckpointIndex];
    }
}
