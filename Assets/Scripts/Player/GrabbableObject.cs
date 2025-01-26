using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using Unity.VisualScripting;

namespace Player
{
    public class GrabbableObject : MonoBehaviour
    {
        private bool isThrown;
        private float adultsRadius = 10f;
        private List<Collider> adultsColliderInRadius = new List<Collider>();
        private Rigidbody rb;
        private GameObject meshCan;

        private ScoreManager scoreManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            scoreManager = ScoreManager.instance;
            rb = GetComponent<Rigidbody>();
            meshCan = gameObject.transform.GetChild(0).gameObject;
        }
        private void OnEnable()
        {
            //scoreManager = ScoreManager.instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (isThrown)
            {
                meshCan.transform.Rotate(Vector3.right, UnityEngine.Random.Range(37, 124));
                meshCan.transform.Rotate(Vector3.up, UnityEngine.Random.Range(37, 124));
                meshCan.transform.Rotate(Vector3.forward, UnityEngine.Random.Range(37, 124));

                // If hits child then invoke scoreManager
            }
        }
        public void SetThrown(bool thrown)
        {
            isThrown = thrown;

            if (isThrown)
            {
                rb.isKinematic = false;
            }

            if (!isThrown)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                meshCan.transform.rotation = Quaternion.identity;
                StartCoroutine(SetKinematicInASec(true));
            }
        }
        public void SetThrowDirection(Vector3 throwingDirection)
        {
            rb.AddForce(throwingDirection, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isThrown) return;
            if (other.gameObject.CompareTag("Player"))
            {
                //Debug.Log("Player hit ja lol ey");
                Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
                return;
            }

            //Debug.Log("Collision ja lol ey" + other.gameObject.name);
            SetThrown(false);

            //Debug.Log("Child hit ja lol ey");
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

        private IEnumerator SetKinematicInASec(bool isKinematic)
        {
            yield return new WaitForSeconds(1);
            rb.isKinematic = isKinematic;
        }
    }
}
