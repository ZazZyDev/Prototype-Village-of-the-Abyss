using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    private Vector2 cameraMovementVector;
    private Vector3 lastMousePosition;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float sensitivity = 0.1f;

    public LayerMask groundMask;

    public Vector2 CameraMovementVector
    {
        get { return cameraMovementVector; }
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckCameraMovement();
    }
    private Vector3Int? RaycastGround()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            // Draw a debug ray in the scene view for visualization
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1f);

            // Adjust the hit point and floor it to the nearest integer grid position
            Vector3Int positionInt = new Vector3Int(Mathf.FloorToInt(hit.point.x + 0.5f), 0, Mathf.FloorToInt(hit.point.z + 0.5f));
            Debug.Log("Adjusted position: " + positionInt);

            return positionInt;
        }
        return null;
    }


    private void CheckCameraMovement()
    {
        cameraMovementVector = Vector2.zero;

        if (Input.GetMouseButtonDown(2)) // Middle mouse button clicked
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2)) // Middle mouse button held down
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            cameraMovementVector = new Vector2(mouseDelta.x * sensitivity, mouseDelta.y * sensitivity);

            lastMousePosition = currentMousePosition;
        }

        // Check for arrow key input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        cameraMovementVector += new Vector2(horizontalInput, verticalInput);
    }

    private void CheckClickHoldEvent()
    {
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButton(0) && !isPointerOverUI)
        {
            var position = RaycastGround();
            if (position.HasValue)
                OnMouseHold?.Invoke(position.Value);
        }
    }

    private void CheckClickUpEvent()
    {
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonUp(0) && !isPointerOverUI)
        {
            OnMouseUp?.Invoke();
        }
    }

    private void CheckClickDownEvent()
    {
        bool isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0) && !isPointerOverUI)
        {
            var position = RaycastGround();
            if (position.HasValue)
            {
                Debug.Log("Valid ground clicked at position: " + position.Value); // Newly added debug log
                OnMouseClick?.Invoke(position.Value);
            }
            else
            {
                Debug.Log("No valid ground detected."); // Newly added debug log
            }
        }
    }

}
