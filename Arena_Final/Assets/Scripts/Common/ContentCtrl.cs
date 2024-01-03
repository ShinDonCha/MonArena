using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//몬스터 슬롯을 드래그앤 드랍 하기 위한 스크립트
//스크롤뷰 하위의 Content오브젝트에 붙여서 사용
//InGameScene, DefDeckScene, MultiGameScene 등 몬스터 배치하는곳에 Content가 있음
public class ContentCtrl : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private MonStorage monStore = null;    //MonStorage 연결

    [SerializeField]
    private Transform uiCanvasTr = null;   //UICanvas 게임오브젝트 연결

    private int startSlotIndex = -1;       //드래그를 시작한 MonSlot의 GetSiblingIndex
    private int endPointIndex = -1;        //드래그가 끝난 Point의 GetSiblingIndex

    private GameObject dragGO = null;      //드래그 시 마우스를 따라다닐 DragSlot을 저장할 변수

    private bool isDrag = false;           //현재 MonSlot을 드래그중인지 확인하기위한 변수
    private ScrollRect parentScroll = null;     //부모(ScrollRect View)의 ScrollRect 컴포넌트를 가져오기위한 변수
    private float calcHeight = 0.0f;       //DragSlot을 생성할 높이를 계산한 값

    private void Awake()
    {
        FindClass.CurContent = this;       //현재 씬의 Content오브젝트의 ContentCtrl을 저장
        parentScroll = GetComponentInParent<ScrollRect>();  //부모이 ScrollRect 컴포넌트 가져오기        
    }

    private void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)        //유저가 보유한 몬스터 수만큼 실행
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();    //몬스터 슬롯 생성 및 태그변경              

        calcHeight = (Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0)).y * GetComponent<RectTransform>().rect.height) / 720;     //DragSlot 생성을 위한 높이 계산

        StartCoroutine(SaveDeckSettings());     //저장된 덱 배치 함수 실행
    }

    private void Update()
    {
        if (!isDrag)     //마우스가 MonSlot을 드래그하는 상태가 아니라면
            return;

        if (Input.mousePosition.y >= calcHeight && dragGO == null)   //마우스가 이 오브젝트(Content)보다 위쪽에 있고, 드래그 슬롯이 생성되지 않았다면
        {
            parentScroll.enabled = false;       //Scroll View의 ScrollRect 끄기(몬스터를 배치할때 Scroll View가 움직이는걸 막기 위함)
            FindClass.CurSetPoint.PColorChange(true);   //Point의 색상을 드래그 상태의 색상으로 변경
            dragGO = Instantiate(transform.GetChild(startSlotIndex).gameObject, Input.mousePosition, Quaternion.identity, uiCanvasTr);   //게임오브젝트 생성
            dragGO.tag = MonSlotTag.Drag.ToString();    //태그 변경
        }
        else if (dragGO != null)        //드래그 슬롯이 생성된 상태라면
            dragGO.transform.position = Input.mousePosition;    //드래그 슬롯이 마우스를 따라다니게 하기

        if (Input.GetMouseButtonUp(0))       //마우스 왼쪽버튼을 떼면
        {
            isDrag = false;     //드래그 상태 취소
            parentScroll.enabled = true;   //Scroll Rect를 드래그 가능하도록 변경

            if (dragGO == null)     //드래그 슬롯이 비어있는 상태면 취소(드래그 슬롯을 안만들고 그냥 마우스를 눌렀다 떼는 경우)
                return;

            Destroy(dragGO);    //드래그 슬롯 파괴
            FindClass.CurSetPoint.PColorChange(false);   //Point의 색상을 기본 상태의 색상으로 변경

            //드래그 종료 위치가 Point라면
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
            {
                endPointIndex = rayHit.transform.parent.GetSiblingIndex();  //드래그가 끝난 Point의 번호를 가져오기
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.MonCreate); //몬스터 오브젝트 생성 효과음 재생
                FindClass.CurSetPoint.MonObjCreate(endPointIndex, transform.GetChild(startSlotIndex).GetComponent<MonSlotCtrl>());  //몬스터 오브젝트 생성
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;          //드래그 중 상태로 변경
        startSlotIndex = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //처음 선택한 몬스터슬롯의 자식번호 가져오기
    }

    private IEnumerator SaveDeckSettings()   //저장된 덱 배치 함수
    {
        if (!System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneList curScene))  //현재 씬의 이름을 SceneList 형식으로 변환(실패 시 코루틴 종료)
            yield break;

        yield return new WaitForFixedUpdate();      //Update() 전에 실행되도록

        List<MonsterName> a_Deck;           //저장된 덱 리스트를 받을 변수
        List<int> a_StarF;                  //저장된 덱의 성급 리스트를 받을 변수
        Stack<Transform> a_MonSlotStack = new();    //저장된 덱과 일치하는 몬스터 정보를 가진 MonSlot의 Transform을 저장할 변수

        switch (curScene)        //각 씬의 맞는 덱 가져오기
        {
            case SceneList.InGameScene:
                a_Deck = PlayerInfo.CombatDeck;
                a_StarF = PlayerInfo.CombatStarF;
                break;

            case SceneList.RankGameScene:
                a_Deck = PlayerInfo.RankDeck;
                a_StarF = PlayerInfo.RankStarF;
                break;

            case SceneList.DefDeckScene:
                a_Deck = PlayerInfo.DefDeck;
                a_StarF = PlayerInfo.DefStarF;
                break;

            default:
                a_Deck = null;
                a_StarF = null;
                break;
        }

        if (a_Deck == null || a_StarF == null)
            yield break;

        for (int i = 0; i < a_Deck.Count; i++)   //불러올 덱의 몬스터 수 만큼 반복
        {
            if (a_Deck[i].Equals(MonsterName.None))     //i번 덱에 저장된 이름이 None(비어있음)이라면 넘어가기
                continue;

            for (int k = 0; k < PlayerInfo.MonList.Count; k++)    //유저가 보유한 전체 몬스터 수만큼 반복
            {
                //i번 덱과 몬스터이름,성급이 같은 몬스터가 전체 몬스터 리스트의 k번에 있다면(같은 몬스터라면) ex)i = 3번덱 k = 0번 몬스터리스트                
                if (a_Deck[i].Equals(PlayerInfo.MonList[k].monName) && a_StarF[i].Equals(PlayerInfo.MonList[k].starForce))
                {
                    Transform a_MonSlotTr = transform.GetChild(k);  //하위의 MonSlot들(전체 몬스터 리스트와 같음)중 k번의 MonSlot의 Transform을 가져오기
                    a_MonSlotStack.Push(a_MonSlotTr);               //k번 MonSlot의 Transform을 Stack에 저장
                    FindClass.CurSetPoint.MonObjCreate(i, a_MonSlotTr.GetComponent<MonSlotCtrl>());    //이 씬의 SetPoint에 몬스터 오브젝트 생성을 요청(i번 포인트 위치에 k번 MonSlot의 정보 넘겨주기)
                    break;
                }
                else //i번 덱과 k번 몬스터 리스트의 몬스터이름 혹은 성급이 다르면(같은 몬스터가 아니면) 넘어가기
                    continue;
            }
        }

        while (a_MonSlotStack.Count > 0)        //위에서 얻은 Stack 수만큼 반복
            a_MonSlotStack.Pop().SetAsFirstSibling();   //MonSlot의 순서 변경        
    }
}
