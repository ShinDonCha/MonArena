using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Pt = ExitGames.Client.Photon;

//멀티게임의 전체적인 세팅을 담당하는 스크립트
//MultiGameScene의 MutiGameMgr에 붙여서 사용
public class MtGameMgr : MonoBehaviourPunCallbacks, IButtonClick, ICMCList, ILoadScene
{
    private enum MsgType        //방 참가자에게 표시할 메시지의 타입
    {
        LogMsg,
        CountMsg,
    }

    public static MtGameMgr Instance;

    [SerializeField]
    private MonStorage monStore = null;         //몬스터 이미지,오브젝트 저장소
    private PhotonView photonV = null;          //PhotonView 컴포넌트를 담을 변수

    [Header("----------- 전투 관련 -----------")]
    [SerializeField]
    private GameObject standByUI = null;       //전투 준비 중 표시해 줄 UI들이 모인 곳 UICanvas의 StandByUI 연결
    [SerializeField]
    private GameObject fightUI = null;         //전투 시작 시 보여줄 Fight이미지가 들어있는 UI탭, UICanvas의 FightUI 연결
    [SerializeField]
    private GameObject combatUI = null;        //전투 중 표시해 줄 UI들이 모인 곳, UICanvas의 CombatUI 연결
    [SerializeField]
    private Text team1NickText = null;         //Team1의 닉네임(마스터의 닉네임)이 들어갈 텍스트
    [SerializeField]
    private Text team2NickText = null;         //Team2의 닉네임(참가자의 닉네임)이 들어갈 텍스트

    [SerializeField]
    private GameObject standBySetPoint = null;  //전투 준비 중 사용될 SetPoint    

    private List<CmnMonCtrl> team1CMCList = new();   //전투에 참여한 방 마스터의 몬스터들의 상태를 파악하기 위한 CmnMonCtrl 리스트, 게임 시작 시 배치되어있는 몬스터가 목록에 추가
    private List<CmnMonCtrl> team2CMCList = new();   //전투에 참여한 방 참가자의 몬스터들의 상태를 파악하기 위한 CmnMonCtrl 리스트, 게임 시작 시 배치되어있는 몬스터가 목록에 추가

    private int numofTeam1 = 0;     //Team1의 몬스터 숫자
    private int numofTeam2 = 0;     //Team2의 몬스터 숫자

    [Header("----------- 방 관련 -----------")]
    [SerializeField]
    private Text logText = null;            //방에 입장, 퇴장 시 로그를 표시해주기 위한 텍스트    
    [SerializeField]
    private Text eMonNickText = null;       //상대 닉네임을 표시하기 위한 텍스트
    [SerializeField]
    private Transform eMonListTr = null;    //상대 몬스터 이미지가 들어갈 Transform (상대의 대표 몬스터를 보여주는 용도)
    [SerializeField]
    private GameObject kickButton = null;   //강퇴버튼(마스터가 아니면 오프시켜주기)
    [SerializeField]
    private Text playerNumText = null;      //방의 참가인원을 보여주기 위한 텍스트
    [SerializeField]
    private GameObject countPanel = null;   //남은시간 표시용 판넬의 게임오브젝트
    [SerializeField]
    private Text countText = null;          //전투 시작까지 남은 시간을 보여주기 위한 텍스트
    private readonly int combatCount = 15;  //전투 시작까지 걸리는 총 시간
    private int curCount = 0;               //전투 시작까지 남은 시간

    [SerializeField]
    private GameObject loadingPanel = null; //로딩 판넬 프리팹(로비로 돌아갈 때 사용)

    private void Awake()
    {
        Instance = this;
        photonV = GetComponent<PhotonView>();

        if (standByUI != null)
            standByUI.SetActive(true);      //준비중 UI 켜기

        if (fightUI != null)
            fightUI.SetActive(false);       //전투 시작 UI 끄기

        if (combatUI != null)
            combatUI.SetActive(false);      //전투중 UI 끄기

        if(countPanel != null)
            countPanel.SetActive(false);    //카운트 판넬 끄기

        if (kickButton != null)
            kickButton.SetActive(false);    //강퇴 버튼 끄기

        ResultData.PrevScene = SceneList.MultiGameScene;    //현재 씬이 무슨 씬인지 전달
        FindClass.GetCMCListFunc = CMCListFuncSet;          //팀에 맞는 CmnMonCtrl 리스트를 찾기위한 함수를 추가
        BGMController.Instance.BGMChange(BGMList.Battle);   //배경음 변경
    }

    // Start is called before the first frame update
    void Start()
    {
        eMonNickText.text = "";

        #region --------------- 방에 입장했을 때 자신의 몬스터 정보 넘겨주기 (마스터와 클라이언트 모두 실행) ---------------
        //------------- 상대에게 내 몬스터 일부의 정보를 주기 위해 정보 전달
        PhotonNetwork.EnableCloseConnection = true;     //강퇴 동작을 위한 설정 변경

        int[] a_MNumList = new int[(int)MathF.Min(PlayerInfo.MonList.Count, 5)];          //나의 몬스터 정보를 담을 변수 (최대 5마리까지)
        int[] a_MStarList = new int[(int)MathF.Min(PlayerInfo.MonList.Count, 5)];         //나의 몬스터의 성급 정보를 담을 변수  (최대 5마리까지)

        for (int i = 0; i < a_MNumList.Length; i++)
        {
            a_MNumList[i] = (int)PlayerInfo.MonList[i].monName;       //몬스터 이름을 int형식으로 저장
            a_MStarList[i] = PlayerInfo.MonList[i].starForce;         //몬스터의 성급을 저장
        }

        Pt.Hashtable a_Hash = new()        //Hashtable로 저장
        {
            { "MonList", a_MNumList },
            { "StarList", a_MStarList }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(a_Hash);      //PhotonNetwork로 정보 설정
         //------------- 상대에게 내 몬스터 일부의 정보를 주기 위해 정보 전달
        #endregion --------------- 방에 입장했을 때 자신의 몬스터 정보 넘겨주기 (마스터와 클라이언트 모두 실행)---------------
    }

    #region -------------------- 인터페이스 --------------------
    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        switch (BtnL)
        {
            case ButtonList.BackButton:         //방나가기 버튼
                PhotonNetwork.LeaveRoom();
                break;

            case ButtonList.KickButton:         //강퇴 버튼
                foreach (Player FindP in PhotonNetwork.PlayerListOthers)  //나를 제외한 전체 플레이어 중에서 찾기
                    if (FindP.NickName.Contains(eMonNickText.text))       //강퇴버튼 클릭한 닉네임과 일치하는 플레이어에게 실행
                        PhotonNetwork.CloseConnection(FindP);             //해당유저 강퇴
                break;
        }
    }

    public void LoadScene(SceneList NextScene)      //씬 불러오기 함수
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    public List<CmnMonCtrl> CMCListFuncSet(string TagStr)       //받아온 태그에 맞는 CmnMonList 제공 함수
    {
        return Enum.Parse<Team>(TagStr) switch
        {
            Team.Team1 => team1CMCList,
            Team.Team2 => team2CMCList,
            _ => null
        };
    }
    #endregion -------------------- 인터페이스 --------------------

    #region ----------------- PunCallbacks, 방 관련 -----------------
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Pt.Hashtable changedProps)    //방에있는 유저의 정보가 변경되었을 경우 방에있는 모두에게 실행 (들어온사람 포함)
    {
        if (changedProps.ContainsKey("MonArrName"))     //StandBySetPoint가 OnDisable될 때 받아온 몬스터 배치 정보일 경우 취소
            return;
        else if (changedProps.ContainsKey("MonList"))   //처음 입장시 전달한 플레이어의 몬스터 정보일 경우
        {
            logText.text += "\n<color=#00ff00>[" + targetPlayer.NickName + "] 님이 방에 입장했습니다.</color>";  //방에 입장한 플레이어의 닉네임 표시

            //----------------- 다른 유저의 정보 가져오기(입장 시 자신의 정보를 CustomProperties로 저장) -----------------
            if (!targetPlayer.Equals(PhotonNetwork.LocalPlayer))      //나의 정보가 아니라면 실행(결과적으로 마스터만 실행)
            {
                kickButton.SetActive(true);             //강퇴버튼 온

                int[] a_MonList = (int[])changedProps["MonList"];   //Hashtable의 키값을 통해 상대의 몬스터 리스트(번호) 받아오기
                int[] a_StarList = (int[])changedProps["StarList"]; //Hashtable의 키값을 통해 상대 몬스터의 성급 받아오기

                eMonNickText.text = targetPlayer.NickName;      //상대 닉네임 표시

                for (int i = 0; i < a_MonList.Length; i++)
                {
                    GameObject a_GO = Instantiate(monStore.monSlot[a_StarList[i]], eMonListTr); //상대 몬스터 목록 생성 (최대 5개)
                    a_GO.name = a_MonList[i].ToString();         //이름 변경
                    a_GO.tag = MonSlotTag.NameParse.ToString();  //태그 변경
                }
            }

            if (!PhotonNetwork.IsMasterClient)   //클라이언트만 실행
            {
                //------------- 마스터의 몬스터 정보 받아와서 출력
                Pt.Hashtable a_MastHash = PhotonNetwork.MasterClient.CustomProperties;
                int[] a_MonList = (int[])a_MastHash["MonList"];   //Hashtable의 키값을 통해 상대의 몬스터 리스트(번호) 받아오기
                int[] a_StarList = (int[])a_MastHash["StarList"]; //Hashtable의 키값을 통해 상대 몬스터의 성급 받아오기

                eMonNickText.text = PhotonNetwork.MasterClient.NickName;      //마스터 닉네임 표시

                for (int i = 0; i < a_MonList.Length; i++)
                {
                    GameObject a_GO = Instantiate(monStore.monSlot[a_StarList[i]], eMonListTr); //상대 몬스터 목록 생성 (최대 5개)
                    a_GO.name = a_MonList[i].ToString();        //이름 변경
                    a_GO.tag = MonSlotTag.NameParse.ToString(); //태그 변경
                }
                //------------- 마스터의 몬스터 정보 받아와서 출력
            }
            //----------------- 다른 유저의 정보 가져오기(입장 시 자신의 정보를 CustomProperties로 저장) -----------------
        }

        playerNumText.text = "참가 인원\n" + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;      //방에 들어와있는 인원 표시
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)      //방 인원이 전부 찼을 경우
        {
            if (PhotonNetwork.IsMasterClient)               //마스터라면
                PhotonNetwork.CurrentRoom.IsOpen = false;   //방 입장못하도록 막기

            StartCoroutine(CountDown());    //전투시작시간 카운트 다운 코루틴 실행
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)   //PhotonNetwork.LeaveRoom(); 누군가 방을 떠날 때 남은 사람들만 실행(1:1 게임이므로 결과적으로 마스터만 실행)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)       //방의 인원이 빈다면
        {
            StopAllCoroutines();        //진행중인 모든 코루틴 종료
            playerNumText.text = "참가 인원\n" + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;      //방에 들어와있는 인원 표시
            countPanel.SetActive(false);                //카운트 판넬 끄기
            PhotonNetwork.CurrentRoom.IsOpen = true;    //방 열린상태로 변경

            //------------- 상대 정보 표시판 변경
            for (int i = 0; i < eMonListTr.childCount; i++)      //상대 몬스터 목록 삭제
                Destroy(eMonListTr.GetChild(i).gameObject);

            eMonNickText.text = "";                 //상대 닉네임 초기화

            if (PhotonNetwork.IsMasterClient)       //마스터인 경우
                kickButton.SetActive(false);        //강퇴버튼 오프
            //------------- 상대 정보 표시판 변경

            logText.text += "\n<color=#ff8700>[" + otherPlayer.NickName + "] 님이 퇴장했습니다.</color>";   //퇴장 시 방에서 나간 유저 닉네임 표시
        }
    }

    public override void OnLeftRoom()   //PhotonNetwork.LeaveRoom(); 함수 성공 시 (누군가 방을 떠날 때 떠난 사람만 실행) (방을 나간 시점이라 방과 관련된 변수 사용 못함)
    {
        StopAllCoroutines();                    //모든 코루틴 종료
        LoadScene(SceneList.MultiLobbyScene);   //이전 씬으로 이동
    }

    public override void OnMasterClientSwitched(Player newMasterClient)     //마스터가 변경된 경우(방장이 나갔을 때)
    {
        logText.text += "\n<color=#ff0000>방의 마스터가 " + newMasterClient.NickName + "님으로 변경되었습니다.</color>";
        PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "MasterNick", newMasterClient.NickName } });    //마스터 정보 변경(로비에 표시되는 방정보에서 방 마스터를 바꿔주는데 사용된다.)     
    }
    #endregion ----------------- PunCallbacks, 방 관련 -----------------

    #region ------------------ 전투 관련 ------------------
    //-------------------- 전투 시작 --------------------
    private IEnumerator CountDown()     //전투시작시간 카운트 다운 코루틴 (마스터가 실행)
    {
        countPanel.SetActive(true);     //카운트 판넬 켜기
        curCount = combatCount;         //시작시간 설정

        while (curCount > 0)
        {
            countText.text = curCount.ToString();           //남은시간 표시
            yield return new WaitForSeconds(1.0f);

            curCount--;     //1초 감소
        }

        CombatReady();      //전투 시작 전 준비를 위한 함수 실행
        yield return new WaitForSeconds(1.5f);      //Fight애니메이션 재생시간동안 대기
        OnCombat();         //전투 시작 함수 실행
    }

    private void CombatReady()     //전투 시작 전 실행되는 함수(UI교체 및 몬스터 오브젝트 생성, Fight애니메이션 재생)
    {
        standBySetPoint.SetActive(false);   //나의 몬스터를 배치하는 SetPoint 끄기 
        standByUI.SetActive(false);         //준비상태 UI 끄기
        fightUI.SetActive(true);            //전투시작 UI 켜기
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Fight);     //전투시작 효과음 재생

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("CombatSetP1", Vector3.zero, Quaternion.identity);      //몬스터 생성을 담당하는 오브젝트 생성
        else
            PhotonNetwork.Instantiate("CombatSetP2", Vector3.zero, Quaternion.identity);      //몬스터 생성을 담당하는 오브젝트 생성
    }

    private void OnCombat()     //전투 시작 함수
    {
        fightUI.SetActive(false);     //전투시작 UI 끄기
        combatUI.SetActive(true);     //전투 중 UI 켜기

        foreach (Player P in PhotonNetwork.PlayerList)
            if (P.NickName.Equals(PhotonNetwork.MasterClient.NickName))
                team1NickText.text = P.NickName;    //마스터의 닉네임 표시
            else
                team2NickText.text = P.NickName;    //참가자의 닉네임 표시

        numofTeam1 = team1CMCList.Count;        //필드에 배치된 Team1의 몬스터 숫자만큼 설정(멀티의 경우 CombatSetPoint에서 몬스터를 생성하면 CmnMonSet에서 자신의 팀 리스트에 추가함)
        numofTeam2 = team2CMCList.Count;        //필드에 배치된 Team2의 몬스터 숫자만큼 설정(멀티의 경우 CombatSetPoint에서 몬스터를 생성하면 CmnMonSet에서 자신의 팀 리스트에 추가함)

        //-------- 마스터와 클라이언트 둘 중 한명이라도 몬스터배치를 안했으면 즉시 전투 종료 아니면 전투 시작 --------
        if (numofTeam1 == 0)
        {
            if (PhotonNetwork.IsMasterClient)
                ResultData.CombatResult = Result.Defeat;      //패배
            else
                ResultData.CombatResult = Result.Victory;     //승리

            StartCoroutine(EndAction());
        }
        else if (numofTeam2 == 0)
        {
            if (PhotonNetwork.IsMasterClient)
                ResultData.CombatResult = Result.Victory;      //승리
            else
                ResultData.CombatResult = Result.Defeat;     //패배

            StartCoroutine(EndAction());
        }
        else
            GameManager.GMEventAct();
        //-------- 마스터와 클라이언트 둘 중 한명이라도 몬스터배치를 안했으면 즉시 전투 종료 아니면 전투 시작 --------
    }
    //-------------------- 전투 시작 --------------------

    public void CalcResults(string Tag)    //몬스터 사망 시 해당 몬스터의 이름, 대미지, 피해량을 받아와 저장, 계산하는 함수 (IsMine만 실행됨)
    {
        switch (Enum.Parse<Team>(Tag))  //죽은 몬스터의 Tag를 Team enum형식으로 변경
        {
            case Team.Team1:         //Team1일 경우
                numofTeam1--;        //현재 남아있는 Team1의 수 감소
                break;

            case Team.Team2:         //Team2일 경우
                numofTeam2--;        //현재 남아있는 Team2의 수 감소
                break;

            default:
                return;
        }

        //Team1 또는 Team2의 몬스터가 다 죽었을 때 (CalcResults 함수 자체가 IsMine인 몬스터가 죽었을 때 그쪽 로컬에서만 실행되기 때문에 어느 한쪽이 다 죽었다는건 내 몬스터가 다 죽었다는 뜻)
        if (numofTeam1 == 0 || numofTeam2 == 0)
            photonV.RPC("EndActionRPC", RpcTarget.All);     //전투 종료 코루틴 로컬과 원격에 모두 실행
    }    

    [PunRPC]
    private void EndActionRPC()
    {     
        if (numofTeam1 == 0 || numofTeam2 == 0)
            ResultData.CombatResult = Result.Defeat;      //패배
        else
            ResultData.CombatResult = Result.Victory;     //승리

        StartCoroutine(EndAction());      //ResultScene으로 넘어가는 코루틴 실행
    }

    private IEnumerator EndAction()   //ResultScene으로 넘어갈 때 효과를 주기위한 코루틴 (RPC를 통해 마스터와 클라이언트 양쪽 모두에서 실행)
    {
        //------------------ 현재 방의 CustomProperties를 사용하여 전투결과 전달
        int[] a_MonName;
        int[] a_Dmg;
        int[] a_HP;

        if (PhotonNetwork.IsMasterClient)       //마스터일 경우
        {
            team1CMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));   //총 대미지가 높은 순서대로 정렬

            a_MonName = new int[team1CMCList.Count];        //전투에 참여한 몬스터들의 이름을 담을 배열변수
            a_HP = new int[team1CMCList.Count];             //전투에 참여한 몬스터들의 받은 피해량을 담을 배열변수
            a_Dmg = new int[team2CMCList.Count];            //전투에 참여한 몬스터들의 입힌 피해량을 담을 배열변수

            for (int i = 0; i < team1CMCList.Count; i++)
            {
                a_MonName[i] = (int)team1CMCList[i].CMCmonStat.monName;
                a_HP[i] = team1CMCList[i].TotalHP;
            }

            for(int i = 0; i < team2CMCList.Count; i++)
                a_Dmg[i] = team2CMCList[i].TotalDmg;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnNameNum", a_MonName } });     //마스터의 몬스터 정보 전달
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnHP", a_HP } });               //마스터의 입은 총 피해 전달
            //참가자가 입힌 총 피해 전달 (입힌 피해의 경우 값을 IsMine이 아닌 원격이 가지고있기 때문에 마스터쪽 화면의 참가자의 몬스터를 통해 전달받아야 한다.)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnDmg", a_Dmg } });             
        }
        else                                    //참가자일 경우
        {
            team2CMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));   //총 대미지가 높은 순서대로 정렬

            a_MonName = new int[team2CMCList.Count];
            a_HP = new int[team2CMCList.Count];
            a_Dmg = new int[team1CMCList.Count];

            for (int i = 0; i < team2CMCList.Count; i++)
            {
                a_MonName[i] = (int)team2CMCList[i].CMCmonStat.monName;
                a_HP[i] = team2CMCList[i].TotalHP;
            }

            for (int i = 0; i < team1CMCList.Count; i++)
                a_Dmg[i] = team1CMCList[i].TotalDmg;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnNameNum", a_MonName } });     //참가자의 몬스터 정보 전달
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnHP", a_HP } });               //참가자의 입은 총 피해 전달
            //마스터가 입힌 총 피해 전달 (입힌 피해의 경우 값을 IsMine이 아닌 원격이 가지고있기 때문에 참가자쪽 화면의 마스터의 몬스터를 통해 전달받아야 한다.)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnDmg", a_Dmg } });
        }
        //------------------ 현재 방의 CustomProperties를 사용하여 전투결과 전달

        while (Time.timeScale > 0.3f)   //일정 수치만큼 느려질 때까지 반복
        {
            Time.timeScale -= Time.deltaTime;       //속도 느려지도록 하기
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Time.timeScale = 1.0f;       //timeScale 정상화

        SceneManager.LoadScene(SceneList.MultiResultScene.ToString());      //MultiResultScene으로 넘어가기
    }
    #endregion ------------------ 전투 관련 ------------------
}
