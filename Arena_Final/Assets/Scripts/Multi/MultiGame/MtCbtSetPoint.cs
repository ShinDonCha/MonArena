using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//멀티에서 해당진영(Team1, Team2)에 맞는 몬스터를 생성하기위한 SetPoint
//Resources폴더의 CombatSetP1, CombatSetP2에 붙여서 사용
//MtGameMgr의 CombatReady()에서 배치를 위한 SetPoint가 꺼지면서 생성, 배치된 몬스터의 정보가 전달된다.
public class MtCbtSetPoint : MonoBehaviourPunCallbacks
{
    private PhotonView photonV = null;
    private Transform[] pointTr;    //하위 Point들의 Transform을 담을 배열변수

    private void Awake()
    {
        photonV = GetComponent<PhotonView>();

        pointTr = new Transform[transform.childCount];      //하위 Point 수만큼 배열의 수 설정

        for(int i = 0; i < pointTr.Length; i++)             //하위 Point들의 Transform 저장
            pointTr[i] = transform.GetChild(i);
    }

    //마스터와 클라이언트가 같은 방에 있기 때문에 마스터 or 클라이언트 둘중 하나의 정보만 변경되어도 CombatSetP1과 CombatSetP2모두에게 실행된다.
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //----------------- 몬스터 오브젝트 생성 -----------------
        if (!changedProps.ContainsKey("MonArrName"))      //MultiGameScene의 StandBySetPoint가 OnDisable될 때 받아온 몬스터 배치 정보일 경우만 실행
            return;

        //CombatSetP1는 마스터의 몬스터만 담당, CombatSetP2는 클라이언트의 몬스터만 담당, 마스터와 클라이언트 모두 이 함수를 사용하여 몬스터를 배치하므로 자신에게 맞는 로컬에서만 실행하도록 해야함
        if (targetPlayer.Equals(photonV.Owner) && photonV.IsMine)       //각 CombatSetP에 맞는 사용자인지 확인
        {
            int[] a_MonArrName = (int[])changedProps["MonArrName"];     //배치된 몬스터의 이름 배열 받아오기
            int[] a_MonArrStar = (int[])changedProps["MonArrStar"];     //배치된 몬스터의 성급 배열 받아오기

            for (int i = 0; i < a_MonArrName.Length; i++)   //배치된 몬스터 수만큼 실행
            {
                if (a_MonArrName[i] == -1)  //몬스터 정보가 없는 Index면 넘어가기
                    continue;

                GameObject a_GO;

                if (photonV.Owner.Equals(PhotonNetwork.MasterClient))       //소유주가 마스터인 경우(이 오브젝트가 CombatSetP1인 경우)
                    a_GO = PhotonNetwork.Instantiate(((MonsterName)a_MonArrName[i]).ToString(), transform.GetChild(i).position, Quaternion.Euler(new(0, 90, 0)));      //몬스터 오브젝트 생성
                else                                                        //소유주가 클라이언트인 경우(이 오브젝트가 CombatSetP2인 경우)
                    a_GO = PhotonNetwork.Instantiate(((MonsterName)a_MonArrName[i]).ToString(), transform.GetChild(i).position, Quaternion.Euler(new(0, -90, 0)));     //몬스터 오브젝트 생성

                a_GO.GetComponent<CmnMonSet>().SetMonBasics(a_MonArrName[i], a_MonArrStar[i], PhotonNetwork.IsMasterClient ? Team.Team1 : Team.Team2);  //몬스터 기본스탯 설정, 팀배치 함수 실행
            }
        }
        //----------------- 몬스터 오브젝트 생성 -----------------          
    }
}
