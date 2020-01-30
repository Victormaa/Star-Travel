using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float AutoRotateRate_y;
    public float AutoRotateRate_x;

    private void AutoRotation()
    {
        //this.transform.RotateAround(Center.transform.position, Center.transform.up, _RotationSpeed);
        this.transform.localRotation *= Quaternion.Euler(AutoRotateRate_x, AutoRotateRate_y, 0);
    }

    
    private void Update()
    {
        AutoRotation();
    }
}
