using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public ScoreManager instance { get; private set; }
    public ScoreEvent scoreEvent;

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
        Debug.Log($"Score received for {interaction} with {witnesscount} witnesses");
    }
}
