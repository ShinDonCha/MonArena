using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System;

//PHP로부터 랭커들의 정보를 받아와 저장하기 위한 스크립트
//RankLobbyScene의 RankLobbyMgr에 붙여서 사용
public class RankLobbyMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    public static RankLobbyMgr Instance;

    private readonly string getRankURL = "http://dhosting.dothome.co.kr/Arena/GetRank.php";       //랭킹정보를 가져올 PHP 주소

    private readonly List<string> rankNickList = new();                             //랭커들의 정보중 닉네임을 저장할 리스트
    public string GetNickList(int Index) { return rankNickList[Index]; }            //외부에서 지정한 랭커의 닉네임을 가져가기 위한 함수
    private readonly List<int> rankCrtList = new();                                 //랭커들의 정보중 캐릭터 이미지 번호를 저장할 리스트
    public int GetCrtList(int Index) { return rankCrtList[Index]; }                 //외부에서 지정한 랭커의 캐릭터 이미지 번호를 가져가기 위한 함수
    private readonly List<int> rankNumList = new();                                 //랭커들의 정보중 순위를 저장할 리스트
    public int GetRankList(int Index) { return rankNumList[Index]; }                //외부에서 지정한 랭커의 순위를 가져가기 위한 함수
    private readonly List<MonsterName[]> rankDNameList = new();                     //랭커들의 정보중 몬스터 목록을 저장할 리스트
    public MonsterName[] GetDNameList(int Index) { return rankDNameList[Index]; }   //외부에서 지정한 랭커의 몬스터 목록을 가져가기 위한 함수
    private readonly List<int[]> rankDStarFList = new();                            //랭커들의 정보중 몬스터 성급을 저장할 리스트
    public int[] GetDStarList(int Index) { return rankDStarFList[Index]; }          //외부에서 지정한 랭커의 몬스터 성급을 가져가기 위한 함수

    [SerializeField]
    private MonStorage monStore = null;     //이미지, 오브젝트 저장소
    [SerializeField]
    private GameObject loadingPanel = null; //로딩 판넬 프리팹
    [SerializeField]
    private Image myCrtImg = null;          //나의 캐릭터 이미지를 넣을 이미지
    [SerializeField]
    private Text myNickText = null;         //나의 닉네임을 표시할 텍스트
    [SerializeField]
    private Text myRankText = null;         //나의 순위를 표시할 텍스트

    [SerializeField]
    private GameObject rankListPrefab = null;   //랭커의 정보를 보여줄 랭킹판 프리팹
    [SerializeField]
    private Transform rankListTr = null;        //rankListPrefab을 생성할 Transform
    private bool getRanking = false;            //현재 랭킹정보를 가지고 오고 있는지 알기위한 변수(네트워크 연결 중인지)

    private void Awake()
    {
        Instance = this;        //싱글턴
        myCrtImg.sprite = monStore.characterImg[PlayerInfo.UserCrtNum]; //나의 캐릭터 이미지 표시
        myNickText.text = PlayerInfo.UserNick;                          //나의 닉네임 표시
        BGMController.Instance.BGMChange(BGMList.RankLobby);            //배경음악 변경
    }

    private void Start()
    {
        StartCoroutine(GetRankCo());        //랭킹정보 가져오기 코루틴 실행
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.DefDeckButton:      //방어 편성
                LoadScene(SceneList.DefDeckScene);
                break;

            case ButtonList.RetryButton:        //새로고침
                if(!getRanking)
                    StartCoroutine(GetRankCo());
                break;

            case ButtonList.BackButton:         //돌아가기
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    public void LoadScene(SceneList NextScene)  //씬 불러오기 함수
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    private IEnumerator GetRankCo()     //랭킹 정보 가져오는 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        for(int i = 0; i < rankListTr.childCount; i++)      //기존에 존재하던 RankList프리팹 모두 제거
            Destroy(rankListTr.GetChild(i).gameObject);

        getRanking = true;

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(getRankURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
        {
            Debug.Log(a_www.error);
            getRanking = false;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_www.downloadHandler.data);     //받아온 데이터를 문자열로 변경

        JSONNode a_JSON = JSON.Parse(a_GetStr);

        if (a_JSON == null)
        {
            getRanking = false;
            yield break;
        }

        for(int i = 0; i < a_JSON.Count; i++)       //받아온 랭커의 수 만큼 반복
        {
            if (a_JSON[i]["User_Nick"].Equals(PlayerInfo.UserNick))      //나의 정보일 경우
                myRankText.text = "순위 : " + a_JSON[i]["User_Rank"];     //순위 표시
            else  //다른 유저의 정보일 경우
            {
                rankNickList.Add(a_JSON[i]["User_Nick"]);       //닉네임 추가
                rankCrtList.Add(a_JSON[i]["User_Crt"]);         //캐릭터 이미지 추가
                rankNumList.Add(a_JSON[i]["User_Rank"]);        //랭킹 순위 추가

                JSONNode a_DefDeckJSON = JSON.Parse(a_JSON[i]["DefDeck"]);         //방어덱 리스트 한번 더 파싱
                JSONNode a_DefStarJSON = JSON.Parse(a_JSON[i]["DefStarF"]);        //방어덱 성급 리스트 한번 더 파싱

                if (a_DefDeckJSON == null)      //오류 방지
                    continue;

                //-------------- 몬스터 목록 받아오기 --------------
                MonsterName[] a_MNameArr = new MonsterName[a_DefDeckJSON.Count];
                int[] a_MStarFArr = new int[a_DefStarJSON.Count];

                for (int k = 0; k < a_DefDeckJSON.Count; k++)
                    if (Enum.TryParse(a_DefDeckJSON[k], out MonsterName MonName))
                    {
                        a_MNameArr[k] = MonName;
                        a_MStarFArr[k] = a_DefStarJSON[k];
                    }

                rankDNameList.Add(a_MNameArr);
                rankDStarFList.Add(a_MStarFArr);
                //-------------- 몬스터 목록 받아오기 --------------

                Instantiate(rankListPrefab, rankListTr);    //랭킹판 생성
            }
        }

        getRanking = false; 
    }
}
