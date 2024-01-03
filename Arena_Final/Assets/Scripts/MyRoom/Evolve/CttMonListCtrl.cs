using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//EvolveScene���� ������ ������ ���͸���� �����ֱ����� ��ũ��Ʈ
//EvolveScene�� Content������Ʈ�� �ٿ��� ���
public class CttMonListCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MonStorage monStore = null;
        
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();      //���� ���� �� �±� ����
    }

    public void ColorChange(int SlotIndex, Color32 ReqColor)        //������ ������ �ٲ��ֱ� ���� �Լ�
    {
        transform.GetChild(SlotIndex).GetComponent<Image>().color = ReqColor;
    }

    public void OnPointerClick(PointerEventData eventData)      //������ Ŭ�� ��
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<MonSlotCtrl>(out var a_SelectMSC))   //�ش� ������ MonSlotCtrl�� ���򺯰� �Լ��� �Ѱ��ֱ�
            EvolveMgr.Instance.EvolveSelect(a_SelectMSC, ColorChange);
    }
}


