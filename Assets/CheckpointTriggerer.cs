using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckpointTriggerer : MonoBehaviour
{
    // points counter variable
    public int points = 0;

    // checkpoint trigger only if sequence is correct
    private void OnTriggerEnter(Collider other)
    {
        // get Checkpoints GameObject
        GameObject checkpoints = GameObject.Find("Checkpoints");
        // get Checkpoints children
        GameObject[] checkpointsChildren = checkpoints.GetComponentsInChildren<GameObject>(); // FIXME: anda mal 
        // print checkpoints children
        Debug.Log(checkpointsChildren);
        foreach (GameObject checkpoint in checkpointsChildren)
        {
            Debug.Log(checkpoint);
        }
        // check if sequence is correct
        if (other.gameObject == checkpointsChildren[points])
        {
            // increase points
            points++;
            Debug.Log( "Checkpoint " + points + " reached!" );
            // destroy checkpoint
            Destroy(other.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
