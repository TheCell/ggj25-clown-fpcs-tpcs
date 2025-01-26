using System;
using System.Collections;
using NPC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [Serializable]
    public class Shove : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReferenceShove;
        [SerializeField] private Transform playerCapsuleTransform;
        [SerializeField] private float shoveDuration;
        [SerializeField] private float distanceBetweenPlayerAndShoveable;
        [SerializeField] private float shovePower;

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

            GetComponent<Move>().FreezePlayer(shoveDuration);
            StartCoroutine(ShoveDelayCoroutine(shoveDuration));
        }
        private IEnumerator ShoveDelayCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);

            Ray ray = new Ray(playerCapsuleTransform.position, playerCapsuleTransform.forward);
            int layerMask = ~LayerMask.GetMask("Ignore Raycast");

            if (Physics.Raycast(ray, out RaycastHit hit, 2, layerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out IShovable shovable))
                {
                    var adult = hit.collider.gameObject.GetComponent<Adult>();
                    if (adult != null)
                    {
                        adult.Shove();
                    }
                    StartCoroutine(shovable.GetShoved(playerCapsuleTransform.forward * distanceBetweenPlayerAndShoveable * shovePower, shoveDuration));
                }
            }
        }
    }
}
