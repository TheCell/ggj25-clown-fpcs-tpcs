using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private float timeUntilCopsArrive = 60f;
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
