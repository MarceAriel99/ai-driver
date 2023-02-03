using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasts : MonoBehaviour
{
    GameObject car;
    void Start()
    {
        car = GameObject.Find("COM");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // set raycast origin to car's center of mass
        Vector3 origin = car.transform.position;
        // set raycast directions to car's N NE E SE S SW W NW
        Vector3[] directions = new Vector3[8];
        directions[0] = car.transform.forward;
        directions[1] = car.transform.forward + car.transform.right;
        directions[2] = car.transform.right;
        directions[3] = car.transform.right - car.transform.forward;
        directions[4] = -car.transform.forward;
        directions[5] = -car.transform.forward - car.transform.right;
        directions[6] = -car.transform.right;
        directions[7] = -car.transform.right + car.transform.forward;

        float length = 20f;
        RaycastHit[] hits = new RaycastHit[8];

        // iterate from 0 to 7
        for (int i = 0; i < 8; i++)
        {
            // cast ray 
            if (Physics.Raycast(origin, directions[i], out hits[i], length) && hits[i].collider.gameObject.tag == "Wall") // TODO: add more tags for other cars
            {
                Debug.DrawRay(origin, directions[i] * hits[i].distance, Color.yellow);
                Debug.Log("Did Hit " + hits[i].collider.gameObject.name + " at " + hits[i].point + " with distance " + hits[i].distance + " and tag " + hits[i].collider.gameObject.tag + "");
            }
            else
            {
                Debug.DrawRay(origin, directions[i] * length, Color.white);
                Debug.Log("Did not Hit");
            }
            // TODO: collect data and send to neural network
        }
        

    }
}
