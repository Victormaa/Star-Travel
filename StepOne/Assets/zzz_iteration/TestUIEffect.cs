using UnityEngine;
using UnityEngine.UI;

public class TestUIEffect : MonoBehaviour
{
    [SerializeField]
    private Image Injuredimage;

    [SerializeField]
    private Color FlashColor;

    [SerializeField]
    private float FlashSpeed;

    private bool damaged = false;

    [SerializeField]
    private FloatReference HitPoints;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            damaged = true;
        }
        PlayInjuredEffect();
    }

    private void PlayInjuredEffect()
    {
        if (damaged)
        {
            Injuredimage.color = FlashColor;
        }
        else
        {
            Injuredimage.color = Color.Lerp(Injuredimage.color, Color.clear, FlashSpeed * Time.deltaTime);

        }
        damaged = false;
    }


}
