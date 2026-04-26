using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    // Class that handles the bumper objects
    // When the player collides, animates the bumper and triggers a velocity change on the player dependent on their position
    
    GameObject m_bouncer;
    Animator m_bouncerAnimator;

    Collider m_bumperTrigger;
    AudioSource m_bumperAudio;

    public float bumpStrength; // Determines how much force is applied to the rigidbody
    public float speedNegation; // how much speed the player loses on contact

    void Start()
    {
        m_bouncer = transform.Find("Bouncer").gameObject;
        m_bouncerAnimator = m_bouncer.GetComponent<Animator>();
        m_bumperTrigger = GetComponent<Collider>();
        m_bumperAudio = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // trigger bumper
            m_bouncerAnimator.SetTrigger("Bounce");
            // play sound effect
            m_bumperAudio.Play();
            // send a bump to the player object
            BumpData bumpdata = new BumpData(transform.position, bumpStrength, speedNegation);
            other.SendMessage("Bump", bumpdata);
        }
    }
}
