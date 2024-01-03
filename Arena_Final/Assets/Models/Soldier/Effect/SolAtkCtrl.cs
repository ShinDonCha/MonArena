using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier�� �Ϲݰ��� ����ü�� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//SolAttack�����տ� �ٿ��� ���
public class SolAtkCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject hitEffect = null;    //Ÿ�ݽ� ��Ÿ���� ����Ʈ

    private Soldier SolAtkSoldier = null;   //������(Soldier)�� ��Ʈ�� ��ũ��Ʈ
    private CmnMonCtrl enemyCMC = null;     //���� ����� CmnMonCtrl

    private void Awake()
    {
        SolAtkSoldier = FindClass.GetMonCMC(transform.parent.tag, MonsterName.Soldier).GetComponent<Soldier>();    //������(Soldier)�� ��Ʈ�� ��ũ��Ʈ ��������
        enemyCMC = SolAtkSoldier.Enemy.GetComponent<CmnMonCtrl>();   //���� ����� CmnMonCtrl ��������
    }

    // Update is called once per frame
    void Update()
    {      
        if (!enemyCMC.gameObject.activeSelf)      //���� ��ǥ�� �������¸� ����
            Destroy(gameObject);

        transform.position = Vector3.MoveTowards(transform.position, enemyCMC.GetHitPoint.position, 10.0f * Time.deltaTime);      //��ǥ�� ���� ��� �̵�
        transform.LookAt(enemyCMC.GetHitPoint);     //���� ��ǥ�� �ٶ󺸵��� ȸ��
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && enemyCMC.gameObject.Equals(other.transform.parent.gameObject))      //�浹 ����� ���� ��ǥ���
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity, transform.parent);    //����Ʈ ����
            SolAtkSoldier.GiveDamage(enemyCMC.gameObject, SolAtkSoldier.CMCcurStat.attackDmg);   //����� ������
            Destroy(gameObject);    //�� ������Ʈ ����
        }
    }
}
