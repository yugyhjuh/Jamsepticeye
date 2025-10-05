using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public CharacterMovement characterMovement;
    public CameraShake camerashake;

    private AudioSource footstepAudio;
    private AudioSource playerAudio;

    private float baseStepSpeed = 0.5f;
    private float crouchStepMultiplier = 1.5f;
    private float sprintStepMultplier = 0.625f;

    public List<Footstep> footstepSounds;

    private float footstepTimer = 0;
    private float GetCurrentOffset => characterMovement.currentSpeed == characterMovement.crouchSpeed ? baseStepSpeed * crouchStepMultiplier : characterMovement.currentSpeed == characterMovement.sprintSpeed ? baseStepSpeed * sprintStepMultplier : baseStepSpeed;

    public float stepDuration;

    // Start is called before the first frame update
    void Start()
    {
        SetFootstep();
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
    }

    void SetFootstep()
    {
        footstepAudio = this.AddComponent<AudioSource>();

        footstepAudio.volume = 1f;
        footstepAudio.spatialBlend = 0.5f;
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
                        PlayFootstep("Grass");
                        break;
                    case "Footsteps/Sand":
                        PlayFootstep("Sand");
                        break;
                    case "Footsteps/Wood":
                        PlayFootstep("Wood");
                        break;
                    default:
                        PlayFootstep("Default");
                        break;
                }

                camerashake.stepDuration = stepDuration;
            }
            footstepTimer = GetCurrentOffset;
        }
    }

    void PlayFootstep(string footstepName)
    {
        foreach (Footstep footstep in footstepSounds)
        {
            if (footstep.name == footstepName && footstep.sounds.Length > 0)
            {
                footstepAudio.PlayOneShot(footstep.sounds[UnityEngine.Random.Range(0, footstep.sounds.Length - 1)]);
            }
        }
    }
}

[System.Serializable]
public class Footstep
{
    public string name;
    public AudioClip[] sounds;
}
