using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Transform HitSoundEmitter;
    [SerializeField] private float duration = 1;
    [SerializeField] private float lineDuration = 0.1f;
    private float elapsedTime;

    private void Start()
    {
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, startPoint);
        GetComponent<LineRenderer>().SetPosition(1, endPoint);
        HitSoundEmitter.position = endPoint;
        HitSoundEmitter.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
        
        var material = GetComponent<Renderer>().material;
        var color = material.color;
 
        material.color = new Color(color.r, color.g, color.b, color.a = Mathf.Lerp(0, 1, (lineDuration - elapsedTime) / lineDuration));
    }
}
