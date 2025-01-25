using System.Collections;
using GamePlay;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Adult : MonoBehaviour, IShovable
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject billboard;
    private AudioSource audioSource;
    private Vector3 billboardRelatieOffset;

    private int destPoint = 0;
    private NavMeshAgent agent;

    public void InterruptHappened()
    {
        agent.isStopped = true;

        billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        animator.Play(nameof(Interaction.Witness));
        StartCoroutine(PlayAudioRandomDelayed());

        Invoke(nameof(InterruptEnded), 2f);
    }

    private void InterruptEnded()
    {
        agent.isStopped = false;
    }

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        billboardRelatieOffset = billboard.transform.position - transform.position;

        FixPointsInWorld();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }

    private void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
        {
            return;
        }

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    private void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
    }

    private void FixPointsInWorld()
    {
        for (var i = points.Length - 1; i >= 0; i--)
        {
            points[i].SetParent(null, true);
        }
    }
    public IEnumerator GetShoved(Vector3 shoveDirection)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 shovePerFrame = shoveDirection / (duration / Time.deltaTime);

        while (elapsedTime < duration)
        {
            transform.Translate(shovePerFrame);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayAudioRandomDelayed()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.3f));
        audioSource.PlayOneShot(audioSource.clip);
    }
}
