using System;
using System.Collections;
using UnityEngine;

public class NewDroneAudio : MonoBehaviour
{

    [Serializable]
    public class AdvancedSetttings // A class for storing the advanced options.
    {
        // public float engineMinDistance = 50f;                   // The min distance of the engine audio source.
        // public float engineMaxDistance = 1000f;                 // The max distance of the engine audio source.
        public float engineDopplerLevel = 1f;                   // The doppler level of the engine audio source.
        [Range(0f, 1f)]
        public float engineMasterVolume = 0.5f; // An overall control of the engine sound volume.
        /*
        // Wind
        public float windMinDistance = 10f;                     // The min distance of the wind audio source.
        public float windMaxDistance = 100f;                    // The max distance of the wind audio source.
        public float windDopplerLevel = 1f;                     // The doppler level of the wind audio source.
        [Range(0f, 1f)]
        public float windMasterVolume = 0.5f;   // An overall control of the wind sound volume.
        */
    }

    [SerializeField]
    private AudioClip m_EngineSound;                     // Looped engine sound, whose pitch and volume are affected by the plane's throttle setting.
    [SerializeField]
    private float m_EngineMinThrottlePitch = 0.4f;       // Pitch of the engine sound when at minimum throttle.
    [SerializeField]
    private float m_EngineMaxThrottlePitch = 2f;         // Pitch of the engine sound when at maximum throttle.
    [SerializeField]
    private float m_EngineFwdSpeedMultiplier = 0.002f;   // Additional multiplier for an increase in pitch of the engine from the plane's speed.
    /*
    // Wind
    [SerializeField]
    private AudioClip m_WindSound;                       // Looped wind sound, whose pitch and volume are affected by the plane's velocity.
    [SerializeField]
    private float m_WindBasePitch = 0.2f;                // starting pitch for wind (when plane is at zero speed)
    [SerializeField]
    private float m_WindSpeedPitchFactor = 0.004f;       // Relative increase in pitch of the wind from the plane's speed.
    [SerializeField]
    private float m_WindMaxSpeedVolume = 100;            // the speed the aircraft much reach before the wind sound reaches maximum volume.
    */

    [SerializeField]
    private AdvancedSetttings m_AdvancedSetttings = new AdvancedSetttings();// container to make advanced settings appear as rollout in inspector

    private AudioSource m_EngineSoundSource;  // Reference to the AudioSource for the engine.
    // private AudioSource m_WindSoundSource;    // Reference to the AudioSource for the wind.
    private NewDroneController m_drone;      // Reference to the aeroplane controller.
    private Rigidbody m_Rigidbody;

    // boost
    bool boostSoundPlaying = false;
    public AudioClip m_BoostSound;
    AudioSource m_BoostSoundSource;

    // checkpoint
    bool checkpointSoundPlaying = false;
    public AudioClip m_CheckpointSound;
    AudioSource m_CheckpointSoundSource;

    // crash sound
    bool crashSoundPlaying = false;
    public AudioClip m_CrashSound;
    AudioSource m_CrashSource;

    // crash explosion sound
    bool crashExplosionSoundPlaying = false;
    public AudioClip m_CrashExplosionSound;
    AudioSource m_CrashExplosionSource;

    // ghost sound
    bool ghostSoundPlaying = false;
    public AudioClip m_GhostSound;
    AudioSource m_GhostSource;

    // glitch sound
    bool glitchSoundPlaying = false;
    public AudioClip m_GlitchSound;
    AudioSource m_GlitchSoundSource;

    // horn
    bool hornSoundPlaying = false;
    public AudioClip m_HornSound;
    AudioSource m_HornSoundSource;

    // start engine sound
    bool startEngineSoundPlaying = false;
    public AudioClip m_StartEngineSound;
    AudioSource m_StartEngineSource;    // Reference to the AudioSource for the start sound.

    // water crash
    bool waterCrashSoundPlaying = false;
    public AudioClip m_WaterCrash;
    AudioSource m_WaterCrashSource;

    // water touch
    bool waterTouchSoundPlaying = false;
    public AudioClip m_WaterTouch;
    AudioSource m_WaterTouchSource;


    private void Awake()
    {
        // Set up the reference to the aeroplane controller.
        m_drone = GetComponent<NewDroneController>();
        m_Rigidbody = GetComponent<Rigidbody>();


        // Add the audiosources and get the references.
        m_EngineSoundSource = gameObject.AddComponent<AudioSource>();
        m_EngineSoundSource.playOnAwake = false;
        // m_WindSoundSource = gameObject.AddComponent<AudioSource>();
        // m_WindSoundSource.playOnAwake = false;

        // Assign clips to the audiosources.
        m_EngineSoundSource.clip = m_EngineSound;
        // m_WindSoundSource.clip = m_WindSound;

        // Set the parameters of the audiosources.
        // m_EngineSoundSource.minDistance = m_AdvancedSetttings.engineMinDistance;
        // m_EngineSoundSource.maxDistance = m_AdvancedSetttings.engineMaxDistance;
        m_EngineSoundSource.loop = true;
        m_EngineSoundSource.dopplerLevel = m_AdvancedSetttings.engineDopplerLevel;

        /*
        m_WindSoundSource.minDistance = m_AdvancedSetttings.windMinDistance;
        m_WindSoundSource.maxDistance = m_AdvancedSetttings.windMaxDistance;
        m_WindSoundSource.loop = true;
        m_WindSoundSource.dopplerLevel = m_AdvancedSetttings.windDopplerLevel;
        */

        // call update here to set the sounds pitch and volumes before they actually play
        Update();

        // Start the permanent sounds playing.
        // m_EngineSoundSource.PlayDelayed(5.0f);
        m_EngineSoundSource.Play();
        // m_WindSoundSource.Play();

        // boost sound
        m_BoostSoundSource = gameObject.AddComponent<AudioSource>();
        m_BoostSoundSource.clip = m_BoostSound;
        m_BoostSoundSource.playOnAwake = false;

        // crash sound
        m_CrashSource = gameObject.AddComponent<AudioSource>();
        m_CrashSource.clip = m_CrashSound;
        m_CrashSource.playOnAwake = false;

        // crash explosion sound
        m_CrashExplosionSource = gameObject.AddComponent<AudioSource>();
        m_CrashExplosionSource.clip = m_CrashExplosionSound;
        m_CrashExplosionSource.playOnAwake = false;

        // checkpoint sound
        m_CheckpointSoundSource = gameObject.AddComponent<AudioSource>();
        m_CheckpointSoundSource.clip = m_CheckpointSound;
        m_CheckpointSoundSource.playOnAwake = false;

        // ghost sound
        m_GhostSource = gameObject.AddComponent<AudioSource>();
        m_GhostSource.clip = m_GhostSound;
        m_GhostSource.playOnAwake = false;

        // glitch sound
        m_GlitchSoundSource = gameObject.AddComponent<AudioSource>();
        m_GlitchSoundSource.clip = m_GlitchSound;
        m_GlitchSoundSource.playOnAwake = false;

        // horn sound
        m_HornSoundSource = gameObject.AddComponent<AudioSource>();
        m_HornSoundSource.clip = m_HornSound;
        m_HornSoundSource.playOnAwake = false;

        // start engine sound
        m_StartEngineSource = gameObject.AddComponent<AudioSource>();
        m_StartEngineSource.clip = m_StartEngineSound;
        m_StartEngineSource.playOnAwake = false;

        // water crash sound
        m_WaterCrashSource = gameObject.AddComponent<AudioSource>();
        m_WaterCrashSource.clip = m_WaterCrash;
        m_WaterCrashSource.playOnAwake = false;

        // water touch sound
        m_WaterTouchSource = gameObject.AddComponent<AudioSource>();
        m_WaterTouchSource.clip = m_WaterTouch;
        m_WaterTouchSource.playOnAwake = false;
    }


    private void Update()
    {
        // Find what proportion of the engine's power is being used.
        var enginePowerProportion = Mathf.InverseLerp(0, m_drone.MaxEnginePowerTotal, m_drone.CurrentEnginePowerTotal);

        // Set the engine's pitch to be proportional to the engine's current power.
        m_EngineSoundSource.pitch = Mathf.Lerp(m_EngineMinThrottlePitch, m_EngineMaxThrottlePitch, enginePowerProportion);

        // Increase the engine's pitch by an amount proportional to the aeroplane's forward speed.
        // (this makes the pitch increase when going into a dive!)
        // m_EngineSoundSource.pitch += m_drone.ForwardSpeed * m_EngineFwdSpeedMultiplier;
        float planeSpeed = m_Rigidbody.velocity.magnitude;
        m_EngineSoundSource.pitch += planeSpeed * m_EngineFwdSpeedMultiplier; // atualmente a usar velocity.magnitude em vez de forwardspeed, para simplificar

        // Set the engine's volume to be proportional to the engine's current power.
        m_EngineSoundSource.volume = Mathf.InverseLerp(0, m_drone.MaxEnginePowerTotal * m_AdvancedSetttings.engineMasterVolume,
                                                        m_drone.CurrentEnginePowerTotal);

        // Set the wind's pitch and volume to be proportional to the aeroplane's forward speed.
        // m_WindSoundSource.pitch = m_WindBasePitch + planeSpeed * m_WindSpeedPitchFactor;
        // m_WindSoundSource.volume = Mathf.InverseLerp(0, m_WindMaxSpeedVolume, planeSpeed) * m_AdvancedSetttings.windMasterVolume;
    }

    public void PlaySound_fixedLength(string sound)
    {
        switch (sound)
        {
            case "checkpoint":
                if (checkpointSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                checkpointSoundPlaying = true;
                PlaySound_aux(m_CheckpointSoundSource, false);
                StartCoroutine(DisablePlayingFlag("checkpoint", m_CheckpointSoundSource.clip.length));
                break;
            case "crash":
                if (crashSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                crashSoundPlaying = true;
                PlaySound_aux(m_CrashSource, false);
                StartCoroutine(DisablePlayingFlag("crash", m_CrashSource.clip.length));
                break;
            case "crashExplosion":
                if (crashSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                crashExplosionSoundPlaying = true;
                PlaySound_aux(m_CrashExplosionSource, false);
                StartCoroutine(DisablePlayingFlag("crashExplosion", m_CrashExplosionSource.clip.length));
                break;
            /*
            case "engineSound":
            PlaySound_aux(m_EngineSoundSource, false);
            break;
            */
            case "horn":
                // print("trying to sound horn");
                if (hornSoundPlaying == true) {
                    print("SOUND ALREADY PLAYING");
                }
                else
                {
                    // print("sounding horn");
                    PlaySound_aux(m_HornSoundSource, false);
                    hornSoundPlaying = true;
                    StartCoroutine(DisablePlayingFlag("horn", m_HornSoundSource.clip.length));
                }                
                break;
            case "startEngine":
                if (startEngineSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                startEngineSoundPlaying = true;
                PlaySound_aux(m_StartEngineSource, false);
                StartCoroutine(DisablePlayingFlag("startEngine", m_StartEngineSource.clip.length));
                break;
            case "waterCrash":
                if (waterCrashSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                waterCrashSoundPlaying = true;
                PlaySound_aux(m_WaterCrashSource, false); // mudar para true?
                StartCoroutine(DisablePlayingFlag("waterCrash", m_WaterCrashSource.clip.length));
                break;
            case "waterTouch":
                if (waterTouchSoundPlaying == true) { print("SOUND ALREADY PLAYING"); break; }
                waterTouchSoundPlaying = true;
                PlaySound_aux(m_WaterTouchSource, false);
                StartCoroutine(DisablePlayingFlag("waterTouch", m_WaterTouchSource.clip.length));
                break;
            default:
                print("ERROR");
                break;
        }
    }

    public void PlaySound_variableLength(string sound, float duration)
    {
        switch (sound)
        {
            case "boost":
                PlaySound_aux(m_BoostSoundSource, false, duration);
                break;
            case "ghost":
                PlaySound_aux(m_GhostSource, false, duration);
                break;
            case "glitch":
                PlaySound_aux(m_GhostSource, false, duration);
                break;
            default:
                break;
        }
    }

    public void PlaySound_aux(AudioSource source, bool disablesEngineSound) // TODO disable engine sound
    {
        // print("playing sound " + source.clip.name + " at " + System.DateTime.Now.ToLongTimeString());
        source.Play();
    }

    public void PlaySound_aux(AudioSource source, bool disablesEngineSound, float duration) // TODO disable engine sound
    {
        // todo duration
        source.Play();
    }

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "boost":
                m_BoostSoundSource.Play();
                break;
            case "ghost":
                m_GhostSource.Play();
                break;
            case "glitch":
                m_GlitchSoundSource.Play();
                break;
            default:
                break;
        }
    }

    public void StopSound(string sound)
    {
        switch (sound)
        {
            case "boost":
                if (m_BoostSoundSource.isPlaying)
                    m_BoostSoundSource.Stop();
                break;
            case "ghost":
                if (m_GhostSource.isPlaying)
                    m_GhostSource.Stop();
                break;
            case "glitch":
                if (m_GlitchSoundSource.isPlaying)
                m_GlitchSoundSource.Stop();
                break;
            default:
                break;
        }
    }
        


    IEnumerator DisablePlayingFlag(string flag, float delay)
    {
        yield return new WaitForSeconds(delay + 0.05f);
        switch (flag)
            {
                case "boost":
                    boostSoundPlaying = false;
                    break;
                case "ghost":
                    ghostSoundPlaying = false;
                    break;
                case "glitch":
                    glitchSoundPlaying = false;
                    break;
                case "checkpoint":
                    checkpointSoundPlaying = true;
                    break;
                case "crash":
                    crashSoundPlaying = false;
                    break;
                case "crashExplosion":
                    crashExplosionSoundPlaying = false;
                    break;
                case "horn":
                    hornSoundPlaying = false;
                    break;
                case "startEngine":
                    startEngineSoundPlaying = false;
                    break;
                case "waterCrash":
                    waterCrashSoundPlaying = false;
                    break;
                case "waterTouch":
                    waterTouchSoundPlaying = false;
                    break;
                default:
                    break;
            }


        // print("entered DisablePlayingFlag");
        // print("disabling flag at " + System.DateTime.Now.ToLongTimeString());
    }
}