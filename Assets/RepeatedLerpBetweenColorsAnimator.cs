using UnityEngine;
using UnityEngine.UI;

public class RepeatedLerpBetweenColorsAnimator : MonoBehaviour
{
    public Color color1 = Color.white;
    public Color color2 = Color.black;

    public float duration = 1f;

    public void StartAnimation()
    {
        StartCoroutine(AnimateColor());
    }

    private System.Collections.IEnumerator AnimateColor()
    {
        Image img = GetComponent<Image>();
        while (true)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(color1, color2, t / duration);
                yield return null;
            }

            t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(color2, color1, t / duration);
                yield return null;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
