using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance = null;

    public PlayerController Player;

    public GameObject Flight;

    public GameObject GameOverHUD;

    public float ShowGameOverTime = 1.5f;

    public int SheepNum;

    public int SpiderNum;

    [SerializeField]
    private Transform SheepBirthPoints;

    [SerializeField]
    private Transform SpiderBirthPoints;

    /// <summary>
    /// Get On Plane
    /// </summary>

        public void GetOnPlane()
    {
        MusicManager.Instance.musicState = MusicManager.MusicState.Relax_m;
        Player.DisableControl();
        Flight.GetComponent<PlayerFlightControl>().enabled = true;
        Flight.GetComponent<FauxGravityBody>().OffPlanet();         //控制飞机需要改变飞机上Rigidbody的值
    }

    public void GetOffPlane()
    {        
        Flight.GetComponent<FauxGravityBody>().StayOnPlanet();
        Flight.GetComponent<PlayerFlightControl>().enabled = false;
        Player.EnableControl();
    }

    void Awake()
    {
        ///
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Dont destroy on reloading the scene
        // DontDestroyOnLoad(gameObject);

        if (Player == null)
        {
            Debug.LogError("You need to assign a Player to the GameManager");
        }

        if (Player != null)
        {
            Player.PlayerDied += Player_PlayerDied;
        }

        //CameraState = CameraPosi.Player;

        CursorLock();     // Lock and hide the cursor;
    }

    private int SheepReNum;
    private int SpiderReNum;

    private void Start()
    {
        //SheepPool.Instance.Get(SheepNum);       
        GenerateSheep();
        GenerateSpider();
        InvokeRepeating("SheepReGen", 5f, 5f);
        InvokeRepeating("SpiderReGen", 5f, 5f);
    }

    private void Update()
    {
        SheepReNum = SheepPool.Instance.prefebs.Count;
        SpiderReNum = SpiderPool.Instance.prefebs.Count;

        if (Player.IsDead)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SpiderReGen()
    {
        var min = 0;
        var max = 4;
        if (SpiderReNum > 0)
        {
            for (int i = 0; i < SpiderReNum; i++)
            {
                var birthPoint = SpiderBirthPoints.GetChild(Random.Range(min, max));
                var spider = SpiderPool.Instance.Get();
                if (spider.gameObject.GetComponent<Patrol>().enabled)
                {
                    Debug.Log("isTrue");
                }
                else
                {
                    Debug.Log("turning patroltoture");
                    spider.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (spider.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    spider.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {
                    Debug.Log("isFalse");
                }
                spider.transform.position = birthPoint.transform.position;
                spider.gameObject.SetActive(true);
            }
        }
    }

    private void SheepReGen()
    {
        //var leafSheep = SheepNum - SheepPool.Instance.prefebs.Count;
        var min = 0;
        var max = 4;
        if (SheepReNum > 0)
        {
            for (int i = 0; i < SheepReNum; i++)
            {
                var birthPoint = SheepBirthPoints.GetChild(Random.Range(min, max));
                var sheep = SheepPool.Instance.Get();
                if (sheep.gameObject.GetComponent<Patrol>().enabled)
                {
                    Debug.Log("isTrue");
                }
                else
                {
                    Debug.Log("turning patroltoture");
                    sheep.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (sheep.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    sheep.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {
                    Debug.Log("isFalse");
                }
                sheep.transform.position = birthPoint.transform.position;
                sheep.gameObject.SetActive(true);
            }
        }        
    }

    private void GenerateSpider()
    {
        var min = 0;
        var max = 4;
        if (SpiderReNum > 0)
        {
            for (int i = 0; i < SpiderReNum; i++)
            {
                var birthPoint = SpiderBirthPoints.GetChild(Random.Range(min, max));
                var spider = SpiderPool.Instance.Get();
                if (spider.gameObject.GetComponent<Patrol>().enabled)
                {
                    Debug.Log("isTrue");
                }
                else
                {
                    Debug.Log("turning patroltoture");
                    spider.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (spider.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    spider.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {
                    Debug.Log("isFalse");
                }
                spider.transform.position = birthPoint.transform.position;
                spider.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < SpiderNum; i++)
            {
                var birthPoint = SpiderBirthPoints.GetChild(Random.Range(min, max));
                var spider = SpiderPool.Instance.Get();
                if (spider.gameObject.GetComponent<Patrol>().enabled)
                {
                    Debug.Log("isTrue");
                }
                else
                {
                    Debug.Log("turning patroltoture");
                    spider.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (spider.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    spider.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {
                    Debug.Log("isFalse");
                }
                spider.transform.position = birthPoint.transform.position;
                spider.gameObject.SetActive(true);
            }
        }
    }

    private void GenerateSheep()
    {
        var min = 0;
        var max = 4;
        if(SheepReNum > 0)
        {
            for (int i = 0; i < SheepReNum; i++)
            {
                var birthPoint = SheepBirthPoints.GetChild(Random.Range(min, max));
                var sheep = SheepPool.Instance.Get();
                if (sheep.gameObject.GetComponent<Patrol>().enabled)
                {
                    Debug.Log("isTrue");
                }
                else
                {
                    Debug.Log("turning patroltoture");
                    sheep.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (sheep.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    sheep.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {
                    Debug.Log("isFalse");
                }
                sheep.transform.position = birthPoint.transform.position;
                sheep.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < SheepNum; i++)
            {
                var birthPoint = SheepBirthPoints.GetChild(Random.Range(min, max));
                var sheep = SheepPool.Instance.Get();
                if (sheep.gameObject.GetComponent<Patrol>().enabled)
                {

                }
                else
                {
                    Debug.Log("turning patroltoture");
                    sheep.gameObject.GetComponent<Patrol>().enabled = true;
                }
                if (sheep.gameObject.GetComponent<AIDestinationSetter>().enabled)
                {
                    Debug.Log("turning patroltofalse");
                    sheep.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                }
                else
                {

                }
                sheep.transform.position = birthPoint.transform.position;
                sheep.gameObject.SetActive(true);
            }
        }     
    }

    private void Player_PlayerDied(object sender, System.EventArgs e)
    {
        if (Player.Hud == null)
        {
            Debug.LogError("You need to assign a HUD to the PlayerController");
        }
        else
        {
            Player.Hud.gameObject.SetActive(false);

            if(GameOverHUD == null)
            {
                Debug.LogError("You need to assign a GameOverHUD to the GameManager");
            }
            else
            {
                Invoke("ShowGameOver", ShowGameOverTime);
            }
        }
    }

    private void ShowGameOver()
    {
        GameOverHUD.SetActive(true);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CursorLock()
    {
        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;
    }
   
}
