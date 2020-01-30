using UnityEngine;

public class WeapenMode_1 : MonoBehaviour
{
    private enum Weapenstate
    {
        High,
        Middle,
        Low
    }

    private bool isshoot = false;

    private Transform _cShootUp;
    private Weapenstate weapenstate;
    private Bullet Currentbullet; 
    private float nextShootTime;

    public Bullet bullet1;
    public Bullet bullet2;
    public Bullet bullet3;

    public Camera maincam;

    public GameObject Gunpoint;

    private void Initiate()
    {
        maincam = Camera.main;
        Currentbullet = bullet1;
        Gunpoint = GameObject.Find("Gun");
    }

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            isshoot = !isshoot;
        }
        if (isshoot)
        {
            _cShootUp = maincam.transform;

            if (Input.GetKeyDown("1"))
            {
                weapenstate = Weapenstate.High;
                Currentbullet = bullet1;
            }
            else if (Input.GetKeyDown("2"))
            {
                weapenstate = Weapenstate.Middle;
                Currentbullet = bullet2;
            }
            else if (Input.GetKeyDown("3"))
            {
                weapenstate = Weapenstate.Low;
                Currentbullet = bullet3;
            }


            switch (weapenstate)
            {
                case Weapenstate.High:
                    if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
                    {
                        nextShootTime = Time.time + 1f / Currentbullet.ShootDelay.Value;
                        Shoot();
                    }
                        break;
                case Weapenstate.Middle:
                    if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
                    {
                        nextShootTime = Time.time + 1f / Currentbullet.ShootDelay.Value;
                        Shoot();
                    }
                    break;
                case Weapenstate.Low:

                    _cShootUp = GameObject.FindWithTag("Player").transform;

                    if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
                    {
                        nextShootTime = Time.time + 1f / Currentbullet.ShootDelay.Value;
                        Shoot();
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            weapenstate = Weapenstate.High;
        }
    }

    private void Shoot()
    {
        Currentbullet.ShootDir = (maincam.transform.forward.normalized * Currentbullet.ShootForce.Value + _cShootUp.up.normalized * Currentbullet.Modify_Verticle.Value
            - maincam.transform.right.normalized * Currentbullet.Modify_Horizontal.Value / 5);

        GameObject bullet = Instantiate(Currentbullet._bullet);

        bullet.transform.position = Gunpoint.transform.position;

        if (bullet.GetComponent<Rigidbody>())
        {
            bullet.GetComponent<Rigidbody>().useGravity = false;
            bullet.GetComponent<Rigidbody>().AddForce(Currentbullet.ShootDir * Currentbullet.ShootForce.Value, ForceMode.Acceleration);    //2019.3.28 Still need to check for a better mode
        }
        else
        {
            Debug.LogError("you need a rigidbody to your bullet");
        }
        Destroy(bullet, 4f);
    }
}
