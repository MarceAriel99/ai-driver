using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class DriveAgent : Agent
{
    public override void OnActionReceived(ActionBuffers actions)
    {   
        foreach (var action in actions.DiscreteActions)
        {
            //Debug.Log(action);
            Debug.Log("HI");
        }

        //base.OnActionReceived(actions);
    }
}
