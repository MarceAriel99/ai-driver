using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class checkpoints_prefabs : MonoBehaviour
{
    public GameObject checkpoint;
    public List<GameObject> checkpoint_list = new List<GameObject>();

    public bool reverse = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            checkpoint_list.Add(child.gameObject);
        }

        if (reverse)
        {
            checkpoint_list.Reverse();
        }

        foreach (GameObject checkpoint in checkpoint_list)
        {
            InstantiatePrefab(checkpoint.transform.position, checkpoint.transform.rotation);
        }

        #if UNITY_EDITOR
        /// The fix is to change the 'Save' call to MarkSceneDirty() as answered by @Ron. 
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        #endif
    }

    void InstantiatePrefab(Vector3 position, Quaternion rotation)
    {
        GameObject _newCheckpointInstance = PrefabUtility.InstantiatePrefab(checkpoint) as GameObject;

        _newCheckpointInstance.transform.position = position;
        _newCheckpointInstance.transform.rotation = rotation;
        _newCheckpointInstance.transform.parent = transform;
    }
}
