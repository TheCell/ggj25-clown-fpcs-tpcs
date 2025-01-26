using TMPro;
using UnityEngine;

public class SetBountyScore : MonoBehaviour
{
    private void Start()
    {
        TextMeshProUGUI scoreText = GetComponentInChildren<TextMeshProUGUI>();
        scoreText.text = GameManager.Instance.score.ToString();
    }
}
