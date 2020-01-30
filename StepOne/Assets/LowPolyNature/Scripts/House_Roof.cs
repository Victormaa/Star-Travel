using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Roof : MonoBehaviour {

    public GameObject Roof;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Roof.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Roof.SetActive(true);
        }
    }
}
