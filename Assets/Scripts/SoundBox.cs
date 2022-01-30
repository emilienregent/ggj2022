using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : MonoBehaviour
{
    public AudioSource SoundBoxSource;
    public AudioSource backgroundMusicSource;
    public AudioClip StartMenuMusic;
    public AudioClip GameMusic;
    public AudioClip VictoryMusic;
    public AudioClip PacmanEatingSFX;
    public AudioClip PacmanDeathSFX;
    public AudioClip GhostDeathSFX;


    private bool _isPowerUp = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnChangeStateHandler += UpdateSFX;
        GameManager.Instance.PelletCollected += EatPelletSfx;
        GameManager.Instance.PacmanDying += EatPacmanSfx;
        GameManager.Instance.PhantomDying += EatGhostSfx;
        UpdateSFX();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy() {
        GameManager.Instance.OnChangeStateHandler -= UpdateSFX;
        GameManager.Instance.PelletCollected -= EatPelletSfx;
        GameManager.Instance.PacmanDying -= EatPacmanSfx;
        GameManager.Instance.PhantomDying -= EatGhostSfx;
    }

    void UpdateSFX() {
        switch(GameManager.Instance.CurrentState)
        {
            case GameState.START:
                backgroundMusicSource.Stop();
                SoundBoxSource.Stop();
                backgroundMusicSource.clip = StartMenuMusic;
                backgroundMusicSource.Play();
                break;
            case GameState.PACMAN:
            case GameState.GHOST:
                backgroundMusicSource.Stop();
                backgroundMusicSource.clip = GameMusic;
                backgroundMusicSource.Play();
                break;
            case GameState.VICTORY:
            case GameState.GAMEOVER:

                if(GameManager.Instance.CurrentState == GameState.GAMEOVER)
                {
                    EatPacmanSfx();
                }
                backgroundMusicSource.Stop();
                backgroundMusicSource.clip = GameMusic;
                backgroundMusicSource.Play();
                break;

        }
    }

    void EatPelletSfx() {
        if(SoundBoxSource.clip != PacmanEatingSFX)
        {
            SoundBoxSource.Stop();
            SoundBoxSource.clip = PacmanEatingSFX;
        }
            
        if(SoundBoxSource.isPlaying)
        {
            return;
        }
        SoundBoxSource.Play();
    }

    void EatGhostSfx() {
        SoundBoxSource.Stop();
        SoundBoxSource.clip = GhostDeathSFX;
        SoundBoxSource.Play();
    }

    void EatPacmanSfx() {
        SoundBoxSource.Stop();
        SoundBoxSource.clip = PacmanDeathSFX;
        SoundBoxSource.Play();
    }
}
