using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInjuredEffect : MonoBehaviour
{
    public AudioSource theAudio;

    private float bloodlevel;

    public FloatReference Hitpoints;

    private bool isLowHealthPlay = false;

    private float mCurrentPercent;

    #region the damaged UI
    [SerializeField]
    private Image Injuredimage;

    [SerializeField]
    private Color FlashColor;

    [SerializeField]
    private float FlashSpeed;

    [HideInInspector]
    public bool damaged = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //mCurrentPercent = new TestHealthBar().CurrentPercent;
        mCurrentPercent = 100;
    }

    private bool isLowHealth()
    {
        return (Hitpoints.Value < 20);
    }

    // Update is called once per frame
    void Update()
    {
        //mCurrentPercent = new TestHealthBar().CurrentPercent;

        if (isLowHealth() && !isLowHealthPlay)
        {
            MusicManager.Instance.StartheartBeat();
            MusicManager.Instance.InjuredAudioEffects(mCurrentPercent);
            isLowHealthPlay = true;

        }
        if (!isLowHealth() && isLowHealthPlay)
        {
            MusicManager.Instance.StopheartBeat();
            isLowHealthPlay = false;
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
