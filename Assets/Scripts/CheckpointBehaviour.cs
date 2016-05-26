using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointBehaviour : MonoBehaviour {

    [Tooltip("Checkpoint previous to this one.")]
    public GameObject previous;
    [Tooltip("List of drones that already have crossed this checkpoint.")]
    public List<GameObject> droneList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        //check if drone has already crossed
        foreach (var drone in droneList) {
            if (drone == other.transform.root.gameObject)
                return;
        }

        //add drone if previous checkpoint has had the same drone cross it
        if (previous == null) {
            droneList.Add(other.transform.root.gameObject);
            this.transform.root.SendMessage("LapCheck");
        } else {
            foreach (var drone in previous.GetComponentInChildren<CheckpointBehaviour>().droneList) {
                if (drone == other.transform.root.gameObject) {
                    droneList.Add(other.transform.root.gameObject);
                    this.transform.root.SendMessage("LapCheck");
                } else
                    return;
            }
        }
    }
}
