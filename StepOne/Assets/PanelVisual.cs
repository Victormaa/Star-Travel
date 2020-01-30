using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelVisual : MonoBehaviour
{
    public Image Panel;
    // Start is called before the first frame update
    void Start()
    {
        Panel = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Panel.CrossFadeAlpha(0, 4f, false);
    }
}
