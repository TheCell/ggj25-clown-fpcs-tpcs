using System.Collections;
using GamePlay;
using UnityEngine;

namespace NPC
{
    public class Child : MonoBehaviour, IShovable
    {
        private bool isBeingShoved;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (isBeingShoved) return;
        }
        public IEnumerator GetShoved(Vector3 shoveDirection)
        {
            float duration = 1f;
            float elapsedTime = 0f;
            Vector3 shovePerFrame = shoveDirection / (duration / Time.deltaTime);

            isBeingShoved = true;
            while (elapsedTime < duration)
            {
                transform.Translate(shovePerFrame);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            isBeingShoved = false;
        }
    }
}
