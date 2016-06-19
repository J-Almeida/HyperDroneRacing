using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuSelectionBehaviour : MonoBehaviour {

    public int currentSelectionIndex = 0;
    public List<GameObject> options;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("up")) {
            currentSelectionIndex--;
        }
        if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("down")) {
            currentSelectionIndex++;
        }
        if (Input.GetKeyDown("joystick button 13") || Input.GetKeyDown(KeyCode.Return)) {
            Application.LoadLevel(1);
        }

        if (currentSelectionIndex >= options.Count)
            currentSelectionIndex = 0;
        if (currentSelectionIndex < 0)
            currentSelectionIndex = options.Count - 1;

        this.transform.position = options[currentSelectionIndex].transform.position;
    }
}
