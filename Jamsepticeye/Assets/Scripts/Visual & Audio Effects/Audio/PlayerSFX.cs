using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class PlayerSFX : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private CameraShake cameraShake;

    public List<AudioSource> sources;

    private AudioSource footstepAudio;
    private AudioSource playerAudio;

    private float baseStepSpeed = 0.5f;
    private float crouchStepMultiplier = 1.5f;
    private float sprintStepMultplier = 0.625f;

    [Header("General Sounds")]
    public List<Sound> ambientSounds;
    public List<Sound> immersionSounds;

    [Header("Footstep & Foley Sounds")]
    public List<SFX> footstepSounds;
    public List<SFX> foleySounds;

    private float footstepTimer = 0;
    private float GetCurrentOffset => characterMovement.currentSpeed == characterMovement.crouchSpeed ? baseStepSpeed * crouchStepMultiplier : characterMovement.currentSpeed == characterMovement.sprintSpeed ? baseStepSpeed * sprintStepMultplier : baseStepSpeed;

    public float stepDuration;

    [Header("Reverb")]
    public AudioReverbZone reverbZone;
    public float blendSpeed;

    private float distance;
    public LayerMask includeLayers;
    // Start is called before the first frame update
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        cameraShake = GetComponent<CameraShake>();

        SetUpAudio();

        AddSources(ambientSounds);
        AddSources(immersionSounds);
    }

    void FixedUpdate()
    {
        //Debug.Log(player.state);

        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;

        Debug.Log(characterMovement.currentSpeed);
        Footsteps(move * characterMovement.currentSpeed * Time.deltaTime);

        Reverb();
    }

    void SetUpAudio()
    {
        footstepAudio = this.AddComponent<AudioSource>();
        playerAudio = this.AddComponent<AudioSource>();
    }

    private void Footsteps(Vector2 input)
    {
        if (input == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Grass":
                        Footstep("Grass");
                        break;
                    case "Footsteps/Sand":
                        Footstep("Sand");
                        break;
                    case "Footsteps/Wood":
                        Footstep("Wood");
                        break;
                    default:
                        Footstep("Default");
                        break;
                }

                cameraShake.stepDuration = stepDuration;
            }
            footstepTimer = GetCurrentOffset;
        }
    }

    void Footstep(string footstepName)
    {
        foreach (SFX footstep in footstepSounds)
        {
            if (footstep.name == footstepName && footstep.sounds.Length > 0)
            {
                footstepAudio.PlayOneShot(footstep.sounds[UnityEngine.Random.Range(0, footstep.sounds.Length - 1)]);
            }
        }
    }
    public void Play(string name, List<Sound> sounds)
    {
        // Ternary operators are used to check if the name matches the sound in the Sound[] array
        Sound sound = Array.Find(sounds.ToArray(), sound => sound.name == name);

        // If the sound does not exist, return
        if (sound == null) return;

        // If the sound does exist, the sound would be played
        sound.source.Play();
    }

    void PlayOneShot(string name, List<SFX> sounds)
    {
        foreach (SFX sound in sounds)
        {
            if (sound.name == name && sound.sounds.Length > 0)
            {
                playerAudio.PlayOneShot(sound.sounds[0]);
            }
        }
    }

    void AddSources(List<Sound> sounds)
    {
        foreach (Sound sound in sounds)
        {
            // An AudioSource component is added to the GameObject that this script is attached
            sound.source = this.AddComponent<AudioSource>();

            // The clip, volume, pitch, and loop variables within the AudioSource are then set
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.spatialBlend = sound.spatialBlend;

            sound.source.loop = sound.loop;
        }

        PlayAudio(sounds);
    }

    void PlayAudio(List<Sound> sounds)
    {
        foreach (Sound sound in sounds)
        {
            Play(sound.name, sounds);
        }
    }

    void Reverb()
    {
        RaycastHit hit;

        if(Physics.Raycast(reverbZone.transform.position, Vector3.up, out hit))
        {
            distance = Vector3.Distance(reverbZone.transform.position, hit.point);
        }
        else
        {
            distance = 0;
        }

        reverbZone.minDistance = Mathf.Lerp(reverbZone.minDistance, distance, blendSpeed * Time.deltaTime);
        reverbZone.maxDistance = Mathf.Lerp(reverbZone.maxDistance, distance * 2, blendSpeed * Time.deltaTime);
    }
}

[System.Serializable]
public class SFX
{
    public string name;

    public SoundSettings settings;

    public AudioClip[] sounds;
}

[System.Serializable]
public class SoundSettings
{
    public AudioMixerGroup output;

    [Range(0f, 1f)]
    public float volume;

    [Range(-3f, 3f)]
    public float pitch;

    [Range(0f, 1f)]
    public float spatialBlend;
}
