using System.Collections;
using System.Threading;
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
    private float checkForPlayerRadius = 2f;
    private float playerLastSeenTimeStamp = 0f;
    private float continueWalkAfterSeconds = 2f;

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
        if (HasInteractedWith(Interaction.EyePoke))
        {
            return;
        }

        agent.isStopped = true;
        //billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        //billboardAnimator.Play(nameof(Interaction.EyePoke));
        //StartCoroutine(PlayAudioRandomDelayed());
        Invoke(nameof(EyePokeEnded), 2f);
        AddHasBeenInteractedWith(Interaction.EyePoke);
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

    bool playerHasBeenSeenInFixedUpdate = false;
    private void FixedUpdate()
    {
        playerHasBeenSeenInFixedUpdate = false;

        // this is in no way performant, but it's a game jam so ¯\_(ツ)_/¯
        var colliders = Physics.OverlapSphere(agent.transform.position, checkForPlayerRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag(nameof(Tag.Player)))
            {
                playerHasBeenSeenInFixedUpdate = true;
                playerLastSeenTimeStamp = Time.time;

                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                }
            }
        }

        if (!playerHasBeenSeenInFixedUpdate && agent.isStopped)
        {
            if ((Time.time - playerLastSeenTimeStamp) > continueWalkAfterSeconds)
            {
                agent.isStopped = false;
            }
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
