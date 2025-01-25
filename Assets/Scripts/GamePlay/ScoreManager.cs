using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }
    private Interaction lastInteraction;
    public int combo { get; private set; } = 1;
    public int score { get; private set; } = 0;
    [SerializeField] private int comboCountdown = 5;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider comboCountdownSlider;
    public ScoreEvent scoreEvent;

    //Anti-Cheat score variables
    private int antiCheatFloatMultiplier;
    private int antiCheatScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        antiCheatFloatMultiplier = Random.Range(2, 100);
    }

    private void Update()
    {
        if (combo > 1)
        {
            comboCountdownSlider.value -= Time.deltaTime / comboCountdown;
            if (comboCountdownSlider.value <= 0)
            {
                combo = 1;
                UpdateComboUI();
                StartCoroutine(ResetComboAnimation());
                StartCoroutine(ScaleUpDownAnimation());
            }
        }
    }

    private void OnEnable()
    {
        scoreEvent.AddListener(OnScoreReceived);
    }

    private void OnDisable()
    {
        scoreEvent.RemoveListener(OnScoreReceived);
    }

    private void OnScoreReceived(Interaction interaction, int witnesscount)
    {
        int calculatedScore = ((int)interaction + witnesscount * (int)Interaction.Witness) * combo;
        score += calculatedScore;

        antiCheatScore += calculatedScore * antiCheatFloatMultiplier;

        GameManager.Instance.score = score;

        Debug.Log($"Score received for {interaction} with {witnesscount} witnesses and calculated score: {calculatedScore}");

        if (interaction == lastInteraction)
        {
            combo = 1;
            StartCoroutine(ResetComboAnimation());
        }
        else
        {
            combo++;
        }

        StartCoroutine(ScaleUpDownAnimation());
        UpdateComboUI();
        lastInteraction = interaction;
        
        AntiCheatDetection();
    }

    private void UpdateComboUI()
    {
        comboText.text = $"X {combo}";
        scoreText.text = score.ToString();
        Debug.Log($"Score: {score}");
        comboCountdownSlider.value = comboCountdownSlider.maxValue;
    }

    private void AntiCheatDetection()
    {
        if (score != antiCheatScore / antiCheatFloatMultiplier)
        {
            Debug.LogWarning("SOMEONE IS CHEATING!1!!!");
            score = 0;
            scoreText.text = "don't cheat please";
        }
    }

    private IEnumerator ResetComboAnimation()
    {
        Color startColor = Color.white;
        Color MiddleColor = Color.red;
        float animationDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            comboText.color = Color.Lerp(startColor, MiddleColor, Mathf.PingPong(elapsedTime / animationDuration, animationDuration));
            yield return null;
        }
    }

    private IEnumerator ScaleUpDownAnimation()
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = startScale * 1.5f;
        float animationDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            comboText.transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.PingPong(elapsedTime / animationDuration, animationDuration));
            yield return null;
        }
    }
}
