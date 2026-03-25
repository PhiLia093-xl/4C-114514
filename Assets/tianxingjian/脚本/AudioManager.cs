using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource audioSource;
    public AudioClip bgmClip;

    [Header("Volume")]
    public Slider volumeSlider;
    [Range(0f, 1f)] public float defaultVolume = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = defaultVolume;
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.RemoveListener(AdjustVolume);
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }

        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmClip == null)
        {
            return;
        }

        if (audioSource.clip != bgmClip)
            audioSource.clip = bgmClip;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void AdjustVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
}