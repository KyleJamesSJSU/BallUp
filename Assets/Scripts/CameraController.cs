using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        Player = 0,
        Fixed = 1
    }
    
    public GameObject player;
    public CameraMode currentCameraMode = CameraMode.Player;

    public float cameraDistance = 8.0f;
    public Vector2 cameraDistanceLimit = new Vector2(2.0f, 32.0f);
    public float horizontalSensitivity = 1.0f;
    public float verticalSensitivity = 0.5f;
    public float zoomSensitivity = 2.0f;
    public float cameraMaxAngle = 89.0f;
    public Vector2 startingRotation = new Vector2(0.0f, 20.0f);

    private Vector2 cameraRotation;
    private float cameraDistanceGoal;

    // Locks camera to a specific orientation until unlocked
    public void SetCameraPosition(Transform newTransform)
    {
        currentCameraMode = CameraMode.Fixed;
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }

    void Start()
    {
        // set default location based on camera direction
        cameraDistanceGoal = cameraDistance;
        cameraRotation = startingRotation;
        transform.position = player.transform.position + (-transform.forward * cameraDistance);
    }

    public void OnLook(InputValue value)
    {
        if (currentCameraMode == CameraMode.Player)
        {
            // rotate camera around player position based on vector from Look
            var v = value.Get<Vector2>();
            cameraRotation += new Vector2(v.x * horizontalSensitivity, v.y * verticalSensitivity); // apply sensitivity here
            
            // limit y angle
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -cameraMaxAngle, cameraMaxAngle);
            // TODO do we need to check if x goes too high/low? to prevent out of bounds

            // set rotation
            transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
        }
    }

    public void OnZoom(InputValue value)
    {
        if (currentCameraMode == CameraMode.Player)
        {
            var v = value.Get<Vector2>();
            cameraDistanceGoal += -v.y * Mathf.Log(cameraDistance, 2);
            cameraDistanceGoal = Mathf.Clamp(cameraDistanceGoal, cameraDistanceLimit.x, cameraDistanceLimit.y);
        }
    }

    void LateUpdate()
    {
        if (currentCameraMode == CameraMode.Player)
        {
            // set position of camera depending on rotation of camera
            cameraDistance = Mathf.MoveTowards(cameraDistance, cameraDistanceGoal, zoomSensitivity * cameraDistance * Time.deltaTime);
            transform.position = player.transform.position + (-transform.forward * cameraDistance);
        }
        else if (currentCameraMode == CameraMode.Fixed)
        {
            // keep camera in place, don't move it
        }
    }
}
