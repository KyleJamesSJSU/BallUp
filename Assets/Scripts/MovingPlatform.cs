using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum MovementMode
    {
        Linear,
        Smoothed
    }

    public enum MovementState
    {
        StoppedAtStart,
        MovingToEnd,
        StoppedAtEnd,
        MovingToStart
    }
    
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;
    private GameObject movingObject;

    [SerializeField]
    [Min(0.0f)]
    public float stopDuration = 1.0f;
    [SerializeField]
    [Min(0.1f)]
    public float movementPeriod = 1.0f;
    [SerializeField]
    public float timeOffset = 0.0f;

    [SerializeField]
    public MovementMode movementMode;

    private Vector3 lastPosition;
    
    private float stayAtStart;
    private float lerpToEnd;
    private float stayAtEnd;
    private float timeLimit;

    public MovementState GetMovementState(float currentTime)
    {
        if (currentTime <= stayAtStart)
        {
            // stay at start
            return MovementState.StoppedAtStart;
        } 
        else if (currentTime > stayAtStart && currentTime <= lerpToEnd)
        {
            // lerp to end point from start
            return MovementState.MovingToEnd;
        }
        else if (currentTime > lerpToEnd && currentTime <= stayAtEnd)
        {
            // stay at end
            return MovementState.StoppedAtEnd;
        }
        else 
        {
            // lerp to start point from end
            return MovementState.MovingToStart;
        }
    }

    private static float SmoothLerpValue(float lerp)
    {
        // simple cosine function to smooth out the value
        return (Mathf.Cos(lerp * Mathf.PI) / -2.0f) + 0.5f;
    }

    void Start()
    {
        movingObject = transform.Find("MovingObject").gameObject;
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update last position
        lastPosition = movingObject.transform.position;
        
        stayAtStart = stopDuration;
        lerpToEnd = movementPeriod + stopDuration;
        stayAtEnd = movementPeriod + (stopDuration * 2);
        timeLimit = 2.0f * (stopDuration + movementPeriod);
        
        // used FixedTime to animate
        float currentTime = (Time.fixedTime + timeOffset) % timeLimit;

        // calculate lerp based on mode
        float lerp = 0.0f;
        // this code sucks
        switch (GetMovementState(currentTime))
        {
            case MovementState.StoppedAtStart:
                lerp = 0.0f;
                break;
            case MovementState.MovingToEnd:
                lerp = (currentTime - stayAtStart) / movementPeriod;
                break;
            case MovementState.StoppedAtEnd:
                lerp = 1.0f;
                break;
            case MovementState.MovingToStart:
                lerp = 1.0f - ((currentTime - stayAtEnd) / movementPeriod);
                break;
            default:
                break;
        }
        
        switch (movementMode)
        {
            case MovementMode.Linear:
                // do not modify the lerp
                break;
            case MovementMode.Smoothed:
                // modify the lerp
                lerp = SmoothLerpValue(lerp);
                break;
            default:
                break;
        }
        // apply lerp
        movingObject.transform.position = Vector3.Lerp(start.transform.position, end.transform.position, lerp);
    }

    void OnDrawGizmos()
    {
        // draw spheres at start and end
        // Gizmos.color = Color.green;
        // Gizmos.DrawSphere(start.transform.position, 0.25f);
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(end.transform.position, 0.25f);
        // draw line between start and end points
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }
}
