using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GamePlay
{
    [Serializable]
    public class Interact : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReferenceInteract;
        private bool hasObjectGrabbed;
        private GameObject grabbedObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //inputActionReferenceInteract.action.performed += OnInteract;
        }

        private void OnEnable()
        {
            Debug.Log("klasdjfikldaö 1");
            inputActionReferenceInteract.action.Enable();
            inputActionReferenceInteract.action.performed += OnInteract;
            Debug.Log("klasdjfikldaö 2");
        }
        private void OnDisable()
        {
            Debug.Log("klasdjfikldaö 3");
            inputActionReferenceInteract.action.performed -= OnInteract;
            inputActionReferenceInteract.action.Disable();
            Debug.Log("klasdjfikldaöv 4");
        }
        // Update is called once per frame
        void Update()
        {
        
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
        }

        private void Throw()
        {
            // Throw the grabbed object
            grabbedObject.transform.SetParent(null);
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.forward * 10, ForceMode.Impulse);
        }
    }
}
