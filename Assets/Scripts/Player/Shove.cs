using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay
{
    [Serializable]
    public class Shove : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReferenceShove;
        [SerializeField] private Transform playerCapsuleTransform;
        [SerializeField] private float shoveDuration;
        [SerializeField] private float distanceBetweenPlayerAndShoveable;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(playerCapsuleTransform.position, playerCapsuleTransform.forward * distanceBetweenPlayerAndShoveable, Color.blue);
        }
        private void OnEnable()
        {
            inputActionReferenceShove.action.Enable();
            inputActionReferenceShove.action.performed += OnShove;
        }
        private void OnDisable()
        {
            inputActionReferenceShove.action.performed -= OnShove;
            inputActionReferenceShove.action.Disable();
        }
        private void OnShove(InputAction.CallbackContext context)
        {
            Debug.Log("Shove");
            
            Ray ray = new Ray(playerCapsuleTransform.position, playerCapsuleTransform.forward);
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(ray, out RaycastHit hit, 2, layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out IShovable shovable))
                {
                    StartCoroutine(shovable.GetShoved(playerCapsuleTransform.forward * distanceBetweenPlayerAndShoveable, shoveDuration));
                }
            }
        }
    }
}
