using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    public Checkpoint PreviousCheckPoint;
    public List<Enemy> RequiredEnemies;
    private bool _untriggered = true;

    public void Restart()
    {
        _untriggered = true;
    }

    public void DestroyEnemies()
    {
        foreach (Enemy i in RequiredEnemies)
        {
            i.gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (_untriggered && other.CompareTag("Player"))
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
                PlayerLink.playerLink.SetCheckpoint(GetComponent<Checkpoint>());
                _untriggered = false;
            }
        }
    }
}
