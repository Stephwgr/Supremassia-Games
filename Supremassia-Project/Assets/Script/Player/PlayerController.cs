using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float _speed = 5f;
    public float _aimSensitivity = 2f;

    private CharacterController _controller;

    private Vector2 _moveInput;
    private Vector2 _aimInput;

    private float yVelocity;
    private float xRotation = 0f;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement(); // Handle Movement = Gérer les déplacement
        HandleAim();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>();
    }
    public void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        if(_controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }
        yVelocity += -9.81f * Time.deltaTime;

        Vector3 velocity = move * _speed;
        velocity.y = yVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }

    public void HandleAim()
    {
        
    }
}
