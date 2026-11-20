using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private float _aimDeadZone = 1.5f;
    [SerializeField] float _deadZoneRadius = 80f; // Rayon de la Dead Zone en pixels

    [SerializeField] private Transform _playerVisual;
    [SerializeField] private Transform _muzzlePoint;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _speedBullet;

    public float _speed = 5f;
    public float _aimSensitivity = 2f;

    private CharacterController _controller;

    private Vector2 _moveInput;
    private Vector2 _aimInput;


    private float yVelocity;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement(); // Handle Movement = Gérer les déplacement
        HandleMouseDeadZone();
        HandleAim();


        if (_muzzlePoint != null)
        {
            Debug.DrawRay(_muzzlePoint.position, _muzzlePoint.forward * 5f, Color.red);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _aimInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var bullet = Instantiate(_bulletPrefab,_muzzlePoint.position, _muzzlePoint.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = _muzzlePoint.forward*_speedBullet;
        }
    }

    public void HandleMovement()
    {
        Vector3 move = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;

        if (_controller.isGrounded && yVelocity < 0)
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
        if (_playerVisual == null || _muzzlePoint == null)
            return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(_aimInput);

        Plane aimPlane = new Plane(
            Vector3.up,
            _muzzlePoint.position
        );

        if (aimPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            // Distance horizontale entre le joueur et la souris
            Vector3 playerToMouse =
                targetPoint - _playerVisual.position;

            playerToMouse.y = 0f;

            // DEAD ZONE :
            // Si la souris est trop proche du joueur,
            // on conserve la dernière rotation.
            if (playerToMouse.sqrMagnitude <
                _aimDeadZone * _aimDeadZone)
            {
                return;
            }

            Vector3 pivot = _playerVisual.position;

            // Correction de la visée depuis le canon
            for (int i = 0; i < 5; i++)
            {
                Vector3 direction =
                    targetPoint - _muzzlePoint.position;

                direction.y = 0f;

                if (direction.sqrMagnitude < 0.0001f)
                    break;

                Vector3 gunForward = _muzzlePoint.forward;
                gunForward.y = 0f;

                if (gunForward.sqrMagnitude < 0.0001f)
                    break;

                float angle = Vector3.SignedAngle(
                    gunForward,
                    direction,
                    Vector3.up
                );

                _playerVisual.RotateAround(
                    pivot,
                    Vector3.up,
                    angle
                );
            }
        }
    }

    private void HandleMouseDeadZone()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        // Centre du joueur sur l'écran
        Vector3 playerScreenPosition =
            Camera.main.WorldToScreenPoint(transform.position);

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector2 center = new Vector2(
            playerScreenPosition.x,
            playerScreenPosition.y
        );

        Vector2 direction = mousePosition - center;

        // Si la souris entre dans la zone interdite
        if (direction.sqrMagnitude <
            _deadZoneRadius * _deadZoneRadius)
        {
            // Évite une direction nulle
            if (direction.sqrMagnitude < 0.001f)
            {
                direction = Vector2.up;
            }

            // Replace la vraie souris sur le bord
            Vector2 newMousePosition =
                center + direction.normalized * _deadZoneRadius;

            Mouse.current.WarpCursorPosition(newMousePosition);

            // Actualise la position utilisée par la visée
            _aimInput = newMousePosition;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (_playerVisual == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_playerVisual.position, _aimDeadZone);
    }


}
