using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Player handles the player's Hammo, time, menu, and win/lose conditions

    public int MaxHammo = 30;
    public int StartingHammo = 10;
    [SerializeField] private int Hammo;

    private float _time = 0f;
    private float _checkpointTime;
    private int _checkpointHammo;
    private int _restartPenaltyCount;
    public float RestartPenalty = 1;
    private Checkpoint _checkpoint;
    private Vector3 _spawnPoint;
    
    public TextMeshProUGUI Hammometer;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI RemainingEnemies;
    private int _maxEnemies;

    public Canvas RestartPrompt;
    
    public Canvas StartScreen;
    public TextMeshProUGUI StartScreenRanks;

    public Canvas HUD;
    
    public Canvas PauseMenu;

    public Canvas WinScreen;
    public TextMeshProUGUI WinTime;
    public TextMeshProUGUI WinRank;
    public TextMeshProUGUI HighScoreTime;
    public TextMeshProUGUI HighScoreRank;
    public TextMeshProUGUI NewRecord;

    public Canvas LoseScreen;
    public Canvas IncompleteScreen;
    public TextMeshProUGUI IncompleteText;

    private GameObject[] _enemies;


    public void Goal()
    {
        WinScreen.enabled = true;
        WinTime.text = FloatToTime.Convert(_time);
        ScoreTracker.AddScore(SceneManager.GetActiveScene().buildIndex, GameMode.Get(), _time);
        float topscore = ScoreTracker.GetScore(SceneManager.GetActiveScene().buildIndex, GameMode.Get());
        HighScoreTime.text = FloatToTime.Convert(topscore);
        NewRecord.enabled = _time == topscore;
        FindObjectOfType<LevelRanks>().SetRankIndicator(WinRank, _time);
        FindObjectOfType<LevelRanks>().SetRankIndicator(HighScoreRank, topscore);
        Time.timeScale = 0f;
    }

    // is called when the player reaches the goal without defeating all enemies
    public void Incomplete()
    {
        int e = 0;
        foreach (GameObject i in _enemies)
        {
            if (i.activeSelf)
            {
                e++;
            }
        }
        IncompleteScreen.enabled = true;
        if (e == 1)
        {
            IncompleteText.text = "You missed a pawn";
        }
        else
        {
            IncompleteText.text = "You missed some pawns";
        }
        
        Time.timeScale = 0;
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint != _checkpoint)
        {
            _checkpoint = checkpoint;
            _checkpointTime = _time;
            _checkpointHammo = Hammo;
        }
    }

    public void Kill()
    {
        Time.timeScale = 0f;
        LoseScreen.enabled = true;
    }

    public void IncrementHammo(int i)
    {
        Hammo += i;
        if (Hammo < 0)
        {
            Kill();
        }
        else if (Hammo > MaxHammo)
        {
            Hammo = MaxHammo;
        }
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

    public void RestartLevel()
    {
        Restart(true);
    }

    public void Restart(bool level)
    {
        // delete all Effects and Projectiles
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Effect"))
        {
            Destroy(i);
        }
        
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            Destroy(i);
        }

        // reset all enemies
        foreach(GameObject i in _enemies)
        {
            i.SetActive(true);
            i.GetComponent<Enemy>().Reset();
        }
        
        // pause the game
        Time.timeScale = 0;
        
        // Check if the level has been completed, or the first checkpoint not reached. perform level restart if so.
        if (WinScreen.enabled || _checkpoint == null)
        {
            level = true;
        }
        
        // disable all popup screens
        WinScreen.enabled = false;
        IncompleteScreen.enabled = false;
        PauseMenu.enabled = false;
        LoseScreen.enabled = false;
        RestartPrompt.enabled = false;
        
        // show the Start Screen
        StartScreen.enabled = true;
        StartScreenRanks.text = FindObjectOfType<LevelRanks>().GetRankPreview(ScoreTracker.GetScore(SceneManager.GetActiveScene().buildIndex, GameMode.Get()));

        GetComponent<PlayerWeapon>().Restart();

        if (level)
        {
            GetComponent<FPSWalkMK3>().Restart(level, Quaternion.identity);
        }
        else
        {
            GetComponent<FPSWalkMK3>().Restart(level, _checkpoint.transform.rotation);
        }
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //HUD.enabled = true;
        
        _restartPenaltyCount += 1;
        _time = _checkpointTime + RestartPenalty * _restartPenaltyCount;

        
        if (level || _checkpoint == null)
        {
            _restartPenaltyCount = 0;
            _time = 0f;
            Hammo = StartingHammo;
            transform.position = _spawnPoint;
            
            // reset checkpoints
            _checkpoint = null;
            foreach (Checkpoint i in FindObjectsOfType<Checkpoint>())
            {
                i.Restart();
            }
        }
        else
        {
            Hammo = _checkpointHammo;
            transform.position = _checkpoint.transform.position;
            transform.rotation = _checkpoint.transform.rotation;
            _checkpoint.DestroyEnemies();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerLink.playerLink = GetComponent<Player>();
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _maxEnemies = _enemies.Length;
        _spawnPoint = transform.position;

        if (GameMode.Get() == 3)
        {
            StartingHammo = 1;
        }
        
        Restart(true);
    }

    void FixedUpdate()
    {
        _time += Time.deltaTime;

        RestartPrompt.enabled = transform.position.y < -50;

        if (GameMode.Get() == 1)
        {
            Hammo = 10;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (GameMode.Get() == 1)
        { 
            Hammometer.text = "";
        }
        else
        {
            Hammometer.text = Hammo.ToString();
        }

        Timer.text = FloatToTime.Convert(_time);

        int enemyCount = 0;
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (i.GetComponent<Enemy>().Alive)
            {
                enemyCount += 1;
            }
        }

        RemainingEnemies.text = enemyCount + "/" + _maxEnemies;
        
        // start the level when the player clicks
        if (StartScreen.enabled)
        {
            //Time.timeScale = 0f;

            if (Input.GetButtonDown("Fire1"))
            {
                GetComponent<PlayerWeapon>().Fire();
                StartScreen.enabled = false;
                Time.timeScale = 1;
            }
            else if 
            (Input.GetButtonDown("Horizontal") ||
             Input.GetButtonDown("Vertical") ||
             Input.GetButtonDown("Jump") ||
             Input.GetButtonDown("Pause"))
            {
                StartScreen.enabled = false;
                Time.timeScale = 1;
            }
        }
        else if (Input.GetButtonDown("Fire1") && Time.timeScale > 0)
        {
            GetComponent<PlayerWeapon>().Fire();
        }

        
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }

        if (Input.GetButtonDown("Restart"))
        {
            Restart(false);
        }
        
        //lock cursor while in menus
        if (PauseMenu.enabled || WinScreen.enabled || LoseScreen.enabled || IncompleteScreen.enabled)
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
