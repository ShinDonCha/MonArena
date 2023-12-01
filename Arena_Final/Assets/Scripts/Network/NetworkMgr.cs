using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System.Linq;

//데이터베이스에 정보저장을 요청하기 위한 스크립트
//정보저장을 요청이 필요한 스크립트에 상속한다.
public class NetworkMgr : MonoBehaviour
{
    //-------- URL
    private readonly string nickSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveNickName.php";       //닉네임 저장 주소
    private readonly string userCrtURL = "http://dhosting.dothome.co.kr/Arena/SaveCrt.php";             //유저 캐릭터이미지 저장 주소
    private readonly string userLvSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveLevel.php";        //유저 레벨 저장 주소
    private readonly string monListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveMonList.php";     //유저의 보유몬스터 저장 주소
    private readonly string goldSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveGold.php";           //유저의 골드 저장 주소
    private readonly string autoExpSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveAutoExp.php";     //자동탐색 보상 저장 주소
    private readonly string stageSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveStage.php";         //스테이지 저장 주소
    private readonly string combatSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveCombatList.php";   //전투덱 저장 주소
    private readonly string defListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveDefList.php";     //방어덱 저장 주소
    private readonly string rankListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveRankList.php";   //랭크덱 저장 주소
    private readonly string rankingSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveRanking.php";     //랭킹 저장 주소
    //-------- URL

    private readonly Queue<PacketType> packetsQueue = new();    //정보 저장이 요청된 패킷 목록을 담을 큐
    private bool isNetworkLock = false;             //현재 서버에 정보를 전송 중인지 판별하기 위한 변수
    protected bool reqExit = false;                 //게임 종료 요청이 들어왔는지 알기위한 변수

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isNetworkLock)     //서버에 정보를 전송중이 아닐 때
        {
            if (packetsQueue.Count > 0)     //요청받은 패킷이 있다면
                ReqNetwork(packetsQueue.Dequeue());   //정보저장 요청 함수 실행
            else if (packetsQueue.Count <= 0 && reqExit)    //요청받은 패킷이 없고, 종료요청이 들어왔다면
                Application.Quit();         //프로그램 종료
        }
    }

    private void ReqNetwork(PacketType PT)  //정보저장 요청 함수
    {
        switch (PT)
        {
            case PacketType.NickName:
                StartCoroutine(NickChangeCo());  //닉네임 변경 코루틴 실행
                break;

            case PacketType.UserCrt:             
                StartCoroutine(UserCrtCo());     //유저 캐릭터 이미지 저장 코루틴 실행
                break;

            case PacketType.UserLv:
                StartCoroutine(UserLvCo());      //유저 레벨 저장 코루틴 실행
                break;

            case PacketType.MonList:
                StartCoroutine(MonListCo());     //유저의 보유몬스터 저장 코루틴 실행
                break;

            case PacketType.AutoTime:
                StartCoroutine(AutoTimeCo());    //자동탐색 저장 코루틴 실행
                break;

            case PacketType.UserGold:
                StartCoroutine(UserGoldCo());    //유저 골드 저장 코루틴 실행
                break;

            case PacketType.CombatStage:
                StartCoroutine(StageCo());       //스테이지 저장 코루틴 실행
                break;

            case PacketType.CombatDeck:
                StartCoroutine(CombatDeckCo());  //전투덱 저장 코루틴 실행
                break;

            case PacketType.DefDeck:
                StartCoroutine(DefDeckCo());   //방어덱 저장 코루틴 실행
                break;

            case PacketType.RankDeck:
                StartCoroutine(RankDeckCo());   //랭크덱 저장 코루틴 실행
                break;

            case PacketType.Ranking:
                StartCoroutine(RankingCo());    //랭킹 저장 코루틴 실행
                break;
        }
    }

    protected void PushPacket(PacketType Type)    //상속받은 스크립트에서 정보저장 요청할때 부르는 함수
    {
        if (packetsQueue.Contains(Type))          //이미 저장요청이 된 패킷과 같은 패킷이 요청될 경우 취소
            return;

        packetsQueue.Enqueue(Type);     //패킷 저장
    }    

    private IEnumerator NickChangeCo()  //닉네임 변경 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        NickPanelCtrl a_NPCtrl = GetComponentInChildren<NickPanelCtrl>();     //닉네임 변경 판넬의 스크립트 가져오기

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("Input_Nick", a_NPCtrl.GetReqNick);

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(nickSaveURL, a_form);  
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        System.Text.Encoding Encd = System.Text.Encoding.UTF8;
        string a_GetStr = Encd.GetString(a_www.downloadHandler.data);

        if (a_GetStr.Contains("exist"))   //같은 닉네임이 있을때 받아오는 문자열
            StartCoroutine(a_NPCtrl.ErrorOnOff("사용중인 닉네임 입니다."));      //닉네임 변경 판넬의 에러메세지 표시 함수 실행
        else if (a_GetStr.Contains("SaveSuccess"))   //닉네임 변경에 성공했을 때
        {
            GetComponent<UserPanelCtrl>().NickChange(a_NPCtrl.GetReqNick);      //닉네임 변경
            Destroy(a_NPCtrl.gameObject);       //닉네임 판넬 삭제
        }

        isNetworkLock = false;   //정보 저장 가능으로 변경
    }

    private IEnumerator UserCrtCo()      //유저 캐릭터 이미지 변경 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("User_Crt", PlayerInfo.UserCrtNum.ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(userCrtURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;    //정보 저장 가능으로 변경
    }

    private IEnumerator UserLvCo()      //유저 레벨 변경 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;
        
        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("Input_Level", PlayerInfo.UserLevel.ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(userLvSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;    //정보 저장 가능으로 변경
    }

    private IEnumerator MonListCo()     //유저의 보유몬스터 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };

        if (PlayerInfo.MonList.Count > 0)                //몬스터를 보유하고 있다면
        {
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();        //몬스터 정렬

            for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            {
                a_JSArr[0].Add(PlayerInfo.MonList[i].monName.ToString());     //몬스터의 이름 저장
                a_JSArr[1].Add(PlayerInfo.MonList[i].starForce);      //몬스터의 starforce 저장
            }
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("MonList", a_JSArr[0].ToString());
        a_form.AddField("MonStar", a_JSArr[1].ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(monListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator UserGoldCo()      //유저 골드 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("UserGold", PlayerInfo.UserGold);

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(goldSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator AutoTimeCo()        //자동 보상 시간 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("AutoExpTime", PlayerInfo.AutoExpTime.ToString("yyyy-MM-dd HH:mm:ss"));

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(autoExpSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator StageCo()      //전투 스테이지 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("CombatStage", PlayerInfo.CombatStage.ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(stageSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator CombatDeckCo()      //전투덱 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;    //정보 저장중으로 변경

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0번은 몬스터 이름 저장하는곳, 1번은 그 몬스터의 성급 저장하는 곳

        for (int i = 0; i < PlayerInfo.CombatDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.CombatDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.CombatStarF[i]);
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("CombatList", a_JSArr[0].ToString());
        a_form.AddField("CombatMonStar", a_JSArr[1].ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(combatSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator DefDeckCo()     //방어덱 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)   //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경
                
        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0번은 몬스터 이름 저장하는곳, 1번은 그 몬스터의 성급 저장하는 곳
                
        for(int i = 0; i < PlayerInfo.DefDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.DefDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.DefStarF[i]);
        }
        
        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("DefMonList", a_JSArr[0].ToString());
        a_form.AddField("DefMonStar", a_JSArr[1].ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(defListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경

        GetComponent<DefDeckMgr>().LoadScene(SceneList.RankLobbyScene);      //랭크 로비씬으로 이동
    }

    private IEnumerator RankDeckCo()    //랭크덱 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)        //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;    //정보 저장중으로 변경

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0번은 몬스터 이름 저장하는곳, 1번은 그 몬스터의 성급 저장하는 곳

        for (int i = 0; i < PlayerInfo.RankDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.RankDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.RankStarF[i]);
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("RankMonName", a_JSArr[0].ToString());
        a_form.AddField("RankMonStar", a_JSArr[1].ToString());

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(rankListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

    private IEnumerator RankingCo()     //랭킹 저장 코루틴
    {
        if (PlayerInfo.UniqueID <= 0)   //올바른 유저가 아니라면
            yield break;

        isNetworkLock = true;   //정보 저장중으로 변경

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("Input_Nick", FindClass.RankNick);

        //--------- 서버 연결 요청
        UnityWebRequest a_www = UnityWebRequest.Post(rankingSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- 서버 연결 요청
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //정보 저장 가능으로 변경
    }

}
