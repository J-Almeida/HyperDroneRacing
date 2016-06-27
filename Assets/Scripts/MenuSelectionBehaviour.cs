using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuSelectionBehaviour : MonoBehaviour {

    public int currentSelectionIndex = 0;
    public List<GameObject> options;

    private bool started = true;

    //private AudioSource = new AudioSource();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("joystick button 4") || Input.GetKeyDown("up")) {
            currentSelectionIndex--;
            GetComponents<AudioSource>()[1].Play();
        }
        if (Input.GetKeyDown("joystick button 6") || Input.GetKeyDown("down")) {
            currentSelectionIndex++;
            GetComponents<AudioSource>()[1].Play();
        }
        if (Input.GetKeyDown("joystick button 13") || Input.GetKeyDown(KeyCode.Return)) {
            if (started) {
                switch (currentSelectionIndex) {
                    case 0:
                        UnityEngine.SceneManagement.SceneManager.LoadScene("RITAscene");
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        Application.Quit();
                        break;
                }
            } else {
                started = true;
            }

            GetComponents<AudioSource>()[2].Play();
        }

        if (currentSelectionIndex >= options.Count)
            currentSelectionIndex = 0;
        if (currentSelectionIndex < 0)
            currentSelectionIndex = options.Count - 1;

        foreach (var option in options)
            option.SetActive(false);
        options[currentSelectionIndex].SetActive(true);
    }
}
