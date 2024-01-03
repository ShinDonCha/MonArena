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

//���漭������, ������� ���õ� ������ �ϴ� ��ũ��Ʈ
//MultiLobbyScene MultiLobbyMgr�� �ٿ��� ���
public class MtLobbyMgr : MonoBehaviourPunCallbacks, IButtonClick
{
    [SerializeField]
    private GameObject roomPrefab = null;       //�� ������
    [SerializeField]
    private Transform contentTr = null;         //�� ������ ������ Transform
    [SerializeField]
    private Text notifiText = null;             //����������� �˸��� �ؽ�Ʈ
    [SerializeField]
    private Text errorText = null;              //���� �˸��� �ؽ�Ʈ

    private readonly string roomDataURL = "http://dhosting.dothome.co.kr/Arena/RoomData.php";     //�� ������ ���� �ּ�

    private void Awake()
    {        
        errorText.text = "";
        BGMController.Instance.BGMChange(BGMList.Lobby);        //����� ����

        if (!PhotonNetwork.IsConnected)     //���� Ŭ���忡 �������ִ� ���°� �ƴ϶��
        {
            notifiText.text = "���� ���� �õ� ��, ��ø� ��ٷ��ּ���.";            
            PhotonNetwork.ConnectUsingSettings();   //���� Ŭ���忡 ����
        }
        else //���� Ŭ���忡 �������ִ� ���¶��
        {
            PhotonNetwork.JoinLobby();      //�κ� ����
        }
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CreatRoomButton:    //�游��� ��ư Ŭ�� ��
                PhotonNetwork.LocalPlayer.NickName = PlayerInfo.UserNick;   //���� �÷��̾��� �̸��� ����

                //�� ����
                RoomOptions a_RO = new()
                {
                    IsOpen = true,     //���� ���� ����
                    IsVisible = true,  //���� ���� ����
                    MaxPlayers = 2,    //�ִ� ���尡���� �ο�
                    CustomRoomProperties = new()
                    {
                       { "MasterNick", PhotonNetwork.LocalPlayer.NickName },
                    },   
                    CustomRoomPropertiesForLobby = new string[] { "MasterNick" }    //�κ񿡼� ����� Hashtable ����
                };
                PhotonNetwork.CreateRoom("Room" + UnityEngine.Random.Range(0, 999), a_RO);     //�� ����
                break;

            case ButtonList.BackButton:         //���ư��� ��ư Ŭ�� ��
                PhotonNetwork.Disconnect();     //���� ���� ����
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());    //�� �̵�
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    public override void OnConnected()  //���� Ŭ���� ���� �ݹ��Լ�
    {
        notifiText.text = "���� ����";
    }

    public override void OnConnectedToMaster()      //������ ���� �ݹ��Լ�
    {
        notifiText.text = "���� ���� �Ϸ�";

        PhotonNetwork.JoinLobby();      //�κ� ����
    }

    public override void OnJoinedLobby()   //�κ� ���� �ݹ��Լ�
    {
        notifiText.text = "�κ� ���ӿϷ�. ���� ���� �����ϰų� �濡 ������ �� �ֽ��ϴ�.";
    }

    public override void OnJoinedRoom()     //�� ���� �ݹ��Լ�
    {
        SceneManager.LoadScene(SceneList.MultiGameScene.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)   //�� ���� ���� �ݹ��Լ�
    {
        StartCoroutine(ErrorTextCo("�� ���� ����! ���� �̸��� ���� �̹� �����մϴ�."));       //�� ���� ���� �ؽ�Ʈ ���
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)    //�� ����Ʈ ������Ʈ �ݹ��Լ�
    {
        foreach (RoomInfo RI in roomList)
        {
            for (int i = 0; i < contentTr.childCount; i++)      //��ü �� ������Ʈ ������ŭ �ݺ�
                if (contentTr.GetChild(i).name.Equals(RI.Name)) Destroy(contentTr.GetChild(i).gameObject);  //�ش� �� ������Ʈ�� �̹� �����Ұ�� ����

            if (!RI.RemovedFromList) StartCoroutine(CreatRoomObj(RI));     //���� ������� �ƴ� ��(�� �� �ִ� ��)�̸� �� ������Ʈ ����
        }
    }

    private IEnumerator CreatRoomObj(RoomInfo RInfo)     //�� ������Ʈ ���� �ڷ�ƾ ����
    {
        WWWForm a_form = new();
        a_form.AddField("MNick", RInfo.CustomProperties["MasterNick"].ToString());      //�� �������� �г��� ����

        UnityWebRequest a_www = UnityWebRequest.Post(roomDataURL, a_form);
        yield return a_www.SendWebRequest();
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)        //�������ῡ ������ ������ �ڷ�ƾ ����
            yield break;

        Encoding a_Encode = Encoding.UTF8;
        string a_GetStr = a_Encode.GetString(a_www.downloadHandler.data);   //PHP�� ���� �޾ƿ� ������ ���ڿ��� ����

        if (!a_GetStr.Contains("Success"))   //������ �������µ� �����ߴٸ� �ڷ�ƾ ����
            yield break;

        JSONNode a_JSON = JSON.Parse(a_GetStr);     //���ڿ��� JSON�������� �Ľ�

        if (a_JSON == null)      //������ �Ϳ� ������ ������ �ڷ�ƾ ����
            yield break;

        GameObject a_GO = Instantiate(roomPrefab, contentTr);     //�� ������ ����
        a_GO.GetComponent<RoomListCtrl>().DispRoomData(RInfo, a_JSON);      //����Ǵ� ���� ���� ����
    }
    
    private IEnumerator ErrorTextCo(string Msg)  //���� �ؽ�Ʈ ǥ�ÿ� �ڷ�ƾ
    {
        errorText.text = Msg;      //�޽��� ���
        yield return new WaitForSeconds(2.0f);     //2�� �� ����
        errorText.text = "";       //�޽��� �ʱ�ȭ
    }
}
