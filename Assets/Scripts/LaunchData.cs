using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchData
{
    public Vector3 destination;
    public float launchAngle;
    public float speedNegation;
    public float restrictMovementTime;
    public bool sticky;
    
    public LaunchData(Vector3 destination, float launchAngle, float speedNegation, float restrictMovementTime, bool sticky)
    {
        this.destination = destination;
        this.launchAngle = launchAngle;
        this.speedNegation = speedNegation;
        this.restrictMovementTime = restrictMovementTime;
        this.sticky = sticky;
    }

    // code for calculating projectile motion taken from unity forums
    public Vector3 LaunchVelocity(Vector3 start)
    {
        Vector3 p = destination;

        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = launchAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(start.x, 0, start.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = start.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > start.x ? 1 : -1);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
        
        return finalVelocity;
    }

    public float TravelTime(Vector3 start)
    {
        Vector3 p = destination;

        float gravity = Physics.gravity.magnitude;
        // Selected angle in radians
        float angle = launchAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(start.x, 0, start.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);

        float yOffset = start.y - p.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        
        // time should be simple
        return distance / (initialVelocity * Mathf.Cos(angle));
    }
}
