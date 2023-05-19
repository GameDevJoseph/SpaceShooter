using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 3.5f;

    [SerializeField] PlayerInput _playerInput;

    [SerializeField] float _leftBoundary = -11.5f;
    [SerializeField] float _rightBounary = 11.5f;
    [SerializeField] float _bottomBoundary = -3.5f;



    Vector3 newPosition;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //take the current position = new position
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

    }

    void CalculateMovement()
    {
        //sets a local variable for direction to current vector3 input
        var direction = _playerInput.actions["Move"].ReadValue<Vector2>();

        //move the player using local direction variable
        transform.Translate(direction * _moveSpeed * Time.deltaTime);


        if (transform.position.x > 11.5f)
            SetNewPosition(new Vector3(_leftBoundary, transform.position.y, transform.position.z));

        if (transform.position.x < -11.5f)
            SetNewPosition(new Vector3(_rightBounary, transform.position.y, transform.position.z));

        if (transform.position.y <= -3.5f)
            SetNewPosition(new Vector3(transform.position.x, _bottomBoundary, transform.position.z));
    }

    Vector3 SetNewPosition(Vector3 pos)
    {
        newPosition = pos;
        transform.position = newPosition;
        return newPosition;
    }
}
