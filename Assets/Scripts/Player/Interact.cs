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
        private bool hasObjectGrabbed;
        private GameObject grabbedObject;
        private GrabbableObject currentgrabbable;
        private Rigidbody currentRigidbody;
        private Collider currentCollider;
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
                grabbedObject.transform.position = playerCapsuleTransform.position + playerCapsuleTransform.forward;
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
                if (hit.collider.gameObject.TryGetComponent(out GrabbableObject grabbableObject))
                {
                    grabbedObject = hit.collider.gameObject;
                    currentgrabbable = grabbableObject;
                    currentRigidbody = grabbedObject.GetComponent<Rigidbody>();
                    currentCollider = grabbedObject.GetComponent<Collider>();
                    currentCollider.enabled = false;
                    grabbedObject.transform.localPosition = new Vector3(0, 0, 1);
                    grabbedObject.transform.localRotation = Quaternion.identity;
                    hasObjectGrabbed = true;
                }
            }
        }
        private void Throw()
        {
            hasObjectGrabbed = false;
            currentRigidbody.AddForce(playerCapsuleTransform.forward * 10, ForceMode.Impulse);
            currentgrabbable.SetThrown(true);
            currentCollider.enabled = true;
            
            grabbedObject = null;
            currentgrabbable = null;
            currentRigidbody = null;
            currentCollider = null;
        }
    }
}
