using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    public Checkpoint PreviousCheckpoint;
    public List<Enemy> RequiredEnemies;
    private bool _untriggered = true;
    private bool _active;
    private bool _previousCheckpointActive;

    public ParticleSystem LockParticles;
    public ParticleSystem UnlockParticles;
    public ParticleSystem UnactivatedParticles;
    public ParticleSystem ActivationParticles;

    private void Start()
    {
        Restart();
    }

    public void Restart()
    {
        // set status
        _untriggered = true;
        _active = false;
        GetComponent<MeshCollider>().enabled = false;

        _previousCheckpointActive = PreviousCheckpoint == null;

        // reset all particle effects
        LockParticles.Play();
        UnlockParticles.Stop();
        UnlockParticles.Clear();
        UnactivatedParticles.Stop();
        UnactivatedParticles.Clear();
        ActivationParticles.Stop();
        ActivationParticles.Clear();
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
        if (_active && _untriggered && _previousCheckpointActive && other.CompareTag("Player"))
        {
            PlayerLink.playerLink.SetCheckpoint(GetComponent<Checkpoint>());
            _untriggered = false;
            UnactivatedParticles.Stop();
            ActivationParticles.Play();
        }
    }

    private void FixedUpdate()
    {
        if (!_active && _previousCheckpointActive)
        {
            bool l = true;
            foreach (Enemy i in RequiredEnemies)
            {
                if (i.gameObject.activeSelf)
                {
                    l = false;
                }
            }

            if (l)
            {
                _active = true;
                LockParticles.Stop();
                UnlockParticles.Play();
                UnactivatedParticles.Play();
                GetComponent<MeshCollider>().enabled = true;
            }
        }
        else if (!_previousCheckpointActive && PreviousCheckpoint._active)
        {
            _previousCheckpointActive = true;
        }
    }
}
