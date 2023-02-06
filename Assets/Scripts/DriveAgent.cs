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
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(rb.angularVelocity);
    }

    public void onCheckpointReached()
    {
        Debug.Log("Checkpoint reached");
        AddReward(1f);
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
            AddReward(-20f);
            EndEpisode();
        }
    }
    
    private void FixedUpdate()
    {
        if (Vector3.Dot(car.transform.forward, rb.velocity) < 0)
        {
            Debug.Log("Backwards driving");
            AddReward(-0.1f);
        }

        if (rb.velocity.magnitude < 0.1f)
        {
            Debug.Log("Stopped");
            AddReward(-0.1f);
        }

        if (rb.velocity.magnitude > 1f)
        {   
            float reward_multiplier = 0.015f;
            float reward = rb.velocity.magnitude * reward_multiplier;

            if (reward > reward_multiplier * 15)
            {
                reward = reward_multiplier * 15;
            }

            AddReward(reward);
        }

        if (carController.CurrentAcceleration < 0)
        {
            AddReward(-0.01f);
        }

        // For every frame the car is alive, give it a small penalty so it learns to finish the track faster
        AddReward(-0.040f);
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

        carController.UpdateControls(forwardAmount, turnAmount, false);
    }
}
