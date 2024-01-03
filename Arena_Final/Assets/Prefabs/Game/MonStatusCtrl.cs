using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//������ ������ ����ǥ��â�� ��Ʈ���ϱ����� ��ũ��Ʈ
//InGame�� RankGame�� MonStatus�� �ٿ��� ���
public class MonStatusCtrl : MonoBehaviour
{
    [SerializeField]
    private MonStorage monStore = null;     //�̹���, ������Ʈ �����
    [SerializeField]
    private Image monImg = null;            //������ ���� �̹��� ������Ʈ
    [SerializeField]
    private Transform starLineTr = null;    //���޿� �°� ���̹����� �����ֱ� ���� Transform
    [SerializeField]
    private Image hpBarImg = null;          //������ HP�� �̹��� ������Ʈ
    [SerializeField]
    private Image ultiBarImg = null;        //������ �ñر�� �̹��� ������Ʈ

    private CmnMonCtrl curPointCMC = null;  //�� ������Ʈ�� �ڽĹ�ȣ�� ��ġ�ϴ� Point�� �����ִ� ������ CmnMonCtrl�� ���� ����
    private Color32 deathColor = new(80, 80, 80, 255);      //�׾��� �� �ٲ��� ����
    private bool isDie = false;             //���� �� ������Ʈ�� ����� ���Ͱ� �׾����� ��Ÿ���� ���� ����

    private void OnEnable()     //Mgr��ũ��Ʈ���� CombatUI�� ���� �� ����
    {
        curPointCMC = FindClass.CurSetPoint[transform.GetSiblingIndex()].GetComponentInChildren<CmnMonCtrl>();  //�� ������Ʈ�� �ڽĹ�ȣ�� ��ġ�ϴ� Point�� �����ִ� ������ CmnMonCtrl�� �޾ƿ���

        if (curPointCMC != null)    //�ش� Point�� ���Ͱ� �ִٸ�
        {
            monImg.sprite = monStore.monstersImg[(int)curPointCMC.CMCmonStat.monName];  //���� ������ �°� �̹��� ����

            for (int i = 0; i < curPointCMC.CMCmonStat.starForce; i++)                  //������ ���޿� �°� ���̹��� �ѱ�
                starLineTr.GetChild(i).gameObject.SetActive(true);
        }
        else                        //�ش� Point�� ���Ͱ� ���ٸ�
            gameObject.SetActive(false);    //�� ������Ʈ ����
    }

    // Update is called once per frame
    void Update()
    {
        if (isDie)      //�� ������Ʈ�� ����� ���Ͱ� �������¸� ���
            return;

        if (curPointCMC.gameObject.activeSelf)    //�ش� ���Ͱ� ����ִٸ�
        {
            hpBarImg.fillAmount = (float)curPointCMC.CMCcurStat.hp / curPointCMC.CMCmonStat.hp;
            ultiBarImg.fillAmount = (float)curPointCMC.UltiGage / 100;
        }
        else    //�ش� ���Ͱ� �׾��ٸ�
        {
            foreach (Image Img in GetComponentsInChildren<Image>())
                Img.color = deathColor;

            isDie = true;       //���� ���·� ����
        }
    }
}
