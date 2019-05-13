using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerWeapon : MonoBehaviour
{
    public int WeaponInUse = 1;
    public Transform BeamStartPoint;
    public Transform RocketStartPoint;
    public GameObject Rocket;
    public GameObject Beam;
    public float RocketCooldown = 1f;
    private float _rocketHeat;
    private float _grenadeActive;
    public GameObject GrenadeCrosshair;
    public GameObject BeamCrosshair;
    public AudioClip RocketLaunchSound;
    public AudioClip BeamFireSound;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Restart()
    {
        _rocketHeat = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _rocketHeat = Mathf.Max(0f, _rocketHeat - Time.deltaTime);

        // switch weapons
        if (WeaponInUse == 1 && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            WeaponInUse = 2;
        }
        else if(WeaponInUse == 2 && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            WeaponInUse = 1;
        }

        if (WeaponInUse == 1)
        {
            // Beam code
            GrenadeCrosshair.SetActive(false);
            BeamCrosshair.SetActive(true);

            if(Input.GetButtonDown("Fire1") && Time.timeScale == 1)
            {
                GetComponent<AudioSource>().PlayOneShot(BeamFireSound);
                GameObject beam = Instantiate(Beam);
                beam.GetComponent<Beam>().startPoint = RocketStartPoint.position;

                GetComponent<Player>().IncrementHammo(-1);
                RaycastHit hit;
                if(Physics.Raycast(BeamStartPoint.position, BeamStartPoint.forward, out hit))
                {
                    beam.GetComponent<Beam>().endPoint = hit.point;

                    if (hit.transform.CompareTag("Enemy"))
                    {

                        if (hit.transform.GetComponent<Enemy>().Stunned)
                        {
                            GetComponent<Player>().IncrementHammo(4);
                        }
                        else
                        {
                            GetComponent<Player>().IncrementHammo(2);
                        }

                        hit.transform.GetComponent<Enemy>().Kill();
                    }
                }
                else
                {
                    beam.GetComponent<Beam>().endPoint = BeamStartPoint.position + BeamStartPoint.forward * 10000f;
                }
            }
        }
        else
        {
            // Rocket Launcher code
            GrenadeCrosshair.SetActive(true);
            BeamCrosshair.SetActive(false);

            if (Input.GetButton("Fire1") && _rocketHeat == 0 && Time.timeScale == 1)
            {
                _rocketHeat = RocketCooldown;
                GameObject r = Instantiate(Rocket, RocketStartPoint.position, BeamStartPoint.rotation);
                r.GetComponent<Rocket>().Player = GetComponent<Player>();
                GetComponent<Player>().IncrementHammo(-1);
            }

            
        }
    }
}
