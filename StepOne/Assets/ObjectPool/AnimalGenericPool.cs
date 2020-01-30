using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalGenericPool<T> : MonoBehaviour where T : Component
{
    [SerializeField]
    private T Prefeb;

    public T _Prefeb
    {
        get
        {
            return Prefeb;
        }
        private set
        {
            Prefeb = value;
        }
    }

    private int _poolnum;

    // About the number of the animals
    public int Number
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

    public static AnimalGenericPool<T> Instance { get; private set; }

    public Queue<T> prefebs = new Queue<T>();

    private void Awake()
    {
        Instance = this;
    }

    public T Get()
    {
        if(prefebs.Count == 0)
        {
            AddObjects();
        }
        return prefebs.Dequeue();
    }

    public void AddObjects()
    {
        var newObject = Instantiate(_Prefeb);
        newObject.gameObject.SetActive(false);
        Number++;
        prefebs.Enqueue(newObject);
    }

    //public void DeleteObjects

    public void ReturnToPool(T objectToReturn)
    {
        prefebs.Enqueue(objectToReturn);
        objectToReturn.gameObject.SetActive(false);
    }
}
