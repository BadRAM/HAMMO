using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Duration;
    private float _timeElapsed;
    
    void FixedUpdate()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > Duration)
        {
            Destroy(gameObject);
        }
    }
}
