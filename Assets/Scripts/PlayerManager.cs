using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class PlayerManager : MonoBehaviour, DriveAgentInterface
{
    public WheelCollider[] wheelColliders;

    public GameObject car;
    public Rigidbody rb;
    public GameObject startPoint;
    public CheckpointTriggerer checkpointTriggerer;

    public RaceTrainManager raceTrainManager { get; set; }

    private CarController carController;

    private void Update()
    {
        carController.UpdateControls(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetButton("Jump"));
    }

    /* AGENT OVERRIDES */

    public void OnCheckpointReached()
    {
        //Debug.Log("Player reached a checkpoint.");
    }

    public void LapCompleted()
    {
        Debug.Log("Player completed a lap.");
        raceTrainManager.LapCompleted();
        //RaceCompleted();
    }

    public void RaceCompleted()
    {
        raceTrainManager.EndEpisodeForAllAgents(transform.gameObject);
    }

    public void Start()
    {
        carController = GetComponent<CarController>();
        car = GameObject.Find("PlayerCar");
        rb = car.GetComponent<Rigidbody>();
        checkpointTriggerer = GetComponent<CheckpointTriggerer>();
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        GetComponent<CheckpointTriggerer>().points = 0;

        Debug.ClearDeveloperConsole();
    }

    public void SetTrainManager(RaceTrainManager raceTrainManager)
    {
        this.raceTrainManager = raceTrainManager;
    }

    public void ResetCar()
    {
        car.transform.position = startPoint.transform.position;
        car.transform.rotation = startPoint.transform.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        checkpointTriggerer.points = 0;
        checkpointTriggerer.currentLap = 0;  
    }
}
