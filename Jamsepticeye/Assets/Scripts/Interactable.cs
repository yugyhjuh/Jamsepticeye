using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform grabPoint;
    private bool isGrabbed;

    [SerializeField] private float followSpeed = 15f; // how quickly it follows
    [SerializeField] private float dropForce = 8f;     // downward impulse when dropped

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Grab(Transform grabPoint)
    {
        this.grabPoint = grabPoint;
        isGrabbed = true;

        rb.useGravity = false;
        rb.drag = 10f;
        rb.velocity = Vector3.zero;
    }

    public void Drop()
    {
        isGrabbed = false;
        grabPoint = null;

        rb.useGravity = true;
        rb.drag = 0f;

        // Apply small downward impulse
        rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (isGrabbed && grabPoint != null)
        {
            Vector3 moveDir = grabPoint.position - rb.position;
            float distance = moveDir.magnitude;

            if (distance > 0.01f) // prevent jitter
            {
                // Cast in move direction to check for collisions
                if (Physics.Raycast(rb.position, moveDir.normalized, out RaycastHit hit, distance))
                {
                    // Move only up to obstacle, with small buffer
                    rb.MovePosition(hit.point - moveDir.normalized * 0.01f);
                }
                else
                {
                    // Free move
                    rb.MovePosition(rb.position + moveDir);
                }
            }
        }
    }
}

