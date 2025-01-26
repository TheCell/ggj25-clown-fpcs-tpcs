using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlideShowHandler : MonoBehaviour
{
    [SerializeField] private Image slideshowImage;
    [SerializeField] private TextMeshProUGUI slideText;
    [SerializeField] private Sprite[] slides;
    [SerializeField] private string[] slideTexts;

    private int currentSlide = 0;

    public void NextSlide()
    {
        currentSlide++;

        if (currentSlide >= slides.Length)
        {
            SceneHandler.GoToNextScene();
            return;
        }

        slideshowImage.sprite = slides[currentSlide];

        if (currentSlide < slideTexts.Length && slideTexts[currentSlide] != null)
            slideText.text = slideTexts[currentSlide];
        else
            slideText.text = "";

    }

    public void PreviousSlide()
    {
        currentSlide--;

        if (currentSlide <= 0)
        {
            return;
        }

        slideshowImage.sprite = slides[currentSlide];

        if (currentSlide < slideTexts.Length && slideTexts[currentSlide] != null)
            slideText.text = slideTexts[currentSlide];
        else
            slideText.text = "";
    }
}
