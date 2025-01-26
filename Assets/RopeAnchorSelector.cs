using System.ComponentModel;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(Rope))]
public class RopeAnchorSelector : MonoBehaviour
{
    [SerializeField] private RandomChildSelector randomChildSelector;
    [Description("The transforms where the rope Start can be attached to, for each child. Needs to be same number as child on ChildSelector.")]
    [SerializeField] private Transform[] ropeAnchorPoints;

    private void Start()
    {
        var rope = GetComponent<Rope>();
        int childIndex = randomChildSelector.selectedChildObjectIndex;

        //Check if that child is even active
        if (childIndex >= ropeAnchorPoints.Length)
        {
            Debug.LogError("Child index is out of bounds for rope anchor points. Please make sure the child index is less than the number of rope anchor points.");
            return;
        }

        if (randomChildSelector.transform.GetChild(childIndex).gameObject.activeSelf == false)
        {
            Debug.LogError("GameObject at Child index is not active. Make sure the order of transforms is the same as the order of children under the RandomChildSelector.");
            return;
        }

        rope.SetStartPoint(ropeAnchorPoints[childIndex]);
    }
}
