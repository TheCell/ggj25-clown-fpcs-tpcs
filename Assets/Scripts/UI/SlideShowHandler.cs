using UnityEngine;
using UnityEngine.UI;

public class SlideShowHandler : MonoBehaviour
{
    public Image slideshowImage;
    public Sprite[] slides;
    private int currentSlide = 0;

    public void NextSlide()
    {
        if (currentSlide >= slides.Length)
        {
            SceneHandler.GoToNextScene();
            return;
        }
        slideshowImage.sprite = slides[currentSlide];
        currentSlide++;
    }
}
