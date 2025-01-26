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

        private Vector3 emotionBillboardRelatieveOffset;
        private bool isBeingShoved;
        private Rigidbody rb;
        private AudioSource audioSource;
        private Emotion currentEmotion = Emotion.Happy;

        private void Start()
        {
            emotionBillboardRelatieveOffset = emotionBillboard.transform.position - transform.position;

            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            //emotionBillboard.transform.position = agent.transform.position + emotionBillboardRelatieveOffset;

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

        public void PlaySameEmotion()
        {
            emotionBillboardAnimator.Play(currentEmotion.ToString());
        }

        public void SetEmotion(Emotion emotion)
        {
            currentEmotion = emotion;
            Material material = GetBillboardMaterial(emotion);
            emotionBillboard.GetComponent<MeshRenderer>().material = material;
            emotionBillboardAnimator.Play(emotion.ToString());
        }

        public void StartShove(Vector3 shoveDirection, float shoveDuration)
        {
            StartCoroutine(GetShoved(shoveDirection, shoveDuration));
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
            transform.rotation = Quaternion.identity;
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
