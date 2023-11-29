using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

//ResultScene에서 표시 할 것들을 설정하는 스크립트 (싱글과 멀티에서 같이 사용)
//ResultScene과 MtResultScene의 Mgr에 붙여서 사용
public class ResultMgr : NetworkMgr, IButtonClick, ILoadScene
{
    private enum ResultText
    {
        Win,
        Defeat
    }
    [SerializeField]
    private Image topTextImg = null;    //판넬 상단에 표시할 결과이미지를 위한 이미지 컴포넌트
    [SerializeField]
    private Sprite[] textImgs = new Sprite[2];    //결과 이미지 스프라이트들
    [SerializeField]
    private GameObject loadingPanel = null;     //로딩 판넬 프리팹

    #region --------- 싱글전용 변수 ---------
    [Header("--------- 싱글전용, 멀티의 경우 비워두기 ---------")]
    [SerializeField]
    private MonStorage monStore = null;     //이미지, 오브젝트 저장소
    [SerializeField]
    private GameObject retryTab = null;     //재도전 버튼
    [SerializeField]
    private Image mvpMonImg = null;         //MVP 몬스터의 이미지를 넣을 이미지 컴포넌트
    #endregion --------- 싱글전용 변수 ---------

    #region --------- 멀티전용 변수 ---------
    [Header("--------- 멀티전용, 싱글의 경우 비워두기 ---------")]
    [SerializeField]
    private Text nameText1 = null;          //좌측(마스터)의 이름을 표시할 텍스트
    [SerializeField]
    private Text nameText2 = null;          //우측(참가자)의 이름을 표시할 텍스트
    #endregion --------- 멀티전용 변수 ---------

    // Start is called before the first frame update
    void Start()
    {
        BGMController.Instance.BGMChange(BGMList.Stop);     //재생중인 배경음악 종료

        if (retryTab != null)        //싱글 결과일 때
        {
            if (ResultData.CombatResult.Equals(Result.Defeat) && ResultData.PrevScene.Equals(SceneList.InGameScene))   //전투에서 패배했을 때(InGame에서 오는 결과만)
                retryTab.SetActive(true);     //재도전 버튼 켜기
            else
                retryTab.SetActive(false);    //재도전 버튼 끄기
        }

        if (mvpMonImg != null)       //싱글 결과일 때
            if(FindClass.GetCMCListFunc(Team.Ally.ToString()).Count != 0)
                mvpMonImg.sprite = monStore.monstersImg[(int)FindClass.GetCMCListFunc(Team.Ally.ToString())[0].CMCmonStat.monName];      //MVP(적에게 가장 많은 대미지를 입힌 몬스터)의 이미지 가져오기
            else
                mvpMonImg.sprite = null;

        switch (ResultData.CombatResult)     //승패 여부에 따라 상단 이미지 변경
        {
            case Result.Victory:
                topTextImg.sprite = textImgs[(int)ResultText.Win];
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ResultVictory);       //승리 효과음 재생
                break;

            case Result.Defeat:
                topTextImg.sprite = textImgs[(int)ResultText.Defeat];
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ResultDefeat);        //패배 효과음 재생
                break;
        }

        switch(ResultData.PrevScene)
        {
            case SceneList.InGameScene:
                if (ResultData.CombatResult.Equals(Result.Victory))     //승리 시
                {
                    if(PlayerInfo.CombatStage < 5)                      //최대 스테이지가 아니라면
                        PlayerInfo.CombatStage++;                       //유저의 전투 스테이지 단계 + 1

                    PushPacket(PacketType.CombatStage);                 //전투 스테이지 저장요청
                }
                break;

            case SceneList.RankGameScene:
                if (ResultData.CombatResult.Equals(Result.Victory))     //승리 시
                    PushPacket(PacketType.Ranking);                     //바뀐 랭킹 저장 요청
                break;

            case SceneList.MultiGameScene:
                foreach (Player p in PhotonNetwork.PlayerList)
                    if (p.NickName.Equals(PhotonNetwork.MasterClient.NickName))   //마스터의 닉네임이라면 좌측 텍스트에 넣기
                        nameText1.text = p.NickName;
                    else                                                          //방 참가자의 닉네임이라면 우측 텍스트에 넣기
                        nameText2.text = p.NickName;
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //상속받은 NetworkMgr의 Update()실행
    }

    public void ButtonOnClick(Button PushBtn)       //버튼클릭 시 동작하는 함수
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.ResultPanel:      //결과 판넬 클릭 시
                switch(ResultData.PrevScene)
                {
                    case SceneList.InGameScene:     
                        LoadScene(SceneList.StageScene);        //스테이지 씬으로 이동 
                        break;

                    case SceneList.RankGameScene:
                        LoadScene(SceneList.RankLobbyScene);    //랭크로비 씬으로 이동
                        break;

                    case SceneList.MultiGameScene:
                        LoadScene(SceneList.MultiLobbyScene);   //멀티로비 씬으로 이동
                        PhotonNetwork.LeaveRoom();              //방 나가기
                        break;
                }
                break;

            case ButtonList.RetryButton:            //재도전 버튼 클릭 시(인게임을 통해 온 결과씬만 있음)
                LoadScene(SceneList.InGameScene);   //인게임 씬으로 이동
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    #region --------------------- 씬 불러오기 함수 ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- 씬 불러오기 함수 ---------------------
}
