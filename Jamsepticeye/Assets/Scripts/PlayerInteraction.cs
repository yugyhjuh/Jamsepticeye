using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactMask;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform grabPointTransform;

    private Interactable heldObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryGrab();
            }
            else
            {
                DropObject();
            }
        }
        if (Time.frameCount % 60 == 0) // print once per second
        {
            Debug.Log($"Gravity: {Physics.gravity}, TimeScale: {Time.timeScale}");
        }
        /*        Debug.Log("Time Scale: " + Time.timeScale);
                Debug.Log("Physics gravity = " + Physics.gravity);
                Debug.Log("Time scale = " + Time.timeScale);*/
    }

    private void TryGrab()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, interactDistance, interactMask))
        {
            if (hit.transform.TryGetComponent(out Interactable interactable))
            {
                heldObject = interactable;
                heldObject.Grab(grabPointTransform);
                Debug.Log("Grabbed: " + hit.transform.name);
            }
        }
        else
        {
            Debug.Log("No object hit within range.");
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.Drop();
            Debug.Log("Dropped object");
            heldObject = null;
        }
    }
}