using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class DriveAgentM2 : DriveAgent
{
    public float currentPositionNormalized = 0;
    public Boolean isOnTrack = true;

    public WheelCollider[] wheelColliders;
    
    public int wheelsOnTrack = 0;

    /* FIXED UPDATE */
    private void FixedUpdate()
    {
        // Add rewards for every step

        CheckWheelsColliders();
    }

    /* DRIVEAGENT OVERRIDES */
    public override void OnCheckpointReached()
    {
        // Add reward for reaching a checkpoint
    }

    public override void LapCompleted()
    {
        // Add reward for completing a lap
    }

    /* AGENT OVERRIDES */

    public override void CollectObservations(VectorSensor sensor)
    {
        // Should contain the following observations:

        // - Velocity
        // - Angular velocity
        // - Current position on race
        // - Is the car on the track
        // - Direction to next checkpoint
        // - Distance to next checkpoint
        // - Direction to secont next checkpoint

        // Normalise all observations

        // Add observations to sensor
    }

    public override void OnEpisodeBegin()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.CheckpointTriggerer.points = 0;
        Debug.ClearDeveloperConsole();
    }

    /* SELF-DEFINED METHODS */

    private void CheckWheelsColliders()
    {
        wheelsOnTrack = 0;
        // If at least 2 wheels are on the track, the car is on the track
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

        if (wheelsOnTrack >= 2)
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
        currentPositionNormalized = 1 - (carCurrentPosition / totalCars);
    }
}
