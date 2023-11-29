using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;
using SimpleJSON;

//포톤서버연결, 방생성과 관련된 동작을 하는 스크립트
//MultiLobbyScene MultiLobbyMgr에 붙여서 사용
public class MtLobbyMgr : MonoBehaviourPunCallbacks, IButtonClick
{
    [SerializeField]
    private GameObject roomPrefab = null;       //룸 프리팹
    [SerializeField]
    private Transform contentTr = null;         //룸 프리팹 생성할 Transform
    [SerializeField]
    private Text notifiText = null;             //서버연결상태 알림용 텍스트
    [SerializeField]
    private Text errorText = null;              //에러 알림용 텍스트

    private readonly string roomDataURL = "http://dhosting.dothome.co.kr/Arena/RoomData.php";     //방 데이터 연결 주소

    private void Awake()
    {        
        errorText.text = "";
        BGMController.Instance.BGMChange(BGMList.Lobby);        //배경음 변경

        if (!PhotonNetwork.IsConnected)     //포톤 클라우드에 접속해있는 상태가 아니라면
        {
            notifiText.text = "서버 연결 시도 중, 잠시만 기다려주세요.";            
            PhotonNetwork.ConnectUsingSettings();   //포톤 클라우드에 접속
        }
        else //포톤 클라우드에 접속해있는 상태라면
        {
            PhotonNetwork.JoinLobby();      //로비 입장
        }
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CreatRoomButton:    //방만들기 버튼 클릭 시
                PhotonNetwork.LocalPlayer.NickName = PlayerInfo.UserNick;   //로컬 플레이어의 이름을 설정

                //방 생성
                RoomOptions a_RO = new()
                {
                    IsOpen = true,     //입장 가능 여부
                    IsVisible = true,  //방의 노출 여부
                    MaxPlayers = 2,    //최대 입장가능한 인원
                    CustomRoomProperties = new()
                    {
                       { "MasterNick", PhotonNetwork.LocalPlayer.NickName },
                    },   
                    CustomRoomPropertiesForLobby = new string[] { "MasterNick" }    //로비에서 사용할 Hashtable 설정
                };
                PhotonNetwork.CreateRoom("Room" + UnityEngine.Random.Range(0, 999), a_RO);     //방 생성
                break;

            case ButtonList.BackButton:         //돌아가기 버튼 클릭 시
                PhotonNetwork.Disconnect();     //포톤 연결 종료
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());    //씬 이동
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    public override void OnConnected()  //포톤 클라우드 연결 콜백함수
    {
        notifiText.text = "연결 성공";
    }

    public override void OnConnectedToMaster()      //마스터 연결 콜백함수
    {
        notifiText.text = "서버 접속 완료";

        PhotonNetwork.JoinLobby();      //로비 입장
    }

    public override void OnJoinedLobby()   //로비 접속 콜백함수
    {
        notifiText.text = "로비 접속완료. 이제 방을 생성하거나 방에 접속할 수 있습니다.";
    }

    public override void OnJoinedRoom()     //방 입장 콜백함수
    {
        SceneManager.LoadScene(SceneList.MultiGameScene.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)   //방 생성 실패 콜백함수
    {
        StartCoroutine(ErrorTextCo("방 생성 실패! 같은 이름의 방이 이미 존재합니다."));       //방 생성 실패 텍스트 출력
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)    //방 리스트 업데이트 콜백함수
    {
        foreach (RoomInfo RI in roomList)
        {
            for (int i = 0; i < contentTr.childCount; i++)      //전체 방 오브젝트 개수만큼 반복
                if (contentTr.GetChild(i).name.Equals(RI.Name)) Destroy(contentTr.GetChild(i).gameObject);  //해당 방 오브젝트가 이미 존재할경우 삭제

            if (!RI.RemovedFromList) StartCoroutine(CreatRoomObj(RI));     //삭제 대기중이 아닌 방(들어갈 수 있는 방)이면 방 오브젝트 생성
        }
    }

    private IEnumerator CreatRoomObj(RoomInfo RInfo)     //방 오브젝트 생성 코루틴 실행
    {
        WWWForm a_form = new();
        a_form.AddField("MNick", RInfo.CustomProperties["MasterNick"].ToString());      //방 마스터의 닉네임 전달

        UnityWebRequest a_www = UnityWebRequest.Post(roomDataURL, a_form);
        yield return a_www.SendWebRequest();
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)        //서버연결에 오류가 있으면 코루틴 종료
            yield break;

        Encoding a_Encode = Encoding.UTF8;
        string a_GetStr = a_Encode.GetString(a_www.downloadHandler.data);   //PHP를 통해 받아온 정보를 문자열로 변경

        if (!a_GetStr.Contains("Success"))   //정보를 가져오는데 실패했다면 코루틴 종료
            yield break;

        JSONNode a_JSON = JSON.Parse(a_GetStr);     //문자열을 JSON형식으로 파싱

        if (a_JSON == null)      //변경한 것에 문제가 있으면 코루틴 종료
            yield break;

        GameObject a_GO = Instantiate(roomPrefab, contentTr);     //방 프리팹 생성
        a_GO.GetComponent<RoomListCtrl>().DispRoomData(RInfo, a_JSON);      //노출되는 방의 정보 변경
    }
    
    private IEnumerator ErrorTextCo(string Msg)  //에러 텍스트 표시용 코루틴
    {
        errorText.text = Msg;      //메시지 출력
        yield return new WaitForSeconds(2.0f);     //2초 후 실행
        errorText.text = "";       //메시지 초기화
    }
}
