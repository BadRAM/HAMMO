using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    public bool Alive = true;

    //public float StunTimer;
    public bool Stunned;
    private Vector3 _startPos;
    public Collider StandingHitBox;
    public Collider StunnedHitBox;
    public bool Alerted;
    public Transform Target;
    public float StunDuration = 0.5f;
    public float AlertRange = 100;
    public float AttackRange = 10;
    public ParticleSystem ChargeParticles;
    public ParticleSystem StunParticles;
    public ParticleSystem DeathParticle;
    private int _layerMask;
    private float _attackTimer = 3;
    private bool _attacking;
    private float _stunTimer;
    public float ChargeDuration;
    public AudioSource ChargeSound;
    public AudioSource FireSound;
    public AudioSource DeathSound;
    public GameObject DisableOnDeath;
    public GameObject Beam;
    public Transform CenterOfMass;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition;
        ChargeParticles.Stop();
        _layerMask = LayerMask.GetMask("Terrain");
        _startPos = transform.position;
        Target = GameObject.Find("Player").transform;
        Recover();
    }

    public void Reset()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = false;
        
        DisableOnDeath.SetActive(true);
        
        ChargeParticles.Stop();
        ChargeParticles.Clear();
        StunParticles.Stop();
        StunParticles.Clear();
        DeathParticle.Stop();
        DeathParticle.Clear();
        
        Alive = true;
        Alerted = false;
        
        Stun();
        
        if (_startPos == Vector3.zero)
        {
            _startPos = transform.position;
        }

        transform.position = _startPos;
        
        Recover();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void behavior()
    {
        if (transform.position.y < -50)
        {
            PlayerLink.playerLink.IncrementHammo(1);
            Kill();
        }

        if (Stunned && _stunTimer == 0 && Vector3.Dot(GetComponent<Rigidbody>().velocity, transform.up) <= 0
            && Physics.Raycast(transform.position + transform.up, -transform.up, 1.01f, _layerMask))
        {
            Recover();
        }

        if (!Alerted && Vector3.Distance(Target.position, transform.position) < AlertRange
                     && !Physics.Linecast(transform.position + Vector3.up, Target.transform.position + Vector3.up, _layerMask))
        {
            Alerted = true;
        }

        if (_attacking && _attackTimer <= 0)
        {
            _attacking = false;
            FireSound.Play();
            GameObject beam = Instantiate(Beam);
            beam.GetComponent<Beam>().startPoint = transform.position + Vector3.up;


            RaycastHit hit;
            if (!Physics.Linecast(transform.position + Vector3.up, Target.transform.position + Vector3.up, out hit, _layerMask))
            {
                beam.GetComponent<Beam>().endPoint = Target.transform.position;
                PlayerLink.playerLink.IncrementHammo(-1);
            }
            else
            {
                beam.GetComponent<Beam>().endPoint = hit.point;
            }
        }
        else if (!Stunned && Alerted && _attackTimer == 0)
        {
            if (Vector3.Distance(Target.position, transform.position) < AttackRange)
            {
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
                _attackTimer = ChargeDuration;
                _attacking = true;
                ChargeSound.Play();
                ChargeParticles.Play();
            }
            else
            {
                GetComponent<NavMeshAgent>().SetDestination(Target.transform.position);
            }
        }
    }

    void FixedUpdate()
    {
        if (Target == null)
        {
            Target = PlayerLink.playerLink.transform;
        }

        if (Alive)
        {
            behavior();
        }

        _attackTimer = Mathf.Max(0, _attackTimer - Time.deltaTime);
        _stunTimer = Mathf.Max(0, _stunTimer - Time.deltaTime);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (Stunned && collision.transform.CompareTag("Terrain"))
        {
            Stunned = false;
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = true;
            StandingHitBox.enabled = true;
            StunnedHitBox.enabled = false;
        }
    }
    */

    public void Kill()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = false;
        
        ChargeSound.Stop();
        DeathSound.Play();
        DisableOnDeath.SetActive(false);
        Alive = false;
        
        ChargeParticles.Stop();
        ChargeParticles.Clear();
        StunParticles.Stop();
        StunParticles.Clear();
        
        DeathParticle.Play();
    }

    public void Stun()
    {
        Stunned = true;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        StandingHitBox.enabled = false;
        StunnedHitBox.enabled = true;
        ChargeSound.Stop();
        ChargeParticles.Stop();
        StunParticles.Play();
        _attacking = false;
        _attackTimer = 0;
        _stunTimer = StunDuration;
    }

    void Recover()
    {
        Stunned = false;
        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
        StandingHitBox.enabled = true;
        StunnedHitBox.enabled = false;
        StunParticles.Stop();
    }
}