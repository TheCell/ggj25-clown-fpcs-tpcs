using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RandomSoundPlayer))]
public class PlayTrueEndingWuhuu : MonoBehaviour
{
    private RandomSoundPlayer randomSoundPlayer;
    private AudioSource audioSource;
    private GameManager gameManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.Instance;
        randomSoundPlayer = GetComponent<RandomSoundPlayer>();

        if (gameManager.isTrueEnding)
        {
            randomSoundPlayer.PlayRandomAudioOneShot(audioSource);
        }
    }
}
