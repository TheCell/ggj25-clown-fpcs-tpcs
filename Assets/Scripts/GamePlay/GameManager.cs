using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float timeUntilCopsArrive = 60f;
    private AudioSource audioSource;
    private AudioMixer audioMixer;

    public float timeUntilCopsArriveCounter = 0f;
    public bool isGamePaused = true;
    public bool isTrueEnding = false;
    public int score = 0;
    public int balloonsPopped = 0;
    public int totalChildren = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (SceneHandler.Instance.currentScene == SceneType.GAMESCENE && !isGamePaused)
        {
            timeUntilCopsArriveCounter += Time.deltaTime;
            if (timeUntilCopsArriveCounter >= timeUntilCopsArrive)
            {
                EndGame();
            }
        }
    }

    public void EndGame()
    {
        isGamePaused = true;
        isTrueEnding = totalChildren == balloonsPopped;
        timeUntilCopsArriveCounter = 0f;
        SceneHandler.GoToNextScene();
    }

    public void AddScore(int scoreToAdd)
    {
        if (score == 0)
        {
            isGamePaused = false;
            balloonsPopped = 0;
        }

        score += scoreToAdd;
    }

}
