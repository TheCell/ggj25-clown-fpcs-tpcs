using System.Collections;
using UnityEngine;

namespace NPC
{
    public class Child : InteractionHistory, IShovable
    {
        private bool isBeingShoved;
        private Rigidbody rb;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (isBeingShoved) return;
        }

        public void BubbleBurstHappened()
        {
            if (HasInteractedWith(Interaction.BubbleBurst))
            {
                return;
            }

            Debug.Log("Bubble Burst");
            AddHasBeenInteractedWith(Interaction.BubbleBurst);
        }

        public void KickHappened()
        {
            if (HasInteractedWith(Interaction.Kick))
            {
                return;
            }

            AddHasBeenInteractedWith(Interaction.Kick);
        }

        public IEnumerator GetShoved(Vector3 shoveDirection, float shoveDuration)
        {
            if (isBeingShoved)
                yield break;

            isBeingShoved = true;
            rb.isKinematic = false;
            rb.AddForce(shoveDirection, ForceMode.Impulse);
            yield return new WaitForSeconds(shoveDuration);

            isBeingShoved = false;

            //Reset RB and set it u pstraight again
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.rotation = Quaternion.identity;
            rb.isKinematic = true;

        }
    }
}
