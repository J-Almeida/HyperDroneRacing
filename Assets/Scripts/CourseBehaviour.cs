using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CourseBehaviour : MonoBehaviour {

    public List<GameObject> checkpoints;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Checks drones for lap completion and updates required data
    void LapCheck () {
        //TODO: Use map to count checkpoints per drone
        //List<GameObject> 

        for(int i = 0; i < this.transform.childCount; i++) {
            //this.transform.GetChild(i).GetComponentInChildren<CheckpointBehaviour>().droneList[i];
        }
    }
}
