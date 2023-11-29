using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;

//LobbyScene에서 필요한 동작들을 담당하는 스크립트
//LobbyScene의 LobbyMgr에 붙여서 사용
public class LobbyMgr : NetworkMgr, IButtonClick
{
    [SerializeField]
    private MonStorage monStore = null;             //이미지,오브젝트 저장소

    [Header("-------- 유저 정보 --------")]
    [SerializeField]
    private GameObject userPanelPrefab = null;     //유저정보판넬 프리팹
    [SerializeField]
    private Text userNickText = null;              //유저 닉네임 표시용 텍스트
    [SerializeField]
    private Image userCrtImage = null;             //유저 캐릭터 이미지
    [SerializeField]
    private Text userLevelText = null;             //유저 레벨 표시용 텍스트
    [SerializeField]
    private Text userGoldText = null;              //유저가 현재 보유한 골드 표시용 텍스트

    [Header("-------- 자동 보상 --------")]
    [SerializeField]
    private Text autoTText = null;                  //자동 보상의 시간 표시용 텍스트
    [SerializeField]
    private Text rewardText = null;                 //자동 보상으로 획득할 수 있는 골드 표시용 텍스트
    [SerializeField]
    private Image autoExpImage = null;              //자동 보상버튼 이미지
    [SerializeField]
    private Sprite[] goldSprite = new Sprite[4];    //자동 보상에 표시해줄 골드 이미지
    [SerializeField]
    private AudioSource goldAudio = null;           //자동 보상 관련 효과음 재생을 위한 AudioSource

    private TimeSpan autoTSpan;                     //자동 보상의 계산된 시간을 저장할 변수
    private int rewardGold = 0;                     //자동 보상으로 획득할 수 있는 골드

    [Header("-------- 게임 종료 --------")]
    [SerializeField]
    private GameObject confirmBoxPrefab = null;      //종료 확인 판넬 프리팹

    private void Start()
    {
        BGMController.Instance.BGMChange(BGMList.Lobby);            //BGM 변경
        userNickText.text = PlayerInfo.UserNick;                    //유저의 닉네임 표시
        userCrtImage.sprite = monStore.characterImg[PlayerInfo.UserCrtNum];   //유저의 캐릭터 이미지 표시
        userLevelText.text = "레벨 " + PlayerInfo.UserLevel.ToString();       //유저의 레벨 표시
        userGoldText.text = PlayerInfo.UserGold.ToString();         //유저의 골드정보 표시
        StartCoroutine(AutoExpCheck());                             //자동보상 골드표시 코루틴 실행

        //보유한 몬스터를 성급이 높은 순서대로 정렬 후 MonSterName 순서대로 정렬
        if (PlayerInfo.MonList.Count != 0)
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //AutoExpCheck();             //자동 보상 표시
        base.Update();              //상속받은 NetworkMgr의 Update() 실행
    }

    #region -------------------------- 버튼 클릭 함수 --------------------------    
    public void ButtonOnClick(Button PushBtn)        //로비씬에있는 버튼 클릭 시 동작(버튼의 OnClick에서 실행)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {            
            case ButtonList.UserInfoButton:     //유저정보 버튼
                Instantiate(userPanelPrefab, PushBtn.transform.parent);     //유저 정보판넬 생성
                break;

            case ButtonList.CombatButton:       //전투 버튼
                SceneManager.LoadScene(SceneList.StageScene.ToString());
                break;

            case ButtonList.ShopButton:         //상점 버튼
                SceneManager.LoadScene(SceneList.ShopScene.ToString());
                break;

            case ButtonList.MyRoomButton:       //마이룸 버튼
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());
                break;

            case ButtonList.RankLobbyButton:      //랭크로비 버튼
                SceneManager.LoadScene(SceneList.RankLobbyScene.ToString());
                break;

            case ButtonList.ExitButton:         //게임 종료 버튼
                Instantiate(confirmBoxPrefab, PushBtn.transform.parent.parent);    //ConfirmBox 생성
                break;

            case ButtonList.AutoExpButton:      //보상 받기 버튼
                PlayerInfo.UserGold += rewardGold;          //보상 제공
                userGoldText.text = PlayerInfo.UserGold.ToString();     //유저 골드 표시
                autoTSpan = TimeSpan.Zero;                  //계산시간 초기화
                PlayerInfo.AutoExpTime = DateTime.Now;      //기준 시간 변경                
                PushPacket(PacketType.UserGold);            //골드 저장요청
                PushPacket(PacketType.AutoTime);            //자동 탐색 시간 저장요청
                break;

            case ButtonList.MultiButton:     //멀티 버튼
                SceneManager.LoadScene(SceneList.MultiLobbyScene.ToString());         
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }
    #endregion -------------------------- 버튼 클릭 함수 --------------------------

    #region -------------------------- 자동 보상 체크 함수 --------------------------
    private IEnumerator AutoExpCheck()    //자동 보상 골드 표시 함수
    {
        while(true)
        {
            autoTSpan = DateTime.Now - PlayerInfo.AutoExpTime;  //자동보상 시간계산
            int a_Hours;        //자동보상의 시간을 담을 변수
            int a_Minutes;      //자동보상의 분을 담을 변수
            int a_Seconds;      //자동보상의 초를 담을 변수

            if (autoTSpan.Days < 1)        //자동보상 시간이 24시간 미만이라면
            {
                a_Hours = autoTSpan.Hours;
                a_Minutes = autoTSpan.Minutes;
                a_Seconds = autoTSpan.Seconds;
            }
            else //(autoTSpan.Days >= 1)   //자동보상 시간이 24이간 이상이라면
            {
                a_Hours = 24;              //최대 24시간 적용
                a_Minutes = 0;
                a_Seconds = 0;
            }

            autoExpImage.sprite = goldSprite[a_Hours / 8];    //자동보상 시간에 따른 골드 이미지 변경(골드 이미지는 8시간 단위로 바뀌므로 몫을 이용해 이미지 번호 설정)
            autoTText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", a_Hours, a_Minutes, a_Seconds);          //자동보상 시간표시
            rewardGold = ((a_Hours * 360) + (a_Minutes * 6) + (a_Seconds / 10)) * PlayerInfo.CombatStage;   //자동 보상으로 획득할 수 있는 골드 계산(10초마다 골드 추가)
            rewardText.text = rewardGold.ToString();          //자동 보상으로 획득할 수 있는 골드 표시

            if (a_Hours != 24 && (a_Seconds % 10) == 0)     //골드가 추가되는시간(10초)마다 효과음 재생
                goldAudio.Play();

            yield return new WaitForSeconds((1000 - (float)autoTSpan.Milliseconds) / 1000);     //갱신 시간동안 대기
        }
    }
    #endregion -------------------------- 자동 보상 체크 함수 --------------------------

    private void OnDestroy()
    {
        StopCoroutine(AutoExpCheck());      //자동 보상골드 표시 코루틴 종료
    }

}
