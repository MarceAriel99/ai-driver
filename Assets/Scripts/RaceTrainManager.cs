using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class RaceTrainManager : MonoBehaviour
{
    public GameObject[] cars;
    public Dictionary<GameObject, int> carsPositions;

    private Dictionary<GameObject, int> carsCurrentCheckpoints;

    public int finishLineCheckpointIndex;
    public int numberRepeatFinishLine = 15;
    public int checkpointsBetweenFinishLines = 2;
    public int counterRepeatFinishLine = 0;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Change this method if we paralelize the training
        cars = GameObject.FindGameObjectsWithTag("Car");
        carsPositions = new Dictionary<GameObject, int>();
        carsCurrentCheckpoints = new Dictionary<GameObject, int>();
        SetRaceTrainManagerForAllCars();
        SetFinishLineCheckpointIndexForAllCars();
    }

    void FixedUpdate()
    {
        UpdateCarsPositions();

        foreach (KeyValuePair<GameObject, int> carPosition in carsPositions)
        {
            Debug.Log(carPosition.Key.name + " is in position " + carPosition.Value);
        }
    }

    void UpdateCarsPositions()
    {
        // For each car, check if it is in a higher position than the other cars
        foreach (GameObject car in cars)
        {
            // Get the current checkpoint of the car
            int carCurrentCheckpoint = car.GetComponent<CheckpointTriggerer>().points;
            int carCurrentPosition = 1;

            // For each other car, check if it is in a higher position than the current car
            foreach (GameObject otherCar in cars)
            {
                if (otherCar != car)
                {
                    int otherCarCurrentCheckpoint = otherCar.GetComponent<CheckpointTriggerer>().points;

                    // If the other car is in a higher checkpoint, the current car is in a lower position
                    if (otherCarCurrentCheckpoint > carCurrentCheckpoint)
                    {
                        carCurrentPosition++;
                    }

                    // If the other car is in the same checkpoint, check the distance to the next checkpoint
                    else if (otherCarCurrentCheckpoint == carCurrentCheckpoint)
                    {   
                        // If the other car is closer to the next checkpoint, the current car is in a lower position
                        if (otherCar.GetComponent<CheckpointTriggerer>().distanceToNextCheckpoint < car.GetComponent<CheckpointTriggerer>().distanceToNextCheckpoint)
                        {
                            carCurrentPosition++;
                        }
                    }
                }
            }
            // This component should be cached
            // If the car has this component, set it's position
            if (car.GetComponent<DriveAgentM3>() != null){
                car.GetComponent<DriveAgentM3>().SetPosition(carCurrentPosition, cars.Length);
            }  
        }
    }

    public void LapCompleted(){
        
        counterRepeatFinishLine++;

        if (counterRepeatFinishLine == numberRepeatFinishLine)
        {
            counterRepeatFinishLine = 0;
            finishLineCheckpointIndex = finishLineCheckpointIndex + checkpointsBetweenFinishLines;
            if (finishLineCheckpointIndex > cars[0].GetComponent<CheckpointTriggerer>().checkpoints.Length)
            {
                finishLineCheckpointIndex = cars[0].GetComponent<CheckpointTriggerer>().checkpoints.Length;
            }
            SetFinishLineCheckpointIndexForAllCars();
        }
    }

    public void EndEpisodeForAllAgents(GameObject carCallingThisMethod)
    {
        foreach (GameObject car in cars)
        {   
            // Print acumulated reward for every car
            Debug.Log(car.name + "was resetted because" + carCallingThisMethod.name  + "won. It has acumulated a reward of " + car.GetComponent<DriveAgentM2>().GetCumulativeReward());
            car.GetComponent<DriveAgentM2>().EndEpisode();
        }
    }

    void SetRaceTrainManagerForAllCars()
    {
        foreach (GameObject car in cars)
        {
            if (car.GetComponent<DriveAgentM2>() != null) 
            {
                car.GetComponent<DriveAgentM2>().SetTrainManager(this);
            }
        }
    }

    void SetFinishLineCheckpointIndexForAllCars()
    {
        Debug.Log("Finish line checkpoint index is " + finishLineCheckpointIndex);
        foreach (GameObject car in cars)
        {
            car.GetComponent<CheckpointTriggerer>().finishLineCheckpointIndex = finishLineCheckpointIndex;
        }
    }
}
