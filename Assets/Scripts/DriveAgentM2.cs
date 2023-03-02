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

    public float notMovingTimer = 0;
    public float noCheckpointTimer = 0;

    public WheelCollider[] wheelColliders;
    
    public int wheelsOnTrack = 0;

    public float normalizedVelocity;
    public float normalizedAngularVelocity;
    public float normalizedPosition;
    public float normalizedDistanceToNextCheckpoint;

    public float[] normalizedDirectionToCheckpoints = new float[8];

    /* FIXED UPDATE */
    private void FixedUpdate()
    {
        /* POSITIVE REWARDS */
        
        // rewards for going forward
        //Debug.Log(transform.InverseTransformDirection(rb.velocity).z);
        if (transform.InverseTransformDirection(rb.velocity).z > 2)
        {
            AddReward(0.001f);
        }

        if (rb.velocity.magnitude > 0.4f)
        {
            notMovingTimer = 0;
        }
        else
        {
            notMovingTimer += Time.fixedDeltaTime;
        }

        noCheckpointTimer += Time.fixedDeltaTime;

        // rewards for position on the race
        //AddReward(0.0005f * currentPositionNormalized);

        // reward for direction to next checkpoint
        //Debug.Log("Adding reward for direction to next checkpoint: " + (0.001f * (1 - Math.Abs(normalizedDirectionToNextCheckpoint))) + "To car " + transform.gameObject.name);
        AddReward(0.0002f * (1 - Math.Abs(normalizedDirectionToCheckpoints[0])));

        // rewards for being on the track
        CheckWheelsColliders();

        if (isOnTrack)
        {
            offTrackTimer = 0;  
            AddReward(0.002f);
        }
        else
        {
            offTrackTimer += Time.fixedDeltaTime;
        }

        /* NEGATIVE REWARDS */

        // penalty for braking
        if (carController.CurrentAcceleration < 0)
        {
            //AddReward(-0.00025f);
        }

        // penalty for time passing
        //AddReward(-0.025f);

        if (offTrackTimer > 2)
        {
            AddReward(-10f);
            Debug.Log("Car " + transform.gameObject.name + " went off track for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
            EndEpisode();
        }
        if (notMovingTimer > 4)
        {
            AddReward(-10f);
            Debug.Log("Car " + transform.gameObject.name + " is not moving for too long. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
            EndEpisode();
        }
        if (noCheckpointTimer > 4)
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

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            //AddReward(-0.005f);
        }
    }

    /* DRIVEAGENT OVERRIDES */
    public override void OnCheckpointReached()
    {
        // Add reward for reaching a checkpoint
        noCheckpointTimer = 0;
        AddReward(2f);
    }

    public override void LapCompleted()
    {
        // Add reward for completing a lap and end episode for all agents
        AddReward(5f);
        Debug.Log("Car" + transform.gameObject.name + " completed a lap. Ending episode with a cummulative reward of " + GetCumulativeReward() + ".");
        raceTrainManager.LapCompleted();
        EndEpisode();
        //raceTrainManager.EndEpisodeForAllAgents(transform.gameObject);
    }

    public override void RaceCompleted()
    {
        throw new System.NotImplementedException();
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
        // - Distance to next checkpoint
        normalizedDistanceToNextCheckpoint = Math.Clamp(CheckpointTriggerer.distanceToNextCheckpoint/50, 0, 1);
        // - Direction to next checkpoints
        Vector3 carForward = new Vector3(car.transform.forward.x, 0, car.transform.forward.z);

        for (int i = 0; i < 8; i++)
        {
            Vector3 carToNextCheckpoint = new Vector3(CheckpointTriggerer.GetNextCheckpointPlusOffset(i).transform.position.x - transform.position.x, 0, CheckpointTriggerer.GetNextCheckpointPlusOffset(i).transform.position.z - transform.position.z);
            normalizedDirectionToCheckpoints[i] = Vector3.SignedAngle(carForward, carToNextCheckpoint, Vector3.up)/180;
        }

        // Add observations to sensor
        sensor.AddObservation(normalizedVelocity);
        sensor.AddObservation(normalizedAngularVelocity);
        //sensor.AddObservation(normalizedPosition);
        sensor.AddObservation(isOnTrack);
        sensor.AddObservation(normalizedDistanceToNextCheckpoint);
        sensor.AddObservation(normalizedDirectionToCheckpoints);
    }

    public override void OnEpisodeBegin()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.CheckpointTriggerer.points = 0;

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
