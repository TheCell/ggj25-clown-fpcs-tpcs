using System.Collections;
using GamePlay;
using UnityEngine;

namespace Data
{
    public class GrabbableObject : MonoBehaviour, IShovable
    {
        public bool isThrown { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (isThrown)
            {
                transform.Rotate(Vector3.right, UnityEngine.Random.Range(37, 124));
                transform.Rotate(Vector3.up, UnityEngine.Random.Range(37, 124));
                transform.Rotate(Vector3.forward, UnityEngine.Random.Range(37, 124));
            }
        }
        public void SetThrown(bool thrown)
        {
            isThrown = thrown;
        }

        public IEnumerator GetShoved(Vector3 shoveDirection)
        {
            float duration = 1f;
            float elapsedTime = 0f;
            Vector3 shovePerFrame = shoveDirection / (duration / Time.deltaTime);

            while (elapsedTime < duration)
            {
                transform.Translate(shovePerFrame);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
    }
}
