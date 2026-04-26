using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launchpad : MonoBehaviour
{

    Collider m_bouncepadTrigger;
    AudioSource m_bouncepadAudio;

    public GameObject target; // Where the launchpad will project the player towards on contact
    public Vector3 offset; // Units to offset target position by
    [Range(1.0f, 89.0f)]
    public float launchAngle; // what angle to try and launch the player at
    [Range(0.0f, 1.0f)]
    public float speedNegation; // how much of the player's current speed is lost when using
    [Min(0.0f)]
    public float restrictMovementTime; // how long the player can't move for after hitting the launchpad
    public bool isSticky = false;

    // Start is called before the first frame update
    void Start()
    {
        m_bouncepadTrigger = GetComponent<Collider>();
        m_bouncepadAudio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // play audio
            m_bouncepadAudio.Play();
            // send launch data to player
            other.SendMessage("Launch", GetLaunchData());
        }
    }

    public LaunchData GetLaunchData()
    {
        return new LaunchData(RealTarget(), launchAngle, speedNegation, restrictMovementTime, isSticky);
    }
    
    public Vector3 RealTarget()
    {
        return target.transform.position + offset;
    }

    // Returns a list of points that visualize the projectile arc when drawn
    static Vector3[] ArcPoints(int points, Vector3 startPosition, Vector3 startVelocity, float travelTime)
    {
        // get timestep per point
        float timePerPoint = travelTime / points;

        Vector3[] output = new Vector3[points+1];
        for (int i = 0; i <= points; i++)
        {
            float time = i * timePerPoint;
            output[i] = startPosition + time * startVelocity;
            output[i].y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2.0f * time * time);
        }
        
        return output;
    }  

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // draw destination
        Gizmos.DrawSphere(RealTarget(), 0.25f);

        // get variables
        LaunchData launchdata = GetLaunchData();
        Vector3 startPosition = transform.position;
        Vector3 velocity = launchdata.LaunchVelocity(transform.position);
        float travelTime = launchdata.TravelTime(transform.position);

        // calculate points
        Vector3[] points = ArcPoints(10, startPosition, velocity, travelTime);
        // draw line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLineStrip(points, false);
        // place an red sphere where the player regains control
        Gizmos.color = Color.red;
        Vector3 controlPoint = startPosition + restrictMovementTime * velocity;
        controlPoint.y = startPosition.y + velocity.y * restrictMovementTime + (Physics.gravity.y / 2.0f * restrictMovementTime * restrictMovementTime);
        Gizmos.DrawSphere(controlPoint, 0.25f);
        
    }
}
