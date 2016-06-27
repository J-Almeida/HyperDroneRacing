using UnityEngine;
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

    int GlitchType = 0; // TODO texto no ecrã!
    // 0 - vertical stabilization malfunction
    // 1 - motor malfunction
    float GlitchAngle = 0f;

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

        GlitchType = 1;

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
        DroneSoundController.PlaySound("glitch");
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
        DroneSoundController.StopSound("glitch");
    }

    void FixedUpdate()
    {
        if (Input.GetKey(GlitchKey) || Input.GetKey("joystick button 4") || UsingGlitch)
        {
            print("entering glitch");
            if (!UsingGlitch) enableGlitch();
            UseGlitch();
        }
        else
        {
            UsingGlitch = false;
        }

    }

    void UseGlitch()
    {
        // actual glitch happens here if it's a continuous effect
        if (GlitchType == 1)
        {
            print("rotationing");
            Vector3 GlitchDirection = new Vector3(GlitchAngle, 0f, GlitchAngle);
            // applies rotation
            transform.Rotate(GlitchDirection * Time.fixedDeltaTime);

            // applies force in same direction
            Vector3 transformedGlitchForce = this.transform.TransformVector(GlitchDirection);
            transformedGlitchForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedGlitchForce);
            

            /*
            Vector3 newForce = new Vector3(GlitchAngle, 0f, GlitchAngle);
            Vector3 transformedNewForce = this.transform.TransformVector(newForce);
            transformedNewForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedNewForce);
            */
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
