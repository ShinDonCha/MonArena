using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using SimpleJSON;

//�� ���� ������ �����ֱ����� ��ũ��Ʈ
//RoomListPrefab�� �ٿ��� ���
public class RoomListCtrl : MonoBehaviour, IButtonClick
{
    [SerializeField]
    private Text nickText = null;           //�� �������� �г����� ǥ�����ֱ� ���� �ؽ�Ʈ    
    [SerializeField]
    private Text roomPNumText = null;       //���� �����ο��� �����ֱ� ���� �ؽ�Ʈ
    [SerializeField]
    private MonStorage monStore = null;     //���� �̹����� ������ �����
    [SerializeField]
    private Transform monstersTr = null;    //���� �̹����� ���� transform

    public void ButtonOnClick(Button PushBtn)       //��ư Ŭ���� ����Ǵ� �Լ� (������Ʈ�� OnClick()���� ����)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CombatButton:           //�� �����ư Ŭ�� ��
                PhotonNetwork.LocalPlayer.NickName = PlayerInfo.UserNick;   //���� �÷��̾��� �г��� ����
                PhotonNetwork.JoinRoom(name);   //�� ����
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    public void DispRoomData(RoomInfo ThisRoomInfo, JSONNode MonJSON)    //�ٸ�������� �������� ���� ������ �����ϴ� �Լ� (MtLobbyMgr���� �� ������ �Բ� ����)
    {
        name = ThisRoomInfo.Name;    //�� ������Ʈ�� �̸��� ���� ���� �̸����� ����
        nickText.text = "������ : " + ThisRoomInfo.CustomProperties["MasterNick"].ToString();            //��뿡�� ǥ������ ���� �̸� ���� (�� �������� �̸�)
        roomPNumText.text = "���� �ο� : " + ThisRoomInfo.PlayerCount + " / " + ThisRoomInfo.MaxPlayers; //���� �ο� ǥ��

        if (MonJSON["MonList"] == null)     //ǥ������ ���Ͱ� �ϳ���������(������ ���͸� 1���� �����ϰ�����������) ���
            return;

        JSONNode a_MLNode = JSON.Parse(MonJSON["MonList"]);    //�޾ƿ� �� �������� ��ü ���� ������ ����1, ����2, ����3....���� ����
        JSONNode a_MSNode = JSON.Parse(MonJSON["MonStar"]);    //�޾ƿ� �� �������� ��ü ���� ���� ������ ����1�� ����, ����2�� ����, ����3�� ����....���� ����

        for (int i = 0; i < Mathf.Min(a_MLNode.Count, 5); i++)    //���� �̹��� ���� (�� �����Ͱ� ������ ������ ���� 5ĭ �� ���� ����ŭ ����)
        {
            GameObject a_GO = Instantiate(monStore.monSlot[a_MSNode[i]], monstersTr);  //���޿� �´� ���� �̹��� ����
            a_GO.tag = MonSlotTag.NameParse.ToString();                                //�̹����� �±� ����
            a_GO.name = ((int)Enum.Parse<MonsterName>(a_MLNode[i])).ToString();        //�̹��� ������Ʈ�� �̸� ����
        }
    }
}
