using UnityEngine;
using WeapenStateMachine;

public class LowDamage : WeapenState<Weapen>
{
    private const string player = "Player";
    #region For only once a game
    private static LowDamage _initialstate;

    public static LowDamage InitialState
    {
        get
        {
            if (_initialstate == null)
            {
                new LowDamage();
            }
            return _initialstate;
        }
    }

    private LowDamage()
    {
        if (_initialstate != null)
        {
            return;
        }
        _initialstate = this;
    }
    #endregion

    public Camera maincam;
    public float ShootForce = 15;
    public float ShootModify = 8;
    private Vector3 ShootDir;
    public GameObject Bullet;
    public GameObject Gunpoint;

    public float ShootDelay = 4f;
    private float nextShootTime;

    //3.29 for a better throw
    public GameObject _Player;

    private void Initiate()
    {
        maincam = Camera.main;
        ShootForce = 18;
        ShootModify = 32;
        ShootDelay = 1;
        Gunpoint = GameObject.Find("Gun");
        Bullet = GameObject.Find("BulletSmallBlueOBJ");
        _Player = GameObject.FindWithTag(player);
    }

    public override void Shoot()
    {
        ShootDir = (maincam.transform.forward.normalized * ShootForce + _Player.transform.up.normalized * ShootModify
            - maincam.transform.right.normalized * ShootModify / 65);
        GameObject bullet = GameObject.Instantiate(Bullet);

        bullet.transform.position = Gunpoint.transform.position;
        if (bullet.GetComponent<Rigidbody>())
        {
            
            bullet.GetComponent<Rigidbody>().AddForce(ShootDir * ShootForce);    //2019.3.28 Still need to check for a better mode
            bullet.GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            Debug.LogError("you need a rigidbody to your bullet");
        }

        GameObject.Destroy(bullet, 3f);

    }

    public override void OnEnter(Weapen _other)
    {
        Initiate();
        
    }

    public override void OnExit(Weapen _other)
    {
        Debug.Log("Exiting LowDamage");
        Bullet.SetActive(false);
    }

    public override void OnUpdate(Weapen _other)
    {
        if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + 1f / ShootDelay;
            Shoot();
        }
    }
}
