using UnityEngine;

public class CameraFolllow : MonoBehaviour
{
    private enum CameraPosi
    {
        Player,
        Sheep,
        None
    }

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

    //public Transform NextCamPosi;


    private void initiate()
    {
        maincam = Camera.main;
        CameraState = CameraPosi.None;
    }

    private void Awake()
    {
        initiate();        
    }
    
    //public CameraFolllow cameraManager;
    private CameraPosi CameraState;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CameraState = CameraPosi.Player;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            CameraState = CameraPosi.Sheep;
        }

        switch (CameraState)
        {
            case CameraPosi.Player:
                //Camera position go to the place
                if (CameraManager.Instance.CameraState != CameraManager.CameraPosi.Player && maincam.transform.parent != CamPosi_1)
                {
                    Camerachange(CamView_1, CamPosi_1);
                }
                else
                {
                    return;
                }
                return;
            case CameraPosi.Sheep:
                if (maincam.transform.parent == CamPosi_2)
                {
                    return;
                }
                else
                {
                    Camerachange( CamView_2, CamPosi_2);
                }
                return;
            default:
                break;
        }
    }

    public void Camerachange(Vector3 rot,Transform parent)
    {
        maincam.transform.parent = parent;
        maincam.transform.position = parent.position;
        maincam.transform.localEulerAngles = rot;
    }
}
