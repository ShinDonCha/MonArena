using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ȿ���� ����� ���� ��ũ��Ʈ
//TitleScene�� EffSoundPlayer�� �ٿ��� ���
public class EffSoundCtrl : MonoBehaviour
{
    public static EffSoundCtrl Instance { get; private set; }
    private AudioSource effAudio = null;    //�� ������Ʈ�� AudioSource������Ʈ

    [SerializeField]
    private AudioClip buttonClickClip = null;       //��ư Ŭ�� �� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip resultVClip = null;           //�¸� �� ��������� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip resultDClip = null;           //�й� �� ��������� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip monCreateClip = null;         //���� ������Ʈ ���� ȿ����
    [SerializeField]
    private AudioClip fightClip = null;             //�������� �� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip failClip = null;              //� ������ �������� �� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip ultiReadyClip = null;         //�ñر� ���� �� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip shopClip = null;              //���� ��ȯ �� ����Ǵ� ȿ����
    [SerializeField]
    private AudioClip evolveClip = null;            //���� ��ȭ �� ����Ǵ� ȿ����

    private readonly float defaultVolume = 0.15f;   //ȿ������ ����� �⺻ ���� ũ��
    
    private void Awake()
    {
        Instance = this;
        effAudio = GetComponent<AudioSource>();
        effAudio.volume = defaultVolume;            //�⺻ �������� ����
        DontDestroyOnLoad(this);                    //���� �Ѿ�� ������ �ʵ��� ����
    }

    public void EffSoundPlay(EffSoundList ReqEffSound)  //ȿ���� ��� �Լ�(ȿ���� ����� �ʿ��Ѱ����� �����û)
    {
        effAudio.clip = ReqEffSound switch  //��û���� ȿ������ �°� �����Ŭ�� ����
        {
            EffSoundList.ButtonClick => buttonClickClip,
            EffSoundList.ResultVictory => resultVClip,
            EffSoundList.ResultDefeat => resultDClip,
            EffSoundList.MonCreate => monCreateClip,
            EffSoundList.Fight => fightClip,
            EffSoundList.fail => failClip,
            EffSoundList.UltiReady => ultiReadyClip,
            EffSoundList.Shop => shopClip,
            EffSoundList.Evolve => evolveClip,
            _ => null
        };

        if (effAudio.clip != null)      //ȿ���� ���
            effAudio.Play();
    }
}
