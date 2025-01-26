using System.Collections;
using NPC;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RandomSoundPlayer))]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }
    private Interaction lastInteraction;
    public int combo { get; private set; } = 1;
    public int score { get; private set; } = 0;
    [SerializeField] Transform playerTransform;
    [SerializeField] private int comboCountdown = 5;
    private float comboCountdownTimeLeft = 0f;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Image comboBackground;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image scoreBackground;
    [SerializeField] private Image[] strikeImages = new Image[3];
    [SerializeField] private Image strikeBackground;
    [SerializeField] private Image[] policeSirenImages = new Image[2];
    [SerializeField] private Slider timeUntilPoliceArriveSlider;
    public ScoreEvent scoreEvent;
    private AudioSource audioSource;
    private RandomSoundPlayer randomSoundPlayer;

    //Anti-Cheat score variables
    private int antiCheatFloatMultiplier;
    private int antiCheatScore = 0;
    private int strikes = 0;

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

    private void Start()
    {
        int childrenInScene = Object.FindObjectsByType<Child>(FindObjectsSortMode.InstanceID).Length;
        audioSource = GetComponent<AudioSource>();
        randomSoundPlayer = GetComponent<RandomSoundPlayer>();

        GameManager.Instance.totalChildren = childrenInScene;
    }

    private void Update()
    {
        if (combo > 1)
        {
            comboCountdownTimeLeft -= Time.deltaTime;
            comboText.color = Color.Lerp(Color.red, Color.black, comboCountdownTimeLeft / comboCountdown);
            if (comboCountdownTimeLeft <= 0)
            {
                combo = 1;
                UpdateComboUI();
                StartCoroutine(FlashTextRed(comboText));
                StartCoroutine(ScaleUpDownAnimation(comboText.rectTransform, 0.5f));
                StartCoroutine(ScaleUpDownAnimation(comboBackground.rectTransform, 0.75f));
            }
        }

        if (!GameManager.Instance.isGamePaused)
        {
            timeUntilPoliceArriveSlider.value = 1f - GameManager.Instance.timeUntilCopsArriveCounter / GameManager.Instance.timeUntilCopsArrive;
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
        if (score == 0)
        {
            audioSource.PlayOneShot(audioSource.clip);
            StartCoroutine(ScaleUpDownAnimation(strikeBackground.rectTransform, 1.25f));
            foreach (Image strikeImage in strikeImages)
            {
                StartCoroutine(ScaleUpDownAnimation(strikeImage.rectTransform, 1.5f));
            }

            foreach (Image policeSirenImage in policeSirenImages)
            {
                policeSirenImage.color = Color.white;
            }

            timeUntilPoliceArriveSlider.GetComponentInChildren<RepeatedLerpBetweenColorsAnimator>().StartAnimation();
        }
        else
        {
            if (interaction != Interaction.Strike)
            {
                randomSoundPlayer.PlayRandomAudioOneShot(audioSource);
            }
        }

        int calculatedScore = ((int)interaction + witnesscount * (int)Interaction.Witness) * combo;
        score += calculatedScore;

        antiCheatScore += calculatedScore * antiCheatFloatMultiplier;

        StartCoroutine(SpawnAndFlyAwayScoreText(calculatedScore, playerTransform.position));

        GameManager.Instance.AddScore(calculatedScore);

        Debug.Log($"Score received for {interaction} with {witnesscount} witnesses and calculated score: {calculatedScore}");

        if (interaction == Interaction.Strike)
        {
            strikeImages[strikes].color = Color.white;
            ScaleUpDownAnimation(strikeImages[strikes].rectTransform, 2f);
            ResetCombo();
            strikes++;
            if (strikes >= 3)
            {
                strikes = 0;
                GameManager.Instance.EndGame();
            }
        }
        else if (interaction == Interaction.BubbleBurst)
        {
            GameManager.Instance.balloonsPopped++;
        }

        if (interaction == lastInteraction)
        {
            ResetCombo();
        }
        else
        {
            combo++;
            StartCoroutine(ScaleUpDownAnimation(comboText.rectTransform, 1.25f));
            StartCoroutine(ScaleUpDownAnimation(comboBackground.rectTransform, 1.5f));
        }

        lastInteraction = interaction;

        UpdateComboUI();
        AntiCheatDetection();
    }

    private void UpdateComboUI()
    {
        comboText.text = $"X {combo}";
        scoreText.text = score.ToString();
        Debug.Log($"Score: {score}");
        comboCountdownTimeLeft = comboCountdown;
    }

    private void ResetCombo()
    {
        combo = 1;
        StartCoroutine(FlashTextRed(comboText));
        StartCoroutine(ScaleUpDownAnimation(comboText.rectTransform, 0.5f));
        StartCoroutine(ScaleUpDownAnimation(comboBackground.rectTransform, 0.75f));
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

    private IEnumerator FlashTextRed(TextMeshProUGUI text)
    {
        Color startColor = Color.black;
        Color MiddleColor = Color.red;
        float animationDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, MiddleColor, Mathf.PingPong(elapsedTime / animationDuration, animationDuration));
            yield return null;
        }
    }

    private IEnumerator ScaleUpDownAnimation(RectTransform rectTransform, float intensity)
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = startScale * intensity;
        float animationDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, Mathf.PingPong(elapsedTime / animationDuration, animationDuration));
            yield return null;
        }
    }

    private IEnumerator SpawnAndFlyAwayScoreText(int score, Vector3 position)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        TextMeshProUGUI scoreTextInstance = Instantiate(scoreText, screenPosition, Quaternion.identity);
        scoreTextInstance.text = score.ToString();
        scoreTextInstance.transform.SetParent(GetComponentInChildren<Canvas>().transform, false);
        scoreTextInstance.transform.position = screenPosition;
        scoreTextInstance.transform.localScale = Vector3.one * 2;
        scoreTextInstance.color = Random.ColorHSV();
        VertexGradient originalGradient = new VertexGradient(Random.ColorHSV(), Random.ColorHSV(), Random.ColorHSV(), Random.ColorHSV());
        scoreTextInstance.colorGradient = originalGradient;

        float amplitude = 0.5f;
        float frequency = 1f;
        float duration = 1f;
        float elapsedTime = 0f;
        float strength = 200f;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float sinWave = Mathf.Sin(elapsedTime * frequency) * amplitude;
            scoreTextInstance.transform.position += randomDirection * Time.deltaTime * strength;
            scoreTextInstance.transform.position += Vector3.up * sinWave;
            Color currentColor = scoreTextInstance.color;
            currentColor.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            scoreTextInstance.color = currentColor;
            yield return null;
        }

        Destroy(scoreTextInstance.gameObject);
    }
}
