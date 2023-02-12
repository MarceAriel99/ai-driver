using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DriveAgent : Agent
{

    private CarController carController;
    private GameObject car;

    private Rigidbody rb;

    public CheckpointTriggerer CheckpointTriggerer;

    public GameObject startPoint;

    void Start()
    {
        car = this.gameObject;
        carController = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
        CheckpointTriggerer = GetComponent<CheckpointTriggerer>();
        // random start position from the list of start points
        //startPoint = GameObject.FindGameObjectsWithTag("StartPoint")[Random.Range(0, GameObject.FindGameObjectsWithTag("StartPoint").Length)];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(rb.angularVelocity);
    }

    public void onCheckpointReached()
    {
        Debug.Log("Checkpoint reached");
        AddReward(3f);
    }

    public void LapCompleted()
    {
        Debug.Log("Lap completed");
        AddReward(50f);
        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Wall hit");
            AddReward(-100f);
            EndEpisode();
        }
    }
    
    private void FixedUpdate()
    {
        DetectBackwardsDriving(-0.1f, false);
        DetectCarStopped(-0.1f, false);
        DetectSpeeding(0.012f, false, 22f);
        //DetectBraking(-0.001f);

        // For every frame the car is alive, give it a small penalty so it learns to finish the track faster
        AddReward(-0.18f);
    }

    private void DetectBackwardsDriving(float reward, bool ends_episode = false)
    {
        if (Vector3.Dot(car.transform.forward, rb.velocity) < 0)
        {
            Debug.Log("Backwards driving");
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
            Debug.Log("Car stopped");
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
    
    public override void OnEpisodeBegin()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.CheckpointTriggerer.points = 0;
        Debug.ClearDeveloperConsole();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {   
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        carController.UpdateControls(forwardAmount, turnAmount, false); //TODO: add brake control
    }
}
