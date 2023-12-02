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
        AudioClip a_ReqAudioClip = null;    //����� �����Ŭ���� ���� ����

        switch (ReqEffSound)
        {
            case EffSoundList.ButtonClick:
                a_ReqAudioClip = buttonClickClip;
                break;

            case EffSoundList.ResultVictory:
                a_ReqAudioClip = resultVClip;
                break;

            case EffSoundList.ResultDefeat:
                a_ReqAudioClip = resultDClip;
                break;

            case EffSoundList.MonCreate:
                a_ReqAudioClip = monCreateClip;
                break;

            case EffSoundList.Fight:
                a_ReqAudioClip = fightClip;
                break;

            case EffSoundList.fail:
                a_ReqAudioClip = failClip;
                break;

            case EffSoundList.UltiReady:
                a_ReqAudioClip = ultiReadyClip;
                break;

            case EffSoundList.Shop:
                a_ReqAudioClip = shopClip;
                break;

            case EffSoundList.Evolve:
                a_ReqAudioClip = evolveClip;
                break;
        }

        if (a_ReqAudioClip == null)     //����� �����Ŭ���� ������ ���
            return;

        effAudio.clip = a_ReqAudioClip; //�����Ŭ�� ����
        effAudio.Play();                //ȿ���� ���
    }
}
