using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float Speed = 15;
    public float BlastRadius = 5;
    public float BlastStrength = 500;
    public float UpwardsAdjustment;
    public ParticleSystem ExplosionParticles;
    public ParticleSystem TrailParticles;
    private float _explosionDuration = -1;
    public AudioClip ExplosionSound;
    public GameObject DisableOnHit;
    private int _layerMask;

    public Player Player;
    //public float StunDuration = 3;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
        _layerMask = LayerMask.GetMask("EnemyCollision", "Terrain", "TransparentTerrain");
    }

    private void FixedUpdate()
    {
        if (_explosionDuration != -1)
        {
            if (_explosionDuration > 0)
            {
                _explosionDuration -= Time.fixedDeltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            if (collision.transform.GetComponent<Enemy>().Stunned)
            {
                Player.IncrementHammo(6);
            }
            else
            {
                Player.IncrementHammo(2);
            }

            collision.transform.GetComponent<Enemy>().Kill();
        }

        Player.transform.GetComponent<Rigidbody>().AddExplosionForce(BlastStrength, transform.position, BlastRadius, 0f,
            ForceMode.VelocityChange);

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            RaycastHit hit;
            if (Vector3.Distance(transform.position, i.transform.position + i.GetComponent<Rigidbody>().centerOfMass) < BlastRadius
                && Physics.Raycast(transform.position, (i.transform.position - transform.position).normalized, out hit, BlastRadius, _layerMask)
                && hit.transform.parent == i.transform)
            {
                i.GetComponent<Enemy>().Stun();
                i.GetComponent<Rigidbody>().AddExplosionForce(BlastStrength, transform.position, BlastRadius,
                    UpwardsAdjustment, ForceMode.VelocityChange);
            }
        }

        DisableOnHit.SetActive(false);
        TrailParticles.Stop();
        _explosionDuration = 3;
        ExplosionParticles.Play();
        GetComponent<AudioSource>().PlayOneShot(ExplosionSound);
    }
}