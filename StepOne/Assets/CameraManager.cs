using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraPosi
    {
        Player,
        Flight
    }

    public CameraPosi CameraState;

    private Transform PlayerCam;

    private Transform FlightCam;

    private Camera maincam;

    public Camera MainCam
    {
        get
        {
            return maincam;
        }
    }
    public Vector3 CamView_1;

    public Vector3 CamView_2;

    public Transform CamPosi_1;

    public Transform CamPosi_2;

    public static CameraManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maincam = Camera.main;
        CameraState = CameraPosi.Player;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CameraState)
        {
            case CameraPosi.Player:
                //Camera position go to the place
                if (maincam.transform.parent == CamPosi_1)
                {
                    return;
                }
                else
                {
                    GameManager.Instance.Player.gameObject.SetActive(true);
                    //.................................
                    Camerachange(CamView_1, CamPosi_1);
                    maincam.GetComponent<CameraFlightFollow>().enabled = false;
                    maincam.GetComponent<CustomPointer>().enabled = false;
                }
                return;
            case CameraPosi.Flight:
                if (maincam.transform.parent == CamPosi_2)
                {
                    return;
                }
                else
                {
                    Camerachange(CamView_2, CamPosi_2);
                    maincam.GetComponent<CameraFlightFollow>().enabled = true;
                    maincam.GetComponent<CustomPointer>().enabled = true;
                    //.........................................
                    GameManager.Instance.Player.gameObject.transform.position = GameManager.Instance.Flight.GetComponent<PlayerFlightControl>().LandPoint.position;
                    GameManager.Instance.Player.gameObject.transform.parent = GameManager.Instance.Flight.GetComponent<PlayerFlightControl>().LandPoint;
                    GameManager.Instance.Player.gameObject.SetActive(false);
                }
                return;
            default:
                break;
        }
    }

    public void Camerachange(Vector3 rot, Transform parent)
    {
        maincam.transform.parent = parent;
        maincam.transform.position = parent.position;
        maincam.transform.localEulerAngles = rot;
    }
}
