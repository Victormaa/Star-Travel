using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Camera maincam;
    public float shootdistance = 100;
    public float damage = 20;

    public float ShootForce = 40;
    public float ShootModify = 5;    
    private Vector3 ShootDir;
    public GameObject Bullet;
    public GameObject GunPoint;
    public float testdivide = 20;

    public float ShootDelay = 4f;
    private float nextShootTime;

    private void Initiate()
    {
        maincam = Camera.main;
        ShootForce = 18;
        ShootModify = 32;
        ShootDelay = 1;
        testdivide = 20;
        GunPoint = GameObject.Find("Gun");
        Bullet = GameObject.Find("Bullet");
    }

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + 1f / ShootDelay;
            OnShoot();
        }
    }

    public void Shoot()
    {

        ShootDir = (maincam.transform.forward.normalized * ShootForce + maincam.transform.up.normalized * ShootModify
            - maincam.transform.right.normalized * ShootModify / testdivide);
        GameObject bullet = GameObject.Instantiate(Bullet);

        bullet.transform.position = GunPoint.transform.position;
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



        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(maincam.transform.position, maincam.transform.forward, out hit, shootdistance))
        {
            Debug.Log(hit.transform.name);
            Debug.Log(ShootDir);

            InjureTarget target = hit.transform.GetComponent<InjureTarget>();
            if (target != null)
            {
                target.Takedamage(damage);
            }
        }

        

    }

    public void OnShoot()
    {        
            Shoot();       
    }

}
