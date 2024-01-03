using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//효과음 재생을 위한 스크립트
//TitleScene의 EffSoundPlayer에 붙여서 사용
public class EffSoundCtrl : MonoBehaviour
{
    public static EffSoundCtrl Instance { get; private set; }
    private AudioSource effAudio = null;    //이 오브젝트의 AudioSource컴포넌트

    [SerializeField]
    private AudioClip buttonClickClip = null;       //버튼 클릭 시 재생되는 효과음
    [SerializeField]
    private AudioClip resultVClip = null;           //승리 시 결과씬에서 재생되는 효과음
    [SerializeField]
    private AudioClip resultDClip = null;           //패배 시 결과씬에서 재생되는 효과음
    [SerializeField]
    private AudioClip monCreateClip = null;         //몬스터 오브젝트 생성 효과음
    [SerializeField]
    private AudioClip fightClip = null;             //전투시작 시 재생되는 효과음
    [SerializeField]
    private AudioClip failClip = null;              //어떤 동작을 실패했을 때 재생되는 효과음
    [SerializeField]
    private AudioClip ultiReadyClip = null;         //궁극기 시전 시 재생되는 효과음
    [SerializeField]
    private AudioClip shopClip = null;              //몬스터 소환 시 재생되는 효과음
    [SerializeField]
    private AudioClip evolveClip = null;            //몬스터 진화 시 재생되는 효과음

    private readonly float defaultVolume = 0.15f;   //효과음을 재생할 기본 볼륨 크기
    
    private void Awake()
    {
        Instance = this;
        effAudio = GetComponent<AudioSource>();
        effAudio.volume = defaultVolume;            //기본 볼륨으로 설정
        DontDestroyOnLoad(this);                    //씬이 넘어가도 꺼지지 않도록 변경
    }

    public void EffSoundPlay(EffSoundList ReqEffSound)  //효과음 재생 함수(효과음 재생이 필요한곳에서 실행요청)
    {
        effAudio.clip = ReqEffSound switch  //요청받은 효과음에 맞게 오디오클립 변경
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

        if (effAudio.clip != null)      //효과음 재생
            effAudio.Play();
    }
}
