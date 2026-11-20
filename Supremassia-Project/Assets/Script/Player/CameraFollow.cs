
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 15f, -5f);
    [SerializeField] private float _smoothSpeed = 10f;

    private Quaternion _initialRotation;

    void Start()
    {
        // Mémorise la rotation initiale de la caméra
        _initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (_target == null) return;

        // Position souhaitée
        Vector3 targetPosition = _target.position + _offset;

        // Suivi fluide du personnage
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            _smoothSpeed * Time.deltaTime
        );

        // Conserve toujours la même rotation
        transform.rotation = _initialRotation;
    }
}
