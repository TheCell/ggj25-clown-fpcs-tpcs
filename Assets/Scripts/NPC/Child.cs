using System.Collections;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(AudioSource))]
    public class Child : InteractionHistory, IShovable
    {
        [SerializeField] private GameObject balloon;
        [SerializeField] Animator emotionBillboardAnimator;
        [SerializeField] private Material[] emotionBillboardMaterials;
        [SerializeField] private GameObject emotionBillboard;

        private bool isBeingShoved;
        private Rigidbody rb;
        private AudioSource audioSource;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
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

            audioSource.PlayOneShot(audioSource.clip);
            Destroy(balloon);
            AddHasBeenInteractedWith(Interaction.BubbleBurst);
        }

        public void KickHappened()
        {
            AddHasBeenInteractedWith(Interaction.Kick);
        }

        public void SetEmotion(Emotion emotion)
        {
            Material material = GetBillboardMaterial(emotion);
            emotionBillboard.GetComponent<MeshRenderer>().material = material;
            emotionBillboardAnimator.Play(emotion.ToString());
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

        private Material GetBillboardMaterial(Emotion emotion)
        {
            switch (emotion)
            {
                case Emotion.Happy:
                    return emotionBillboardMaterials[0];
                case Emotion.Sad:
                    return emotionBillboardMaterials[1];
                case Emotion.Depressed:
                    return emotionBillboardMaterials[2];
                default:
                    Debug.LogError("No emotion found for interaction: " + emotion);
                    throw new System.ArgumentException();
            }
        }
    }
}
