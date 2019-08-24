using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerWeapon : MonoBehaviour
{
    public int WeaponInUse = 1; // 1 = beam, 2 = rocket
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
    private LayerMask _layerMask;


    // Start is called before the first frame update
    void Start()
    {
        _layerMask = LayerMask.GetMask("EnemyCollision", "Terrain");
        GrenadeCrosshair.SetActive(true);
        BeamCrosshair.SetActive(false);
    }

    public void Restart()
    {
        _rocketHeat = 0;
    }

    public void Fire()
    {

        if (WeaponInUse == 1)
        {
            GetComponent<AudioSource>().PlayOneShot(BeamFireSound);
            GameObject beam = Instantiate(Beam);
            beam.GetComponent<Beam>().startPoint = RocketStartPoint.position;

            RaycastHit hit;
            if (Physics.Raycast(BeamStartPoint.position, BeamStartPoint.forward, out hit, Mathf.Infinity, _layerMask))
            {
                beam.GetComponent<Beam>().endPoint = hit.point;

                if (hit.transform.CompareTag("Enemy"))
                {

                    if (hit.transform.GetComponent<Enemy>().Stunned)
                    {
                        GetComponent<Player>().IncrementHammo(3); // takes cost of firing into account. 4 - 1 = 3
                    }
                    else
                    {
                        GetComponent<Player>().IncrementHammo(1); // 2 - 1 = 1
                    }

                    hit.transform.GetComponent<Enemy>().Kill();
                }
                else
                {
                    GetComponent<Player>().IncrementHammo(-1);
                }
            }
            else
            {
                GetComponent<Player>().IncrementHammo(-1);
                beam.GetComponent<Beam>().endPoint = BeamStartPoint.position + BeamStartPoint.forward * 10000f;
            }
        }
        else
        {
            if (_rocketHeat == 0)
            {
                _rocketHeat = RocketCooldown;
                GameObject r = Instantiate(Rocket, RocketStartPoint.position, BeamStartPoint.rotation);
                r.GetComponent<Rocket>().Player = GetComponent<Player>();
                GetComponent<Player>().IncrementHammo(-1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _rocketHeat = Mathf.Max(0f, _rocketHeat - Time.deltaTime);

        // switch weapons
        if (WeaponInUse == 1 && Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            GrenadeCrosshair.SetActive(true);
            BeamCrosshair.SetActive(false);
            WeaponInUse = 2;
        }
        else if(WeaponInUse == 2 && Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            GrenadeCrosshair.SetActive(false);
            BeamCrosshair.SetActive(true);
            WeaponInUse = 1;
        }

        if (Input.GetButtonDown("Fire1") && Time.timeScale == 1)
        {
            Fire();
        }
    }

    private void FixedUpdate()
    {
        if (WeaponInUse == 2 && Input.GetButton("Fire1"))
        {
            Fire();
        }
    }
}
