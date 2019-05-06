using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSWalkMK3 : MonoBehaviour
{

    // this is a First Person movement controller that uses a box collider and a boxcast to interact with map geometry.


    [Header("Size")]
    public float Height = 2;
    public float Width = 1;
    public float StepUpDistance = 0.5f;
    public float Weight;


    [Header("Movement Characteristics")]
    public float LookSensitivity = 1f;
    public float WalkSpeed = 5f;
    public float MaxAirSpeed = 1f; // the maximum speed that the player can reach in the air by conventional means. should be ~10% of walk speed.
    public float Acceleration = 1.5f;
    public float AirStrafeForce = 0.1f;
    public float MaxSlope = 45;
    public float JumpForce = 5;
    [SerializeField] private bool _grounded;
    public bool LockLook;

    [Header("Sounds")]
    public AudioClip JumpSound;

    // --- Private Variables ---

    // Transforms
    private Transform _playerCamera;
    private Transform _rotator;

    // Input handling
    private float _cameraX;
    private float _cameraY;
    private bool _jumpHeld;

    // Status
    private int _jumpcooldown;
    private Vector3 _normal;

    // Boxcast parameters
    private Vector3 _boxCastHalfExtents;
    private int _layerMask;


    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.up * Height / 2f;

        _layerMask = LayerMask.GetMask("Enemies", "Terrain");

        // set the rotator and camera
        _rotator = transform.GetChild(0);
        _playerCamera = _rotator.GetChild(0);

        // set the boxcast parameters
        _boxCastHalfExtents = new Vector3(Width / 2, Width / 2, Width / 2);
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        //check for a collision with ground, and stop the player if one is detected

        _grounded = false;
        RaycastHit hit;
        _normal = Vector3.up;
        if (_jumpcooldown == 0 && 
            Physics.BoxCast(transform.position + transform.up * (Height - (Width / 2)), _boxCastHalfExtents, -transform.up, out hit, transform.rotation, Height - Width, _layerMask)
            && !hit.collider.isTrigger && Vector3.Dot(GetComponent<Rigidbody>().velocity, hit.normal) < 0)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y , transform.position.z);
            GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, hit.normal);
            //if(Vector3.Angle(transform.up, hit.normal) < MaxSlope)
            _normal = hit.normal;
            _grounded = true;
        }

        //jump
        if (_grounded && Input.GetButton("Jump") && _jumpcooldown == 0 && !_jumpHeld)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.up * JumpForce;
            _grounded = false;
            _jumpcooldown = 5;
            _jumpHeld = true;
            GetComponent<AudioSource>().PlayOneShot(JumpSound);
        }

        // accelerate the player according to input

        // Decide which direction we should be moving based on current player rotation and input
        Vector3 moveVector = new Vector3();
        moveVector = moveVector + _rotator.forward * Input.GetAxis("Vertical");
        moveVector = moveVector + _rotator.right * Input.GetAxis("Horizontal");
        bool sliding = false;

        if (Vector3.Angle(transform.up, _normal) > MaxSlope)
        {
            sliding = true;
        }


        if (_grounded && !sliding)
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
            GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, transform.up).normalized;
            Vector3 vc = moveVector.normalized * AirStrafeForce;

            //give the player a little boost if they're stationary
            GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(vc, horizontalVelocity.magnitude - MaxAirSpeed), ForceMode.VelocityChange);

            // apply air strafe force only if the force is not being applied forwards
            if (Vector3.Dot(horizontalVelocity, moveVector) < 0)
            {
                GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);
            }
        }

        //increment the jump cooldown
        _jumpcooldown = Mathf.Max(0, _jumpcooldown - 1);

        
    }

    // Update is called once per frame
    void Update()
    {
        if(!LockLook)
        {
            //jump
            if (_grounded && Input.GetButtonDown("Jump") && _jumpcooldown == 0 && !_jumpHeld)
            {
                GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.up * JumpForce;
                _grounded = false;
                _jumpcooldown = 5;
                _jumpHeld = true;
                GetComponent<AudioSource>().PlayOneShot(JumpSound);
            }
            if (Input.GetButtonUp("Jump"))
            {
                _jumpHeld = false;
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
