using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� ���� �´� BGM�� ����ϱ� ���� ��ũ��Ʈ
//TitleScene�� BGMPlayer�� �ٿ��� ���
public class BGMController : MonoBehaviour
{
    public static BGMController Instance { get; private set; }
    private AudioSource BGMAudio = null;    //�� ������Ʈ�� AudioSource������Ʈ

    [SerializeField]
    private AudioClip titleBGM = null;      //Ÿ��Ʋ���� ����Ǵ� �����
    [SerializeField]
    private AudioClip lobbyBGM = null;      //�κ񿡼� ����Ǵ� �����
    [SerializeField]
    private AudioClip battleBGM = null;     //���������� ����Ǵ� �����
    [SerializeField]
    private AudioClip rankLobbyBGM = null;  //��ŷ�� �κ񿡼� ����Ǵ� �����

    private readonly float defaultVolume = 0.2f; //BGM�� ����� ���� ũ��
    private readonly float minVolume = 0.0f;     //BGM��ü�� �ʿ��� ���� ���� ũ��
    private readonly float vChangeTime = 1.2f;   //������ ������ �ð�(1.0f�� �� 1��, 2.0f�� �� 0.5��..)

    private bool isChanging = false;             //���� BGM ��ü �ڷ�ƾ�� ���������� �˱����� ����
    private AudioClip prevReqClip = null;        //������ �����û���� �����Ŭ���� ������ ����(����, �ߺ�����)

    private void Awake()
    {
        Instance = this;
        BGMAudio = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);        //���� �Ѿ�� ������ �ʵ��� ����
    }   

    public void BGMChange(BGMList ReqBGM)       //��û���� ����������� �����ϴ� �Լ�
    {
        AudioClip a_ReqAudioClip = null;        //���� ��� ��û���� �����Ŭ���� ������ ����

        switch (ReqBGM)
        {
            case BGMList.Title:
                a_ReqAudioClip = titleBGM;
                break;

            case BGMList.Lobby:
                a_ReqAudioClip = lobbyBGM;
                break;

            case BGMList.Battle:
                a_ReqAudioClip = battleBGM;
                break;

            case BGMList.RankLobby:
                a_ReqAudioClip = rankLobbyBGM;
                break;

            case BGMList.Stop:
                break;
        }

        if (prevReqClip == a_ReqAudioClip)       //������ ���� BGM�� ��û�޾Ҵٸ� ���
            return;

        if (isChanging)                          //������ �����ϴ� BGM ��ü �ڷ�ƾ�� �ִٸ� ���
            StopAllCoroutines();

        StartCoroutine(ChangeAction(a_ReqAudioClip));   //BGM ��ü �ڷ�ƾ ����
    }

    private IEnumerator ChangeAction(AudioClip ReqClip) //BGM�� �ε巴�� ��ü�ϱ� ���� �ڷ�ƾ
    {
        prevReqClip = ReqClip;      //������ ��û�� �����Ŭ���� ����

        isChanging = true;

        while (BGMAudio.volume > minVolume)     //���� �ּҰ����� ������ ����
        {
            BGMAudio.volume -= Time.deltaTime * vChangeTime * defaultVolume;
            yield return Time.deltaTime;
        }

        BGMAudio.clip = ReqClip;  //����� Ŭ�� ����

        if (ReqClip != null)
            BGMAudio.Play();      //BGM ���
        else
            BGMAudio.Stop();      //BGM ����

        while (BGMAudio.volume < defaultVolume) //���� �ִ밪���� ������ ����
        {
            BGMAudio.volume += Time.deltaTime * vChangeTime * defaultVolume;
            yield return Time.deltaTime;
        }

        isChanging = false;
    }
}
