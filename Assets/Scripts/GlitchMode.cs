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

        this.GetComponent<NewDroneController>().hoverValue *= 0.25f;

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
        if (Input.GetKey("joystick button 4") || UsingGlitch)
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
        // o glitch em si acontece aqui se for um efeito contínuo e não uma mudança como o hovervalue
        // glitch()

        // enableGlitch();

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
