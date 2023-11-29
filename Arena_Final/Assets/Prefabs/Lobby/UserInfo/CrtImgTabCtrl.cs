using UnityEngine;
using UnityEngine.EventSystems;

//ĳ���� �̹��� Ŭ�� �� �����û�ϱ� ���� ��ũ��Ʈ
//UserInfoPanel�� CharacterImgTab�� �ٿ��� ���
public class CrtImgTabCtrl : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GetComponentInParent<UserPanelCtrl>().CrtChange(eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex());   //ĳ���� �̹��� �����Լ� ȣ��
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }
}
