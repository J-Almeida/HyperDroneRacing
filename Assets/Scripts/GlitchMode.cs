﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlitchMode : MonoBehaviour
{
    NewDroneAudio DroneSoundController;

    public float glitchDuration = 3f;
    // public AudioSource glitchAudio;

    float GlitchStamina = 1.0f; // current ghost stamina [0-1]
    private bool UsingGlitch;
    private float GlitchTimer;
    
    [SerializeField]
    private Sprite GlitchIcon_enabled;
    [SerializeField]
    private Image GlitchBar;
    [SerializeField]
    private Image GlitchIcon;
    [SerializeField]
    private Image GlitchMeter;
    
    public KeyCode GlitchKey;

    int GlitchType = 0;
    // 0 - vertical stabilization malfunction
    // 1 - motor malfunction
    float GlitchAngle = 0f;

    public Text GlitchText;

    public enum ControlState
    {
        Gamepad,
        KeyBoard
    };
    public ControlState ControlType = new ControlState();

    // Use this for initialization
    void Start()
    {
        DroneSoundController = this.GetComponent<NewDroneAudio>();
        UsingGlitch = false;
        GlitchTimer = glitchDuration;

        GlitchBar.enabled = false;
        GlitchMeter.enabled = false;
        GlitchIcon.enabled = false;
    }

    private void enableGlitch()
    {
        if (UsingGlitch) return;

        GlitchType = (int) Mathf.RoundToInt(Random.Range(0f, 1f));

        // the actual glitch goes here if it's (just) a value set and not a continuous effect
        if (GlitchType == 0)
            this.GetComponent<NewDroneController>().hoverValue *= 0.25f;
        else if (GlitchType == 1)
            GlitchAngle = Random.Range(30f, 50.0f);
        // the glitch ends here

        GlitchStamina = 1.0f;
        glitchDuration = 3f;
        GlitchTimer = glitchDuration;
        UsingGlitch = true;
        GlitchBar.enabled = true;
        GlitchMeter.enabled = true;
        GlitchIcon.enabled = true;
        // glitchAudio.Play();

        GlitchText.enabled = true;
        if (GlitchType == 0) {
            GlitchText.text = "Vertical Stabilization Malfunction!";
            DroneSoundController.PlaySound("glitch");
        }
        else if (GlitchType == 1)
        {
            GlitchText.text = "One motor is glitching!";
            DroneSoundController.PlaySound("glitch_2");
        }
    }

    private void disableGlitch()
    {
        GlitchStamina = 0.0f;
        UsingGlitch = false;
        this.GetComponent<NewDroneController>().ResetHoverValue();
        GlitchTimer = glitchDuration; // ?
        GlitchBar.enabled = false;
        GlitchMeter.enabled = false;
        GlitchIcon.enabled = false;
        // glitchAudio.Stop();

        if (GlitchType == 0)
            DroneSoundController.StopSound("glitch");
        else if (GlitchType == 1)
            DroneSoundController.StopSound("glitch_2");
        GlitchText.text = "";
        GlitchText.enabled = false;
    }

    void FixedUpdate()
    {
        if (ControlType == ControlState.KeyBoard)
        {
            if (Input.GetKey(GlitchKey) || Input.GetKey("joystick button 4") || UsingGlitch)
            {
                // print("entering glitch");
                if (!UsingGlitch) enableGlitch();
                UseGlitch();
            }
            else
            {
                UsingGlitch = false;
            }
        }
        else if (ControlType == ControlState.Gamepad)
        {
            if (Input.GetKey("joystick button 4") || UsingGlitch)
            {
                // print("entering glitch");
                if (!UsingGlitch) enableGlitch();
                UseGlitch();
            }
            else
            {
                UsingGlitch = false;
            }
        }
    }

    void UseGlitch()
    {
        // actual glitch happens here if it's a continuous effect
        if (GlitchType == 1)
        {
            Vector3 rotationVector = new Vector3(GlitchAngle, 0f, GlitchAngle);
            // applies rotation
            transform.Rotate(rotationVector * Time.fixedDeltaTime);
            Vector3 currentDirection = this.transform.TransformDirection(this.transform.localPosition); // igual a position normal?
            // finds a direction for the applied force through the cross product of the drone's local position in global coordinates and the applied rotation
            // 20 is a possible magnitude for an average horizontal force applied to the drone
            Vector3 cross = Vector3.Cross(currentDirection, rotationVector).normalized * 20f;
            // negates vertical component of resulting force
            cross.y = 0f;
            GetComponent<Rigidbody>().AddForce(cross);
        }

        UsingGlitch = true;
        GlitchStamina -= Time.fixedDeltaTime * 0.25f;
        GlitchMeter.fillAmount = GlitchStamina;

        if (GlitchStamina < 0.0f)
        {
            disableGlitch();
        }
    }

    IEnumerator GlitchBarBlink()
    {
        // GlitchIcon.sprite = GhostIcon_disabled;
        GlitchBar.enabled = false;
        GlitchMeter.enabled = false;

        for (int i = 0; i < 10; i++)
        {
            if (i % 2 == 0)
                GlitchBar.enabled = false;
            else
                GlitchBar.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        // Ghost ends here
        GlitchBar.enabled = true;
        GlitchMeter.fillAmount = GlitchStamina;
        GlitchIcon.sprite = GlitchIcon_enabled;
    }
}
