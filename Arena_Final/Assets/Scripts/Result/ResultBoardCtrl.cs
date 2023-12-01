using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

//ResultScene에서 전투결과를 표시하는 Board의 컨트롤 스크립트(ResultScene과 MultiResultScene에서 사용)
//ResultScene에서는 RightBoard만, MultiResultScene에서는 LeftBoard, RightBoard에 붙여서 사용
public class ResultBoardCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject monPartPrefab = null; //하위에 생성할 몬스터 셀 프리팹

    //--------------- 몬스터들의 전투결과를 담고있는 변수(Start에서 생성되는 MonPart에서 값을 가져감)
    public int[] CmnListNameNum { get; private set; }   //전투결과를 표시해주기위해 이전씬에서 가져온 몬스터 정보(이름)
    public int[] CmnListDmg { get; private set; }       //전투결과를 표시해주기위해 이전씬에서 가져온 몬스터 정보(가한 대미지)
    public int[] CmnListHP { get; private set; }        //전투결과를 표시해주기위해 이전씬에서 가져온 몬스터 정보(받은 대미지)

    public int TotalMonDmg { get; private set; }   //위 배열의 몬스터들이 입힌 총 대미지
    public int TotalMonHP { get; private set; }    //위 배열의 몬스터들이 받은 총 피해량
    //--------------- 몬스터들의 전투결과를 담고있는 변수(Start에서 생성되는 MonPart에서 값을 가져감)

    private void Awake()
    {
        if (!Enum.TryParse(tag, out Team TeamTag))      //ResultScene(싱글)에서는 Ally, MultiResultScene(멀티)에서는 Team1, Team2
            return;

        switch (TeamTag)
        {
            //------------- 싱글(InGameScene, RankGameScene 공통)
            case Team.Ally:
                List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //이 게임오브젝트의 태그에 맞는CmnMonCtrl 리스트를 가져오기

                CmnListNameNum = new int[a_CMCList.Count];       //몬스터의 수만큼 배열 생성
                CmnListDmg = new int[a_CMCList.Count];           //몬스터의 수만큼 배열 생성
                CmnListHP = new int[a_CMCList.Count];            //몬스터의 수만큼 배열 생성

                for (int i = 0; i < CmnListNameNum.Length; i++)   //몬스터의 정보를 가져와서 저장
                {
                    CmnListNameNum[i] = (int)a_CMCList[i].CMCmonStat.monName;
                    CmnListDmg[i] = a_CMCList[i].TotalDmg;
                    CmnListHP[i] = a_CMCList[i].TotalHP;
                }
                break;
            //------------- 싱글(InGameScene, RankGameScene 공통)

            //------------ 멀티의 경우 CustomProperties에 저장된 정보를 가져오기
            case Team.Team1:    //LeftBoard일 경우
                CmnListNameNum = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnNameNum"];
                CmnListDmg = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnDmg"];
                CmnListHP = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnHP"];
                break;

            case Team.Team2:   //RightBoard일 경우
                CmnListNameNum = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnNameNum"];
                CmnListDmg = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnDmg"];
                CmnListHP = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnHP"];
                break;
           //------------ 멀티의 경우 CustomProperties에 저장된 정보를 가져오기
        }
    }    

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < CmnListDmg.Length; i++)
        {
            TotalMonDmg += CmnListDmg[i];   //몬스터가 입힌 총 피해량
            TotalMonHP += CmnListHP[i];     //몬스터가 받은 총 피해량
            Instantiate(monPartPrefab, transform);      //몬스터 점수판(MonPart) 생성
        }
    }
}
