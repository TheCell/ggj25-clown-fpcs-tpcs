using System.Collections;
using UnityEngine;

namespace GamePlay
{
    public interface IShovable
    {
        IEnumerator GetShoved(Vector3 shoveDirection, float shoveDuration);
    }
    // This is an example of how to implement the IShovable interface
    /*
    public IEnumerator GetShoved(Vector3 shoveDirection)
    {
        isBeingShoved = true;
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 shovePerFrame = shoveDirection / (duration / Time.deltaTime);

        while (elapsedTime < duration)
        {
            transform.Translate(shovePerFrame);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isBeingShoved = false;
    }
    */
}
