using UnityEngine;
using UnityEngine.EventSystems;

//캐릭터 이미지 클릭 시 변경요청하기 위한 스크립트
//UserInfoPanel의 CharacterImgTab에 붙여서 사용
public class CrtImgTabCtrl : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<UserPanelCtrl>().CrtChange(eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());   //캐릭터 이미지 변경함수 호출
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }
}
