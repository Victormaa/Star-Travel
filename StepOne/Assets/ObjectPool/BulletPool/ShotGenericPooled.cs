using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(TrailRenderer))]
public class ShotGenericPooled : MonoBehaviour
{
    public Bullet CurrentBullet;
    private Transform _cShootUp;
    private Camera maincam;
    private float LifeTime;
    public float maxlifeTime = 4f;

    private Rigidbody Bullet_rigb;
    private TrailRenderer trailRenderer;

    private bool hasCollided = false;

    [HideInInspector]
    public Vector3 impactNormal; //Used to rotate impactparticle.

    private void Instantiate()
    {
        maxlifeTime = CurrentBullet.maxlifeTime;
        LifeTime = 0f;
        maincam = Camera.main;
        if (CurrentBullet.isWeapen3)
        {
            _cShootUp = GameObject.FindWithTag("Player").transform;
        }
        else
        {
            _cShootUp = maincam.transform;
        }

        
    }
    private void Awake()
    {
        if (this.GetComponent<Rigidbody>())
        {
            Bullet_rigb = this.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("you need a rigidbody to your bullet");
        }

        Bullet_rigb.useGravity = false;

        if (this.GetComponent<TrailRenderer>())
        {
            trailRenderer = this.GetComponent<TrailRenderer>();
        }
        else
        {
            Debug.LogError("you need a trailrenderer to your bullet");
        }
    }

    private void OnEnable()
    {
        Instantiate();
    }

    // Update is called once per frame
    void Update()
    {
        LifeTime += Time.deltaTime;
        if (LifeTime > maxlifeTime)
        {
            LifeTime = 0;
            //make sure every time the things is shoot from still
            Bullet_rigb.velocity = Vector3.zero;

            //For only first 4 bullets use trailrenderer
            trailRenderer.enabled = false;

            BulletPool.Instance.ReturnToPool(this.gameObject);
        }
    }

    public void Shoot()
    {
        CurrentBullet.ShootDir = (maincam.transform.forward.normalized * CurrentBullet.ShootForce.Value + _cShootUp.up.normalized * CurrentBullet.Modify_Verticle.Value
        - _cShootUp.right.normalized * CurrentBullet.Modify_Horizontal.Value / 5);
              
        Bullet_rigb.AddForce(CurrentBullet.ShootDir * CurrentBullet.ShootForce.Value, ForceMode.Force);    //2019.3.28 Still need to check for a better mode
    }
}
