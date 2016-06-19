using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CourseBehaviour : MonoBehaviour {

    public int numberOfLaps = 1;
    public List<GameObject> checkpoints;
    public List<GameObject> mapCheckpoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Checks drones for lap completion and updates required data
    void LapCheck () {
        
        for(int i = 0; i < checkpoints.Count; i++) {
            //this.transform.GetChild(i).GetComponentInChildren<CheckpointBehaviour>().droneList[i];
        }
    }
}
