using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable]
    public class Interact : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReferenceInteract;
        [SerializeField] private Transform playerCapsuleTransform;
        [SerializeField] private float forwardDirectionRayLength;
        [SerializeField] private float upDirectionRayLength;
        [SerializeField] private float throwingDistanceOfObjects;
        private bool hasObjectGrabbed;
        private GameObject grabbedObject;
        private GrabbableObject currentgrabbable;
        private Collider currentCollider;
        private float holdingLength = 1.5f;
        void Start()
        {
        }

        private void OnEnable()
        {
            inputActionReferenceInteract.action.Enable();
            inputActionReferenceInteract.action.performed += OnInteract;
        }
        private void OnDisable()
        {
            inputActionReferenceInteract.action.performed -= OnInteract;
            inputActionReferenceInteract.action.Disable();
        }
        void Update()
        {
            Debug.DrawRay(playerCapsuleTransform.position, forwardDirectionRayLength * playerCapsuleTransform.forward + playerCapsuleTransform.up * upDirectionRayLength, Color.red);
            if (hasObjectGrabbed)
            {
                grabbedObject.transform.position = playerCapsuleTransform.position + playerCapsuleTransform.forward * holdingLength;
                grabbedObject.transform.rotation = playerCapsuleTransform.rotation;
            }
        }
        private void OnInteract(InputAction.CallbackContext context)
        {
            if (context.action == inputActionReferenceInteract.action)
            {
                if (hasObjectGrabbed)
                {
                    Throw();
                }
                else
                {
                    Grab();
                }
            }
        }

        private void Grab()
        {
            // Call the grabbed object and set it to the player
            Ray ray = new Ray(playerCapsuleTransform.position, playerCapsuleTransform.forward * forwardDirectionRayLength + playerCapsuleTransform.up * upDirectionRayLength);
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(ray, out RaycastHit hit, 2, layerMask))
            {
                currentgrabbable = hit.collider.GetComponentInParent<GrabbableObject>();
                if (currentgrabbable != null)
                {
                    grabbedObject = currentgrabbable.gameObject;
                    currentCollider = hit.collider;
                    currentCollider.enabled = false;
                    grabbedObject.transform.localPosition = Vector3.forward * holdingLength;
                    grabbedObject.transform.localRotation = Quaternion.identity;
                    hasObjectGrabbed = true;
                }
            }
        }
        private void Throw()
        {
            hasObjectGrabbed = false;
            currentgrabbable.SetThrown(true);
            currentgrabbable.SetThrowDirection(playerCapsuleTransform.forward * throwingDistanceOfObjects);
            currentCollider.enabled = true;
            
            grabbedObject = null;
            currentgrabbable = null;
            currentCollider = null;
        }
    }
}
