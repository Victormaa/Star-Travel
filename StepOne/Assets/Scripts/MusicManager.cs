using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource RelaxMusic;

    [SerializeField]
    private AudioSource BattleMusic;

    [SerializeField]
    private AudioSource ExploreMusic;

    [SerializeField]
    private AudioSource HeartBeat;

    private bool isHeartBeated = false;

    private AudioSource CurrentMusic;

    private float PitchLevel;

    [Range(0,1)]
    public float CurrentMusicVolum;

    public enum MusicState
    {
        Relax_m,
        Battle_m,
        Explore_m,
        LowBloodLevel
    }

    public MusicState musicState = MusicState.Relax_m;

    public static MusicManager Instance;

    private void Start()
    {
        if (Instance != null)
        {
            return;
        }
        else
        {
            Instance = this;
        }
        CurrentMusic = BattleMusic;
        musicState = MusicState.Relax_m;
    }

    public void ReplaceMusic(AudioSource audio)
    {
        if(CurrentMusic == audio)
        {
            CurrentMusicVolum = 0.8f;
            return;
        }
        else
        {
            CurrentMusic.Stop();
            CurrentMusic = audio;
        }
        CurrentMusic.volume = 0.8f;
        CurrentMusic.Play();
    }

    public void StartheartBeat()
    {
        HeartBeat.Play();
        isHeartBeated = true;
    }

    public void StopheartBeat()
    {
        HeartBeat.Stop();
        isHeartBeated = false;
    }

    public void InjuredAudioEffects(float bloodLevel)
    {
        if (bloodLevel > 0.2)
        {
            PitchLevel = 0;
        }
        else
        {
            PitchLevel = (0.2f - bloodLevel) * 100;
        }

        if (HeartBeat.isPlaying)
        {
            HeartBeat.pitch = (1 + 0.02f * PitchLevel);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            musicState = MusicState.Relax_m;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            musicState = MusicState.Battle_m;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            musicState = MusicState.Explore_m;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            musicState = MusicState.LowBloodLevel;
        }

        if (isHeartBeated)
        {
            CurrentMusic.volume = 0.2f;
        }
        else
        {
            CurrentMusic.volume = 0.8f;
        }
        //CurrentMusic.volume = CurrentMusicVolum;

        switch (musicState)
        {
            case MusicState.Relax_m:
                ReplaceMusic(RelaxMusic);
                return;
            case MusicState.Battle_m:
                ReplaceMusic(BattleMusic);
                return;
            case MusicState.Explore_m:
                ReplaceMusic(ExploreMusic);
                return;
            case MusicState.LowBloodLevel:
                return;
            default:
                break;
        }
    }
}
