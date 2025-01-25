using System;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using Unity.VisualScripting;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class GrabbableObject : MonoBehaviour
    {
        private bool isThrown;
        private float adultsRadius = 10f;
        private List<Collider> adultsColliderInRadius = new List<Collider>();

        private ScoreManager scoreManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            
        }

        private void OnEnable()
        {
            scoreManager = ScoreManager.instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (isThrown)
            {
                transform.Rotate(Vector3.right, UnityEngine.Random.Range(37, 124));
                transform.Rotate(Vector3.up, UnityEngine.Random.Range(37, 124));
                transform.Rotate(Vector3.forward, UnityEngine.Random.Range(37, 124));

                // If hits child then invoke scoreManager
            }
        }
        public void SetThrown(bool thrown)
        {
            isThrown = thrown;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isThrown) return;
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player hit ja lol ey");
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
                return;
            }
            
            Debug.Log("Collision ja lol ey" + other.gameObject.name);
            isThrown = false;
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            if (other.gameObject.CompareTag(nameof(Tag.Child)))
            {
                Debug.Log("Child hit ja lol ey");
                int adultAmount = 0;
                foreach (var adult in Physics.OverlapSphere(transform.position, adultsRadius))
                {
                    if (adult.CompareTag(nameof(Tag.Adult)))
                    {
                        adult.GetComponentInParent<Adult>().WitnessHappened();
                        adultAmount++;
                    }
                }
                if (adultAmount == 0)
                    adultAmount = 1;
                Debug.Log(adultAmount);
                scoreManager.scoreEvent.Invoke(Interaction.Throw, adultAmount);
            }

        }
    }
}
