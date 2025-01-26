using UnityEngine;

/// <summary>
/// Randomly selects a child object to be active.
/// </summary>
public class RandomChildSelector : MonoBehaviour
{
    public int selectedChildObjectIndex { get; private set; }

    private void Awake()
    {
        selectedChildObjectIndex = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == selectedChildObjectIndex)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

    }
}
