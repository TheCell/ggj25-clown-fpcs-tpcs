using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float timeUntilCopsArrive = 60f;
    [SerializeField] private AudioClip introAudioClip;
    [SerializeField] private AudioClip gameAudioClip;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource soundEffectsAudioSource;
    private AudioMixer audioMixer;
    [SerializeField] private AudioClip mumblingAudioClip;
    [SerializeField] private RandomSoundPlayer policeScannerRandomSoundPlayer;
    [SerializeField] private RandomSoundPlayer sirenRandomSoundPlayer;

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
        backgroundAudioSource.clip = introAudioClip;
        backgroundAudioSource.Play();
    }

    private bool playedCopArriving = false;
    private bool playedMumbling = false;
    private void Update()
    {
        if (SceneHandler.Instance.currentScene == SceneType.GAMESCENE && backgroundAudioSource.clip != gameAudioClip)
        {
            backgroundAudioSource.clip = gameAudioClip;
            backgroundAudioSource.Play();
        }

        if (SceneHandler.Instance.currentScene == SceneType.STARTSCENE && backgroundAudioSource.clip != introAudioClip)
        {
            backgroundAudioSource.clip = introAudioClip;
            backgroundAudioSource.Play();
        }

        if (SceneHandler.Instance.currentScene == SceneType.GAMESCENE && !isGamePaused)
        {
            if (!playedMumbling)
            {
                playedMumbling = true;
                soundEffectsAudioSource.PlayOneShot(mumblingAudioClip);
            }

            timeUntilCopsArriveCounter += Time.deltaTime;
            if (timeUntilCopsArriveCounter >= (timeUntilCopsArrive - 6f))
            {
                playedCopArriving = true;
                policeScannerRandomSoundPlayer.PlayRandomAudioOneShot(soundEffectsAudioSource);
            }

            if (timeUntilCopsArriveCounter >= timeUntilCopsArrive)
            {
                EndGame();
            }
        }
    }

    public void EndGame()
    {
        isGamePaused = true;
        playedCopArriving = false;
        playedMumbling = false;
        sirenRandomSoundPlayer.PlayRandomAudioOneShot(soundEffectsAudioSource);
        isTrueEnding = totalChildren == balloonsPopped;
        timeUntilCopsArriveCounter = 0f;
        SceneHandler.GoToNextScene();
    }

    public void AddScore(int scoreToAdd)
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            balloonsPopped = 0;
        }

        score += scoreToAdd;
    }

}
