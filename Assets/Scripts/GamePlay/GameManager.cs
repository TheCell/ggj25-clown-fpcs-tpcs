using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private float timeUntilCopsArrive = 60f;
    private AudioSource audioSource;
    private AudioMixer audioMixer;

    private float timeUntilCopsArriveCounter = 0f;
    public bool isGamePaused = false;
    public bool isTrueEnding = false;
    public int score = 0;

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
                isGamePaused = true;
                timeUntilCopsArriveCounter = 0f;
                SceneHandler.GoToNextScene();
            }
        }
    }

}
