using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Checkpoint PreviousCheckpoint;
    public List<Enemy> RequiredEnemies;
    private bool _untriggered = true;
    private bool _locked;
    private bool _previousCheckpointActive;

    public ParticleSystem LockParticles;
    //public ParticleSystem UnlockParticles;
    public ParticleSystem UnactivatedParticles;
    public ParticleSystem ActivationParticles;

    public AudioClip ActivationSound;

    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        // set status
        _untriggered = true;
        _locked = false;
        GetComponent<Collider>().enabled = false;

        _previousCheckpointActive = PreviousCheckpoint == null;

        // reset all particle effects
        LockParticles.Play();
        //UnlockParticles.Stop();
        //UnlockParticles.Clear();
        UnactivatedParticles.Stop();
        UnactivatedParticles.Clear();
        //ActivationParticles.Stop();
        //ActivationParticles.Clear();
    }

    public void DestroyEnemies()
    {
        foreach (Enemy i in RequiredEnemies)
        {
            i.gameObject.SetActive(false);
        }

        if (PreviousCheckpoint != null)
        {
            PreviousCheckpoint.DestroyEnemies();
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (_locked && _untriggered && _previousCheckpointActive && other.CompareTag("Player"))
        {
            PlayerLink.playerLink.SetCheckpoint(GetComponent<Checkpoint>());
            _untriggered = false;
            UnactivatedParticles.Stop();

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[UnactivatedParticles.main.maxParticles];
            int liveParticles = UnactivatedParticles.GetParticles(particles);
            for(int i = 0; i < liveParticles; i++)
            {
                ParticleSystem.EmitParams e = new ParticleSystem.EmitParams();
                e.position = particles[i].position;
                ActivationParticles.Emit(e, 1);
                //Debug.Log("emitted particle");
            }
            
            UnactivatedParticles.Clear();
            //ActivationParticles.Play();
            
            GetComponent<AudioSource>().PlayOneShot(ActivationSound);
        }
    }

    private void FixedUpdate()
    {
        if (!_locked && _previousCheckpointActive)
        {
            bool l = true;
            foreach (Enemy i in RequiredEnemies)
            {
                if (i.GetComponent<Enemy>().Alive)
                {
                    l = false;
                }
            }

            if (l)
            {
                _locked = true;
                LockParticles.Stop();
                LockParticles.Clear();
                //UnlockParticles.Play();
                UnactivatedParticles.Play();
                GetComponent<Collider>().enabled = true;
            }
        }
        else if (!_previousCheckpointActive && PreviousCheckpoint._locked)
        {
            _previousCheckpointActive = true;
        }
    }
}
