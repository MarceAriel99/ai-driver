using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class RaceTrainManager : MonoBehaviour
{
    public GameObject[] cars;
    public Dictionary<GameObject, int> carsPositions;

    private Dictionary<GameObject, int> carsCurrentCheckpoints;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Change this method if we paralelize the training
        cars = GameObject.FindGameObjectsWithTag("Car");
        carsPositions = new Dictionary<GameObject, int>();
        carsCurrentCheckpoints = new Dictionary<GameObject, int>();
        SetRaceTrainManagerForAllCars();
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
            car.GetComponent<DriveAgentM2>().SetPosition(carCurrentPosition, cars.Length);
        }
    }

    public void EndEpisodeForAllAgents(GameObject carCallingThisMethod)
    {
        foreach (GameObject car in cars)
        {   
            car.GetComponent<DriveAgentM2>().EndEpisode();
            /*
            if (car != carCallingThisMethod){
                
            }
            */
        }
    }

    void SetRaceTrainManagerForAllCars()
    {
        foreach (GameObject car in cars)
        {
            car.GetComponent<DriveAgentM2>().SetTrainManager(this);
        }
    }
}
