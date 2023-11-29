using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//������ ���� ���� ����� �����ְ� Ŭ�� �� �ش� ������ �������� �Ѿ�� ���� ��ũ��Ʈ
//MyRoomScene�� Content ������Ʈ�� �ٿ��� ���
public class MonViewCtrl : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private MonStorage monStore = null;

    private void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();      //���� ���� �� �±� ����
    }

    public void OnPointerClick(PointerEventData eventData)      //������ ���� ������ Ŭ�� ���� ���
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);      //��ư Ŭ�� ȿ���� ���
        FindClass.MISelNum = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //�ش� ���ͽ����� ���° �ڽ����� ����
        SceneManager.LoadScene(SceneList.MonInformScene.ToString());       //���� ���� ������ �̵�                
    }
}
