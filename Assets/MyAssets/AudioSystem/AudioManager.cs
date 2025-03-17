using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音效控制")]
    [SerializeField]
    private AudioMixer audioMixer;
    [Header("BGM音效")]
    [SerializeField]
    private List<AudioClip> bgmClipList = new List<AudioClip>();

    [Header("SE音效")]
    [SerializeField]
    private List<AudioClip> seClipList = new List<AudioClip>();

    [Header("Menu音效")]
    [SerializeField]
    private AudioClip menuEnterClip;

    [SerializeField]
    private AudioClip menuExitClip;

    [Header("Player音效")]
    [SerializeField]
    private AudioClip hurt;

    [SerializeField]
    private AudioClip death;
    [Header("跑酷音效")]
    [SerializeField]
    private AudioClip runningBreathingClip;
    [SerializeField]
    private AudioClip rainRunStepClip;
    public AudioSource MenuSource { get; private set; }
    public AudioSource SESource { get; private set; }
    public AudioSource PlayerSource { get; private set; }
    public AudioSource BGMSource { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    private void Initialize()
    {
        DontDestroyOnLoad(this);
        MenuSource = transform.GetChild(0).GetComponent<AudioSource>();
        SESource = transform.GetChild(1).GetComponent<AudioSource>();
        PlayerSource = transform.GetChild(2).GetComponent<AudioSource>();
        BGMSource = transform.GetChild(3).GetComponent<AudioSource>();
        Instance.BGMSource.loop = true;
    }
    public void BGMAudio(int index)
    {
        if (index < 0 || index >= bgmClipList.Count || BGMSource.clip == bgmClipList[index])
        {
            return;
        }
        BGMSource.Stop();
        BGMSource.clip = bgmClipList[index];
        BGMSource.Play();
    }
    public void SEAudio(int index)
    {
        if (index < 0 || index >= seClipList.Count || SESource.clip == seClipList[index])
        {
            return;
        }
        SESource.Stop();
        SESource.PlayOneShot(seClipList[index]);
    }
    public void ClearAllAudioClip()
    {
        MenuSource.clip = null;
        SESource.clip = null;
        PlayerSource.clip = null;
        BGMSource.clip = null;
    }
    public void ParkourAudio()
    {
        Instance.SESource.clip = Instance.rainRunStepClip;
        Instance.SESource.spread = 2;
        Instance.SESource.Play();
        Instance.PlayerSource.clip = Instance.runningBreathingClip;
        Instance.PlayerSource.loop = true;
        Instance.PlayerSource.Play();
    }
    public void MenuEnterAudio()
    {
        Instance.MenuSource.clip = Instance.menuEnterClip;
        Instance.MenuSource.Play();
    }

    public void MenuExitAudio()
    {
        Instance.MenuSource.clip = Instance.menuExitClip;
        Instance.MenuSource.Play();
    }



    public void PlayerHurted()
    {
        Instance.PlayerSource.clip = Instance.hurt;
        Instance.PlayerSource.loop = false;
        Instance.PlayerSource.Play();
    }

    public void PlayerDied()
    {
        Instance.PlayerSource.clip = Instance.death;
        Instance.PlayerSource.loop = false;
        Instance.PlayerSource.Play();
    }



    public void ChanageAudioVolume(string sourceName, float value)
    {
        audioMixer.SetFloat(sourceName, Mathf.Log10(value) * 20);
    }

}
