using System;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Bullet : ScriptableObject
{
    #region Bullet and its Parametre

    public GameObject _bullet;
    public FloatReference ShootForce;
    public FloatReference Modify_Verticle;
    public FloatReference Modify_Horizontal;
    public Vector3 ShootDir;
    public FloatReference ShootDelay;

    public int bulletid;
    public float maxlifeTime;
    public bool isWeapen3 = false;

    #endregion

    #region Bullet Effects

    //public GameObject impactParticle;
    public GameObject projectileParticle;
    //public GameObject muzzleParticle;
    //public GameObject[] trailParticles;

    #endregion
}
