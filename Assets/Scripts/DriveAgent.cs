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

    public bool has_touched_wall = false;
    float wall_stuck_timer = 0f;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Wall hit");
            has_touched_wall = true;
            AddReward(-20f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Wall hit stay");
            AddReward(-0.1f);
            wall_stuck_timer += Time.deltaTime;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Wall hit exit");
            wall_stuck_timer = 0f;
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

        if (rb.velocity.magnitude > 1.5f)
        {
            AddReward(0.001f);
        }

        if (!has_touched_wall)
        {
            AddReward(0.05f);
        }

        if (carController.CurrentAcceleration > 0)
        {
            //AddReward(0.0001f);
        }

        if (wall_stuck_timer > 5f)
        {
            Debug.Log("Stuck in wall");
            AddReward(-5f);
            EndEpisode();
        }
    }
    
    public override void OnEpisodeBegin()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.CheckpointTriggerer.points = 0;
        has_touched_wall = false;
        Debug.ClearDeveloperConsole();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {   
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        carController.UpdateControls(forwardAmount, turnAmount, false);
    }
}
