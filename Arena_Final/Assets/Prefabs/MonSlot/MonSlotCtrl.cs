using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//각 GameScene에서 Content에 생성된 MonSlot프리팹을 컨트롤 하기위한 스크립트
//MonSlot프리팹에 붙여서 사용
public class MonSlotCtrl : MonoBehaviour
{    
    [SerializeField]
    private MonStorage monStorage;      //몬스터 이미지, 오브젝트 등이 저장되어있는 저장소
    [SerializeField]
    private Image monImg = null;        //이 슬롯의 몬스터이미지를 담당하는 게임오브젝트의 이미지 컴포넌트

    private Color32 defaultColor = new(255, 255, 255, 255);  //현재 미배치된 몬스터의 색깔
    private Color32 usingColor = new(0, 0, 0, 180);          //현재 배치중인 몬스터의 색깔
    public MonsterStat MSCMonStat { get; private set; }      //유저의 전체 몬스터 리스트에서 몬스터 정보를 받아올 변수

    // Start is called before the first frame update
    void Start()
    {
        if (!Enum.TryParse(tag, out MonSlotTag MST))
            return;
                
        //----------- 다른곳에서 MonSlot을 생성하고 태그를 바꿔주기 때문에 이 동작은 반드시 Start()에서 실행해야함
        switch (MST)
        {
            case MonSlotTag.Content:        //이 슬롯이 Content의 슬롯인 경우, MyRoomScene의 MonSlot일 경우
                MSCMonStat = PlayerInfo.MonList[transform.GetSiblingIndex()];        //이 슬롯의 순서에 맞게 유저의 몬스터목록에서 정보 가져오기
                monImg.sprite = monStorage.monstersImg[(int)MSCMonStat.monName];       //몬스터 정보에 맞게 이미지 변경
                break;

            case MonSlotTag.Drag:         //이 슬롯이 드래그를 위한 슬롯인 경우
                //드래그를 위한 MonSlot은 MonStorage에서 가져오는게 아닌 생성되어있는 MonSlot과 같은 오브젝트를 생성하므로 사이즈가 (0,0)으로 되어있기 때문에 사이즈변경이 필요함
                GetComponent<RectTransform>().sizeDelta = new(110, 120);        //이 슬롯의 사이즈 조절
                UsingSet(false);          //기본상태로 변경
                break;

            case MonSlotTag.NameParse:      //슬롯의 이름으로 이미지를 찾는 경우
                monImg.sprite = monStorage.monstersImg[int.Parse(name)];     //이름에 맞는 이미지로 변경
                break;
        }
        //----------- 다른곳에서 MonSlot을 생성하고 태그를 바꿔주기 때문에 이 동작은 반드시 Start()에서 실행해야함
    }

    public void UsingSet(bool Using)    //이 슬롯의 사용 유무에 따른 색상 변경 (Drag&Drop 할 때만 실행)
    {
        Image a_Img = GetComponent<Image>();

        switch(Using)
        {
            case true:          //이 슬롯의 정보가 사용중이면
                a_Img.color = usingColor;            //사용중 색으로
                a_Img.raycastTarget = false;         //사용중이면 선택안되도록
                break;

            case false:         //이 슬롯의 정보가 사용중이 아니면
                a_Img.color = defaultColor;          //원래 색으로..
                a_Img.raycastTarget = true;          //사용중이 아니면 선택되도록
                break;
        }
    }
}
