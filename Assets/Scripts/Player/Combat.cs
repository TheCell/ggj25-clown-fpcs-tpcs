using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    [SerializeField] private InputActionReference attackTop;
    [SerializeField] private InputActionReference attackBottom;
    [SerializeField] Animator billboardAnimator;

    private float notifyAdultsRadius = 10f;
    private bool drawDebug = false;
    
    private void Start()
    {
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
        NotifyAdults();
        billboardAnimator.Play(nameof(Interactions.EyePoke));
    }

    private void OnAttackBottomPerformed(InputAction.CallbackContext ctx)
    {
    }

    private void NotifyAdults()
    {
        var adultsInRange = Physics.OverlapSphere(transform.position, notifyAdultsRadius);

        foreach (var adult in adultsInRange)
        {
            if (adult.CompareTag(nameof(Tags.Adult)))
            {
                adult.GetComponent<Adult>().InterruptHappened();
            }
        }
    }
}
