using NPC;
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
    private ScoreManager scoreManager;
    private AudioSource audioSource;

    private float notifyAdultsRadius = 10f;
    private float closeRangeCheck = 1f;
    private bool drawDebug = false;
    
    private void Start()
    {
        scoreManager = ScoreManager.instance;
        audioSource = GetComponent<AudioSource>();
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
        var ray = new Ray(interactionPosition.transform.position, interactionPosition.transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, closeRangeCheck);
        
        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.gameObject.CompareTag(nameof(Tag.Adult)))
        {
            var adult = hit.collider.gameObject.GetComponentInParent<Adult>();
            if (adult.HasInteractedWith(Interaction.EyePoke))
            {
                billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Strike);
                billboardAnimator.Play(nameof(Interaction.Strike));
                scoreManager.scoreEvent.Invoke(Interaction.Strike, 0);

                return;
            }

            NotifyAdults(Interaction.EyePoke, hit.collider.gameObject);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.EyePoke);
            billboardAnimator.Play(nameof(Interaction.EyePoke));
            adult.EyePokeHappened();
        }
        else if (hit.collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            var child = hit.collider.gameObject.GetComponent<Child>();
            if (child.HasInteractedWith(Interaction.BubbleBurst))
            {
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
        var ray = new Ray(interactionPosition.transform.position, interactionPosition.transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, closeRangeCheck);

        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.gameObject.CompareTag(nameof(Tag.Adult)))
        {
            var adult = hit.collider.gameObject.GetComponentInParent<Adult>();
            audioSource.PlayOneShot(audioSource.clip);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Kick);
            billboardAnimator.Play(nameof(Interaction.Kick));
            adult.KickHappened();
            NotifyAdults(Interaction.Kick, hit.collider.gameObject);
            //if (!adult.HasInteractedWith(Interaction.Kick))
            //{
            //}
        }
        else if (hit.collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            var child = hit.collider.gameObject.GetComponent<Child>();
            audioSource.PlayOneShot(audioSource.clip);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.Kick);
            billboardAnimator.Play(nameof(Interaction.Kick));
            child.KickHappened();
            NotifyAdults(Interaction.Kick, null);
            //if (!child.HasInteractedWith(Interaction.Kick))
            //{
            //}
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
