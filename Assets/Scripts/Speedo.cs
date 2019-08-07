using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedo : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Rigidbody Target;
    public bool IgnoreVertical;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IgnoreVertical)
        {
            Text.text = Mathf.Round(Vector3.ProjectOnPlane(Target.velocity, Vector3.up).magnitude) + "";
        }
        else
        {
            Text.text = Mathf.Round(Target.velocity.magnitude) + "";
        }
    }
}
