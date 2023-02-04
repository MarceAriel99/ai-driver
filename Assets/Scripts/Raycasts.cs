using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasts : MonoBehaviour
{   
    public bool show_raycast_line;
    public bool debug_raycast_console;

    public float length = 50f;

    Transform car;
    RaycastHit hit;
    bool raycast_hitted = false;

    Vector3 origin;
    Vector3[] directions = new Vector3[8];

    void Start()
    {   
        // Get car's transform
        car = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set raycast origin to car's center
        origin = car.position + new Vector3(0, 0.5f, 0);

        // Set raycast directions to car's N NE E SE S SW W NW
        directions[0] = car.forward;
        directions[1] = car.forward + car.right;
        directions[2] = car.right;
        directions[3] = car.right - car.forward;
        directions[4] = -car.forward;
        directions[5] = -car.forward - car.right;
        directions[6] = -car.right;
        directions[7] = -car.right + car.forward;

        // Normalize directions
        for (int i = 0; i < 8; i++)
        {
            directions[i] = directions[i].normalized;
        }
        
        // Iterate through raycasts
        for (int i = 0; i < 8; i++)
        {
            // Cast ray 
            raycast_hitted =  Physics.Raycast(origin, directions[i], out hit, length);

            // TODO: send data to neural network
            
            // Debug raycasts
            if (show_raycast_line)
                DrawRaycast(origin, directions[i], length, hit, i, raycast_hitted);
            if (debug_raycast_console)
                LogRaycast(origin, directions[i], length, hit, i, raycast_hitted);
        }
    }

    private void DrawRaycast(Vector3 origin, Vector3 direction, float length, RaycastHit hit, int raycast_number, bool raycast_hitted)
    {
        if (raycast_hitted && hit.collider.gameObject.tag == "Wall")
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(origin, direction * length, Color.white);
        }
    }

    private void LogRaycast(Vector3 origin, Vector3 direction, float length, RaycastHit hit, int raycast_number, bool raycast_hitted)
    {
        if (raycast_hitted && hit.collider.gameObject.tag == "Wall")
        {
            Debug.Log("Raycast number " + raycast_number + " did hit " + hit.collider.gameObject.name + " at " + hit.point + " with distance " + hit.distance + " and tag " + hit.collider.gameObject.tag);
        }
        else
        {
            Debug.Log("Raycast number " + raycast_number + " did not hit anything");
        }
    }
}
