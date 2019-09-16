using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float Speed = 15;
    public float ColliderRadius = 0.1f;
    public float BlastRadius = 5;
    public float BlastStrength = 50;
    public float UpwardsAdjustment;
    public float ExpDuration = 3;
    public ParticleSystem TrailParticles;
    private float _expCount = -1;
    public GameObject EnableOnHit;
    public GameObject DisableOnHit;
    private int _layerMask;
    private int _layerMaskTerrain;

    public Player Player;
    //public float StunDuration = 3;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
        _layerMask = LayerMask.GetMask("EnemyHitbox", "Terrain");
        _layerMaskTerrain = LayerMask.GetMask("Terrain");
    }

    private void FixedUpdate()
    {
        if (_expCount != -1)
        {
            if (_expCount > 0)
            {
                _expCount -= Time.fixedDeltaTime;
                
                if (_expCount < ExpDuration - 0.1f)
                {
                    GetComponentInChildren<ParticleSystemForceField>().gravity = 0;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Vector3 pos = transform.position - transform.forward;
            Vector3 dir = transform.forward;
            RaycastHit hit;
            float dist = Speed * Time.fixedDeltaTime;
            
            if (Physics.SphereCast(pos, ColliderRadius, dir, out hit, dist, _layerMask))
            {
                // move the transform to the appropriate collision position
                transform.position = hit.point + transform.forward * -ColliderRadius;
                Collide(hit);
            }
        }
    }

    private void Collide (RaycastHit collision)
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
            if (!i.GetComponent<Enemy>().Alive)
            {
                continue;
            }
            Vector3 ipos = i.GetComponent<Rigidbody>().centerOfMass + i.transform.position;
            if (Vector3.Distance(transform.position, ipos) < BlastRadius 
                && !Physics.Linecast(transform.position, ipos, _layerMaskTerrain))
            {
                i.GetComponent<Enemy>().Stun();
                i.GetComponent<Rigidbody>().AddExplosionForce(BlastStrength, transform.position, BlastRadius,
                    UpwardsAdjustment, ForceMode.VelocityChange);
            }
        }

        DisableOnHit.SetActive(false);
        TrailParticles.Stop();
        _expCount = ExpDuration;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        EnableOnHit.SetActive(true);
    }
}