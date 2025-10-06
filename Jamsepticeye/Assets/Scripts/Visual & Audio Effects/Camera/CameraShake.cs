using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class CameraShake : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool _enable = true;
    [SerializeField] private float _Amplitude; 
    [SerializeField] private float _frequency;
    [SerializeField] private Transform _camera = null; 
    [SerializeField] private Transform _cameraHolder = null;


    private Quaternion previousRotation;
    private Vector3 _startPos;
    public CharacterMovement characterMovement;

    public float blendSpeed;

    public float swayMultiplier;

    private float rotation;
    private float shake;
    [HideInInspector] public float stepDuration;

    [Header("Roll")]
    public float frequency;
    public float amplitude;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _startPos = _camera.localPosition;
    }

    void Update()
    {
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;

        Sway();
        Shake();

        ResetPosition();
        ResetRotation();

        CheckMotion(move * characterMovement.currentSpeed);
        
    }
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency * (characterMovement.currentSpeed / 10)) * (_Amplitude / (characterMovement.currentSpeed / 10));
        pos.x += Mathf.Cos(Time.time * _frequency * (characterMovement.currentSpeed / 10) / 2) * (_Amplitude * 2) / (characterMovement.currentSpeed / 10);
        return pos;
    }

    private void CheckMotion(Vector2 input)
    {
        if (input == Vector2.zero) return;
        PlayMotion(FootStepMotion());
    }
    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }
    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    private void Sway()
    {
        Quaternion deltaRotation = _cameraHolder.parent.rotation * Quaternion.Inverse(previousRotation);
        previousRotation = _cameraHolder.parent.rotation;

        rotation = Mathf.Lerp(rotation, deltaRotation.y * swayMultiplier, blendSpeed * Time.deltaTime);

        _cameraHolder.localEulerAngles = new Vector3(_cameraHolder.localEulerAngles.x, _cameraHolder.localEulerAngles.y, _cameraHolder.localEulerAngles.z + rotation);
    }

    private void Shake()
    {
        if (stepDuration > 0)
        {
            shake = Mathf.Sin(Time.time * Mathf.PI * Random.Range(frequency / 2, frequency * 2)) * amplitude;
            stepDuration -= Time.deltaTime;
        }
        else
        {
            shake = Mathf.Lerp(shake, 0, blendSpeed * Time.deltaTime);
        }

        _cameraHolder.localEulerAngles = new Vector3(_cameraHolder.localEulerAngles.x, _cameraHolder.localEulerAngles.y, _cameraHolder.localEulerAngles.z + shake);
    }

    private void ResetRotation()
    {
        _cameraHolder.localRotation = Quaternion.Lerp(_cameraHolder.localRotation, Quaternion.identity, blendSpeed * Time.deltaTime);
    }
}
