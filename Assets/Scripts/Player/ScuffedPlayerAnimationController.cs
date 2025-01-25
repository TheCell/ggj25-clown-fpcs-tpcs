using UnityEngine;
using UnityEngine.InputSystem;

public class ScuffedPlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionReference shoveAction;
    [SerializeField] private InputActionReference kickAction;
    [SerializeField] private InputActionReference punchAction;

    private void OnDisable()
    {
        shoveAction.action.performed -= OnShove;
        kickAction.action.performed -= OnKick;
        punchAction.action.performed -= OnPunch;
    }

    private void OnEnable()
    {
        shoveAction.action.performed += OnShove;
        kickAction.action.performed += OnKick;
        punchAction.action.performed += OnPunch;
    }
    private void OnShove(InputAction.CallbackContext context)
    {
        animator.Play("Shove");
    }

    private void OnKick(InputAction.CallbackContext context)
    {
        animator.Play("Kick");
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        animator.Play("Punch");
    }
}
