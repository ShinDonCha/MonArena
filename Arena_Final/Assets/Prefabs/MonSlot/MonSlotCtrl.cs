using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//�� GameScene���� Content�� ������ MonSlot�������� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//MonSlot�����տ� �ٿ��� ���
public class MonSlotCtrl : MonoBehaviour
{    
    [SerializeField]
    private MonStorage monStorage;      //���� �̹���, ������Ʈ ���� ����Ǿ��ִ� �����
    [SerializeField]
    private Image monImg = null;        //�� ������ �����̹����� ����ϴ� ���ӿ�����Ʈ�� �̹��� ������Ʈ

    private Color32 defaultColor = new(255, 255, 255, 255);  //���� �̹�ġ�� ������ ����
    private Color32 usingColor = new(0, 0, 0, 180);          //���� ��ġ���� ������ ����
    public MonsterStat MSCMonStat { get; private set; }      //������ ��ü ���� ����Ʈ���� ���� ������ �޾ƿ� ����

    // Start is called before the first frame update
    void Start()
    {
        if (!Enum.TryParse(tag, out MonSlotTag MST))
            return;
                
        //----------- �ٸ������� MonSlot�� �����ϰ� �±׸� �ٲ��ֱ� ������ �� ������ �ݵ�� Start()���� �����ؾ���
        switch (MST)
        {
            case MonSlotTag.Content:        //�� ������ Content�� ������ ���, MyRoomScene�� MonSlot�� ���
                MSCMonStat = PlayerInfo.MonList[transform.GetSiblingIndex()];        //�� ������ ������ �°� ������ ���͸�Ͽ��� ���� ��������
                monImg.sprite = monStorage.monstersImg[(int)MSCMonStat.monName];       //���� ������ �°� �̹��� ����
                break;

            case MonSlotTag.Drag:         //�� ������ �巡�׸� ���� ������ ���
                //�巡�׸� ���� MonSlot�� MonStorage���� �������°� �ƴ� �����Ǿ��ִ� MonSlot�� ���� ������Ʈ�� �����ϹǷ� ����� (0,0)���� �Ǿ��ֱ� ������ ��������� �ʿ���
                GetComponent<RectTransform>().sizeDelta = new(110, 120);        //�� ������ ������ ����
                UsingSet(false);          //�⺻���·� ����
                break;

            case MonSlotTag.NameParse:      //������ �̸����� �̹����� ã�� ���
                monImg.sprite = monStorage.monstersImg[int.Parse(name)];     //�̸��� �´� �̹����� ����
                break;
        }
        //----------- �ٸ������� MonSlot�� �����ϰ� �±׸� �ٲ��ֱ� ������ �� ������ �ݵ�� Start()���� �����ؾ���
    }

    public void UsingSet(bool Using)    //�� ������ ��� ������ ���� ���� ���� (Drag&Drop �� ���� ����)
    {
        Image a_Img = GetComponent<Image>();

        switch(Using)
        {
            case true:          //�� ������ ������ ������̸�
                a_Img.color = usingColor;            //����� ������
                a_Img.raycastTarget = false;         //������̸� ���þȵǵ���
                break;

            case false:         //�� ������ ������ ������� �ƴϸ�
                a_Img.color = defaultColor;          //���� ������..
                a_Img.raycastTarget = true;          //������� �ƴϸ� ���õǵ���
                break;
        }
    }
}
