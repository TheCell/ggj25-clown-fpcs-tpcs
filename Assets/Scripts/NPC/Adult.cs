using System.Collections;
using System.Threading;
using NPC;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(RandomSoundPlayer))]
public class Adult : InteractionHistory, IShovable
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Animator billboardAnimator;
    [SerializeField] private GameObject billboard;
    [SerializeField] Animator emotionBillboardAnimator;
    [SerializeField] private Material[] emotionBillboardMaterials;
    [SerializeField] private GameObject emotionBillboard;

    private bool isBeingShoved;
    private AudioSource audioSource;
    private Vector3 billboardRelativeOffset;
    private Vector3 emotionBillboardRelatieveOffset;
    private float checkForPlayerRadius = 2f;
    private float playerLastSeenTimeStamp = 0f;
    private float continueWalkAfterSeconds = 2f;
    private Emotion currentEmotion = Emotion.Happy;
    private Animator animator;
    private RandomSoundPlayer randomSoundPlayer;

    private int destPoint = 0;
    private NavMeshAgent agent;

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void WitnessHappened()
    {
        agent.isStopped = true;
        animator.Play("Idle");

        billboard.transform.position = agent.transform.position + billboardRelativeOffset;
        billboardAnimator.Play(nameof(Interaction.Witness));
        randomSoundPlayer.PlayRandomAudioOneShot(audioSource);

        Invoke(nameof(WitnessEnded), 2f);
    }

    private void WitnessEnded()
    {
        agent.isStopped = false;
        animator.Play("Walk");
    }

    public void EyePokeHappened()
    {
        if (HasInteractedWith(Interaction.EyePoke))
        {
            return;
        }

        agent.isStopped = true;
        animator.Play("Idle");

        //billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        //billboardAnimator.Play(nameof(Interaction.EyePoke));
        //StartCoroutine(PlayAudioRandomDelayed());
        Invoke(nameof(EyePokeEnded), 2f);
        AddHasBeenInteractedWith(Interaction.EyePoke);
    }

    public void KickHappened()
    {
        agent.isStopped = true;
        animator.Play("Idle");
        //billboard.transform.position = agent.transform.position + billboardRelatieOffset;
        //billboardAnimator.Play(nameof(Interaction.Kick));
        //StartCoroutine(PlayAudioRandomDelayed());
        //Invoke(nameof(KickEnded), 2f);
        AddHasBeenInteractedWith(Interaction.Kick);
    }

    private void EyePokeEnded()
    {
        agent.isStopped = false;
        animator.Play("Walk");
    }

    public void PlaySameEmotion()
    {
        emotionBillboardAnimator.Play(currentEmotion.ToString());
    }

    public void SetEmotion(Emotion emotion)
    {
        currentEmotion = emotion;
        Material material = GetBillboardMaterial(emotion);
        emotionBillboard.GetComponent<MeshRenderer>().material = material;
        emotionBillboardAnimator.Play(emotion.ToString());
    }

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        randomSoundPlayer = GetComponent<RandomSoundPlayer>();

        billboardRelativeOffset = billboard.transform.position - transform.position;
        emotionBillboardRelatieveOffset = emotionBillboard.transform.position - transform.position;

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
        emotionBillboard.transform.position = agent.transform.position + emotionBillboardRelatieveOffset;

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
                    animator.Play("Idle");
                }
            }
        }

        if (!playerHasBeenSeenInFixedUpdate && agent.isStopped)
        {
            if ((Time.time - playerLastSeenTimeStamp) > continueWalkAfterSeconds)
            {
                agent.isStopped = false;
                animator.Play("Walk");
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

    private Material GetBillboardMaterial(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Happy:
                return emotionBillboardMaterials[0];
            case Emotion.Sad:
                return emotionBillboardMaterials[1];
            case Emotion.Depressed:
                return emotionBillboardMaterials[2];
            default:
                Debug.LogError("No emotion found for interaction: " + emotion);
                throw new System.ArgumentException();
        }
    }
}
