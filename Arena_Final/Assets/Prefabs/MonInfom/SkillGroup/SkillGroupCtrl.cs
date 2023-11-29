using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//선택한 스킬의 설명을 보기위한 스킬 정보판을 생성하는 스크립트
//SkillGroup(몬스터이름) 프리팹에 붙여서 사용
public class SkillGroupCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject skInformPrefab = null;    //스킬 정보판 프리팹

    public void OnPointerClick(PointerEventData eventData)      //SkillGroup내의 아이콘을 클릭했을 때
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
        FindClass.SISelNum = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //현재 선택한 스킬의 번호를 저장
        Instantiate(skInformPrefab);       //스킬 정보판 생성
    }    
}
