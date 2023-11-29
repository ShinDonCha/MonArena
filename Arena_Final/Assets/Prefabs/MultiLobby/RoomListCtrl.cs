using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using SimpleJSON;

//이 방의 정보를 보여주기위한 스크립트
//RoomListPrefab에 붙여서 사용
public class RoomListCtrl : MonoBehaviour, IButtonClick
{
    [SerializeField]
    private Text nickText = null;           //방 마스터의 닉네임을 표시해주기 위한 텍스트    
    [SerializeField]
    private Text roomPNumText = null;       //방의 참가인원을 보여주기 위한 텍스트
    [SerializeField]
    private MonStorage monStore = null;     //몬스터 이미지를 가져올 저장소
    [SerializeField]
    private Transform monstersTr = null;    //몬스터 이미지를 넣을 transform

    public void ButtonOnClick(Button PushBtn)       //버튼 클릭시 실행되는 함수 (오브젝트의 OnClick()으로 실행)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CombatButton:           //방 입장버튼 클릭 시
                PhotonNetwork.LocalPlayer.NickName = PlayerInfo.UserNick;   //로컬 플레이어의 닉네임 저장
                PhotonNetwork.JoinRoom(name);   //방 입장
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    public void DispRoomData(RoomInfo ThisRoomInfo, JSONNode MonJSON)    //다른사람에게 보여지는 방의 정보를 설정하는 함수 (MtLobbyMgr에서 방 생성과 함께 실행)
    {
        name = ThisRoomInfo.Name;    //이 오브젝트의 이름을 방의 실제 이름으로 변경
        nickText.text = "마스터 : " + ThisRoomInfo.CustomProperties["MasterNick"].ToString();            //상대에게 표시해줄 방의 이름 변경 (방 마스터의 이름)
        roomPNumText.text = "참가 인원 : " + ThisRoomInfo.PlayerCount + " / " + ThisRoomInfo.MaxPlayers; //방의 인원 표시

        if (MonJSON["MonList"] == null)     //표시해줄 몬스터가 하나도없으면(유저가 몬스터를 1개도 보유하고있지않으면) 취소
            return;

        JSONNode a_MLNode = JSON.Parse(MonJSON["MonList"]);    //받아온 방 마스터의 전체 몬스터 정보를 몬스터1, 몬스터2, 몬스터3....으로 변경
        JSONNode a_MSNode = JSON.Parse(MonJSON["MonStar"]);    //받아온 방 마스터의 전체 몬스터 성급 정보를 몬스터1의 성급, 몬스터2의 성급, 몬스터3의 성급....으로 변경

        for (int i = 0; i < Mathf.Min(a_MLNode.Count, 5); i++)    //몬스터 이미지 생성 (방 마스터가 보유한 몬스터의 수와 5칸 중 적은 수만큼 실행)
        {
            GameObject a_GO = Instantiate(monStore.monSlot[a_MSNode[i]], monstersTr);  //성급에 맞는 몬스터 이미지 생성
            a_GO.tag = MonSlotTag.NameParse.ToString();                                //이미지의 태그 변경
            a_GO.name = ((int)Enum.Parse<MonsterName>(a_MLNode[i])).ToString();        //이미지 오브젝트의 이름 변경
        }
    }
}
