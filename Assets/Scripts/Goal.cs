using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool l = true;
            foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (i.activeSelf)
                {
                    l = false;
                }
            }

            if (l || GameMode.Get() == 2)
            {
                PlayerLink.playerLink.Goal();
            }
            else
            {
                PlayerLink.playerLink.Incomplete();
            }
        }
    }
}
