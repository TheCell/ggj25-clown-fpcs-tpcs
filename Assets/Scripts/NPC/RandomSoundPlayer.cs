using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    public void PlayRandomAudioOneShot(AudioSource audioSource)
    {
        audioSource.PlayOneShot(GetRandomClip());
    }

    private AudioClip GetRandomClip()
    {
        return sounds[Random.Range(0, sounds.Length)];
    }
}
