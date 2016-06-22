using UnityEngine;
using System.Collections;

public class GhostMode : MonoBehaviour {
    public Collider colliderToDisable;
    public float ghostDuration;
    public float ghostCooldown;
    public AudioSource ghostAudio;
    public Renderer[] renderers;
    public Shader ghostShader;

    private bool ghostEngaged;
    private float ghostTimer;
    private float ghostCooldownTimer;
    private Color[] originalColors;
    private Shader[] originalShaders;


    // Use this for initialization
    void Start() {
        ghostEngaged = false;
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
        colliderToDisable.isTrigger = true;
        ghostEngaged = true;
        ghostAudio.Play();
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
        ghostEngaged = false;
        ghostAudio.Stop();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.shader = originalShaders[i];
        }
        
    }

	// Update is called once per frame
	void Update () {
        if(ghostEngaged)
        {
            ghostTimer -= Time.deltaTime;

            if(ghostTimer <= 0)
            {
                disableGhost();
            }
        }
        else
        {
            if(ghostCooldownTimer > 0.0f)
            {
                ghostCooldownTimer -= Time.deltaTime;
            }
            else
            {
                ghostCooldownTimer = 0.0f;
            }
                
            if (ghostCooldownTimer <= 0.0f && Input.GetKeyDown("joystick button 2"))
            {
                enableGhost();
            }
        }

        
    }
}
