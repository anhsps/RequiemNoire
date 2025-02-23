using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager10 : Singleton<SoundManager10>
{
    public bool soundEnabled { get; private set; }

    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = gameObject.AddComponent<AudioSource>();
        soundEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        if (audioClips.Length > 0) PlaySound(0);

        UpdateSound();
        SetUpButtons();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        if (soundEnabled) PlaySound(0);
        PlayerPrefs.SetInt("SoundEnabled", soundEnabled ? 1 : 0);
        UpdateSound();
    }

    public void UpdateSound()
    {
        audioSource.mute = !soundEnabled;

        Button[] btns = FindObjectsOfType<Button>(true);// true: tim ca button bi tat + bat, false: tim button bat
        foreach (Button btn in btns)
        {
            if (btn.name == "MusicButton") btn.gameObject.SetActive(soundEnabled);
            if (btn.name == "MuteButton") btn.gameObject.SetActive(!soundEnabled);
        }
    }

    private void SetUpButtons()
    {
        Button[] btns = FindObjectsOfType<Button>(true);
        foreach (Button btn in btns)
        {
            if (btn.name == "MusicButton" || btn.name == "MuteButton")
                btn.onClick.AddListener(ToggleSound);
        }
    }

    // index=0 (bg), =1 (click), =2 (win), =3 (lose),...
    public void PlaySound(int index)
    {
        if (!soundEnabled || index < 0 || index >= audioClips.Length || !audioClips[index]) return;

        if (index == 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.loop = true;
            audioSource.Play();
        }
        else audioSource.PlayOneShot(audioClips[index]);
    }

    public void SoundClick() => PlaySound(1);
}
