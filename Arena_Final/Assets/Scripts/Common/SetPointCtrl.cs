using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

//몬스터들을 배치하는 곳인 SetPoints오브젝트에서 발생하는 드래그 앤 드랍과 몬스터 정보전달을 컨트롤 하기 위한 스크립트
//이미 각 Point에 배치되어있는 몬스터들을 배치 취소하거나 다른 Point로 옮기는 동작을 한다.
//InGameScene과 DefDeckScene 등 몬스터를 배치하는 곳의 SetPoints 게임오브젝트에 붙여 사용
public class SetPointCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("-------- Common --------")]
    [SerializeField]
    private MonStorage monStore = null;      //MonStorage 연결

    [SerializeField]
    private Transform uiCanvasTr = null;     //UICanvas 게임오브젝트 연결

    [SerializeField]
    private GameObject errorText = null;     //ErrorText 게임오브젝트 연결
    private readonly float errorTime = 2.0f; //ErrorText 출력 시간

    [SerializeField]
    private Material pointMtrl = null;      //Point에 사용되는 Material
    private Color32 defaultPColor = new(0, 0, 0, 255);   //Point의 기본 색상
    private Color32 dragPColor = new(0, 0, 255, 255);    //드래그 상태에서 Point의 색상

    private int startPointIndex = -1;       //드래그를 시작한 Point의 GetSiblingIndex
    private int endPointIndex = -1;         //드래그가 끝난 Point의 GetSiblingIndex

    private GameObject dragGO = null;       //드래그 시 마우스를 따라다닐 DragSlot 게임오브젝트를 저장할 변수

    private MonSlotCtrl[] MSCIndex;         //하위 Point들이 어떤 번호의 MonSlotCtrl과 연결되어있는지 알기 위한 배열
    public MonSlotCtrl GetPointMSC(int PointNum) {  return MSCIndex[PointNum]; }   //외부에서 Point와 연결되어있는 MonSlotCtrl을 가져가기 위한 함수
    
    private Transform[] PointsTr;          //SetPoints 하위의 Point들의 transform
    public Transform this[int index] { get { return PointsTr[index]; } }      //하위 Point들의 transform을 가져가기 위한 인덱서    

    // Start is called before the first frame update
    private void Awake()
    {
        if(errorText != null)
            errorText.SetActive(false);     //오류 텍스트 게임오브젝트 끄기

        PointsTr = new Transform[transform.childCount];    //Point 수만큼 배열 생성
        for (int i = 0; i < PointsTr.Length; i++)          //Point의 순서에 맞게 transform 저장
            PointsTr[i] = transform.GetChild(i);

        MSCIndex = new MonSlotCtrl[transform.childCount];   //Point 수만큼 배열 생성        
        Array.Clear(MSCIndex, 0, MSCIndex.Length);          //배열 초기화

        FindClass.CurSetPoint = this;      //외부에서 이 스크립트를 찾기위해 정적변수에 등록
    }

    #region -------------------- 드래그 앤 드랍 --------------------
    public void OnBeginDrag(PointerEventData eventData)     //드래그 시작 할 때 실행
    {
        //광선을 쏴 정해놓은 Layer와 일치하는 오브젝트 찾기(Point). SetPoint만 감지하도록 layerMask 세팅
        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
            startPointIndex = rayHit.transform.parent.GetSiblingIndex();       //대상(Point)이 몇번 자식인지 가져오기
                
        if (MSCIndex[startPointIndex] == null)  //광선에 맞은 Point에 몬스터가 배치되어있지 않으면 취소
            return;

        PColorChange(true);     //Point색상변경 함수 실행
        dragGO = Instantiate(MSCIndex[startPointIndex].gameObject, Input.mousePosition, Quaternion.identity, uiCanvasTr);   //드래그 슬롯 오브젝트 생성
        dragGO.tag = MonSlotTag.Drag.ToString();                     //태그 변경

        Destroy(PointsTr[startPointIndex].GetChild(1).gameObject);   //선택한 Point에 있는 몬스터 오브젝트 삭제
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGO == null)      //OnBeginDrag가 취소됐다면
            return;

        dragGO.transform.position = Input.mousePosition;    //드래그 슬롯이 마우스를 따라다니게 하기
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGO == null)      //OnBeginDrag가 취소됐다면
            return;

        //몬스터 배치 Point에다 끌어다 놓았으면
        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
        {
            EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.MonCreate); //몬스터 오브젝트 생성 효과음 재생
            endPointIndex = rayHit.transform.parent.GetSiblingIndex();  //드래그가 끝난 Point의 번호를 가져오기
            MonSlotCtrl a_StartMSC = MSCIndex[startPointIndex];         //드래그 시작 Point와 연결되어있던 MonSlotCtrl 임시저장
            MonSlotCtrl a_EndMSC = MSCIndex[endPointIndex];             //드래그 종료 Point와 연결되어있던 MonSlotCtrl 임시저장
            MSCIndex[startPointIndex] = null;                           //드래그 시작 Point와 연결되어있던 MonSlotCtrl 초기화
            MSCIndex[endPointIndex] = null;                             //드래그 종료 Point와 연결되어있던 MonSlotCtrl 초기화
            MonObjCreate(startPointIndex, a_EndMSC);                    //드래그 시작 Point와 연결될 MonSlotCtrl을 바꿔주고 몬스터 생성
            MonObjCreate(endPointIndex, a_StartMSC);                    //드래그 종료 Point와 연결될 MonSlotCtrl을 바꿔주고 몬스터 생성
        }
        else    //배치Point가 아닌 다른곳에 놓았으면 (배치 취소)
        {
            MSCIndex[startPointIndex].UsingSet(false);    //시작 Point와 연결되어있던 MonSlot을 사용중이 아닌 상태로 변경
            MSCIndex[startPointIndex] = null;             //시작 Point와 연결된 MonSlotCtrl이 없도록 변경
        }

        Destroy(dragGO);         //드래그 슬롯 파괴
        PColorChange(false);     //Point색상변경 함수 실행
    }
    #endregion -------------------- 드래그 앤 드랍 --------------------

    #region ------------------- 몬스터 오브젝트 생성 -------------------    
    public void MonObjCreate(int PointNum, MonSlotCtrl SlotCtrl)  //(몬스터를 생성할 Point번호, 정보를가져올 MonSlot번호)
    {
        if (SlotCtrl == null)   //원래 몬스터가 없던 Point와의 변경일 경우(매개변수 SlotCtrl이 Null) 몬스터 생성 막기
            return;

        //-------------- 같은 몬스터 배치 막기 & 같은 몬스터를 같은 자리에 놓을 경우 교체
        for (int i = 0; i < MSCIndex.Length; i++)    //Point와 연결되어있는 모든 MonSlotCtrl에 대해 실행
        {            
            if (MSCIndex[i] == null)        //전체 목록 중 비어있는 곳은 넘어가기
                continue;
            else if (MSCIndex[i].MSCMonStat.monName.Equals(SlotCtrl.MSCMonStat.monName))    //전체 목록 중 비어있지 않은 곳에 배치된 몬스터 이름과 요청받은 몬스터의 이름이 같을 때(같은 몬스터를 배치요청)
                if (i.Equals(PointNum))     //선택한 Point와 몬스터가 배치된 Point가 같은 곳이라면 배치하기(같은 이름의 몬스터가 있는 곳이면 교체하도록 하기 위함)
                    break;
                else  //선택한 Point와 몬스터가 배치된 Point가 같은 곳이아니라면 오류발생
                {
                    StartCoroutine(ErrorOnOff());       //오류 텍스트 출력 코루틴 실행(이미 같은 몬스터가 존재합니다.)
                    EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.fail);      //실패 효과음 재생
                    return;     //몬스터 배치 취소
                }
        }
        //-------------- 같은 몬스터 배치 막기 & 같은 몬스터를 같은 자리에 놓을 경우 교체

        //------------ 몬스터 오브젝트 생성
        if (PointsTr[PointNum].childCount > 1)      //해당 위치에 이미 몬스터가 존재하면
        {
            Destroy(PointsTr[PointNum].GetChild(1).gameObject); //존재하는 몬스터 삭제

            if (MSCIndex[PointNum] != null)         //기존에 배치되어있던 몬스터의 정보를 가진 MonSlot 사용가능 상태로 변경
                MSCIndex[PointNum].UsingSet(false); 
        }

        MSCIndex[PointNum] = SlotCtrl;        //해당 Point의 MonSlotCtrl 변경

        if(SceneManager.GetActiveScene().name.Contains(SceneList.MultiGameScene.ToString()))      //멀티일 경우
            Instantiate(monStore.monstersObj[(int)SlotCtrl.MSCMonStat.monName], PointsTr[PointNum]);     //몬스터 생성
        else    //싱글일 경우
            Instantiate(monStore.monstersObj[(int)SlotCtrl.MSCMonStat.monName], PointsTr[PointNum]).tag = Team.Ally.ToString();     //몬스터 생성

        SlotCtrl.UsingSet(true);        //새로 배치하는 몬스터의 정보를 가진 MonSlot 사용상태로 변경
        //------------ 몬스터 오브젝트 생성
    }
    #endregion ------------------- 몬스터 오브젝트 생성 -------------------

    #region ----------------- 색상 변경, 오류 출력 -----------------
    public void PColorChange(bool Dragging)     //하위 Point들의 색상변경 함수
    {
        pointMtrl.color = Dragging ? dragPColor : defaultPColor;    //드래그 상태에 따라 Point의 색상 변경
    }

    private IEnumerator ErrorOnOff()        //오류 텍스트 출력을 위한 코루틴
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }
    #endregion ----------------- 색상 변경, 오류 출력 -----------------

    #region -------------------------- 멀티 --------------------------
    //전투가 시작될 때 MtGameMgr에서 SetPoint를 오프시키는데 그때 실행된다. 이 SetPoint대신 켜지는 CombatSetPoint의 MtCbtSetPoint 스크립트에서 정보를 가져가 동작
    private void OnDisable()
    {
        if (!SceneManager.GetActiveScene().name.Contains(SceneList.MultiGameScene.ToString()))        //멀티만 실행
            return;

        int[] a_MonArrName = new int[PointsTr.Length];     //현재 배치된 몬스터들의 이름 배열. Point게임 오브젝트의 숫자만큼 생성(6개)
        int[] a_MonArrStar = new int[PointsTr.Length];     //현재 배치된 몬스터들의 성급 배열. Point게임 오브젝트의 숫자만큼 생성(6개)

        for (int i = 0; i < PointsTr.Length; i++)
        {
            if (PointsTr[i].childCount > 1)     //몬스터가 배치된 Point
            {
                MonsterStat a_MonStat = MSCIndex[i].MSCMonStat;       //이 Point와 연결되어있는 MonSlotCtrl에서 MonStat을 받아오기
                a_MonArrName[i] = (int)a_MonStat.monName;             //몬스터의 이름을 이 Point의 번호에 맞게 int형식으로 저장
                a_MonArrStar[i] = a_MonStat.starForce;                //몬스터의 성급을 이 Point의 번호에 맞게 저장
                Destroy(PointsTr[i].GetChild(1).gameObject);          //몬스터 오브젝트 삭제
            }
            else//(Tr.childCount <= 1)  //몬스터가 배치되지 않은 Point
            {
                a_MonArrName[i] = -1;
                a_MonArrStar[i] = -1;
            }
        }
        Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(new() { { "MonArrName", a_MonArrName }, { "MonArrStar", a_MonArrStar } });     //로컬 플레이어의 정보 추가
    }
    #endregion -------------------------- 멀티 --------------------------

    private void OnDestroy()
    {
        StopAllCoroutines();        //파괴될 때 모든 코루틴 종료
    }
}
