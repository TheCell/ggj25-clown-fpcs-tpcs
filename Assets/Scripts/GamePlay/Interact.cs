using System;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = System.Random;

namespace GamePlay
{
    [Serializable]
    public class Interact : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReferenceInteract;
        [SerializeField] private bool hasObjectGrabbed;
        [SerializeField] private GameObject grabbedObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
        }
        // Interact can:
        // 1. Grab "grabbable Objects
        // 2. Throw the already grabbed grabbable objects
        private void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log("klasdjfikldaö");
            if (context.action == inputActionReferenceInteract.action)
            {
                if (hasObjectGrabbed)
                {
                    Throw();
                }
                else
                {
                    // Check if there is a grabbable object in front of the player
                    Grab();
                }
            }
        }

        private void Grab()
        {
            // Call the grabbed object and set it to the player
            Ray ray = new Ray(transform.position, transform.forward);
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(ray, out RaycastHit hit, 2, layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out GrabbableObject grabbableObject))
                {
                    grabbedObject = hit.collider.gameObject;
                    grabbedObject.transform.SetParent(transform); // Maybe we can do different
                    Physics.IgnoreCollision(grabbedObject.GetComponent<BoxCollider>(), GetComponent<CapsuleCollider>(), true);
                    grabbedObject.GetComponent<BoxCollider>().enabled = false;
                    grabbedObject.transform.localPosition = new Vector3(0, 0, 1);
                    grabbedObject.transform.localRotation = Quaternion.identity;
                    hasObjectGrabbed = true;
                }
            }
        }

        private void Throw()
        {
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            GrabbableObject gr = grabbedObject.GetComponent<GrabbableObject>();
            BoxCollider bc = grabbedObject.GetComponent<BoxCollider>();
            
            Physics.IgnoreCollision(bc, GetComponent<CapsuleCollider>(), true);
            // Throw the grabbed object
            grabbedObject.transform.SetParent(null);
            rb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
            gr.SetThrown(true);
            hasObjectGrabbed = false;
            bc.enabled = true;
        }
    }
}
