using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _mouseSensitivity = 3.0f;

    private float _rotationY;
    private float _rotationX;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform dirTarget;

    [SerializeField]
    private float _distanceFromTarget = 3f;

    Vector3 _currentRotation;
    Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.4f;

    [SerializeField]
    private Vector2 _rotationMinMax = new Vector2(-40, 40);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            _rotationY += mouseX;
            _rotationX += mouseY;

            _rotationX = Mathf.Clamp(_rotationX, _rotationMinMax.x ,_rotationMinMax.y);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

            _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
            transform.localEulerAngles = _currentRotation;
        }
        transform.position = _target.position - transform.forward.normalized * _distanceFromTarget;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            transform.forward = dirTarget.forward;
        }
    }
}
