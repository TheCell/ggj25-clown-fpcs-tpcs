using System.Collections;
using NPC;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Adult : InteractionHistory, IShovable
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Animator billboardAnimator;
    [SerializeField] private GameObject billboard;

    private bool isBeingShoved;
    private AudioSource audioSource;
    private Vector3 billboardRelatieOffset;

    private int destPoint = 0;
    private NavMeshAgent agent;

    public void WitnessHappened()
    {
        agent.isStopped = true;

        billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        billboardAnimator.Play(nameof(Interaction.Witness));
        StartCoroutine(PlayAudioRandomDelayed());

        Invoke(nameof(WitnessEnded), 2f);
    }

    private void WitnessEnded()
    {
        agent.isStopped = false;
    }

    public void EyePokeHappened()
    {
        agent.isStopped = true;
        billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        billboardAnimator.Play(nameof(Interaction.EyePoke));
        //StartCoroutine(PlayAudioRandomDelayed());
        Invoke(nameof(EyePokeEnded), 2f);
    }

    private void EyePokeEnded()
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
        if (isBeingShoved) return;
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
    public IEnumerator GetShoved(Vector3 shoveDirection, float shoveDuration)
    {
        float elapsedTime = 0f;
        Vector3 shovePerFrame = shoveDirection / (shoveDuration / Time.deltaTime);

        isBeingShoved = true;
        agent.enabled = false;

        while (elapsedTime < shoveDuration)
        {
            transform.Translate(shovePerFrame);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        agent.enabled = true;
        isBeingShoved = false;
    }

    private IEnumerator PlayAudioRandomDelayed()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.3f));
        audioSource.PlayOneShot(audioSource.clip);
    }
}
