using NPC;
using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    [SerializeField] private InputActionReference attackTop;
    [SerializeField] private InputActionReference attackBottom;
    [SerializeField] Animator billboardAnimator;
    [SerializeField] GameObject interactionPosition;
    [SerializeField] private Material[] billboardMaterials;
    [SerializeField] private GameObject billboard;
    private ScoreManager scoreManager;


    private float notifyAdultsRadius = 10f;
    private float closeRangeCheck = 1f;
    private bool drawDebug = false;
    
    private void Start()
    {
        scoreManager = Object.FindObjectsByType<ScoreManager>(FindObjectsSortMode.InstanceID)[0];
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
            NotifyAdults(Interaction.EyePoke, hit.collider.gameObject);
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.EyePoke);
            billboardAnimator.Play(nameof(Interaction.EyePoke));
        }
        else if (hit.collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.BubbleBurst);
            billboardAnimator.Play(nameof(Interaction.BubbleBurst));
            hit.collider.gameObject.GetComponent<Child>().BubbleBurstHappened();
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
            NotifyAdults(Interaction.Kick, hit.collider.gameObject);
            //billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.k);
            billboardAnimator.Play(nameof(Interaction.Kick));
        }
        else if (hit.collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            //billboard.GetComponent<MeshRenderer>().material = GetBillboardMaterial(Interaction.BubbleBurst);
            billboardAnimator.Play(nameof(Interaction.Kick));
            //hit.collider.gameObject.GetComponent<Child>().BubbleBurstHappened();
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

    private Material GetBillboardMaterial(Interaction interaction)
    {
        switch (interaction)
        {
            case Interaction.EyePoke:
                return billboardMaterials[0];
            case Interaction.BubbleBurst:
                return billboardMaterials[1];
            default:
                Debug.LogError("No material found for interaction: " + interaction);
                throw new System.ArgumentException();
        }
    }
}
