using UnityEngine;
using System.Collections;
using System;

public class StageSelectController : MonoBehaviour
{

    string[] StageNames = new string[2] { "Map01", "Unavailable Map" };
    int SelectedStageIndex = 0;

    GameObject StageDisplay;
    GameObject DoneButton;

    GameObject SelectedObject;

    GameObject[] Selectables;
    int selectedIndex = 0;

    [Tooltip("Map01 UnavailableMap")]
    public Sprite[] StageSprites;

    // Use this for initialization
    void Start()
    {
        StageDisplay = GameObject.Find("StageDisplay");
        DoneButton = GameObject.Find("DoneButton");

        Selectables = new GameObject[2] { StageDisplay, DoneButton };

        SelectedObject = Selectables[selectedIndex];
        EnableArrows(ref SelectedObject);
    }

    private void DisableArrows(ref GameObject selectedObject)
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            GameObject current = selectedObject.transform.GetChild(i).gameObject;
            if (current.name.Contains("Arrow"))
                current.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
    }

    private void EnableArrows(ref GameObject selectedObject)
    {
        for (int i = 0; i < selectedObject.transform.childCount; i++)
        {
            GameObject current = selectedObject.transform.GetChild(i).gameObject;
            if (current.name.Contains("Arrow"))
                current.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
    }

    private void IncreaseArrowIndex()
    {
        DisableArrows(ref SelectedObject);
        selectedIndex++;
        if (selectedIndex >= Selectables.Length)
            selectedIndex = 0;
        SelectedObject = Selectables[selectedIndex];
        EnableArrows(ref SelectedObject);
    }

    private void DecreaseArrowIndex()
    {
        DisableArrows(ref SelectedObject);
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = Selectables.Length - 1;
        SelectedObject = Selectables[selectedIndex];
        EnableArrows(ref SelectedObject);
    }

    private void NextStage()
    {
        print("next stage");
        SelectedStageIndex++;
        if (SelectedStageIndex >= StageNames.Length)
            SelectedStageIndex = 0;
        StageDisplay.GetComponent<UnityEngine.UI.Image>().sprite = StageSprites[SelectedStageIndex];
    }

    private void PreviousStage()
    {
        print("previous stage");
        SelectedStageIndex--;
        if (SelectedStageIndex < 0)
            SelectedStageIndex = StageNames.Length - 1;
        StageDisplay.GetComponent<UnityEngine.UI.Image>().sprite = StageSprites[SelectedStageIndex];
    }

    private void NextItem(ref GameObject selected)
    {
        switch (selected.name)
        {
            case "StageDisplay":
                NextStage();
                break;
        }
    }

    private void PreviousItem(ref GameObject selected)
    {
        switch (selected.name)
        {
            case "StageDisplay":
                PreviousStage();
                break;
        }
    }

    void Done()
    {
        if (selectedIndex == 1)
            UnityEngine.SceneManagement.SceneManager.LoadScene("DroneSelect");
        else
            print("Error. SelectedIndex = " + selectedIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            DecreaseArrowIndex();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            IncreaseArrowIndex();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            NextItem(ref SelectedObject);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            PreviousItem(ref SelectedObject);

        else if (Input.GetKeyDown(KeyCode.Return))
            Done();
    }
}
