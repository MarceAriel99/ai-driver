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
        Vector3 origin = car.transform.position + new Vector3(0, 0.5f, 0);
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

        // Normalize directions
        for (int i = 0; i < 8; i++)
        {
            directions[i] = directions[i].normalized;
        }

        float length = 50f;
        bool raycast_hitted = false;

        // iterate from 0 to 7
        for (int i = 0; i < 8; i++)
        {
            RaycastHit hit;

            // Cast ray 
            raycast_hitted =  Physics.Raycast(origin, directions[i], out hit, length);

            // TODO: send data to neural network
            
            // Debug raycasts
            //DebugRaycast(origin, directions[i], length, hit, i, raycast_hitted);
        }
    }

    private void DebugRaycast(Vector3 origin, Vector3 direction, float length, RaycastHit hit, int raycast_number, bool raycast_hitted)
    {
        if (raycast_hitted && hit.collider.gameObject.tag == "Wall")
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
            Debug.Log("Raycast number " + raycast_number + " did hit " + hit.collider.gameObject.name + " at " + hit.point + " with distance " + hit.distance + " and tag " + hit.collider.gameObject.tag);
        }
        else
        {
            Debug.DrawRay(origin, direction * length, Color.white);
            Debug.Log("Raycast number " + raycast_number + " did not hit anything");
        }
    }
}
