using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Min(0.0f)]
    public float rollSpeed = 1.0f;
    [Min(0.0f)]
    public float brakeStrength = 1.0f;
    [Min(0.0f)]
    public float brakeCostFactor = 1.0f;
    [Min(0.0f)]
    public float brakeLimit = 100.0f;
    [Min(0.0f)]
    public float brakeRechargePerSecond = 20.0f;
    [Min(0.0f)]
    public float brakeEmitterFactor = 1.5f;
    [Min(0.0f)]
    public float jumpStrength = 1.0f;
    [Min(0.0f)]
    public float jumpCooldown = 2.0f;

    public float groundCastDistance = 0.125f;
    [Min(0.0f)]
    public float groundCastRadius = 0.4f;

    public GameObject effectsHandler;

    Rigidbody rb;
    PlayerInput playerInput; 
    SphereCollider sphereCollider;


    private ParticleSystem brakeEmitter;
    

    private Vector2 movementVector;

    private bool isJumping = false;
    private bool isBraking = false;
    private float lastJumpTime;
    private float brakeCapacity;

    private GameObject movingPlatform;
    public GameObject playerContainer;

    float lockedMovementTime;

    private Vector3 extraMovement;
    private MovingPlatform movingPlatformRef;

    private Vector3 startingPosition;

    [Space]
    [Header("Audio References")]
    [SerializeField]
    private AudioSource audioJump;
    [SerializeField]
    private AudioSource audioRoll;
    [SerializeField]
    private AudioSource audioBrake;


    // helper function because vector2 doesn't have a rotate function
    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float delta = degrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        sphereCollider = GetComponent<SphereCollider>();
        lastJumpTime = -jumpCooldown;
        movementVector = new Vector2(0.0f, 0.0f);
        brakeCapacity = brakeLimit;
        brakeEmitter = effectsHandler.GetComponent<ParticleSystem>();
        lockedMovementTime = Time.fixedTime;
        startingPosition = transform.position;
    }
    
    // These don't work on WebGL
    /*void OnMove(InputValue value)
    {
        
        Vector2 v = value.Get<Vector2>();
        // normalize if magnitude is too high
        if (v.magnitude > 1)
        {
            v = v.normalized;
        }
        // rotate vector to reflect look direction
        movementVector = v;
    }

    void OnJump(InputValue value)
    {
        isJumping = (value.Get<float>() > 0);
    }

    void OnBrake(InputValue value)
    {
        isBraking = (value.Get<float>() > 0);
    }*/

    void InputCheck()
    {
        // calculate movement vector
        Vector2 v = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (v.magnitude > 1)
        {
            v = v.normalized;
        }
        movementVector = v;
        // jump check
        isJumping = Input.GetKey(KeyCode.Space) ? true : false;
        // brake check
        isBraking = Input.GetKey(KeyCode.LeftShift) ? true : false;
    }

    public bool isGrounded()
    {
        if (Physics.SphereCast(transform.position, groundCastRadius, Vector3.down, out RaycastHit hitInfo, groundCastDistance))
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

    public RaycastHit RaycastGrounded()
    {
        Physics.SphereCast(transform.position, groundCastRadius, Vector3.down, out RaycastHit hitInfo, groundCastDistance);
        return hitInfo;
    }

    public bool isPlayerBraking()
    {
        return isBraking;
    }

    public bool canJump()
    {
        return Time.fixedTime - jumpCooldown >= lastJumpTime;
    }

    public float getTimeUntilNextJump()
    {
        return Mathf.Clamp(lastJumpTime + jumpCooldown - Time.fixedTime, 0.0f, jumpCooldown);
    }

    private Vector3 getBrakeVector()
    {
        return new Vector3(-rb.velocity.x, 0.0f, -rb.velocity.z);
    }

    public float getBrakeCapacity()
    {
        return brakeCapacity;
    }

    // bumper handler
    public void Bump(BumpData bumpdata)
    {
        // neutralize some of the player's current momentum? 
        rb.velocity *= 1.0f - bumpdata.speedNegation;
        // calculate direction vector from data
        Vector3 bumpDirection = Vector3.Normalize(transform.position - bumpdata.origin);
        // apply force in that direction
        rb.AddForce(bumpDirection * bumpdata.strength, ForceMode.Impulse);
        
    }

    public void Launch(LaunchData launchdata)
    {
        // lock player movement
        lockedMovementTime = Time.fixedTime + launchdata.restrictMovementTime;

        // apply speed negation
        rb.velocity *= 1.0f - launchdata.speedNegation;
        rb.angularVelocity *= 1.0f - launchdata.speedNegation;
        
        // apply force to launch there
        rb.AddForce(launchdata.LaunchVelocity(transform.position) * rb.mass, ForceMode.Impulse);

        // stop sticking to current platform?
        if (!launchdata.sticky)
        {
            movingPlatform = null;
            movingPlatformRef = null;
            extraMovement = Vector3.zero;
        } 
        /*else if (movingPlatform)
        {
            // try to adjust movement for moving object?
            extraMovement = movingPlatform.transform.position - movingPlatformRef.GetLastPosition();
            rb.AddForce(extraMovement / Time.fixedDeltaTime, ForceMode.Impulse);
        }*/
    }


    // note: changing inputs to go through the old Input manager because new one has an issue in WebGL
    // Update is called once per frame
    void FixedUpdate() 
    {
        InputCheck(); // workaround because new InputSystem is broken in WebGL

        // rotate inputted move vector here
        float cameraDirection = playerInput.camera.transform.eulerAngles.y;
        Vector2 rotatedVector = Rotate(movementVector, -cameraDirection);

        // store grounded state
        bool grounded = isGrounded();

        // move with sticky triggers
        if (movingPlatform)
        {
            // move with sticky trigger
            extraMovement = movingPlatform.transform.position - movingPlatformRef.GetLastPosition();
            transform.position += extraMovement;
            //Debug.Log(extraMovement);
        }

        if (Time.fixedTime >= lockedMovementTime)
        {
            // apply player momentum
            Vector3 movement = new Vector3(rotatedVector.x, 0.0f, rotatedVector.y);
            rb.AddForce(movement * rollSpeed);


            // only do if grounded
            if (grounded)
            {
                // if braking, try to apply brake
                if (isBraking)
                {
                    // compare magnitude with brake limit
                    if (brakeCapacity > 0)
                    {
                        Vector3 brakeVector = getBrakeVector();
                        // set brake volume to brakevector magnitude
                        audioBrake.volume = Mathf.Clamp(brakeVector.magnitude / (rollSpeed * 1.5f), 0.0f, 1.0f) * 0.7f;
                        
                        // add movement vector to brake vector to calculate additional cost
                        brakeCapacity = Mathf.Clamp(brakeCapacity - ((brakeVector.magnitude + movement.magnitude) * brakeCostFactor * Time.fixedDeltaTime), 0.0f, brakeLimit);
                        // apply force equal to the player's velocity in the opposite direction, ignoring vertical components
                        rb.AddForce(brakeVector * brakeStrength, ForceMode.Force);
                        // reduce angular velocity
                        rb.angularVelocity *= 0.75f;
                        // control particle emitter, emit based on magnitude
                        brakeEmitter.Emit((int)Mathf.Clamp(Mathf.Log(brakeVector.magnitude, brakeEmitterFactor), 1.0f, 100.0f));
                    } else
                    {
                        // can't brake, mute audio
                        audioBrake.volume = 0.0f;
                    }
                } 
                else // if not braking, recharge brakes
                { 
                    brakeCapacity = Mathf.Clamp(brakeCapacity + brakeRechargePerSecond * Time.fixedDeltaTime, 0.0f, brakeLimit);
                    // mute brake audio
                    audioBrake.volume = 0.0f;
                }
                // apply jump
                if (isJumping && canJump())
                {
                    // apply jump momentum
                    rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                    lastJumpTime = Time.fixedTime;
                    // play jump audio
                    audioJump.Play();
                }

                // play sound with volume based on velocity
                if (rb.velocity.magnitude > 0.2f)
                {
                    audioRoll.volume = Mathf.Clamp(rb.velocity.magnitude / (rollSpeed * 1.5f), 0.0f, 1.0f) * 0.6f;
                } 
                else
                {
                    audioRoll.volume = 0.0f;
                }
            } 
            else
            {
                // mute rolling sound
                audioRoll.volume = 0.0f;
            }
            
        }

        // check if player is out of bounds
        if (transform.position.y < -10.0f)
        {
            // reset player position and velocity
            transform.position = startingPosition;
            rb.velocity = Vector3.zero;
        }

        // reset scale?
        //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // print velocity?
        //Debug.Log("rb.velocity = " + rb.velocity);
        //Debug.Log("rb.angularVelocity = " + rb.angularVelocity);

        // print debug info
        //Debug.Log(getTimeUntilNextJump());
    }

    void LateUpdate()
    {
        // move effects handler to current position
        effectsHandler.transform.position = transform.position;
    }

    /*void OnTriggerEnter(Collider other)
    {   
        if (other.tag == "Sticky")
        {
            // handle sticking to moving object
            movingPlatform = other.transform.parent.gameObject;
            stickyCollider = other;
            //Debug.Log("Entered sticky region: " + other);
            movingPlatformRef = movingPlatform.transform.parent.gameObject.GetComponent<MovingPlatform>();
            // apply momentum change based on platform's current velocity
            extraMovement = movingPlatform.transform.position - movingPlatformRef.GetLastPosition();
            rb.AddForce((movingPlatformRef.GetLastPosition() - movingPlatform.transform.position) / Time.fixedDeltaTime, ForceMode.Impulse);
            
        }
        
    }*/

    void OnTriggerEnter(Collider other)
    {
        // check for win
        if (other.tag == "Win")
        {
            // zero the velocity and stop the rolling sounds
            rb.velocity = Vector3.zero;
            audioRoll.Stop();
            // call win function in menu
            GameObject.Find("PlayerUICanvas").GetComponent<MenuManager>().OnWin();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Sticky" && other.transform.parent.gameObject == movingPlatform)
        {
            // apply momentum change based on platform's movement
            extraMovement = movingPlatform.transform.position - movingPlatformRef.GetLastPosition();
            rb.AddForce(extraMovement / Time.fixedDeltaTime, ForceMode.Impulse);
            extraMovement = Vector3.zero;
            // exit platform
            movingPlatform = null;
            movingPlatformRef = null;
            //Debug.Log("Exited sticky region: " + other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Sticky")
        {
            bool applyForce = other.transform.parent.gameObject != movingPlatform;
            
            // handle sticking with object
            movingPlatform = other.transform.parent.gameObject;
            movingPlatformRef = movingPlatform.transform.parent.gameObject.GetComponent<MovingPlatform>();
            // check if we're on the same platform
            if (applyForce)
            {
                // handle entering trigger
                // apply momentum change based on platform's current velocity
                extraMovement = movingPlatform.transform.position - movingPlatformRef.GetLastPosition();
                rb.AddForce((movingPlatformRef.GetLastPosition() - movingPlatform.transform.position) / Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
    }

    // debug
    void OnDrawGizmos()
    {
        bool grounded = isGrounded();
        bool jump = canJump();
        if (grounded && jump)
        {
            Gizmos.color = Color.green;
        } 
        else if (grounded)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCastDistance, groundCastRadius);
    }
}
