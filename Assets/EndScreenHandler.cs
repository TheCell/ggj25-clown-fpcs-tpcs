using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenHandler : MonoBehaviour
{
    [SerializeField] private Image imageHolder;
    [SerializeField] private Sprite trueEnding;
    [SerializeField] private Sprite regularEnding;
    [SerializeField] private Sprite bountyPoster;
    [SerializeField] private AnimationCurve bountySlapOntoSurfaceCurve;

    private void Start() {
        if (GameManager.Instance.isTrueEnding)
        {
            imageHolder.sprite = trueEnding;
        }
        else
        {
            imageHolder.sprite = regularEnding;
        }
    }

    public void DisplayPlayerBounty()
    {
        StartCoroutine(BountySlapOntoSurface());
    }
    
    //Fade in Bounty and program in animation to make it look like it is slapped onto the surface of the screen
    public IEnumerator BountySlapOntoSurface()
    {
        //Create new image under the image holder for the bounty
        Image bountyImage = new GameObject("BountyImage").AddComponent<Image>();
        bountyImage.transform.SetParent(imageHolder.transform);
        bountyImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        bountyImage.rectTransform.sizeDelta = new Vector2(500, 500);
        bountyImage.sprite = bountyPoster;
        
        //Animate the poster by scaling it down and slightly moving it and slightly rotation it to make it look like it is slapped onto the surface
        float time = 0;
        while (time < 1)
        {
            bountyImage.rectTransform.localScale = Vector3.one * bountySlapOntoSurfaceCurve.Evaluate(time);
            bountyImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0) + new Vector3(0, -50, 0) * bountySlapOntoSurfaceCurve.Evaluate(time);
            bountyImage.rectTransform.localEulerAngles = new Vector3(0, 0, 360) * bountySlapOntoSurfaceCurve.Evaluate(time);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
