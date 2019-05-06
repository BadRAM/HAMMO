using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    // Player handles the player's Hammo, time, menu, and win/lose conditions

    public int MaxHammo = 20;
    public int StartingHammo = 10;
    [SerializeField] private int Hammo;
    private float _walkSpeed;

    private float _time = 0f;
    private Vector3 _spawnPoint;
    
    public TextMeshProUGUI Hammometer;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI RemainingEnemies;
    private int _maxEnemies;

    public Canvas HUD;
    public Canvas PauseMenu;

    public Canvas WinScreen;
    public TextMeshProUGUI WinTime;

    public Canvas LoseScreen;
    public Canvas StartScreen;

    private GameObject[] _enemies;


    public void Goal()
    {
        WinScreen.enabled = true;
        WinTime.text = "Time: " + FloatToTime(_time);
        Time.timeScale = 0f;
    }

    public void IncrementHammo(int i)
    {
        Hammo += i;
    }

    public void TogglePause()
    {
        if(PauseMenu.isActiveAndEnabled)
        {
            Time.timeScale = 1;
            PauseMenu.enabled = false;
        }
        else
        {
            Time.timeScale = 0;
            PauseMenu.enabled = true;
        }

    }

    string FloatToTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void Restart()
    {
        StartScreen.enabled = true;
        Time.timeScale = 0;

        HUD.enabled = true;

        _time = 0f;
        Hammo = StartingHammo;
        transform.position = _spawnPoint;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        WinScreen.enabled = false;
        PauseMenu.enabled = false;
        LoseScreen.enabled = false;

        GetComponent<PlayerWeapon>().Restart();

        foreach(GameObject i in _enemies)
        {
            i.SetActive(true);
            i.GetComponent<Enemy>().Reset();
        }

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Effect"))
        {
            Destroy(i);
        }
        
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            Destroy(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerLink.playerLink = GetComponent<Player>();
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _maxEnemies = _enemies.Length;
        _spawnPoint = transform.position;

        Restart();
    }

    void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;

        // kill the player if hammo is less than zero
        if (Hammo < 0)
        {
            Time.timeScale = 0f;
            LoseScreen.enabled = true;
        }

        if (transform.position.y < -50)
        {
            IncrementHammo(-100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Hammometer.text = Hammo.ToString();
        //Timer.text = _time.ToString();

        Timer.text = FloatToTime(_time);

        int enemyCount = 0;
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (i.activeSelf)
            {
                enemyCount += 1;
            }
        }

        RemainingEnemies.text = enemyCount + "/" + _maxEnemies;
        
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }

        if (Input.GetButtonDown("Restart"))
        {
            Restart();
        }

        // start the level when the player clicks
        if (StartScreen.enabled)
        {
            _time = 0f;
            Time.timeScale = 0f;

            if (Input.GetButtonDown("Horizontal") ||
                Input.GetButtonDown("Vertical") ||
                Input.GetButtonDown("Jump") ||
                Input.GetButtonDown("Fire1"))
            {
                StartScreen.enabled = false;
                Time.timeScale = 1f;
            }
        }


        //lock cursor while in menus
        if (PauseMenu.enabled || WinScreen.enabled || LoseScreen.enabled)
        {
            GetComponent<FPSWalkMK3>().LockLook = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            GetComponent<FPSWalkMK3>().LockLook = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
