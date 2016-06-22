using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GhostMode : MonoBehaviour {

    NewDroneAudio DroneSoundController;
    public Collider colliderToDisable;
    public float ghostDuration = 3f;
    public float ghostCooldown = 5f;
    // public AudioSource ghostAudio;
    public Renderer[] renderers;
    public Shader ghostShader;

    float GhostStamina = 1.0f; // current ghost stamina [0-1]
    private bool UsingGhost;
    private float ghostTimer;
    private float ghostCooldownTimer;
    private Color[] originalColors;
    private Shader[] originalShaders;

    bool GhostOnCooldown = false;

    [SerializeField]
    private Sprite GhostIcon_enabled;
    [SerializeField]
    private Sprite GhostIcon_disabled;
    [SerializeField]
    private Image GhostBar;
    [SerializeField]
    private Image GhostIcon;
    [SerializeField]
    private Image GhostMeter;

    // Use this for initialization
    void Start() {
        DroneSoundController = this.GetComponent<NewDroneAudio>();
        UsingGhost = false;
        ghostTimer = ghostDuration;
        originalColors = new Color[renderers.Length];
        originalShaders = new Shader[renderers.Length];

        for(int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
            originalShaders[i] = renderers[i].material.shader;
        }
    }
	
    private void enableGhost()
    {
        if (UsingGhost) return;

        colliderToDisable.isTrigger = true;
        UsingGhost = true;
        // ghostAudio.Play();
        DroneSoundController.PlaySound("ghost");
        ghostCooldownTimer = ghostCooldown;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.shader = ghostShader;
        }
    }

    private void disableGhost()
    {
        ghostTimer = ghostDuration;
        colliderToDisable.isTrigger = false;
        UsingGhost = false;
        // ghostAudio.Stop();
        DroneSoundController.StopSound("ghost");
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.shader = originalShaders[i];
        }
        
    }
    
	void FixedUpdate () {
       if (Input.GetKey("joystick button 2") || UsingGhost)
        {
            UseGhost();
        }
        else
        {
            UsingGhost= false;
            ChargeGhost();
        }

    }

    void UseGhost()
    {
        if (GhostOnCooldown) return;

        // enableGhost();

        UsingGhost = true;
        GhostStamina -= Time.fixedDeltaTime * 0.25f;
        GhostMeter.fillAmount = GhostStamina;

        // if (currentBoost > 1.0f) currentBoost = 1.0f;
        if (GhostStamina< 0.0f)
        {
            GhostStamina = 0.0f;
            UsingGhost = false;
            GhostOnCooldown = true;
            GhostCoolDown();
            disableGhost();
        }
    }

    void ChargeGhost()
    {
        if (GhostOnCooldown) return;

        GhostStamina += Time.deltaTime * 0.15f;
        GhostMeter.fillAmount = GhostStamina;

        if (GhostStamina > 1.0f)
        {
            GhostStamina = 1.0f;
        }
    }

    void GhostCoolDown()
    {
        StartCoroutine("GhostBarBlink");
    }

    IEnumerator GhostBarBlink()
    {
        GhostIcon.sprite = GhostIcon_disabled;

        for (int i = 0; i < 10; i++)
        {
            if (i % 2 == 0)
                GhostBar.enabled = false;
            else
                GhostBar.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        // Ghost ends here
        GhostBar.enabled = true;
        GhostOnCooldown = false;
        GhostMeter.fillAmount = GhostStamina;
        GhostIcon.sprite = GhostIcon_enabled;
    }
}
