using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedo : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Rigidbody Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = Mathf.Round(Target.velocity.magnitude) + "";
    }
}
