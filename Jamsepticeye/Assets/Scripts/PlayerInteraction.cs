using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f; // How far player can interact
    public LayerMask interactMask;
    public float pickUpDistance = 50f;
    //public LayerMask grabbableMask;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;

    private Interactable interactable;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactable == null)
            {
                //for not carrying an obj currently
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, interactMask))
                {

                    Debug.Log(raycastHit.transform);
                    if (raycastHit.transform.TryGetComponent(out interactable))
                    {
                        interactable.Grab(objectGrabPointTransform);
                        Debug.Log(interactable + "from player script");
                    }
                }
            }
            else
            {
                //Drop item
                interactable.Drop();
                interactable = null;
            }
            
            
            
        }
    }

}
