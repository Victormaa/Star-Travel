using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapenMode_2 : MonoBehaviour
{
    private enum Weapenstate
    {
        High,
        Middle,
        Low
    }

    public Bullet Bullet_1;
    public Bullet Bullet_2;
    public Bullet Bullet_3;

    private bool isshoot = false;

    private Weapenstate weapenstate;
    private bool isLoaded = false;
    private bool isChanged = false;

    private float nextShootTime = 0f;
    public GameObject Gunpoint;

    private void Start()
    {
        nextShootTime = 0;
        Gunpoint = GameObject.Find("Gun");
        weapenstate = Weapenstate.High;
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            isshoot = !isshoot;
        }
        if (isshoot)
        {           
            if (Input.GetKeyDown("1"))
            {
                isChanged = true;
                weapenstate = Weapenstate.High;
            }
            else if (Input.GetKeyDown("2"))
            {
                isChanged = true;
                weapenstate = Weapenstate.Middle;
                
            }
            else if (Input.GetKeyDown("3"))
            {
                isChanged = true;
                weapenstate = Weapenstate.Low;
                
            }

            switch (weapenstate)
            {
                case Weapenstate.High:
                    if (isChanged)
                    {
                        BulletPool.Instance.DeleteObjects(BulletPool.Instance.bullets.Count);
                        if(BulletPool.Instance.Poolnum == 0)
                        {
                            isChanged = false;
                            isLoaded = false;
                        }      
                    }

                    if (!isLoaded)
                    {
                        isLoaded = true;
                        BulletPool.Instance._Prefeb = Bullet_1;
                        BulletPool.Instance.AddObjects(4);
                    }
                    break;
                case Weapenstate.Middle:
                    if (isChanged)
                    {
                        BulletPool.Instance.DeleteObjects(BulletPool.Instance.bullets.Count);
                        if (BulletPool.Instance.Poolnum == 0)
                        {
                            isChanged = false;
                            isLoaded = false;
                        }
                    }
                    if (!isLoaded)
                    {
                        isLoaded = true;
                        BulletPool.Instance._Prefeb = Bullet_2;
                        BulletPool.Instance.AddObjects(4);
                    }
                    break;
                case Weapenstate.Low:
                    if (isChanged)
                    {
                        BulletPool.Instance.DeleteObjects(BulletPool.Instance.bullets.Count);
                        if (BulletPool.Instance.Poolnum == 0)
                        {
                            isChanged = false;
                            isLoaded = false;
                        }
                    }

                    if (!isLoaded)
                    {
                        isLoaded = true;
                        BulletPool.Instance._Prefeb = Bullet_3;
                        BulletPool.Instance.AddObjects(4);
                    }
                    break;
                default:
                    break;
            }

            var CurrentBullet = BulletPool.Instance._Prefeb;

            if (!isChanged)
            {
                if (Input.GetButton("Fire1") && Time.time >= nextShootTime)
                {
                    nextShootTime = Time.time + 1f / CurrentBullet.ShootDelay.Value;
                    var bullet = BulletPool.Instance.Get();
                    bullet.transform.position = Gunpoint.transform.position;
                    foreach (Transform child in bullet.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    //bullet.transform.GetChild(0).gameObject.SetActive(true);
                    bullet.gameObject.SetActive(true);

                    //  具体的射击调用在此---因为最初的枪的属性是根据这种判断时间来设置的
                    bullet.GetComponent<ShotGenericPooled>().Shoot();
                    //
                }
            }
        }
        else
        {
            weapenstate = Weapenstate.High;
        }
    }
}
