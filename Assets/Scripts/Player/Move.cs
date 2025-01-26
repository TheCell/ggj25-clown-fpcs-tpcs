using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    /// <summary>
    /// Inform the player to look at a position. Position is in world space.
    /// </summary>
    public PositionEvent OnPlayerLookAt;

    [SerializeField] private float speedBase = 5f;
    [SerializeField] private float runningMultiplier = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform playerModel;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference sprint;
    private bool isRunning = false;
    public bool canMove = true;
    private float speed;


    private Vector3 velocity;
    private bool isGrounded;

    private void OnEnable()
    {
        move.action.Enable();
        sprint.action.Enable();
        sprint.action.performed += OnChangeRunning;
        OnPlayerLookAt.AddListener(TurnClownToLookAt);
    }

    private void OnDisable()
    {
        sprint.action.performed -= OnChangeRunning;
        move.action.Disable();
        sprint.action.Disable();
    }

    /// <summary>
    ///  Manually turn the clown to look at a position
    /// </summary>
    /// <param name="position">Position in world space</param>
    private void TurnClownToLookAt(Vector3 position)
    {
        RotatePlayer(position - transform.position);
    }

    private void OnChangeRunning(InputAction.CallbackContext context)
    {
        isRunning = !isRunning;
    }

    //Draw sphere gizmo at ground
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    private void Update()
    {
        if (!canMove)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 input = move.action.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * input.x + -transform.right * input.y; //Weird stuff
        speed = isRunning ? speedBase * runningMultiplier : speedBase;

        controller.Move(moveDirection * speed * Time.deltaTime);

        if (sprint.action.ReadValue<float>() > 0)
        {
            controller.Move(moveDirection * speed * 2 * Time.deltaTime);
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y -= gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        //Rotate towards direction
        if (moveDirection != Vector3.zero)
        {
            RotatePlayer(moveDirection);
        }
    }

    private void RotatePlayer(Vector3 direction)
    {
        playerModel.rotation = Quaternion.LookRotation(direction);
    }

    public void FreezePlayer(float duration)
    {
        StartCoroutine(FreezePlayerMovement(duration));
    }


    private IEnumerator FreezePlayerMovement(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }
}
