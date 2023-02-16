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
    public float offTrackTimer = 0;

    public WheelCollider[] wheelColliders;
    
    public int wheelsOnTrack = 0;

    public RaceTrainManager raceTrainManager;

    public float normalizedVelocity;
    public float normalizedAngularVelocity;
    public float normalizedPosition;
    public float normalizedDirectionToNextCheckpoint;
    public float normalizedDistanceToNextCheckpoint;
    public float normalizedDirectionToNextNextCheckpoint;

    /* FIXED UPDATE */
    private void FixedUpdate()
    {
        // Add rewards for every step
        
        // rewards for going forward
        if (transform.InverseTransformDirection(rb.velocity).z > 0)
        {
            AddReward(0.005f);
        }

        // penalty for braking
        if (carController.CurrentAcceleration < 0)
        {
            //AddReward(-0.0005f);
        }

        // rewards for position on the race
        AddReward(0.0025f * currentPositionNormalized);

        // rewards for being on the track
        CheckWheelsColliders();

        if (!isOnTrack)
        {
            offTrackTimer += Time.fixedDeltaTime;
            AddReward(-2f);
        }
        else
        {
            offTrackTimer = 0;
        }

        if (offTrackTimer > 2)
        {
            SetReward(-50f);
            Debug.Log("Car " + transform.gameObject.name + " went off track for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
            EndEpisode();
        }

        // reward for direction to next checkpoint
        //Debug.Log("Adding reward for direction to next checkpoint: " + (0.001f * (1 - Math.Abs(normalizedDirectionToNextCheckpoint))) + "To car " + transform.gameObject.name);
        AddReward(0.01f * (1 - Math.Abs(normalizedDirectionToNextCheckpoint)));

        // penalty for time passing
        AddReward(-0.025f);
    }

    /* DRIVEAGENT OVERRIDES */
    public override void OnCheckpointReached()
    {
        // Add reward for reaching a checkpoint
        AddReward(8f);
    }

    public override void LapCompleted()
    {
        // Add reward for completing a lap and end episode for all agents
        AddReward(20f);
        Debug.Log("Car" + transform.gameObject.name + " completed a lap. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
        EndEpisode();
        //raceTrainManager.EndEpisodeForAllAgents(transform.gameObject);
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
        // - Is the car on the track
        // - Direction to next checkpoint
        Vector3 carForward = new Vector3(car.transform.forward.x, 0, car.transform.forward.z);
        Vector3 carToNextCheckpoint = new Vector3(CheckpointTriggerer.GetNextCheckpointPlusOffset(0).transform.position.x - transform.position.x, 0, CheckpointTriggerer.GetNextCheckpointPlusOffset(0).transform.position.z - transform.position.z);
        normalizedDirectionToNextCheckpoint = Vector3.SignedAngle(carForward, carToNextCheckpoint, Vector3.up)/180;
        // - Distance to next checkpoint
        normalizedDistanceToNextCheckpoint = Math.Clamp(CheckpointTriggerer.distanceToNextCheckpoint/50, 0, 1);
        // - Direction to second next checkpoint
        Vector3 carToNextNextCheckpoint = new Vector3(CheckpointTriggerer.GetNextCheckpointPlusOffset(1).transform.position.x - transform.position.x, 0, CheckpointTriggerer.GetNextCheckpointPlusOffset(1).transform.position.z - transform.position.z);
        normalizedDirectionToNextNextCheckpoint = Vector3.SignedAngle(carForward, carToNextNextCheckpoint, Vector3.up)/180;
        // Normalise all observations

        // Add observations to sensor

        sensor.AddObservation(normalizedVelocity);
        sensor.AddObservation(normalizedAngularVelocity);
        sensor.AddObservation(normalizedPosition);
        sensor.AddObservation(isOnTrack);
        sensor.AddObservation(normalizedDirectionToNextCheckpoint);
        sensor.AddObservation(normalizedDistanceToNextCheckpoint);
        sensor.AddObservation(normalizedDirectionToNextNextCheckpoint);
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
