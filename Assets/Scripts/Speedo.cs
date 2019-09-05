using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedo : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Rigidbody Target;
    public bool IgnoreVertical;
    public bool Round = true; //should this speedometer round to the nearest whole number?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 0;
        if (IgnoreVertical)
        {
            speed = Vector3.ProjectOnPlane(Target.velocity, Vector3.up).magnitude;
        }
        else
        {
            speed = Target.velocity.magnitude;
        }

        if (Round)
        {
            speed = Mathf.Round(speed);
        }

        Text.text = speed.ToString();
    }
}
