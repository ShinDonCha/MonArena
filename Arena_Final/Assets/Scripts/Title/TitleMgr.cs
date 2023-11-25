using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.Text;

//로그인과 신규가입, 데이터베이스로부터 정보가져오기 등을 담당하는 스크립트
//TitleScene의 TitleMgr에 붙여서 사용
public class TitleMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    private enum RetryType  //재시도 요청타입
    {
        MonData,   //몬스터 데이터
        Login,     //로그인
        SignUp     //계정생성
    }

    [SerializeField]
    private GameObject loadingPanel = null;     //로딩 판넬 프리팹

    [Header("------- Login -------")]
    [SerializeField]
    private GameObject loginPanel = null;        //로그인 판넬 게임오브젝트
    [SerializeField]
    private InputField loginIDIFD = null;        //로그인 판넬의 ID_InputField
    [SerializeField]
    private InputField loginPWIFD = null;        //로그인 판넬의 PW_InputField
    [SerializeField]
    private GameObject startPanel = null;        //게임시작 판넬
    private string inputID = "";        //로그인 시 입력했던 ID의 임시저장을 위한 변수
    private string inputPW = "";        //로그인 시 입력했던 PW의 임시저장을 위한 변수

    [Header("------- Signup -------")]
    [SerializeField]
    private GameObject signupPanel = null;       //SignUp 판넬
    [SerializeField]
    private InputField signupIDIFD = null;       //SignUp 판넬의 ID_InputField
    [SerializeField]
    private InputField signupPWIFD = null;       //SignUp 판넬의 PW_InputField

    [Header("------- Connecting -------")]
    [SerializeField]
    private Text connectText = null;            //연결 및 오류상태 표시용 텍스트
    private bool isTrying = false;              //현재 서버와 연결중인지를 알기위한 변수
    private Color connectColor = Color.white;   //연결 상태 표시용 텍스트 색상
    private Color errorColor = Color.red;       //오류 상태 표시용 텍스트 색상
    private readonly string loginURL = "http://dhosting.dothome.co.kr/Arena/Login.php";     //로그인 연결 주소
    private readonly string dicDataURL = "http://dhosting.dothome.co.kr/Arena/MonData.php"; //몬스터 데이터 연결 주소
    private readonly string signupURL = "http://dhosting.dothome.co.kr/Arena/SignUp.php";   //계정 생성 연결 주소
    private readonly Queue<RetryType> retryQueue = new();       //재시도 요청 큐


    // Start is called before the first frame update
    void Start()
    {
        startPanel.SetActive(false);        //시작 판넬 끄기
        loginPanel.SetActive(false);        //로그인 판넬 끄기
        signupPanel.SetActive(false);       //계정생성 판넬 끄기
        connectText.gameObject.SetActive(true); //연결 텍스트 켜기

        StartCoroutine(DataInit());      //딕셔너리 설정(몬스터 데이터 가져오기)
        BGMController.Instance.BGMChange(BGMList.Title);     //BGM 변경
    }

    // Update is called once per frame
    void Update()
    {      
        //------------ 재시도 요청받은 경우 실행
        if (retryQueue.Count > 0 && !isTrying)     //서버와의 연결요청이 필요하고, 이전작업 진행중이 아니라면
        {
            switch (retryQueue.Dequeue())
            {
                case RetryType.MonData:     //몬스터 데이터 다시 요청
                    StartCoroutine(DataInit());
                    break;

                case RetryType.Login:       //로그인 다시 요청
                    StartCoroutine(LoginCo(inputID, inputPW));
                    break;

                case RetryType.SignUp:      //계정생성 다시 요청
                    StartCoroutine(SignUpCo());
                    break;
            }           
        }
        //------------ 재시도 요청받은 경우 실행

    }
    #region ---------------------------- 버튼 동작 함수 ----------------------------    
    public void ButtonOnClick(Button PushBtn)    //타이틀씬에있는 버튼 클릭 시 동작(버튼의 OnClick에서 실행)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))  //받아온 게임오브젝트의 이름을 ButtonName의 enum형으로 변경
            return;

        switch(BtnL)
        {
            case ButtonList.LoginButton:        //로그인 버튼
                if (isTrying) return;           //이미 서버와의 연결이 시도중이라면 취소
                StartCoroutine(LoginCo(loginIDIFD.text.Trim(), loginPWIFD.text.Trim()));   //로그인 코루틴(수동) 실행
                break;

            case ButtonList.CreateButton:       //생성 버튼
                if (isTrying) return;           //이미 서버와의 연결이 시도중이라면 취소
                StartCoroutine(SignUpCo());     //계정생성 코루틴 실행
                break;

            case ButtonList.SignupButton:       //계정생성 판넬 생성 버튼
                loginPanel.SetActive(false);    //로그인 판넬 끄기
                signupPanel.SetActive(true);    //Sign Up판넬 활성화
                break;            

            case ButtonList.CancelButton:       //계정생성 판넬 삭제 버튼
                signupPanel.SetActive(false);   //Sign Up판넬 끄기
                loginPanel.SetActive(true);     //로그인 판넬 켜기
                break;

            case ButtonList.StartButton:         //게임시작 버튼(StartPanel에 있음)
                LoadScene(SceneList.LobbyScene); //씬이동 함수 실행
                break;

            case ButtonList.LogoutButton:        //계정전환 버튼(StartPanel에 있음)
                startPanel.SetActive(false);     //시작 판넬 끄기
                loginPanel.SetActive(true);      //로그인 판넬 켜기
                connectText.text = "";           //연결 텍스트 초기화
                connectText.gameObject.SetActive(true); //연결 텍스트 켜기
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }
    #endregion ---------------------------- 버튼 동작 함수 ----------------------------

    #region --------------------- 씬 불러오기 함수 ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- 씬 불러오기 함수 ---------------------

    #region ---------------------------- 몬스터 데이터 불러오기 ----------------------------
    private IEnumerator DataInit()         //서버에 저장되어있는 몬스터들의 정보를 딕셔너리에 가져오기
    {
        if (isTrying)   //이미 서버와의 연결이 시도중이라면 취소
            yield break;

        isTrying = true;        //서버 연결중 상태로 변경
        connectText.color = connectColor;
        connectText.text = "서버로부터 데이터를 받아오는 중입니다. 잠시만 기다려주세요.";

        WWWForm a_Form = new();                

        for (int i = 0; i < (int)MonsterName.MonsterCount; i++)         //몬스터의 종류만큼 각각의 키값을 정해주기
            a_Form.AddField(("Mon" + i).ToString(), ((MonsterName)i).ToString());

        a_Form.AddField("MonCount", (int)MonsterName.MonsterCount);     //PHP의 반복문을 몬스터 수 만큼 하기위한 값

        UnityWebRequest a_WWW = UnityWebRequest.Post(dicDataURL, a_Form);
        yield return a_WWW.SendWebRequest();    //서버 연결 요청
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)            //서버 연결에 오류가 있다면
        {
            ReqRetry(RetryType.MonData);    //재시도 요청
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //받아온 데이터를 문자열로 변경

        if (!a_GetStr.Contains("Success!!"))    //몬스터 정보를 가져오는데 성공하지 못했다면 (데이터베이스 접근불가, 해당 몬스터의 정보가 존재하지 않음)
        {
            ErrorMessage(a_GetStr);     //에러메세지 표시
            yield break;
        }
        
        JSONNode a_JSON = JSON.Parse(a_GetStr);       //가져온 문자열 JSON형식으로 파싱

        if (a_JSON == null)     //파싱한 것이 오류가 있다면
        {
            ReqRetry(RetryType.MonData);    //재시도 요청
            yield break;
        }

        //----------------- 딕셔너리 설정
        for (int monnum = 0; monnum < (int)MonsterName.MonsterCount; monnum++)          //몬스터 수 만큼 반복
        {
            List<MonsterStat> a_List = new();   //0성 ~ 5성까지의 몬스터 스탯을 담을 리스트
            MonsterStat a_Stat = new();     //위의 리스트에 추가할 몬스터 스탯

            //Soldier 부터 Jammo까지 성급에 따라 달라지는 수치 분리 (HP ~ AttackSpd)(값들이 JSONArray 형식으로 저장 되어있음)
            a_JSON[monnum]["HP"] = JSON.Parse(a_JSON[monnum]["HP"]);
            a_JSON[monnum]["AttackDmg"] = JSON.Parse(a_JSON[monnum]["AttackDmg"]);
            a_JSON[monnum]["DefPower"] = JSON.Parse(a_JSON[monnum]["DefPower"]);
            a_JSON[monnum]["MDefPower"] = JSON.Parse(a_JSON[monnum]["MDefPower"]);
            a_JSON[monnum]["AttackSpd"] = JSON.Parse(a_JSON[monnum]["AttackSpd"]);
            //Soldier 부터 Jammo까지 성급에 따라 달라지는 수치 분리 (HP ~ AttackSpd)(값들이 JSONArray 형식으로 저장 되어있음)

            for (int starforce = 0; starforce < 6; starforce++)      //몬스터의 성급 (0성 ~ 5성)에 따른 스탯 저장
            {
                a_Stat.monName = Enum.Parse<MonsterName>(a_JSON[monnum]["MonsterName"]);
                a_Stat.attackType = Enum.Parse<AttackType>(a_JSON[monnum]["AttackType"]);
                a_Stat.hp = a_JSON[monnum]["HP"][starforce];
                a_Stat.attackDmg = a_JSON[monnum]["AttackDmg"][starforce];
                a_Stat.defPower = a_JSON[monnum]["DefPower"][starforce];
                a_Stat.mdefPower = a_JSON[monnum]["MDefPower"][starforce];
                a_Stat.attackSpd = a_JSON[monnum]["AttackSpd"][starforce];
                a_Stat.moveSpd = a_JSON[monnum]["MoveSpd"];
                a_Stat.attackRange = a_JSON[monnum]["AttackRange"];
                a_Stat.starForce = starforce;

                a_List.Add(a_Stat);
            }
            MonsterData.MonDic.Add((MonsterName)monnum, a_List);     //몬스터 정보를 딕셔너리에 저장
        }
        //----------------- 딕셔너리 설정

        isTrying = false;       //서버 연결 종료 상태로 변경

        if(PlayerPrefs.GetString("UserID") == "")       //저장된 유저의 ID가 없으면
        {
            connectText.color = errorColor;
            connectText.text = "등록된 계정 정보가 없습니다. 계정을 새로 생성하거나 직접 로그인 해주세요.";
            loginPanel.SetActive(true);
        }
        else //저장된 유저의 ID가 있으면
            StartCoroutine(LoginCo(PlayerPrefs.GetString("UserID"), PlayerPrefs.GetString("UserPW")));      //자동 로그인 실행
    }
    #endregion ---------------------------- 몬스터 데이터 불러오기 ----------------------------

    #region ---------------------------- 로그인 ----------------------------
    private IEnumerator LoginCo(string IDStr, string PWStr)    //로그인버튼 클릭 시 실행되는 로그인 코루틴  
    {
        isTrying = true;    //서버 연결중 상태로 변경
        connectText.color = connectColor;
        connectText.text = "유저의 정보를 가져오는 중... 잠시만 기다려주세요.";

        inputID = IDStr;     //연결 오류에 따른 재시도 요청을 위한 입력된 ID 저장
        inputPW = PWStr;     //연결 오류에 따른 재시도 요청을 위한 입력된 PW 저장

        if (IDStr.Length <= 0)       //ID 입력하지 않았을 때
        {
            ErrorMessage(IDStr);     //에러 메세지 표시
            yield break;
        }
        else if (PWStr.Length <= 0)  //PW 입력하지 않았을 때
        {
            ErrorMessage(PWStr);    //에러 메세지 표시
            yield break;
        }

        loginPanel.SetActive(false);        //로그인 판넬 끄기

        WWWForm a_Form = new();
        a_Form.AddField("User_ID", inputID);
        a_Form.AddField("User_PW", inputPW);

        UnityWebRequest a_WWW = UnityWebRequest.Post(loginURL, a_Form);
        yield return a_WWW.SendWebRequest();    //서버 연결 요청
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)            //서버 연결에 에러가 있다면 같은 ID와 PW로 재시도
        {
            ReqRetry(RetryType.Login);      //재시도 요청
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //받아온 데이터를 문자열로 변경

        if (!a_GetStr.Contains("Success"))   //로그인 실패 시 (데이터베이스에 접근을 못했거나 아이디 혹은 비밀번호를 틀렸을 때)
        {
            ErrorMessage(a_GetStr);     //에러메세지 표시
            yield break;
        }

        JSONNode a_JSON = JSON.Parse(a_GetStr);

        if (a_JSON == null)     //파싱한 것이 오류가 있다면 같은 ID와 PW로 재시도
        {
            ReqRetry(RetryType.Login);  //재시도 요청
            yield break;
        }

        //--------------- 저장되어있던 플레이어의 정보 받아오기
        PlayerInfo.UserNick = a_JSON["User_Nick"];      //데이터베이스의 NULL값 제외하고 전부 "값" 형식으로 받아옴 근데 JSON자체에서 알아서 int로 바꿔주는듯
        PlayerInfo.UniqueID = a_JSON["Unique_ID"];
        PlayerInfo.UserCrtNum = a_JSON["User_Crt"];
        PlayerInfo.UserLevel = a_JSON["User_Lv"];
        PlayerInfo.UserGold = a_JSON["User_Gold"];
        PlayerInfo.CombatStage = a_JSON["User_Stage"];

        if (a_JSON["User_ATime"] != null)          //기존 유저일 경우
            PlayerInfo.AutoExpTime = DateTime.Parse(a_JSON["User_ATime"]);            
        else                                       //신규 유저일 경우
            PlayerInfo.AutoExpTime = DateTime.Now;

        //--------- 유저 정보를 담을 리스트 전체 초기화
        PlayerInfo.MonList.Clear();
        PlayerInfo.CombatDeck.Clear();
        PlayerInfo.CombatStarF.Clear();
        PlayerInfo.DefDeck.Clear();
        PlayerInfo.DefStarF.Clear();
        PlayerInfo.RankDeck.Clear();
        PlayerInfo.RankStarF.Clear();
        //--------- 유저 정보를 담을 리스트 전체 초기화

        if (a_JSON["MonList"] != null)       //기존 유저일 경우
        {
            JSONNode a_MonJSON = JSON.Parse(a_JSON["MonList"]);       //string 형식으로 사용하기 위해 한번더 파싱 // 현재는 [\"Soldier\"] <-- 이상태) // 파싱하고나면 ["Soldier", "Zombie"] 이렇게 됨
            JSONNode a_StarJSON = JSON.Parse(a_JSON["MonStar"]);      //배열로 바꾸기위해 한번더 파싱 //현재는 "[0,0,0,0]" <이렇게 돼있음

            for (int i = 0; i < a_MonJSON.Count; i++)      //저장되어있던 보유 몬스터의 정보 연결
            {
                if (Enum.TryParse(a_MonJSON[i], out MonsterName MonName))
                    PlayerInfo.MonList.Add(MonsterData.MonDic[MonName][a_StarJSON[i]]);    //저장된 유저의 몬스터이름과 성급에 맞는 정보를 딕셔너리에서 찾아 넣기
            }
        }

        if (a_JSON["CombatDeck"] != null)     //기존 유저일 경우
        {
            JSONNode a_CombatJSON = JSON.Parse(a_JSON["CombatDeck"]);
            for (int i = 0; i < a_CombatJSON.Count; i++)     //저장되어있던 전투 덱 몬스터의 정보 연결
            {
                if (Enum.TryParse(a_CombatJSON[i], out MonsterName MonName))
                    PlayerInfo.CombatDeck.Add(MonName);   //몬스터 이름 가져와서 저장
            }
        }

        if (a_JSON["CombatStarF"] != null)     //기존 유저일 경우
        {          
            JSONNode a_StarJSON = JSON.Parse(a_JSON["CombatStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //저장되어있던 전투 덱의 성급을 가져와서 저장
                PlayerInfo.CombatStarF.Add(a_StarJSON[i]);
        }

        if (a_JSON["DefDeck"] != null)      //기존 유저일 경우
        {
            JSONNode a_DefJSON = JSON.Parse(a_JSON["DefDeck"]);
            for (int i = 0; i < a_DefJSON.Count; i++)        //저장되어있던 방어 덱 몬스터의 정보 연결
            {
                if (Enum.TryParse(a_DefJSON[i], out MonsterName MonName))
                    PlayerInfo.DefDeck.Add(MonName);         //몬스터 이름 가져와서 저장
            }                  
        }

        if (a_JSON["DefStarF"] != null)     //기존 유저일 경우
        {
            JSONNode a_StarJSON = JSON.Parse(a_JSON["DefStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //저장되어있던 방어 덱의 성급을 가져와서 저장
                PlayerInfo.DefStarF.Add(a_StarJSON[i]);
        }

        if (a_JSON["RankDeck"] != null)     //기존 유저일 경우
        {
            JSONNode a_RankJSON = JSON.Parse(a_JSON["RankDeck"]);
            for (int i = 0; i < a_RankJSON.Count; i++)     //저장되어있던 랭크 덱 몬스터의 정보 연결
            {
                if (Enum.TryParse(a_RankJSON[i], out MonsterName MonName))
                    PlayerInfo.RankDeck.Add(MonName);   //몬스터 이름 가져와서 저장
            }
        }

        if (a_JSON["RankStarF"] != null)     //기존 유저일 경우
        {
            JSONNode a_StarJSON = JSON.Parse(a_JSON["RankStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //저장되어있던 랭크 덱의 성급을 가져와서 저장
                PlayerInfo.RankStarF.Add(a_StarJSON[i]);
        }
        //--------------- 저장되어있던 플레이어의 정보 받아오기

        PlayerPrefs.SetString("UserID", inputID);       //유저의 아이디 로컬에 저장(다음 게임 실행 시 자동로그인을 위해)
        PlayerPrefs.SetString("UserPW", inputPW);       //유저의 비밀번호 로컬에 저장(다음 게임 실행 시 자동로그인을 위해)

        isTrying = false;       //서버 연결 종료 상태로 변경

        connectText.gameObject.SetActive(false);        //연결 텍스트 끄기
        startPanel.SetActive(true);     //시작판넬 생성
    }
    #endregion ---------------------------- 로그인 ----------------------------

    #region ---------------------------- 계정생성 ----------------------------
    private IEnumerator SignUpCo()   //계정생성 확인버튼 클릭 시 실행되는 계정생성 코루틴
    {
        isTrying = true;    //서버 연결중 상태로 변경
        connectText.color = connectColor;
        connectText.text = "계정을 생성하는 중... 잠시만 기다려주세요.";

        string a_IDStr = signupIDIFD.text.Trim();   //띄어쓰기를 제외한 입력된 텍스트 가져오기
        string a_PWStr = signupPWIFD.text.Trim();   //띄어쓰기를 제외한 입력된 텍스트 가져오기

        if (a_IDStr.Length <= 0)    //ID를 입력하지 않았을 때
        {
            ErrorMessage(a_IDStr);  //에러 메세지 표시
            yield break;
        }
        else if (a_PWStr.Length <= 0)   //PW를 입력하지 않았을 때
        {
            ErrorMessage(a_PWStr);  //에러 메세지 표시
            yield break;
        }

        signupPanel.SetActive(false);       //계정생성 판넬 끄기

        WWWForm a_Form = new();
        a_Form.AddField("Creat_ID", a_IDStr, Encoding.UTF8);
        a_Form.AddField("Creat_PW", a_PWStr, Encoding.UTF8);

        UnityWebRequest a_WWW = UnityWebRequest.Post(signupURL, a_Form);
        yield return a_WWW.SendWebRequest();    //서버 연결 요청
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)    //서버 연결에 실패했을 때
        {
            ReqRetry(RetryType.SignUp); //재시도 요청
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //받아온 데이터를 문자열로 변경

        if (!a_GetStr.Contains("Success"))   //계정 생성 실패 시 (데이터베이스에 접근 불가 or 같은 아이디가 존재 or 계정을 저장하는 과정에서 오류가 발생했을 때)
        {
            ErrorMessage(a_GetStr);
            yield break;
        }

        isTrying = false;   //서버 연결 종료 상태로 변경
        loginPanel.SetActive(true);   //로그인 판넬 켜기
        connectText.text = "계정 생성이 완료되었습니다. 로그인 후 게임을 시작할 수 있습니다.";
    }
    #endregion ---------------------------- 계정생성 ----------------------------

    #region ---------------------------- 오류 관련 ----------------------------
    private void ReqRetry(RetryType Type)   //서버연결 실패 시 재시도 요청 함수
    {
        if (!retryQueue.Contains(Type))      //재시도 목록에 없다면 추가
            retryQueue.Enqueue(Type);

        isTrying = false;
    }

    private void ErrorMessage(string Str)   //오류 발생 시 메세지 출력 혹은 재시도 요청을 위한 함수
    {
        connectText.color = errorColor;

        if (Str.Length <= 0)    //ID 혹은 PW를 입력하지 않음(LoginCo, SignUpCo)
            connectText.text = "ID 혹은 PW가 제대로 입력되지 않았습니다.";        
        else if (Str.Contains("ID does not exist."))    //ID를 잘못 입력했을 경우(LoginCo)
        {
            connectText.text = "존재하지 않는 ID입니다. 로그인을 다시 시도해 주세요.";
            loginPanel.SetActive(true);
        }
        else if (Str.Contains("The PW doesn't fit."))   //PW를 잘못 입력했을 경우(LoginCo)
        {
            connectText.text = "비밀번호가 틀렸습니다. 로그인을 다시 시도해 주세요.";
            loginPanel.SetActive(true);
        }
        else if (Str.Contains("ID does exist."))    //계정생성 시 중복 ID일 경우(SignupCo)
        {
            connectText.text = "같은 ID가 이미 존재합니다. ID를 변경하여 다시 시도해 주세요.";
            signupPanel.SetActive(true);
        }
        else if (Str.Contains("Creat Error"))       //계정생성 마지막 단계에서 오류 발생 (SignupCo)
            ReqRetry(RetryType.SignUp);
        else if (Str.Contains("Could not Connect"))      //데이터베이스에 아예 접근하지 못했을 경우(PHP내에서 데이터베이스 접근을 위한 아이디와 비밀번호 등에 오류가 있음)
            connectText.text = "서버와 연결 중 문제가 발생했습니다. 문의를 남겨주세요.";
        else if (Str.Contains("Data does not exist"))   //몬스터 정보를 가져오지 못했을 경우(PHP내에서 요청하는 몬스터의 이름에 오류가 있음)
            connectText.text = "몬스터의 데이터를 가져오던 중 문제가 발생했습니다. 문의를 남겨주세요.";

        isTrying = false;
    }
    #endregion ---------------------------- 오류 관련 ----------------------------
}
