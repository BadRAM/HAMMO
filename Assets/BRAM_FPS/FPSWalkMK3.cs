using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSWalkMK3 : MonoBehaviour
{

    // this is a First Person movement controller that uses a box collider and a boxcast to interact with map geometry.
    
    //TODO:
    //smooth out vertical camera movement on step up
    //fix the little boost for stationary players in air.
    //add a small boost for air strafing
    //prevent jumping when skidding
    //fix bounce not resetting
    //make surfing retain velocity better
    //fix falling into the floor near tall steps
    

    [Header("Size")]
    public float Height = 2;
    public float Width = 1;
    public float StepUpDistance = 0.5f;
    public float Weight;
    private float _cameraHeight;


    [Header("Movement Characteristics")]
    public float LookSensitivity = 1f;
    public float CameraStepUpSpeed = 0.25f;
    public float WalkSpeed = 5f;
    public float MaxAirSpeed = 1f; // the maximum speed that the player can reach in the air by conventional means. should be ~10% of walk speed.
    public float Acceleration = 1.5f;
    public float AirStrafeForce = 0.1f;
    public float MaxSlope = 45;
    public float JumpForce = 5;
    [SerializeField] private bool _grounded;
    public bool LockLook;
    public float BounceChargeTime = 0.5f;
    public float BounceEffectiveness = 0.5f;

    [Header("Effects")]
    public AudioClip JumpSound;
    public ParticleSystem BounceParticles;
    public RectTransform BounceMeter;
    public TextMeshProUGUI BounceReady;

    // --- Private Variables ---

    // Transforms
    private Transform _playerCamera;
    private Transform _rotator;

    // Input handling
    private float _cameraX;
    private float _cameraY;
    private bool _jumpLock;

    // Status
    private int _jumpCoolDown; //only necessary for preventing boxcast from stopping jump
    private float _bounceCharge;
    private bool _bounceReady;
    private Vector3 _normal;
    private bool _sliding;

    // Boxcast parameters
    private Vector3 _boxCastHalfExtents;
    private int _layerMask;

    private void Jump()
    {
        GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.up * JumpForce;
        _grounded = false;
        _jumpCoolDown = 3;
        _jumpLock = true;
        GetComponent<AudioSource>().PlayOneShot(JumpSound);
    }

    private void Bounce()
    {
        float v = Vector3.Project(GetComponent<Rigidbody>().velocity, _rotator.up).magnitude;
        GetComponent<Rigidbody>().AddForce(_rotator.up * (v * BounceEffectiveness * (_bounceCharge / BounceChargeTime) + v), ForceMode.VelocityChange);
        Debug.Log("bounced with strength: " + _bounceCharge / BounceChargeTime);
        _grounded = false;
        _jumpCoolDown = 3;
        _jumpLock = true;
        GetComponent<AudioSource>().PlayOneShot(JumpSound);
        BounceParticles.Play();
        GetComponent<Player>().IncrementHammo(-1);
    }

    public void Restart()
    {
        _bounceCharge = 0;
        _bounceReady = false;
    }
    
    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.up * Height / 2f;

        _layerMask = LayerMask.GetMask("Enemies", "Terrain");

        // set the rotator and camera
        _rotator = transform.GetChild(0);
        _playerCamera = _rotator.GetChild(0);
        _cameraHeight = _playerCamera.localPosition.y;

        // set the boxcast parameters
        _boxCastHalfExtents = new Vector3(Width / 2, Width / 2, Width / 2);
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
/*        // If the ground was flat last frame, check a little further down than usual.
        // BAD CODE, FIX THIS FOR REAL
        float isFlat = 0f;
        if (_normal == Vector3.up && _grounded)
        {
            Debug.Log("isflatting");
            isFlat = 0f;
        }*/
        
        //check for a collision with ground, and stop the player if one is detected
        RaycastHit hit;
        _normal = Vector3.up;
        
        if (_jumpCoolDown == 0 && 
            Physics.BoxCast(transform.position + transform.up * (Height - (Width / 2)), _boxCastHalfExtents, -transform.up, out hit, transform.rotation, Height - Width, _layerMask)
            && !hit.collider.isTrigger && Vector3.Dot(GetComponent<Rigidbody>().velocity, hit.normal) <= 0)
        {
            // move to collision
            transform.position = new Vector3(transform.position.x, hit.point.y , transform.position.z);
            
            // bounce if charged
            if (_bounceReady)
            {
                Bounce();
            }
            else
            {
                // stop the player from moving into the object collided with
                GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, hit.normal);
                
                // set status
                _normal = hit.normal;
                _sliding = Vector3.Angle(transform.up, _normal) > MaxSlope;
                _grounded = true;
            }

        }
        else
        {
            _sliding = false;
            _grounded = false;
        }

        // Charge Bounce
        if (!_grounded && !_sliding && !_jumpLock)
        {
            if (Input.GetButton("Jump"))
            {
                _bounceCharge += Time.deltaTime;
                
                if (_bounceCharge >= BounceChargeTime)
                {
                    _bounceCharge = BounceChargeTime;
                    _bounceReady = true;
                }
            }
            else
            {
                _bounceCharge = Mathf.Max(0f, _bounceCharge - Time.deltaTime);

                if (_bounceCharge <= 0)
                {
                    _bounceCharge = 0;
                    _bounceReady = false;
                }
            }
        }
        else
        {
            _bounceCharge = 0f;
            _bounceReady = false;
        }
        
        BounceMeter.localScale = new Vector2(_bounceCharge / BounceChargeTime, 1);
        BounceReady.enabled = _bounceReady;

        // accelerate the player according to input

        // Decide which direction we should be moving based on current player rotation and input
        Vector3 moveVector = new Vector3();
        moveVector = moveVector + _rotator.forward * Input.GetAxis("Vertical");
        moveVector = moveVector + _rotator.right * Input.GetAxis("Horizontal");
        

        //Debug.Log("grounded: " + _grounded + ", sliding: " + _sliding + ", normal: " + _normal);

        if (_grounded && !_sliding)
        {
            //project the direction to move in onto the surface the player is standing on
            Vector3 targetVelocity = moveVector;
            targetVelocity = Vector3.ProjectOnPlane(targetVelocity, _normal).normalized; //risky
            targetVelocity *= WalkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 vc = Vector3.ClampMagnitude(targetVelocity - GetComponent<Rigidbody>().velocity, Acceleration);
            GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);
        }
        else
        {
            // apply gravity
            GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);

            // boost control if surfing
            float boost = 1f;
            if (_sliding)
            { 
                boost = 2f;
            }

            // calculate air strafe force
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, transform.up).normalized;
            Vector3 vc = Vector3.ClampMagnitude(moveVector.normalized * AirStrafeForce * boost, Vector3.Project(GetComponent<Rigidbody>().velocity, moveVector).magnitude);
            
            //give the player a little boost if they're stationary
            GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(moveVector.normalized, horizontalVelocity.magnitude - MaxAirSpeed), ForceMode.VelocityChange);            
            
            // apply air strafe force only if the force is not being applied forwards
            if (Vector3.Dot(horizontalVelocity, moveVector) < 0)
            {
                GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);
            }
            else
            {
                //GetComponent<Rigidbody>().AddForce(vc * (1 - Vector3.Dot(horizontalVelocity, moveVector)), ForceMode.VelocityChange);
            }
            
        }

        //increment the jump cooldown
        _jumpCoolDown = Mathf.Max(0, _jumpCoolDown - 1);

        
    }

    // Update is called once per frame
    void Update()
    {
        if(!LockLook)
        {
            if (_grounded && !_sliding && !_jumpLock && Input.GetButtonDown("Jump"))
            {
                Jump();
            }
            
            if (Input.GetButtonUp("Jump"))
            {
                _jumpLock = false;
            }

            // rotate the camera
            _cameraX += Input.GetAxis("Mouse X") * LookSensitivity;
            if (_cameraX > 180)
            {
                _cameraX -= 360;
            }
            else if (_cameraX < -180)
            {
                _cameraX += 360;
            }
            _cameraY += -Input.GetAxis("Mouse Y") * LookSensitivity;
            _cameraY = Mathf.Clamp(_cameraY, -90, 90);
            _rotator.eulerAngles = new Vector2(_rotator.eulerAngles.x, _cameraX);
            _playerCamera.eulerAngles = new Vector2(_cameraY, _playerCamera.eulerAngles.y);
        }
    }
}
