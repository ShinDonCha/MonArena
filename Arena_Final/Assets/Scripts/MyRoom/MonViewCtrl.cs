using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//유저의 보유 몬스터 목록을 보여주고 클릭 시 해당 몬스터의 상세정보로 넘어가기 위한 스크립트
//MyRoomScene의 Content 오브젝트에 붙여서 사용
public class MonViewCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MonStorage monStore = null;

    private void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();      //슬롯 생성 후 태그 변경
    }

    public void OnPointerClick(PointerEventData eventData)      //하위의 몬스터 슬롯을 클릭 했을 경우
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);      //버튼 클릭 효과음 재생
        FindClass.MISelNum = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //해당 몬스터슬롯이 몇번째 자식인지 저장
        SceneManager.LoadScene(SceneList.MonInformScene.ToString());       //몬스터 정보 씬으로 이동                
    }
}
