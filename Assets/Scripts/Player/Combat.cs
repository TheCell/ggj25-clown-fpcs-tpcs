using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    [SerializeField] private InputActionReference attackTop;
    [SerializeField] private InputActionReference attackBottom;
    [SerializeField] Animator billboardAnimator;
    [SerializeField] GameObject interactionPosition;
    private ScoreManager scoreManager;

    private float notifyAdultsRadius = 10f;
    private float closeRangeCheck = 1f;
    private bool drawDebug = true;
    
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

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(interactionPosition.transform.position, closeRangeCheck);
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
        Physics.SphereCast(interactionPosition.transform.position, closeRangeCheck, transform.forward, out RaycastHit hit, closeRangeCheck);
        Debug.DrawRay(interactionPosition.transform.position, transform.forward, Color.red, 1f);
        Debug.Log(hit.collider);
        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.gameObject.CompareTag(nameof(Tag.Child)))
        {
            NotifyAdults();
            billboardAnimator.Play(nameof(Interaction.EyePoke));
        }
    }

    private void OnAttackBottomPerformed(InputAction.CallbackContext ctx)
    {
    }

    private void NotifyAdults()
    {
        var adultsInRange = Physics.OverlapSphere(transform.position, notifyAdultsRadius);
        var witnessCount = 0;

        foreach (var adult in adultsInRange)
        {
            if (adult.CompareTag(nameof(Tag.Adult)))
            {
                adult.GetComponentInParent<Adult>().InterruptHappened();
                witnessCount++;
            }
        }

        scoreManager.scoreEvent.Invoke(Interaction.EyePoke, witnessCount);
    }
}
