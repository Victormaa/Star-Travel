using UnityEngine;
using WeapenStateMachine;

public class HighDamage : WeapenState<Weapen>
{
    #region For only once a game
    private static HighDamage _initialstate;

    public static HighDamage InitialState
    {
        get
        {
            if(_initialstate == null)
            {
                new HighDamage();
            }
            return _initialstate;
        }        
    }

    private HighDamage()
    {
        if(_initialstate != null)
        {
            return;
        }
        _initialstate = this;
    }
    #endregion

    public Camera maincam;
    public float ShootForce = 40;
    public float ShootModify = 8;
    private Vector3 ShootDir;
    public GameObject Bullet;
    public GameObject Gunpoint;

    public float ShootDelay = 4f;
    private float nextShootTime;

    private void Initiate()
    {
        maincam = Camera.main;
        ShootForce = 40;
        ShootModify = 8;
        ShootDelay = 6;
        Gunpoint = GameObject.Find("Gun");
        Bullet = GameObject.Find("NukeBlueOBJ");
    }

    public override void Shoot()
    {

        ShootDir = (maincam.transform.forward.normalized * ShootForce + maincam.transform.up.normalized * ShootModify 
            - maincam.transform.right.normalized * ShootModify / 5);
        //GameObject projectile = Object.Instantiate(projectiles[currentProjectile], Gunpoint.transform.position, Quaternion.identity) as GameObject;
        GameObject bullet = Object.Instantiate(Bullet);

        bullet.transform.position = Gunpoint.transform.position;
        if (bullet.GetComponent<Rigidbody>())
        {
            bullet.GetComponent<Rigidbody>().AddForce(ShootDir * ShootForce, ForceMode.Acceleration);    //2019.3.28 Still need to check for a better mode
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
        Debug.Log("Exiting Highdamage");
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
