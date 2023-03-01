using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class DriveAgentM3 : DriveAgent
{
    public float currentPositionNormalized = 0;
    public Boolean isOnTrack = true;
    public float offTrackTimer = 0;

    public float notMovingTimer = 0;
    public float noCheckpointTimer = 0;

    public WheelCollider[] wheelColliders;
    
    public int wheelsOnTrack = 0;

    public RaceTrainManager raceTrainManager;

    public float normalizedVelocity;
    public float normalizedAngularVelocity;
    public float normalizedPosition;

    public float[] normalizedDirectionToCheckpoints = new float[12];

    public float[] normalizedDistancesToCheckpoints = new float[12];

    public GameObject[] nextCheckpoints = new GameObject[12];

    /* FIXED UPDATE */
    private void FixedUpdate()
    {
        /* POSITIVE REWARDS */
        
        // reward for going forward
        if (transform.InverseTransformDirection(rb.velocity).z > 2)
        {
            AddReward(0.001f);
        }

        // Update not moving timer
        if (rb.velocity.magnitude > 0.4f)
        {
            notMovingTimer = 0;
        }
        else
        {
            notMovingTimer += Time.fixedDeltaTime;
        }

        // Update no checkpoint timer
        noCheckpointTimer += Time.fixedDeltaTime;

        // reward for position on the race
        //AddReward(0.0002f * currentPositionNormalized);

        // reward for direction to next checkpoint
        AddReward(0.0002f * (1 - Math.Abs(normalizedDirectionToCheckpoints[0])));

        // rewards for being on the track
        CheckWheelsColliders();

        if (isOnTrack)
        {
            offTrackTimer = 0;  
            AddReward(0.005f);
        }
        else
        {
            offTrackTimer += Time.fixedDeltaTime;
            AddReward(-0.05f);
        }

        /* NEGATIVE REWARDS */

        //penalty for time passing
        //AddReward(-0.025f);
        
        // If car is off track for too long, end episode
        // if (offTrackTimer > 2)
        // {
        //     AddReward(-30f);
        //     Debug.Log("Car " + transform.gameObject.name + " went off track for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
        //     EndEpisode();
        // }
        // If car is not moving for too long, end episode
        if (notMovingTimer > 4)
        {
            AddReward(-30f);
            Debug.Log("Car " + transform.gameObject.name + " is not moving for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
            EndEpisode();
        }
        //If car is not advancing checkpoints and it is not at the start of the race, end episode
        if (noCheckpointTimer > 6/* && CheckpointTriggerer.points > 1*/)
        {
            AddReward(-10f);
            Debug.Log("Car " + transform.gameObject.name + " is not advancing checkpoints for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
            EndEpisode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            // Get collision speed
            float collisionSpeed = rb.velocity.magnitude;
            // Add reward for collision speed
            AddReward(-0.1f * collisionSpeed);
        }
    }

    /* DRIVEAGENT OVERRIDES */
    public override void OnCheckpointReached()
    {
        // Add reward for reaching a checkpoint
        noCheckpointTimer = 0;
        AddReward(3.5f);
        for (int i = 0; i < 12; i++)
        {
            nextCheckpoints[i] = CheckpointTriggerer.GetNextCheckpointPlusOffset(i);
        }
    }

    public override void LapCompleted()
    {
        // Add reward for completing a lap and end episode for all agents
        AddReward(5f);
        Debug.Log("Car" + transform.gameObject.name + " completed a lap. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
        raceTrainManager.LapCompleted();
        //EndEpisode();
        //raceTrainManager.EndEpisodeForAllAgents(transform.gameObject);
    }

    public override void RaceCompleted()
    {
        AddReward(50f);
        raceTrainManager.EndEpisodeForAllAgents(transform.gameObject);
    }

    /* AGENT OVERRIDES */
    public override void CollectObservations(VectorSensor sensor)
    {
        // Should contain the following observations:

        // - Velocity
        normalizedVelocity = Math.Clamp(transform.InverseTransformDirection(rb.velocity).z/60, -1, 1);
        // - Angular velocity
        normalizedAngularVelocity = Math.Clamp(rb.angularVelocity.y/4, -1, 1);
        // - Current position on race
        normalizedPosition = currentPositionNormalized;

        // - Direction and distances to next checkpoints
        Vector3 carForward = new Vector3(car.transform.forward.x, 0, car.transform.forward.z);

        for (int i = 0; i < 12; i++)
        {
            Vector3 carToNextCheckpoint = new Vector3(CheckpointTriggerer.GetNextCheckpointPlusOffset(i).transform.position.x - transform.position.x, 0, CheckpointTriggerer.GetNextCheckpointPlusOffset(i).transform.position.z - transform.position.z);
            normalizedDirectionToCheckpoints[i] = Vector3.SignedAngle(carForward, carToNextCheckpoint, Vector3.up)/180;
            normalizedDistancesToCheckpoints[i] = Math.Clamp(Vector3.Distance(transform.position, CheckpointTriggerer.GetNextCheckpointPlusOffset(i).transform.position)/150, 0, 1);
        }

        // Add observations to sensor
        sensor.AddObservation(normalizedVelocity);
        sensor.AddObservation(normalizedAngularVelocity);
        sensor.AddObservation(normalizedPosition);
        sensor.AddObservation(isOnTrack);
        sensor.AddObservation(normalizedDistancesToCheckpoints);
        sensor.AddObservation(normalizedDirectionToCheckpoints);
    }

    public override void OnEpisodeBegin()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.CheckpointTriggerer.points = 0;
        this.CheckpointTriggerer.currentLap = 0;

        // Reset timers
        notMovingTimer = 0;
        offTrackTimer = 0;
        noCheckpointTimer = 0;

        Debug.ClearDeveloperConsole();
    }

    /* SELF-DEFINED METHODS */
    private void CheckWheelsColliders()
    {
        wheelsOnTrack = 0;
        // If all wheels are on the track, the car is on the track
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.GetGroundHit(out WheelHit hit);
            if (hit.collider == null)
            {
                continue;
            }
            if (hit.collider.tag == "Track")
            {
                wheelsOnTrack++;
            }
        }

        if (wheelsOnTrack == 4)
        {
            isOnTrack = true;
        }
        else
        {
            isOnTrack = false;
        }
    }

    internal void SetPosition(int carCurrentPosition, int totalCars)
    {
        // Set the current position of the car
        currentPositionNormalized = 1 - ((float)carCurrentPosition / totalCars);
    }

    public void SetTrainManager(RaceTrainManager raceTrainManager)
    {
        this.raceTrainManager = raceTrainManager;
    }
}
