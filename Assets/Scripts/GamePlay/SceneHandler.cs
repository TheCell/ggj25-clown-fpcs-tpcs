using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum SceneType
{
    STARTSCENE = 0,
    GAMESCENE = 1,
    ENDSCENE = 2,
    EMPTY = -1,
}

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }
    public SceneType currentScene { get; private set; } = SceneType.STARTSCENE;
    public SceneType nextScene { get; private set; } = SceneType.EMPTY;
    [SerializeField] private SceneTransitions sceneTransitions;

    [SerializeField] private SpriteRenderer transitionOverlay;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private float fadeDuration = 1.5f;

    private AsyncOperation asyncLoad;
    private bool leavingSceneLeft = false;
    [SerializeField] private float transitionMoveDistance = 9f;

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
        currentScene = (SceneType)SceneManager.GetActiveScene().buildIndex;
    }

    public static void GoToNextScene(int sceneIndexIncrement = 1)
    {
        int nextSceneIndex = (int)Instance.currentScene + sceneIndexIncrement;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Instance.SwitchScene((SceneType)nextSceneIndex);
        }
        else
        {
            Debug.LogError("No next scene in build order!");
        }

    }

    public static void GoToLastScene(int sceneIndexDecrement = 1)
    {
        int lastSceneIndex = (int)Instance.currentScene - sceneIndexDecrement;

        //handle exceptions for scenes that are not in the build order first
        SceneType currentScene = (SceneType)SceneManager.GetActiveScene().buildIndex;


        if (lastSceneIndex >= 0)
        {
            Instance.SwitchScene((SceneType)lastSceneIndex);
        }
        else
        {
            Debug.LogError("No last scene in build order!");
        }
    }

    public void SwitchScene(SceneType sceneType)
    {
        int targetBuildIndex = (int)sceneType;
        nextScene = sceneType;

        if (targetBuildIndex == -1)
        {
            Debug.LogError($"Scene {sceneType} not found!");
            return;
        }

        asyncLoad = SceneManager.LoadSceneAsync(targetBuildIndex);
        asyncLoad.allowSceneActivation = false;

        StartCoroutine(LoadSceneWithTransition());
    }

    private IEnumerator LoadSceneWithTransition()
    {
        yield return StartCoroutine(FadeSceneOut());

        asyncLoad.allowSceneActivation = true;

        yield return new WaitUntil(() => asyncLoad.isDone);

        //yield return null; // Wait for one frame after loading

        StartCoroutine(FadeSceneIn());
    }

    private SceneTransitionData GetTransitionData(SceneType sceneType)
    {
        return sceneTransitions.sceneTransitionsData.FirstOrDefault(x => x.sceneName == sceneType);
    }

    private IEnumerator FadeSceneOut()
    {
        float elapsedTime = 0f;

        Vector3 startPos = Camera.main.transform.position;
        SceneTransitionData transition = GetTransitionData(currentScene);
        Vector3 endPos = leavingSceneLeft ? new Vector3(transition.fadeInDirection.x * transitionMoveDistance, transition.fadeInDirection.y * transitionMoveDistance / 1.5f, Camera.main.transform.position.z) : new Vector3(transition.fadeOutDirection.x * transitionMoveDistance, transition.fadeOutDirection.y * transitionMoveDistance / 1.5f, Camera.main.transform.position.z);
        Color startColor = Color.clear;
        Color endColor = Color.clear;

        transitionOverlay.enabled = true;

        while (elapsedTime < fadeDuration && asyncLoad.progress <= 1f)
        {
            float t = fadeOutCurve.Evaluate(elapsedTime / fadeDuration);

            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, t);
            transitionOverlay.color = Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeSceneIn()
    {
        currentScene = nextScene;

        float elapsedTime = 0f;

        SceneTransitionData transition = GetTransitionData(currentScene);
        Vector3 startPos = leavingSceneLeft ? new Vector3(transition.fadeOutDirection.x * transitionMoveDistance, transition.fadeOutDirection.y * transitionMoveDistance / 1.5f, Camera.main.transform.position.z) : new Vector3(transition.fadeInDirection.x * transitionMoveDistance, transition.fadeInDirection.y * transitionMoveDistance / 1.5f, Camera.main.transform.position.z);
        Vector3 endPos = new Vector3(0, 0, Camera.main.transform.position.z);
        Color startColor = Color.clear;
        Color endColor = Color.clear;

        transitionOverlay.enabled = true;

        while (elapsedTime < fadeDuration)
        {
            float t = fadeInCurve.Evaluate(elapsedTime / fadeDuration);

            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, t);
            transitionOverlay.color = Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transitionOverlay.enabled = false;
        nextScene = SceneType.EMPTY;
    }
}
