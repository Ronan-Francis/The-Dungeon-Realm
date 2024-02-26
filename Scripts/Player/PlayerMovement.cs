using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private float speed = 5f;
    public Vector2 moveInput;
    public bool canMoveRight = true;
    public bool canMoveLeft = true;
    public bool canMoveUp = true;
    public bool canMoveDown = true;

    public bool isTouchingWall = false;
    public float breakTime = 1f;

    private void OnEnable()
    {
        movementAction.action.Enable();
        movementAction.action.performed += OnMovementPerformed;
        movementAction.action.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        movementAction.action.Disable();
        movementAction.action.performed -= OnMovementPerformed;
        movementAction.action.canceled -= OnMovementCanceled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 move = Vector3.zero;
        if ((moveInput.x > 0 && canMoveRight) || (moveInput.x < 0 && canMoveLeft))
        {
            move.x = moveInput.x;
        }
        if ((moveInput.y > 0 && canMoveUp) || (moveInput.y < 0 && canMoveDown))
        {
            move.y = moveInput.y;
        }

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
