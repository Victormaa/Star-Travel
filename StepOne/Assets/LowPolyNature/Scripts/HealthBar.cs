using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Image ImgHealthBar;

    public Text TxtHealth;

    public int Min;

    public int Max;

    public FloatReference AttributeValue;

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

    private int mCurrentValue;

    private float mCurrentPercent;
    
    public void SetValue(int health)
    {
        if(health != mCurrentValue)
        {
            if(Max - Min == 0)
            {
                mCurrentValue = 0;
                mCurrentPercent = 0;
            }
            else
            {
                mCurrentValue = health;
                mCurrentPercent = (float)mCurrentValue / (float)(Max - Min);
            }

            TxtHealth.text = string.Format("{0} %", Mathf.RoundToInt(mCurrentPercent * 100));

            ImgHealthBar.fillAmount = mCurrentPercent;
        }
    }

    public float CurrentPercent
    {
        get { return mCurrentPercent; }
    }

    public int CurrentValue
    {
        get { return mCurrentValue;  }
    }

	// Use this for initialization
	void Start () {
        AttributeValue.Value = 100;
        Max = (int)AttributeValue.Value;
        setValue();
    }

    private void Update()
    {
        setValue();
    }   
}
