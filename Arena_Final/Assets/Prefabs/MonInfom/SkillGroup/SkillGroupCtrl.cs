using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//������ ��ų�� ������ �������� ��ų �������� �����ϴ� ��ũ��Ʈ
//SkillGroup(�����̸�) �����տ� �ٿ��� ���
public class SkillGroupCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject skInformPrefab = null;    //��ų ������ ������

    public void OnPointerClick(PointerEventData eventData)      //SkillGroup���� �������� Ŭ������ ��
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
        FindClass.SISelNum = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //���� ������ ��ų�� ��ȣ�� ����
        Instantiate(skInformPrefab);       //��ų ������ ����
    }    
}
