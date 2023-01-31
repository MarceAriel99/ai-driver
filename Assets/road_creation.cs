using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class road_creation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // get road system object from RoadArchitect namespace
        GameObject roadSystem = GameObject.Find("RoadSystem");
        RoadArchitect.RoadSystem rs = roadSystem.GetComponent<RoadArchitect.RoadSystem>();

        // generate random road with 10 nodes and 10 segments within 100x100 area
        GameObject road = rs.AddRoad();
        //Road r = road.GetComponent<Road>();
        //r.GenerateRoad(10, 10, 100, 100);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
