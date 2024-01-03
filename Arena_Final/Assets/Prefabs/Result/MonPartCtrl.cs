using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� �����ǿ� �˸��� ������ �ֱ����� ��ũ��Ʈ
//MonPart �����տ� �ٿ��� ���
public class MonPartCtrl : MonoBehaviour
{
    //--------- ���ӿ�����Ʈ ����
    [SerializeField]
    private MonStorage monStore = null;     //�̹���, ������Ʈ �����
    [SerializeField]
    private Image monImg = null;            //���� �̹���
    [SerializeField]
    private Image dmgBarImg = null;         //����� �� �̹���
    [SerializeField]
    private Text dmgBarText = null;         //����� �� �ؽ�Ʈ
    [SerializeField]
    private Image HPBarImg = null;          //HP �� �̹���
    [SerializeField]
    private Text HPBarText = null;          //HP �� �ؽ�Ʈ
    //--------- ���ӿ�����Ʈ ����

    // Start is called before the first frame update
    void Start()
    {
        ResultBoardCtrl a_RBC = GetComponentInParent<ResultBoardCtrl>();    //�θ��� ��ũ��Ʈ ��������

        if (a_RBC == null)
            return;

        int a_Num = transform.GetSiblingIndex();    //�� MonPart�������� �ڽĹ�ȣ

        monImg.sprite = monStore.monstersImg[a_RBC.CmnListNameNum[a_Num]];                        //���� �̹��� ����
        dmgBarImg.fillAmount = (float)a_RBC.CmnListDmg[a_Num] / a_RBC.TotalMonDmg;                //����� �� ��ġ ����
        dmgBarText.text = string.Format("{0} / {1}", a_RBC.CmnListDmg[a_Num], a_RBC.TotalMonDmg); //����� �� �ؽ�Ʈ ����
        HPBarImg.fillAmount = (float)a_RBC.CmnListHP[a_Num] / a_RBC.TotalMonHP;                   //HP �� ��ġ ����
        HPBarText.text = string.Format("{0} / {1}", a_RBC.CmnListHP[a_Num], a_RBC.TotalMonHP);    //HP�� �ؽ�Ʈ ����
    }
}
