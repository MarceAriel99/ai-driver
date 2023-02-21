using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DriveAgentM1 : DriveAgent
{
    /* FIXED UPDATE */
    private void FixedUpdate()
    {
        DetectBackwardsDriving(-0.1f, false);
        DetectCarStopped(-0.1f, false);
        DetectSpeeding(0.012f, false, 22f);

        // For every frame the car is alive, give it a small penalty so it learns to finish the track faster
        AddReward(-0.18f);
    }

    /* DRIVEAGENT OVERRIDES */
    public override void OnCheckpointReached()
    {
        //Debug.Log("Checkpoint reached");
        AddReward(3f);
    }

    public override void LapCompleted()
    {
        //Debug.Log("Lap completed");
        AddReward(50f);
        EndEpisode();
    }

    public override void RaceCompleted()
    {
        throw new System.NotImplementedException();
    }

    /* AGENT OVERRIDES */

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(rb.angularVelocity);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-100f);
            EndEpisode();
        }
    }

    private void DetectBackwardsDriving(float reward, bool ends_episode = false)
    {
        if (Vector3.Dot(car.transform.forward, rb.velocity) < 0)
        {
            //Debug.Log("Backwards driving");
            AddReward(reward);
            if (ends_episode)
            {
                EndEpisode();
            }
        }
    }

    private void DetectCarStopped(float reward, bool ends_episode = false)
    {
        if (rb.velocity.magnitude < 0.05f)
        {
            //Debug.Log("Car stopped");
            AddReward(reward);
            if (ends_episode)
            {
                EndEpisode();
            }
        }
    }

    private void DetectSpeeding(float reward_multiplier, bool ends_episode = false, float threshold = 20f)
    {
        if (rb.velocity.magnitude > threshold)
        {
            float reward = rb.velocity.magnitude * reward_multiplier;

            if (reward > reward_multiplier * threshold)
            {
                reward = reward_multiplier * threshold;
            }
            AddReward(reward);
        }
    }

    private void DetectBraking(float reward)
    {
        if (carController.CurrentAcceleration < 0)
        {
            AddReward(reward);
        }
    }
}
