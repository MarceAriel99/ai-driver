using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public abstract class DriveAgent : Agent
{
    protected CarController carController;
    protected GameObject car;

    protected Rigidbody rb;

    protected CheckpointTriggerer CheckpointTriggerer;

    public GameObject startPoint;
    
    void Start()
    {
        car = this.gameObject;
        carController = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
        CheckpointTriggerer = GetComponent<CheckpointTriggerer>();
    }

    /* ABSTRACT METHODS */

    public abstract void LapCompleted();
    public abstract void OnCheckpointReached();

    /* AGENT OVERRIDES */

    public override void OnActionReceived(ActionBuffers actions)
    {   
        float forwardAmount = actions.ContinuousActions[0];
        float turnAmount = actions.ContinuousActions[1];

        carController.UpdateControls(forwardAmount, turnAmount, false); //TODO: add brake control
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }
}
