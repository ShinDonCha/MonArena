using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//GameScene에서 필요한 세팅이나 저장등을 하는 스크립트
//InGameScene의 InGameMgr와 RankGameScene의 RankGameMgr에 붙여서 사용
public class GameMgr : NetworkMgr, IButtonClick, ICMCList
{
    //------------- UICanvas의 UI탭들(InGameScene과 RankGameScene에서만 사용)
    [Header("-------- UICanvas(InGame, RankGame) --------")]
    [SerializeField]
    private GameObject standByUI = null;      //대기상태 중 표시해 줄 UI들이 모인 곳, UICanvas의 StandByUI 연결
    [SerializeField]
    private GameObject fightUI = null;        //전투 시작 시 애니메이션을 띄워주기 위한 UI
    [SerializeField]
    private GameObject combatUI = null;       //전투상태 중 표시해 줄 UI들이 모인곳, UICanvas의 CombatUI 연결
    //------------- UICanvas의 UI탭들(InGameScene과 RankGameScene에서만 사용)
    
    [SerializeField]
    private GameObject loadingPanel = null;   //로딩 판넬 프리팹

    public static GameMgr Instance;           //싱글턴
    private List<CmnMonCtrl> myCMCList = new();      //현재 전투중인 몬스터들의 상태를 파악하기 위한 CmnMonCtrl, 게임 시작 시 몬스터가 목록에 추가된다.
    private List<CmnMonCtrl> enemyCMCList = new();   //현재 전투중인 적 몬스터들의 상태를 파악하기 위한 CmnMonCtrl, 게임 시작 시 몬스터가 목록에 추가된다.
    private int numofPMon = 0;           //유저의 몬스터 숫자 (전투 시작 시 아군 진열에 배치된 몬스터 수만큼 숫자가 정해지고 0이되면 전투 종료)
    private int numofEMon = 0;           //적의 몬스터 숫자 (전투 시작 시 적 진열에 배치된 몬스터 수만큼 숫자가 정해지고 0이되면 전투 종료)

    private SceneList curSceneName;     //현재 씬의 이름을 담을 변수

    private void Awake()
    {
        Instance = this;

        if (standByUI != null)
            standByUI.SetActive(true);      //준비중 UI 켜기

        if (fightUI != null)
            fightUI.SetActive(false);       //전투 시작 UI 끄기

        if (combatUI != null)
            combatUI.SetActive(false);      //전투중 UI 끄기

        if (Enum.TryParse(SceneManager.GetActiveScene().name, out curSceneName))  //현재 씬의 이름을 SceneList 형식으로 변경
            ResultData.PrevScene = curSceneName;           //현재 씬의 이름 저장

        FindClass.GetCMCListFunc = CMCListFuncSet;         //팀에 맞는 CmnMonCtrl 리스트를 찾기위한 함수를 추가

        BGMController.Instance.BGMChange(BGMList.Battle);  //BGM 변경
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //상속받은 NetworkMgr의 Update() 실행
    }

    #region -------------------- 인터페이스 --------------------
    public void ButtonOnClick(Button PushBtn)   //Hierarchy의 버튼 오브젝트들의 OnClick()을 통해 실행
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))       //버튼 오브젝트의 이름을 enum형으로 변경
            return;

        switch (BtnL)
        {            
            case ButtonList.BackButton:     //돌아가기 버튼
                switch(curSceneName)
                {
                    case SceneList.InGameScene:
                        LoadScene(SceneList.StageScene);        //스테이지 씬으로 이동
                        break;

                    case SceneList.RankGameScene:
                        LoadScene(SceneList.RankLobbyScene);    //랭크 로비 씬으로 이동
                        break;
                }
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);   //버튼 클릭 효과음 재생
                break;

            case ButtonList.StartButton:     //전투시작 버튼
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Fight);        //전투시작 효과음 재생
                StartCoroutine(StartSettings());
                break;
        }        
    }
    
    public void LoadScene(SceneList NextScene)  //씬 불러오기 함수
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    public List<CmnMonCtrl> CMCListFuncSet(string TagStr)   //외부에서 FindClass.GetCMCListFunc를 통해 CMCList를 가져가는 함수
    {
        if (!Enum.TryParse(TagStr, out Team TeamTag))
            return null;

        switch (TeamTag)
        {
            case Team.Ally:
                return myCMCList;

            case Team.Enemy:
                return enemyCMCList;

            default:
                return null;
        }
    }
    #endregion -------------------- 인터페이스 --------------------

    #region ------------------- 전투 시작시 실행 함수 -------------------
    private IEnumerator StartSettings()     //전투시작 시 실행해야할 것들을 모아놓은 함수(배치저장, UI변경)
    {
        //--------------------------- 덱 저장 ---------------------------
        List<MonsterName> a_NameDeck = new();
        List<int> a_StarDeck = new();
        PacketType a_PType = PacketType.CombatDeck;

        switch (curSceneName)
        {
            case SceneList.InGameScene:
                a_NameDeck = PlayerInfo.CombatDeck;
                a_StarDeck = PlayerInfo.CombatStarF;
                a_PType = PacketType.CombatDeck;
                break;

            case SceneList.RankGameScene:
                a_NameDeck = PlayerInfo.RankDeck;
                a_StarDeck = PlayerInfo.RankStarF;
                a_PType = PacketType.RankDeck;
                break;
        }

        a_NameDeck.Clear();    //덱 리스트 전부 삭제 (다시 저장하기 위해서 이전 목록 삭제)
        a_StarDeck.Clear();    //덱 성급 리스트 전부 삭제 (다시 저장하기 위해서 이전 목록 삭제)

        for (int i = 0; i < FindClass.CurSetPoint.transform.childCount; i++)
        {
            FindClass.CurSetPoint[i].GetChild(0).gameObject.SetActive(false);   //Point의 이미지를 나타내는 게임 오브젝트 끄기

            if (FindClass.CurSetPoint[i].childCount > 1)    //활성화된(몬스터가 존재하는) Point만 실행
            {
                a_NameDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.monName);    //해당 몬스터의 이름을 저장
                a_StarDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.starForce);  //해당 몬스터의 성급을 저장

                myCMCList.Add(FindClass.CurSetPoint[i].GetComponentInChildren<CmnMonCtrl>());   //배치된 몬스터들의 CommonMonCtrl을 리스트에 추가
            }
            else
            {
                a_NameDeck.Add(MonsterName.None);   //비어있음을 저장
                a_StarDeck.Add(-1);                 //비어있음을 저장
            }
        }
        PushPacket(a_PType);   //NetworkMgr에 몬스터 배치도 저장 요청
        //--------------------------- 덱 저장 ---------------------------

        standByUI.SetActive(false);     //전투 준비중 UI 끄기
        fightUI.SetActive(true);        //전투시작 UI 켜기

        numofPMon = myCMCList.Count;   //유저의 몬스터가 배치된 수만큼 숫자 세팅
        numofEMon = enemyCMCList.Count;  //적 몬스터가 배치된 수만큼 숫자 세팅

        yield return new WaitForSeconds(1.5f);      //애니메이션 재생시간만큼 대기

        fightUI.SetActive(false);       //전투시작 UI 끄기
        combatUI.SetActive(true);       //전투 UI 켜기

        //-------------- 유저와 적 중 한쪽이라도 몬스터를 하나도 배치하지 않고 시작했다면 즉시 종료 아니면 전투시작 --------------
        if (numofPMon == 0)
        {
            ResultData.CombatResult = Result.Defeat;   //패배
            StartCoroutine(EndAction());
        }
        else if (numofEMon == 0)
        {
            ResultData.CombatResult = Result.Victory;   //승리
            StartCoroutine(EndAction());
        }
        else
            GameManager.GMEventAct();   //이벤트 실행(전투시작), CmnMonCtrl의 StartFight이 들어가있음
        //-------------- 유저와 적 중 한쪽이라도 몬스터를 하나도 배치하지 않고 시작했다면 즉시 종료 아니면 전투시작 --------------
    }
    #endregion ------------------- 전투 시작시 실행 함수 -------------------

    #region ------------------ 전투 결과 계산, 전투 종료 액션 ------------------
    public void CalcResults (string TeamTag)    //몬스터 사망 시 해당 몬스터의 이름, 대미지, 피해량을 받아와 저장, 계산하는 함수
    {
        if (!Enum.TryParse(TeamTag, out Team TeamName))      //죽은 몬스터의 Tag를 Team enum형식으로 변경
            return;

        switch(TeamName)
        {
            case Team.Ally:         //아군일 경우
                numofPMon--;         //현재 남아있는 아군의 수 감소
                break;

            case Team.Enemy:        //적군일 경우
                numofEMon--;        //현재 남아있는 적군의 수 감소
                break;
        }

        if (numofPMon == 0 || numofEMon == 0)     //아군과 적군 중 어느쪽이든 다 죽으면 실행
        {
            if (numofPMon == 0)              //아군이 다 죽었으면
                ResultData.CombatResult = Result.Defeat;     //패배
            else if (numofEMon == 0)
                ResultData.CombatResult = Result.Victory;   //승리

            StartCoroutine(EndAction());      //ResultScene으로 넘어가는 코루틴 실행
        }
    }

    private IEnumerator EndAction()   //ResultScene으로 넘어갈 때 효과를 주기위한 코루틴
    {
        myCMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));      //총 대미지가 높은 순서대로 정렬

        while (Time.timeScale > 0.3f)   //일정 수치만큼 느려질 때까지 반복
        {
            Time.timeScale -= Time.deltaTime;       //속도 느려지도록 하기
            yield return null;
        }

        Time.timeScale = 1.0f;       //timeScale 정상화

        SceneManager.LoadScene(SceneList.ResultScene.ToString());      //ResultScene으로 넘어가기
    }
    #endregion ------------------ 전투 결과 계산, 전투 종료 액션 ------------------
}
