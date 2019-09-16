using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSWalkMK3 : MonoBehaviour
{

    // this is a First Person movement controller that uses a box collider and a boxcast to interact with map geometry.
    
    //TODO:
    //fix falling into the floor near tall steps - fixed: ensure that tall steps are not a part of the same collider
    

    [Header("Size")]
    public float Height = 2;
    public float Width = 1;
    public float StepUpDistance = 0.5f;
    public float Weight;
    private float _cameraHeight;


    [Header("Movement Characteristics")]
    public float LookSensitivity = 1f;
    public float CameraStepUpSpeed = 5f;
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
    public ParticleSystem SmokeTrail;
    public AudioClip JumpSound;
    public AudioClip BounceSound;
    public AudioSource BounceChargeSound;
    public AudioSource BounceDeChargeSound;
    public ParticleSystem BounceParticles;
    public RectTransform BounceMeter;
    public TextMeshProUGUI BounceReady;
    public TextMeshProUGUI GroundedIndicator;
    public TextMeshProUGUI SlidingIndicator;
    public RectTransform AirStrafeIndicator;
    public Gradient AirStrafeIndicatorColor;

    // --- Private Variables ---

    // Transforms
    private Transform _playerCamera;
    private Transform _rotator;
    private float _spawnFacing;

    // Input handling
    private float _cameraX;
    private float _cameraY;
    private bool _jumpLock;
    private bool _airJumpLock;

    // Status
    private int _jumpCounter; //useless, remove
    [SerializeField] private float _bounceCharge;
    private bool _bounceReady;
    private Vector3 _normal;
    private bool _sliding;
    private bool _skidding;
    private float _airStrafeIndicatorForce;

    // Boxcast parameters
    private Vector3 _boxCastHalfExtents;
    private int _layerMask;

    private void Jump()
    {
        Vector3 v = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, _rotator.up);
        GetComponent<Rigidbody>().velocity = v;
        
        GetComponent<Rigidbody>().AddForce(_rotator.up * JumpForce, ForceMode.VelocityChange);

        _grounded = false;
        _jumpCounter = 0;
        _jumpLock = true;
        GetComponent<AudioSource>().PlayOneShot(JumpSound);
    }

    private void Bounce(Vector3 normal)
    {
        // calculate vertical velocity
        float v = Vector3.Project(GetComponent<Rigidbody>().velocity, _rotator.up).magnitude;
        
        // cancel vertical velocity
        Rigidbody r = GetComponent<Rigidbody>();
        r.velocity = Vector3.ProjectOnPlane(r.velocity, normal);
        
        // calculate and apply bounce force
        float f = Mathf.Max(JumpForce, v * BounceEffectiveness) * (_bounceCharge / BounceChargeTime);
        GetComponent<Rigidbody>().AddForce(normal * f, ForceMode.VelocityChange);
        
        // manage state
        _grounded = false;
        //Debug.Log("Bounced after " + -_jumpCounter * Time.fixedDeltaTime + " seconds."); 
        _jumpCounter = 0;

        if (Input.GetButton("Jump"))
        {
            _jumpLock = true;
        }
        GetComponent<Player>().IncrementHammo(-1);

        // activate effects
        GetComponent<AudioSource>().PlayOneShot(BounceSound);
        if (!BounceDeChargeSound.isPlaying)
        {
            BounceDeChargeSound.time = 0;
            BounceDeChargeSound.Play();
        }
        BounceChargeSound.Stop();
        ParticleSystem.MainModule psmain = BounceParticles.main;
        psmain.startSpeed = v - 5;
        BounceParticles.Play();
        
    }

    private void TerrainCollide(Vector3 normal)
    {
        if (Vector3.Dot(GetComponent<Rigidbody>().velocity, normal) <= 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, normal);
        }
    }
    
    private void GroundMovement(Vector3 moveVector)
    {
        if (_jumpCounter < 0)
        {
            //Debug.Log("Landed after " + _jumpCounter + " ticks.");
            _jumpCounter = 0;
        }

        // set skidding to true if we're moving faster than walkspeed
        _skidding = GetComponent<Rigidbody>().velocity.magnitude > WalkSpeed;

        //project the direction to move in onto the surface the player is standing on
        Vector3 targetVelocity = moveVector;
        targetVelocity = Vector3.ProjectOnPlane(targetVelocity, _normal).normalized; //risky
        targetVelocity *= WalkSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 vc = Vector3.ClampMagnitude(targetVelocity - GetComponent<Rigidbody>().velocity, Acceleration);
        GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);
    }

    
    void AirMovement(Vector3 vector3)
    {
        //increment the jump cooldown
        _jumpCounter -= 1;

        // air control
        // project the velocity onto the movevector
        Vector3 projVel = Vector3.Project(GetComponent<Rigidbody>().velocity, vector3);

        // check if the movevector is moving towards or away from the projected velocity
        bool isAway = Vector3.Dot(vector3, projVel) <= 0f;

        // only apply force if moving away from velocity or velocity is below MaxAirSpeed
        if (projVel.magnitude < MaxAirSpeed || isAway)
        {
            // calculate the ideal movement force
            Vector3 vc = vector3.normalized * AirStrafeForce;

            // cap it if it would accelerate beyond MaxAirSpeed directly.
            if (!isAway)
            {
                vc = Vector3.ClampMagnitude(vc, MaxAirSpeed - projVel.magnitude);
            }
            else
            {
                vc = Vector3.ClampMagnitude(vc, MaxAirSpeed + projVel.magnitude);

                // check if the player is strafing into a slope
                if (_sliding && Vector3.Dot(vc, Vector3.ProjectOnPlane(_normal, Vector3.up)) < 0)
                {
                    // prevent them from sliding up the slope
                    vc = Vector3.ClampMagnitude(vc, projVel.magnitude);
                }
            }

            // Apply the force
            GetComponent<Rigidbody>().AddForce(vc, ForceMode.VelocityChange);

            if (_sliding)
            {
                TerrainCollide(_normal);
            }

            // set airstrafe indicator size and color
            _airStrafeIndicatorForce = vc.magnitude / AirStrafeForce;
        }
    }

    public void Restart(bool level, Quaternion checkpointForward)
    {
        _bounceCharge = 0;
        _bounceReady = false;
        
        BounceParticles.Clear();
        BounceChargeSound.Stop();
        BounceDeChargeSound.Stop();

        BounceMeter.localScale = new Vector2(_bounceCharge / BounceChargeTime, 1);
        BounceReady.enabled = _bounceReady;

        _cameraY = 0;
        if (level)
        {
            _cameraX = _spawnFacing;
        }
        else
        {
            _cameraX = checkpointForward.eulerAngles.y;
        }
    }
    
    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.up * Height / 2f;

        _layerMask = LayerMask.GetMask("EnemyCollision", "Terrain", "TransparentTerrain");

        // set the rotator and camera
        _rotator = transform.GetChild(0);
        _playerCamera = _rotator.GetChild(0);
        _cameraHeight = _playerCamera.localPosition.y;
        
        // set the facing
        _spawnFacing = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _cameraX = _spawnFacing;
        
        // set the camera sensitivity
        LookSensitivity = SettingsManager.CurrentSettings.Sensitivity;

        // set the boxcast parameters
        _boxCastHalfExtents = new Vector3(Width / 2, Width / 2, Width / 2);
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        _normal = Vector3.up;

        //check for a collision with ground, and stop the player if one is detected
        Vector3 center = transform.position + transform.up * (Height - (Width / 2));
        Vector3 direction = -transform.up;
        RaycastHit hit;
        float distance = Height - Width;
        if (_grounded)
        {
            distance += StepUpDistance;
        }

        if (Physics.BoxCast(center, _boxCastHalfExtents, direction, out hit, transform.rotation, distance, _layerMask)
            && !hit.collider.isTrigger && Vector3.Dot(GetComponent<Rigidbody>().velocity, hit.normal) < 0.1)
        {
            var pos = _playerCamera.position;
            pos = new Vector3(pos.x, pos.y - (hit.point.y - _rotator.position.y), pos.z);
            _playerCamera.position = pos;

            // move to collision
            transform.position = new Vector3(transform.position.x, hit.point.y , transform.position.z);
            
            // decide if sliding
            _sliding = Vector3.Angle(transform.up, hit.normal) > MaxSlope;
            if (hit.transform.CompareTag("FrictionlessTerrain"))
            {
                _sliding = true;
            }
            
            // bhop if classic mode
            if (GameMode.Get() == 4 && !_jumpLock && !_sliding && Input.GetButton("Jump"))
            {
                Jump();
            }
            // bounce if charged
            else if (_bounceReady && !_sliding)
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<Enemy>().Kill();
                    GetComponent<Player>().IncrementHammo(2);
                    Bounce(_rotator.up);
                }
                else
                {
                    Bounce(hit.normal);
                }
            }
            else
            {
                // stop the player from moving into the object collided with
                //GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, hit.normal);
                TerrainCollide(hit.normal);
                
                // set status
                _normal = hit.normal;
                _grounded = true;
            }

        }
        else
        {
            _sliding = false;
            _grounded = false;
        }

        // Charge Bounce
        if ((!_grounded || _sliding) && !_jumpLock && GameMode.Get() != 4)
        {
            if (Input.GetButton("Jump"))
            {
                if (!BounceChargeSound.isPlaying)
                {
                    BounceChargeSound.Play();
                }
                
                _airJumpLock = true;
                
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
        else if (_bounceCharge > 0)
        {
            BounceChargeSound.Stop();
            BounceDeChargeSound.time = -(_bounceCharge / BounceChargeTime) + 1;
            BounceDeChargeSound.Play();
            _bounceCharge = 0f;
            _bounceReady = false;
        }

        BounceMeter.localScale = new Vector2(_bounceCharge / BounceChargeTime, 1);
        BounceReady.enabled = _bounceReady;

        // turn the smoke trail on or off if the player is in the air or not
        if (!_grounded && !SmokeTrail.isPlaying)
        {
            SmokeTrail.Play();
        }
        if (_grounded && SmokeTrail.isPlaying)
        {
            SmokeTrail.Pause();
        }

        // accelerate the player according to input

        // Decide which direction we should be moving based on current player rotation and input
        Vector3 moveVector = new Vector3();
        moveVector = moveVector + _rotator.forward * Input.GetAxis("Vertical");
        moveVector = moveVector + _rotator.right * Input.GetAxis("Horizontal");
        

        //Debug.Log("grounded: " + _grounded + ", sliding: " + _sliding + ", normal: " + _normal);

        if (_grounded && !_sliding && GameMode.Get() != 5)
        {
            GroundMovement(moveVector);
        }
        else
        {
            AirMovement(moveVector);
            
            // apply gravity
            GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        GroundedIndicator.enabled = _grounded;
        SlidingIndicator.enabled = _sliding;
        
        if(!LockLook)
        {
            // return the camera to it's resting height after stepping up
            //_playerCamera.position = new Vector3(_playerCamera.position.x, Mathf.Min(_rotator.position.y + _cameraHeight, _playerCamera.position.y + Time.deltaTime * CameraStepUpSpeed), _playerCamera.position.z);
            _playerCamera.localPosition = Vector3.MoveTowards(_playerCamera.localPosition, Vector3.up * _cameraHeight, CameraStepUpSpeed * Time.deltaTime);
            
            // jump when appropriate
            if (_grounded && !_sliding && !_skidding && !_jumpLock && !_airJumpLock && Input.GetButton("Jump"))
            {
                Jump();
            }
            
            // reset jumplock
            if (Input.GetButtonUp("Jump"))
            {
                _jumpLock = false;
                _airJumpLock = false;

                if (_bounceCharge > 0)
                {
                    BounceDeChargeSound.time = -(_bounceCharge / BounceChargeTime) + 1;
                    BounceChargeSound.Stop();
                    BounceDeChargeSound.Play();
                }
            }
            
            // update airstrafe indicator
            if (_grounded && !_sliding)
            {
                AirStrafeIndicator.localScale = new Vector2(0, 1);
            }
            else
            {
                AirStrafeIndicator.localScale = new Vector2(Mathf.Lerp(AirStrafeIndicator.localScale.x, _airStrafeIndicatorForce, Time.deltaTime * 10), 1);
                AirStrafeIndicator.GetComponent<RawImage>().color = AirStrafeIndicatorColor.Evaluate(_airStrafeIndicatorForce);
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
