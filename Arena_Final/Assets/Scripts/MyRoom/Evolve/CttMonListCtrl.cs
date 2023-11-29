using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//EvolveScene에서 유저가 보유한 몬스터목록을 보여주기위한 스크립트
//EvolveScene의 Content오브젝트에 붙여서 사용
public class CttMonListCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MonStorage monStore = null;
        
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();      //슬롯 생성 후 태그 변경
    }

    public void ColorChange(int SlotIndex, Color32 ReqColor)        //슬롯의 색깔을 바꿔주기 위한 함수
    {
        transform.GetChild(SlotIndex).GetComponent<Image>().color = ReqColor;
    }

    public void OnPointerClick(PointerEventData eventData)      //슬롯을 클릭 시
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<MonSlotCtrl>(out var a_SelectMSC))   //해당 슬롯의 MonSlotCtrl과 색깔변경 함수를 넘겨주기
            EvolveMgr.Instance.EvolveSelect(a_SelectMSC, ColorChange);
    }
}


