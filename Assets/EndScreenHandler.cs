using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenHandler : MonoBehaviour
{
    [SerializeField] private Image imageHolder;
    [SerializeField] private Sprite trueEnding;
    [SerializeField] private Sprite regularEnding;
    [SerializeField] private GameObject bountyPoster;
    [SerializeField] private AnimationCurve bountySlapOntoSurfaceCurve;
    [SerializeField] private TextMeshProUGUI PoppedBalloonsText;

    private void Start()
    {
        if (GameManager.Instance.isTrueEnding)
        {
            imageHolder.sprite = trueEnding;
        }
        else
        {
            imageHolder.sprite = regularEnding;
        }

        string rewardMessage = "Popped Balloons: " + GameManager.Instance.balloonsPopped + "/" + GameManager.Instance.totalChildren + ". ";
        if (GameManager.Instance.balloonsPopped < GameManager.Instance.totalChildren / 4f)
        {
            rewardMessage += "Better luck next time agent!";
        }
        else if (GameManager.Instance.balloonsPopped < GameManager.Instance.totalChildren / 2f)
        {
            rewardMessage += "Good job agent! You showed them kids who's boss!";
        }
        else if (GameManager.Instance.balloonsPopped < GameManager.Instance.totalChildren)
        {
            rewardMessage += "Great job agent! They won't be happy for a while!";
        }
        else
        {
            rewardMessage += "Perfect job agent! You created the perfect consumers for FunCo!";
        }
        PoppedBalloonsText.text = rewardMessage;
    }

    public void DisplayPlayerBounty()
    {
        StartCoroutine(BountySlapOntoSurface());
    }

    //Fade in Bounty and program in animation to make it look like it is slapped onto the surface of the screen
    public IEnumerator BountySlapOntoSurface()
    {
        GameObject bounty = Instantiate(bountyPoster, imageHolder.transform);
        Image bountyImage = bounty.GetComponentInChildren<Image>();

        bounty.transform.SetParent(imageHolder.transform);
        RectTransform bountyRectTransform = bounty.GetComponent<RectTransform>();
        bountyRectTransform.position = imageHolder.rectTransform.position + new Vector3(0, 0, -1);
        bountyRectTransform.anchoredPosition = new Vector3(0, 0, 0);

        //Animate the poster by scaling it down and slightly moving it and slightly rotation it to make it look like it is slapped onto the surface
        float time = 0;
        while (time < 1)
        {
            bountyImage.rectTransform.localScale = Vector3.one * bountySlapOntoSurfaceCurve.Evaluate(time);
            bountyImage.rectTransform.localEulerAngles = new Vector3(0, 0, 360) * bountySlapOntoSurfaceCurve.Evaluate(time);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
