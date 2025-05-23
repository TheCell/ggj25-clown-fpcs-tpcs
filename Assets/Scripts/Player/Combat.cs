using NPC;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Combat : InteractionHistory
{
    [SerializeField] private InputActionReference attackTop;
    [SerializeField] private InputActionReference attackBottom;
    [SerializeField] Animator billboardAnimator;
    [SerializeField] GameObject interactionPosition;
    [SerializeField] private Material[] billboardMaterials;
    [SerializeField] private GameObject billboard;
    [SerializeField] private AudioClip kickAudioClip;
    [SerializeField] private AudioClip pokeAudioClip;
    [SerializeField] private AudioClip balloonPopAudioClip;
    private ScoreManager scoreManager;
    private AudioSource audioSource;
    private Move moveScript;

    private float notifyAdultsRadius = 10f;
    private float closeRangeCheck = 1f;
    private bool drawDebug = false;
    
    private void Start()
    {
        scoreManager = ScoreManager.instance;
        audioSource = GetComponent<AudioSource>();
        moveScript = GetComponent<Move>();
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (drawDebug)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, notifyAdultsRadius);
        }
    }

    private void OnEnable()
    {
        attackTop.action.Enable();
        attackBottom.action.Enable();
        attackTop.action.performed += OnAttackTopPerformed;
        attackBottom.action.performed += OnAttackBottomPerformed;
    }

    private void OnDisable()
    {
        attackTop.action.performed -= OnAttackTopPerformed;
        attackBottom.action.performed -= OnAttackBottomPerformed;
        attackTop.action.Disable();
        attackBottom.action.Disable();
    }

    private void OnAttackTopPerformed(InputAction.CallbackContext ctx)
    {
        moveScript.FreezePlayer(1f);
        //var ray = new Ray(interactionPosition.transform.position, interactionPosition.transform.forward);
        //Physics.Raycast(ray, out RaycastHit hit, closeRangeCheck);

        var collider = NullableHitScanObject();
        if (collider == null)
        {
            return;
        }

        moveScript.OnPlayerLookAt.Invoke(collider.gameObject.transform.position);
        if (collider.gameObject.CompareTag(nameof(Tag.Adult)))
        {
            var adult = collider.gameObject.GetComponentInParent<Adult>();
            if (adult.HasInteractedWith(Interaction.EyePoke))
            {
                audioSource.PlayOneShot(pokeAudioClip);
                billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Strike);
                billboardAnimator.Play(nameof(Interaction.Strike));
                scoreManager.scoreEvent.Invoke(Interaction.Strike, 0);
                adult.SetEmotion(Emotion.Depressed);

                return;
            }

            NotifyAdults(Interaction.EyePoke, collider.gameObject);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.EyePoke);
            billboardAnimator.Play(nameof(Interaction.EyePoke));
            adult.SetEmotion(Emotion.Sad);
            adult.EyePokeHappened();
        }
        else if (collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            var child = collider.gameObject.GetComponent<Child>();
            if (child.HasInteractedWith(Interaction.BubbleBurst))
            {
                audioSource.PlayOneShot(balloonPopAudioClip);
                billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Strike);
                billboardAnimator.Play(nameof(Interaction.Strike));
                scoreManager.scoreEvent.Invoke(Interaction.Strike, 0);
                child.SetEmotion(Emotion.Depressed);

                return;
            }

            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.BubbleBurst);
            billboardAnimator.Play(nameof(Interaction.BubbleBurst));
            child.BubbleBurstHappened();
            child.SetEmotion(Emotion.Sad);
            NotifyAdults(Interaction.BubbleBurst, null);
        }
    }

    private void OnAttackBottomPerformed(InputAction.CallbackContext ctx)
    {
        moveScript.FreezePlayer(1f);
        //var ray = new Ray(interactionPosition.transform.position, interactionPosition.transform.forward);
        //Physics.Raycast(ray, out RaycastHit hit, closeRangeCheck);
        var collider = NullableHitScanObject();
        if (collider == null)
        {
            return;
        }

        moveScript.OnPlayerLookAt.Invoke(collider.gameObject.transform.position);
        if (collider.gameObject.CompareTag(nameof(Tag.Adult)))
        {
            var adult = collider.gameObject.GetComponentInParent<Adult>();
            audioSource.PlayOneShot(kickAudioClip);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Kick);
            billboardAnimator.Play(nameof(Interaction.Kick));
            adult.KickHappened();
            adult.PlaySameEmotion();
            NotifyAdults(Interaction.Kick, collider.gameObject);
        }
        else if (collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            var child = collider.gameObject.GetComponent<Child>();
            audioSource.PlayOneShot(kickAudioClip);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Kick);
            billboardAnimator.Play(nameof(Interaction.Kick));
            child.KickHappened();
            child.PlaySameEmotion();
            NotifyAdults(Interaction.Kick, null);
        }
    }

    private void NotifyAdults(Interaction interaction, GameObject objectToIgnore)
    {
        var adultsInRange = Physics.OverlapSphere(transform.position, notifyAdultsRadius);
        var witnessCount = 0;

        foreach (var adult in adultsInRange)
        {
            if (objectToIgnore != null && adult.gameObject == objectToIgnore)
            {
                continue;
            }

            if (adult.CompareTag(nameof(Tag.Adult)))
            {
                adult.GetComponentInParent<Adult>().WitnessHappened();
                witnessCount++;
            }
        }

        scoreManager.scoreEvent.Invoke(interaction, witnessCount);
    }

    private Collider NullableHitScanObject()
    {
        var ray = new Ray(interactionPosition.transform.position, interactionPosition.transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, closeRangeCheck);
        Debug.DrawRay(interactionPosition.transform.position, interactionPosition.transform.forward, Color.cyan, 0.5f);
        if (hit.collider != null)
        {
            return hit.collider;
        }

        // check 45 degrees to the right
        ray = new Ray(interactionPosition.transform.position, Quaternion.Euler(0, 45, 0) * interactionPosition.transform.forward);
        Physics.Raycast(ray, out hit, closeRangeCheck);
        //Debug.DrawRay(interactionPosition.transform.position, Quaternion.Euler(0, 45, 0) * interactionPosition.transform.forward, Color.cyan, 0.5f);
        if (hit.collider != null)
        {
            return hit.collider;
        }

        // check 45 degrees to the left
        ray = new Ray(interactionPosition.transform.position, Quaternion.Euler(0, -45, 0) * interactionPosition.transform.forward);
        Physics.Raycast(ray, out hit, closeRangeCheck);
        //Debug.DrawRay(interactionPosition.transform.position, Quaternion.Euler(0, -45, 0) * interactionPosition.transform.forward, Color.cyan, 0.5f);
        if (hit.collider != null)
        {
            return hit.collider;
        }

        return null;
    }

    private Material GetBillboardMaterial(Interaction interaction)
    {
        switch (interaction)
        {
            case Interaction.EyePoke:
                return billboardMaterials[0];
            case Interaction.BubbleBurst:
                return billboardMaterials[1];
            case Interaction.Kick:
                return billboardMaterials[2];
            case Interaction.Strike:
                return billboardMaterials[3];
            default:
                Debug.LogError("No material found for interaction: " + interaction);
                throw new System.ArgumentException();
        }
    }
}
