using UnityEngine.UI;
using UnityEngine;
using System;
using Photon.Pun;

//버프&디버프, 대미지 등 전투 시 몬스터가 받는 효과의 텍스트를 출력하기위한 스크립트
//이 스크립트에 있는 함수는 정적 클래스인 TextRequest 클래스의 액션을 통해 실행한다.(여기는 등록만 해주는 것)
//전투화면이 있는 각 씬의 CombatUI -> TextArea 게임오브젝트에 붙여서 사용
public class TextAreaCtrl : MonoBehaviour
{
    [Header("----------- 싱글 -----------")]
    //-------------- 싱글(GameMgr)에서만 사용
    [SerializeField]
    private Transform playerTabTr = null;       //하위 Player 게임오브젝트의 Transform(유저 몬스터가 받는 효과 텍스트 담당)
    [SerializeField]
    private Transform enemyTabTr = null;        //하위 Enemy 게임오브젝트의 Transform(적 몬스터가 받는 효과 텍스트 담당)
    //-------------- 싱글(GameMgr)에서만 사용

    [Header("----------- 멀티 -----------")]
    //-------------- 멀티(MtGameMgr)에서만 사용
    [SerializeField]
    private Transform team1TabTr = null;        //하위 Team1 게임오브젝트의 Transform(멀티에서 Team1의 몬스터가 받는 효과 텍스트 담당)
    [SerializeField]
    private Transform team2TabTr = null;        //하위 Team2 게임오브젝트의 Transform(멀티에서 Team2의 몬스터가 받는 효과 텍스트 담당)
    //-------------- 멀티(MtGameMgr)에서만 사용

    [Header("----------- 공통 -----------")]
    [SerializeField]
    private GameObject dmgTextPrefab = null;    //대미지 텍스트를 출력해주는 프리팹
    [SerializeField]
    private GameObject healTextPrefab = null;   //힐 텍스트를 출력해주는 프리팹

    private PhotonView PhotonV = null;          //멀티에서 필요한 Photon View를 담을 변수

    private void Awake()
    {
        PhotonV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TextRequest.BuffTxtReqAct = BuffTextReq;        //버프&디버프 텍스트 출력 액션에 함수 추가
        TextRequest.InstantTxtReqAct = HealTextReq;     //힐&대미지 텍스트 출력 액션에 함수 추가
    }

    #region ----------------- 힐&대미지 텍스트 요청 함수 -----------------
    //힐&대미지 텍스트 요청 함수
    private void HealTextReq(Vector3 ObjPos, TxtAnimList AnimList, int Value)     //1회용 텍스트 요청 함수
    {
        if (PhotonV == null)        //싱글
            HealTextInstant(ObjPos, AnimList, Value);       //1회용 텍스트 생성 함수 실행
        else//(PhotonV != null)     //멀티
            PhotonV.RPC("HealTextInstant", RpcTarget.All, ObjPos, AnimList, Value);  //로컬과 원격에 1회용 텍스트 생성 함수 실행
    }

    [PunRPC]
    private void HealTextInstant(Vector3 ObjPos, TxtAnimList AnimList, int Value)       //1회용 텍스트 생성 함수
    {
        Vector2 a_PosVtr2 = Camera.main.WorldToScreenPoint(new Vector3(ObjPos.x, ObjPos.y + 1.8f, ObjPos.z));   //텍스트를 생성할 World상의 위치

        GameObject a_GO;    //텍스트 오브젝트를 담을 변수

        switch (AnimList)
        {
            case TxtAnimList.DmgTxtAnim:
                if (dmgTextPrefab == null)    //대미지 텍스트 출력 프리팹을 등록하지 않았으면 취소(오류 방지)
                    return;

                a_GO = Instantiate(dmgTextPrefab, a_PosVtr2, Quaternion.identity, transform);      //대미지 텍스트 생성
                a_GO.GetComponentInChildren<Text>().text = Value + " Dmg";  //대미지 표기 변경
                Destroy(a_GO, 0.5f);        //일정시간 뒤에 텍스트 오브젝트 삭제
                break;

            case TxtAnimList.HealTxtAnim:
                if (healTextPrefab == null)   //힐 텍스트 출력 프리팹을 등록하지 않았으면 취소(오류 방지)
                    return;

                a_GO = Instantiate(healTextPrefab, a_PosVtr2, Quaternion.identity, transform);      //힐 텍스트 생성
                a_GO.GetComponentInChildren<Text>().text = "+ " + Value + " HP";  //힐량 표기 변경
                Destroy(a_GO, 0.8f);        //일정시간 뒤에 텍스트 오브젝트 삭제
                break;
        }
    }
    #endregion ----------------- 힐&대미지 텍스트 요청 함수 -----------------

    #region ----------------- 버프&디버프 텍스트 요청 함수 -----------------
    //버프&디버프 텍스트 요청 함수
    private void BuffTextReq(string TeamTag, int SiblingIndex, Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)      //(팀태그, 몇번째 자식인지, 몬스터 오브젝트의 위치, 버프or디버프, 상태변화 종류)
    {
        if (!Enum.TryParse(TeamTag, out Team TeamName))
            return;

        if (PhotonV == null)        //싱글
            BuffTextPlay(TeamName, SiblingIndex, ObjPos, AnimList, Act);    //버프&디버프 텍스트 생성 함수 실행
        else//(PhotonV != null)     //멀티
            PhotonV.RPC("BuffTextPlay", RpcTarget.All, TeamName, SiblingIndex, ObjPos, AnimList, Act);  //로컬과 원격에 버프&디버프 텍스트 생성 함수 실행
    }

    [PunRPC]
    private void BuffTextPlay(Team TeamName, int SiblingIndex, Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)      //버프&디버프 텍스트 생성 함수
    {
        Transform a_ReqTr = null;

        switch (TeamName)
        {
            case Team.Ally:
                a_ReqTr = playerTabTr;
                break;

            case Team.Enemy:
                a_ReqTr = enemyTabTr;
                break;

            case Team.Team1:
                a_ReqTr = team1TabTr;
                break;

            case Team.Team2:
                a_ReqTr = team2TabTr;
                break;
        }

        //switch를 통해 얻어온 게임 오브젝트 하위의 BuffTextRoot의 BuffTextRootCtrl에 접근해 텍스트 생성 함수 실행
        a_ReqTr.GetChild(SiblingIndex).GetComponent<BuffTextRootCtrl>().AnimPlay(ObjPos, AnimList, Act);
    }
    #endregion ----------------- 버프&디버프 텍스트 요청 함수 -----------------
}
