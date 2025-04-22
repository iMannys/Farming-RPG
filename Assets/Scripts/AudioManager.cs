using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip Music ----------")]
    public AudioClip overture;

    public AudioClip spring1;
    public AudioClip spring2;
    public AudioClip spring3;

    public AudioClip summer1;
    public AudioClip summer2;
    public AudioClip summer3;

    public AudioClip fall1;
    public AudioClip fall2;
    public AudioClip fall3;

    public AudioClip winter1;
    public AudioClip winter2;
    public AudioClip winter3;

    [Header("---------- Audio Clip SFX ----------")]
    public AudioClip digging;
    public AudioClip watering;
    public AudioClip axe;
    public AudioClip stoneCrack;
    public AudioClip newRecord;
    public AudioClip questComplete;

    [Header("---------- Time Reference ----------")]
    private GameTimestamp currentTimestamp => TimeManager.Instance.GetGameTimestamp();

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional: Keep music through scene loads
    }

    private AudioClip[] currentSeasonTracks;
    private int lastPlayedIndex = -1;
    private bool isSleeping = false;

    private void Start()
    {
        StartCoroutine(InitSeasonMusic());
    }

    private IEnumerator InitSeasonMusic()
    {
        // Wait a frame to let TimeManager initialize
        yield return null;

        UpdateSeasonTracks();
        PlayNextSeasonTrack();
    }

    private void Update()
    {
        if (isSleeping) return;

        /*if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
        {
            UpdateSeasonTracks();
            PlayNextSeasonTrack();
        }*/

        if (!musicSource.isPlaying)
        {
            UpdateSeasonTracks();
            PlayNextSeasonTrack();
        }
    }

    private void UpdateSeasonTracks()
    {
        Debug.Log("Current season: " + currentTimestamp.season);
        switch (currentTimestamp.season)
        {
            case GameTimestamp.Season.Spring:
                currentSeasonTracks = new AudioClip[] { spring1, spring2, spring3 };
                break;
            case GameTimestamp.Season.Summer:
                currentSeasonTracks = new AudioClip[] { summer1, summer2, summer3 };
                break;
            case GameTimestamp.Season.Fall:
                currentSeasonTracks = new AudioClip[] { fall1, fall2, fall3 };
                break;
            case GameTimestamp.Season.Winter:
                currentSeasonTracks = new AudioClip[] { winter1, winter2, winter3 };
                break;
        }
    }

    private void PlayNextSeasonTrack()
    {
        if (currentSeasonTracks == null || currentSeasonTracks.Length == 0) return;

        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, currentSeasonTracks.Length);
        } while (nextIndex == lastPlayedIndex && currentSeasonTracks.Length > 1);

        lastPlayedIndex = nextIndex;
        musicSource.clip = currentSeasonTracks[nextIndex];
        musicSource.Play();
    }

    public void SleepTransition()
    {
        StartCoroutine(HandleSleepTransition());
    }

    private IEnumerator HandleSleepTransition()
    {
        isSleeping = true;

        // Actually perform the sleep (advance day, change season, etc.)
        GameStateManager.Instance.Sleep();

        // Fade out current music
        float duration = 1f;
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;

        // Wait a short moment to allow new time state to apply
        yield return new WaitForSeconds(0.5f);

        // Update and play new season track
        UpdateSeasonTracks();
        PlayNextSeasonTrack();

        isSleeping = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
