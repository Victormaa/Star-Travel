using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour
{

    //public float gravity = -20;   

    public void Attract(Transform body,float Custom)
    {
        Vector3 Gravityup = (body.position - this.transform.position).normalized;
        Vector3 Bodyup = body.up;

        // 在unity3d中, quaternion 的乘法操作 (operator  * ) 有两种操作:
        //(1) quaternion* quaternion, 例如 q = t * p; 这是将一个点先进行t 操作旋转, 然后进行p操作旋转.

        body.rotation = Quaternion.FromToRotation(Bodyup, Gravityup) * body.rotation;
        body.GetComponent<Rigidbody>().AddForce(Gravityup * Custom);
    }
}
