using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    public AudioSource footsetp1;
    public AudioSource chop1;
    public AudioSource enemy1;
    public AudioSource down;
    public AudioSource soda1;
    public AudioSource fruit1;
    public AudioSource fruit2;

    [Header("Música")]
    public AudioSource musicMenu;
    public AudioSource musicGame;

    [Range(0f, 1f)] public float soundVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyVolumes();
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = Mathf.Clamp01(value);

        if (footsetp1) footsetp1.volume = soundVolume;
        if (chop1)     chop1.volume     = soundVolume;
        if (enemy1)    enemy1.volume    = soundVolume;
        if (down)      down.volume      = soundVolume;
        if (soda1)     soda1.volume     = soundVolume;
        if (fruit1)    fruit1.volume    = soundVolume;
        if (fruit2)    fruit2.volume    = soundVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);

        if (musicMenu) musicMenu.volume = musicVolume;
        if (musicGame) musicGame.volume = musicVolume;
    }

    public void ApplyVolumes()
    {
        SetSoundVolume(soundVolume);
        SetMusicVolume(musicVolume);
    }

    // ---- SFX ----
    public void PlayFootstep() => footsetp1?.Play();
    public void PlayChop()     => chop1?.Play();
    public void PlayEnemy()    => enemy1?.Play();
    public void PlayDown()     => down?.Play();
    public void PlaySoda()     => soda1?.Play();
    public void PlayFruit()    => fruit1?.Play();
    public void PlayBroke()    => fruit2?.Play();

    // ---- MÚSICA ----
    public void PlayMenuMusic()
    {
        Debug.Log("[AUDIO] PlayMenuMusic");

        if (musicGame != null && musicGame.isPlaying)
            musicGame.Stop();

        if (musicMenu != null)
        {
            musicMenu.loop = true;
            if (!musicMenu.isPlaying)
                musicMenu.Play();
        }
    }

    public void PlayGameMusic()
    {
        Debug.Log("[AUDIO] PlayGameMusic");

        if (musicMenu != null && musicMenu.isPlaying)
            musicMenu.Stop();

        if (musicGame != null)
        {
            musicGame.loop = true;
            if (!musicGame.isPlaying)
                musicGame.Play();
        }
    }

    public void StopAllMusic()
    {
        Debug.Log("[AUDIO] StopAllMusic");

        if (musicMenu != null) musicMenu.Stop();
        if (musicGame != null) musicGame.Stop();
    }
}
