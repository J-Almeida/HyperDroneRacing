using UnityEngine;
using System.Collections;
using System;

public class DroneSelectController : MonoBehaviour {

    string[] DroneBodies = new string[2] { "IronMan", "GreenGoblin" };
    int SelectedDroneBodyIndex = 0;
    string[] BonusSkills = new string[2]{ "Boost", "Ghost" };
    int SelectedBonusSkillIndex = 0;

    GameObject DroneBodyDisplay;
    GameObject BonusSkillDisplay;
    GameObject TopSpeedDisplay;
    GameObject HandlingDisplay;
    GameObject AccelerationDisplay;
    GameObject WeightDisplay;
    GameObject DoneButton;

    GameObject SelectedObject;

    GameObject[] Selectables;
    int selectedIndex = 0;

    [Tooltip("IronMan GreenGoblin")]
    public Sprite[] DroneNameSprites;
    [Tooltip("Boost Ghost")]
    public Sprite[] BonusSkillSprites;

    GameObject ActiveDrone;

    // Use this for initialization
    void Start() {
        DroneBodyDisplay = GameObject.Find("DroneBodyDisplay");
        BonusSkillDisplay = GameObject.Find("BonusSkillDisplay");
        TopSpeedDisplay = GameObject.Find("TopSpeedDisplay");
        HandlingDisplay = GameObject.Find("HandlingDisplay");
        AccelerationDisplay = GameObject.Find("AccelerationDisplay");
        WeightDisplay = GameObject.Find("WeightDisplay");
        DoneButton = GameObject.Find("DoneButton");

        Selectables = new GameObject[3] { DroneBodyDisplay, BonusSkillDisplay, DoneButton};

        SelectedObject = Selectables[selectedIndex];
        EnableArrows(ref SelectedObject);

        ActiveDrone = GameObject.Find("Drone");
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

    private void NextDroneBody()
    {
        SelectedDroneBodyIndex++;
        if (SelectedDroneBodyIndex >= DroneBodies.Length)
            SelectedDroneBodyIndex = 0;
        DroneBodyDisplay.GetComponent<UnityEngine.UI.Image>().sprite = DroneNameSprites[SelectedDroneBodyIndex];
    }

    private void PreviousDroneBody()
    {
        SelectedDroneBodyIndex--;
        if (SelectedDroneBodyIndex < 0)
            SelectedDroneBodyIndex = DroneBodies.Length - 1;
        DroneBodyDisplay.GetComponent<UnityEngine.UI.Image>().sprite = DroneNameSprites[SelectedDroneBodyIndex];
    }

    private void NextBonusSkill()
    {
        SelectedBonusSkillIndex++;
        if (SelectedBonusSkillIndex >= BonusSkills.Length)
            SelectedBonusSkillIndex = 0;
        BonusSkillDisplay.GetComponent<UnityEngine.UI.Image>().sprite = BonusSkillSprites[SelectedBonusSkillIndex];
    }

    private void PreviousBonusSkill()
    {
        SelectedBonusSkillIndex--;
        if (SelectedBonusSkillIndex < 0)
            SelectedBonusSkillIndex = BonusSkills.Length - 1;
        BonusSkillDisplay.GetComponent<UnityEngine.UI.Image>().sprite = BonusSkillSprites[SelectedBonusSkillIndex];
    }

    private void NextItem(ref GameObject selected)
    {
        switch (selected.name)
        {
            case "DroneBodyDisplay":
                NextDroneBody();
                break;
            case "BonusSkillDisplay":
                NextBonusSkill();
                break;
        }
    }

    private void PreviousItem(ref GameObject selected)
    {
        switch (selected.name)
        {
            case "DroneBodyDisplay":
                PreviousDroneBody();
                break;
            case "BonusSkillDisplay":
                PreviousBonusSkill();
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            DecreaseArrowIndex();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            IncreaseArrowIndex();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            NextItem(ref SelectedObject);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            PreviousItem(ref SelectedObject);
        /*
        else if (Input.GetKeyDown(KeyCode.Return) && (selectedIndex == Selectables.Length - 1) ) // activates the 'done' button (last item on Selectables array)
            // TODO o que faz o botão done
        */

        ActiveDrone.transform.Rotate(new Vector3(0f, 10 * Time.fixedDeltaTime, 0f));
    }
}
