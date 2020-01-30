using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TestHealthBar : MonoBehaviour
{
    public Image ImgHealthBar;

    public Text TxtHealth;

    public FloatReference AttributeValue;

    public int Max;

    private float mCurrentPercent;

    public float CurrentPercent
    {
        get { return mCurrentPercent; }
    }

    private void setValue()
    {
        if (AttributeValue.Value < 0)
        {
            AttributeValue.Value = 0;
        }
        if (AttributeValue.Value > 100)
        {
            AttributeValue.Value = 100;
        }
        mCurrentPercent = AttributeValue.Value / Max;
        ImgHealthBar.fillAmount = mCurrentPercent;
        TxtHealth.text = string.Format("{0} %", Mathf.RoundToInt(mCurrentPercent * 100));
    }

    // Use this for initialization
    void Start()
    { 
        AttributeValue.Value = 100;
        Max = (int)AttributeValue.Value;
        setValue();
    }

    private void Update()
    {
        setValue();
    } 
}
