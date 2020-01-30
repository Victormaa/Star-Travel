using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryOn : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
