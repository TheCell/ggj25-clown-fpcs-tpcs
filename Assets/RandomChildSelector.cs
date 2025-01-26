using UnityEngine;

/// <summary>
/// Randomly selects a child object to be active.
/// </summary>
public class RandomChildSelector : MonoBehaviour
{
    public int selectedChildObjectIndex { get; private set; }
    [SerializeField] private Adult adult;

    private GameObject activeObject;

    private void Awake()
    {
        selectedChildObjectIndex = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (i == selectedChildObjectIndex)
            {
                child.gameObject.SetActive(true);
                activeObject = child.gameObject;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        var animator = activeObject.GetComponent<Animator>();
        animator.Play("Walk");

        if (adult != null)
        {
            adult.SetAnimator(animator);
        }
    }
}
