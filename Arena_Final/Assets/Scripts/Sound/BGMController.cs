using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//각 씬에 맞는 BGM을 재생하기 위한 스크립트
//TitleScene의 BGMPlayer에 붙여서 사용
public class BGMController : MonoBehaviour
{
    public static BGMController Instance { get; private set; }
    private AudioSource BGMAudio = null;    //이 오브젝트의 AudioSource컴포넌트

    [SerializeField]
    private AudioClip titleBGM = null;      //타이틀에서 재생되는 배경음
    [SerializeField]
    private AudioClip lobbyBGM = null;      //로비에서 재생되는 배경음
    [SerializeField]
    private AudioClip battleBGM = null;     //전투씬에서 재생되는 배경음
    [SerializeField]
    private AudioClip rankLobbyBGM = null;  //랭킹전 로비에서 재생되는 배경음

    private readonly float defaultVolume = 0.2f; //BGM을 재생할 볼륨 크기
    private readonly float minVolume = 0.0f;     //BGM교체시 필요한 최저 볼륨 크기
    private readonly float vChangeTime = 1.2f;   //볼륨을 조절할 시간(1.0f일 때 1초, 2.0f일 때 0.5초..)

    private bool isChanging = false;             //현재 BGM 교체 코루틴을 진행중인지 알기위한 변수
    private AudioClip prevReqClip = null;        //이전에 재생요청받은 오디오클립을 저장할 변수(오류, 중복방지)

    private void Awake()
    {
        Instance = this;
        BGMAudio = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);        //씬이 넘어가도 꺼지지 않도록 변경
    }   

    public void BGMChange(BGMList ReqBGM)       //요청받은 배경음악으로 변경하는 함수
    {
        AudioClip a_ReqAudioClip = null;        //지금 재생 요청받은 오디오클립을 저장할 변수

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

        if (prevReqClip == a_ReqAudioClip)       //이전과 같은 BGM을 요청받았다면 취소
            return;

        if (isChanging)                          //기존에 진행하던 BGM 교체 코루틴이 있다면 취소
            StopAllCoroutines();

        StartCoroutine(ChangeAction(a_ReqAudioClip));   //BGM 교체 코루틴 실행
    }

    private IEnumerator ChangeAction(AudioClip ReqClip) //BGM을 부드럽게 교체하기 위한 코루틴
    {
        prevReqClip = ReqClip;      //이전에 요청한 오디오클립에 저장

        isChanging = true;

        while (BGMAudio.volume > minVolume)     //볼륨 최소값까지 서서히 감소
        {
            BGMAudio.volume -= Time.deltaTime * vChangeTime * defaultVolume;
            yield return Time.deltaTime;
        }

        BGMAudio.clip = ReqClip;  //오디오 클립 변경

        if (ReqClip != null)
            BGMAudio.Play();      //BGM 재생
        else
            BGMAudio.Stop();      //BGM 중지

        while (BGMAudio.volume < defaultVolume) //볼륨 최대값까지 서서히 증가
        {
            BGMAudio.volume += Time.deltaTime * vChangeTime * defaultVolume;
            yield return Time.deltaTime;
        }

        isChanging = false;
    }
}
