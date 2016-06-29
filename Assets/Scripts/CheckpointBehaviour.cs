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
        //check if drone has already crossed, and finish if so
        foreach (var drone in droneList) {
            if (drone == other.transform.root.gameObject)
                return;
        }

        //add drone if previous checkpoint has had the same drone cross it
        if (getCurrentIndex() == 0) {
            droneList.Add(other.transform.root.gameObject);
            this.transform.root.SendMessage("LapCheck");
        } else {
            foreach (var drone in this.transform.root.GetComponent<CourseBehaviour>().checkpoints[getCurrentIndex() - 1].GetComponentInChildren<CheckpointBehaviour>().droneList) {
                if (drone == other.transform.root.gameObject) {
                    droneList.Add(other.transform.root.gameObject);
                    this.transform.root.SendMessage("LapCheck");

                    if (getCurrentIndex() == this.transform.root.GetComponent<CourseBehaviour>().checkpoints.Count - 1) {
                        //other.GetComponent<DroneBehaviour>().currentNumberOfLaps++; //old version
                        other.GetComponent<NewDroneController>().IncreaseCurrentNumberOfLaps(); //new version
                        deleteDroneFromAllCheckPoints(drone);

                        if (other.GetComponent<NewDroneController>().currentNumberOfLaps > 3)
                            other.GetComponent<NewDroneController>().Win();
                    }
                }
            }
        }

        //make next checkpoint highlighted
        if (getCurrentIndex() + 1 >= this.transform.root.GetComponent<CourseBehaviour>().checkpoints.Count) {
            this.transform.root.GetComponent<CourseBehaviour>().checkpoints[0].SetActive(true);
            this.transform.root.GetComponent<CourseBehaviour>().mapCheckpoints[this.transform.root.GetComponent<CourseBehaviour>().checkpoints.Count - 1].SetActive(true);
            this.transform.root.GetComponent<CourseBehaviour>().mapCheckpoints[0].SetActive(false);
        } else {
            this.transform.root.GetComponent<CourseBehaviour>().checkpoints[getCurrentIndex() + 1].SetActive(true);
            this.transform.root.GetComponent<CourseBehaviour>().mapCheckpoints[getCurrentIndex() + 1].SetActive(false);
            this.transform.root.GetComponent<CourseBehaviour>().mapCheckpoints[getCurrentIndex()].SetActive(true);
        }
        //make this one regular again
        this.transform.root.GetComponent<CourseBehaviour>().checkpoints[getCurrentIndex()].SetActive(false);

        //play checkpoint sound
        this.transform.root.GetComponent<AudioSource>().Play();
    }

    //gets this checkpoint index in course
    int getCurrentIndex() {
        for(int i = 0; i < this.transform.root.GetComponent<CourseBehaviour>().checkpoints.Count; i++) {
            if (this.transform.parent.gameObject == this.transform.root.GetComponent<CourseBehaviour>().checkpoints[i].gameObject)
                return i;
        }

        return -1;
    }

    void deleteDroneFromAllCheckPoints(GameObject drone) {
        foreach(var checkPoint in this.transform.root.GetComponent<CourseBehaviour>().checkpoints) {
            List<GameObject> tmp = new List<GameObject>(checkPoint.GetComponentInChildren<CheckpointBehaviour>().droneList);
            for (int i = 0; i < tmp.Count; i++) {
                if (tmp[i] == drone) {
                    tmp.RemoveAt(i);
                    i--;
                }
            }
            checkPoint.GetComponentInChildren<CheckpointBehaviour>().droneList = tmp;
        }
    }
}
