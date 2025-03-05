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
    private AudioClip mainClip;

    [SerializeField]
    private AudioClip battleClip;
    [SerializeField]
    private AudioClip danceClip;

    [Header("SE音效")]
    [SerializeField]
    private AudioClip buttonTouchClip;

    [SerializeField]
    private AudioClip swordSlashClip_1;

    [SerializeField]
    private AudioClip swordSlashClip_2;

    [SerializeField]
    private AudioClip impact;
    [SerializeField]
    private AudioClip openDoorClip;
    [SerializeField]
    private AudioClip deathClueClip;
    [SerializeField]
    private List<AudioClip> heavyAttackClips;

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
    public void DanceAudio()
    {
        Instance.BGMSource.clip = Instance.danceClip;
        Instance.BGMSource.Play();
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

    public void ButtonTouchAudio()
    {
        Instance.SESource.clip = Instance.buttonTouchClip;
        Instance.SESource.Play();
    }

    public void HeavyAttackAudio(int id)
    {
        Instance.SESource.clip = Instance.heavyAttackClips[id];
        Instance.SESource.Play();
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

    public void MainAudio()
    {
        if (Instance.BGMSource.clip == Instance.mainClip)
            return;
        Instance.BGMSource.clip = Instance.mainClip;
        Instance.BGMSource.Play();
    }

    public void BattleAudio()
    {
        if (Instance.BGMSource.clip == Instance.battleClip)
            return;
        Instance.BGMSource.clip = Instance.battleClip;
        Instance.BGMSource.Play();
    }

    public void Impact()
    {
        Instance.SESource.clip = Instance.impact;
        Instance.SESource.Play();
    }

    public void SlashAudio(int count)
    {
        switch (count)
        {
            case 1:
                Instance.SESource.clip = Instance.swordSlashClip_1;
                break;
            case 2:
                Instance.SESource.clip = Instance.swordSlashClip_2;
                break;
        }
        Instance.SESource.Play();
    }
    public void OpenDoor()
    {
        Instance.SESource.clip = Instance.openDoorClip;
        Instance.SESource.Play();
    }
    public void DeathClue()
    {
        Instance.SESource.clip = Instance.deathClueClip;
        Instance.SESource.Play();
    }
    public void ChanageAudioVolume(string sourceName, float value)
    {
        audioMixer.SetFloat(sourceName, Mathf.Log10(value) * 20);
    }

}
