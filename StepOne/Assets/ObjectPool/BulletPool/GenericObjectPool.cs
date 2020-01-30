using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericObjectPool<T> : MonoBehaviour where T : Bullet
{

    [SerializeField]
    private T Prefeb;

    public T _Prefeb { get { return Prefeb; }  set { Prefeb = value; } }

    public GameObject PooledBullet
    {
        get
        {
            return _Prefeb._bullet;
        }
        private set
        {
            PooledBullet = value;
        }
    }

    public GameObject ProjectileParticle
    {
        get
        {
            return _Prefeb.projectileParticle;
        }
        private set
        {
            ProjectileParticle = value;
        }
    }

    private int _poolnum;

    public int Poolnum
    {
        get
        {
            return _poolnum;
        }
        private set
        {
            _poolnum = value;
        }
    }

    public static GenericObjectPool<T> Instance { get; private set; }

    public Queue<GameObject> bullets = new Queue<GameObject>();

    public Queue<GameObject> b_Effects = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject Get()
    {
        if (bullets.Count == 0)
        {
            AddObjects(1);
        }
        //every bullet's thing should be dequeue here
        b_Effects.Dequeue();
        return bullets.Dequeue();
    }

    //public GameObject GetEffect(GameObject effectType)
    //{
    //    if(b_Effects.Count == 0)
    //    {
    //        AddEffects(effectType);
    //    }
    //    return b_Effects.Dequeue();
    //}

    //private void AddEffects(GameObject effectType)
    //{
    //    var particle = Instantiate(ProjectileParticle);
    //    particle.gameObject.SetActive(false);
    //    b_Effects.Enqueue(particle);
    //}

    public void AddObjects(int count)
    {
        Poolnum += count;
        for (int i = 0; i < count; i++)
        {
            var newObject = Instantiate(PooledBullet);

            var particle = Instantiate(ProjectileParticle);
            particle.gameObject.transform.position = newObject.transform.position;
            particle.gameObject.transform.rotation = newObject.transform.rotation;
            particle.gameObject.transform.parent = newObject.transform;

            particle.gameObject.SetActive(false);
            b_Effects.Enqueue(particle);

            newObject.gameObject.SetActive(false);
            bullets.Enqueue(newObject);
        }
        
    }

    public void DeleteObjects(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Destroy(bullets.Dequeue());
            Destroy(b_Effects.Dequeue());

            Poolnum--;
        }
    }


    public void ReturnToPool(GameObject objectToReturn)
    {
        bullets.Enqueue(objectToReturn);        
        foreach (Transform child in objectToReturn.transform)
        {
            b_Effects.Enqueue(child.gameObject);
            child.gameObject.SetActive(false);
        }
        objectToReturn.gameObject.SetActive(false);
    }

}
