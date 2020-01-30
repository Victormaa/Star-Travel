using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeapenStateMachine;

public enum TheWeapenState{
    High,
    Middle,
    Low,
    First,
    Sec,
    Third
}

public class Weapen : MonoBehaviour
{  
    public WeapenFSM<Weapen> Weapenmode { get; set; }

    private bool isshoot = false;
    private TheWeapenState weapenstate;

    public GameObject High_Bullet;
    public GameObject Middle_Bullet;
    public GameObject Low_Bullet;

    public GameObject High_Bullet1;
    public GameObject Middle_Bullet1;
    public GameObject Low_Bullet1;


    private void Initialized()
    {

        //High_Bullet.SetActive(true);
        High_Bullet1.SetActive(true);


        Weapenmode = new WeapenFSM<Weapen>(this);

        weapenstate = TheWeapenState.High;
        Weapenmode.ReplaceState(HighDamage.InitialState);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialized();
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
            if (Input.GetKeyDown("1"))
            {
                if(!High_Bullet1.activeSelf)
                {
                    High_Bullet1.SetActive(true);
                    weapenstate = TheWeapenState.High;
                    Weapenmode.ReplaceState(HighDamage.InitialState);
                }
            }
            else if (Input.GetKeyDown("2"))
            {
                if (!Middle_Bullet1.activeSelf)
                {
                    Middle_Bullet1.SetActive(true);
                    weapenstate = TheWeapenState.Middle;
                    Weapenmode.ReplaceState(MiddleDamage.InitialState);
                }                
            }
            else if (Input.GetKeyDown("3"))
            {
                if (!Low_Bullet1.activeSelf)
                {
                    Low_Bullet1.SetActive(true);
                    weapenstate = TheWeapenState.Low;
                    Weapenmode.ReplaceState(LowDamage.InitialState);
                }                 
            }
            switch (weapenstate)
            {
                case TheWeapenState.High:
                    //Something Easy
                    break;
                case TheWeapenState.Middle:
                    //Something Normal
                    break;
                case TheWeapenState.Low:
                    //Something Hard
                    break;
                default:
                    break;
            }

            Weapenmode.UpdateState();

        }
        else
        {
            weapenstate = TheWeapenState.High;
        }
    }
}
