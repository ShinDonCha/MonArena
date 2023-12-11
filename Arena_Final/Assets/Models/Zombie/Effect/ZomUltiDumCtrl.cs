using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie�� �ñر� ���� ������� �ֱ����� ��ũ��Ʈ
//ZomUltimate�������� ZomUltiDum�� �ٿ��� ���
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]

public class ZomUltiDumCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject ultiEffect = null;           //�ñر� �ǰ� ����Ʈ
    private Zombie ultiZombie = null;               //��ų�� �������� ��Ʈ�� ��ũ��Ʈ�� ������ ����
    private SphereCollider ultiSColl = null;        //����� �����ϱ� ���� SphereCollider

    private void Awake()
    {
        ultiZombie = GetComponentInParent<Zombie>();    //�������� ��Ʈ�� ��ũ��Ʈ ��������
        ultiSColl = GetComponent<SphereCollider>();     //�� ������Ʈ�� SphereCollider ��������
        ultiSColl.radius = ultiZombie.ZomUltiRadius;    //�� ���� ���� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !transform.parent.CompareTag(other.transform.parent.tag))     //���� ���
        {
            Instantiate(ultiEffect, other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(transform.parent.tag));
            int a_CalcDmg = ultiZombie.GiveDamage(other.transform.parent.gameObject, ultiZombie.GetUltiDmg(ultiZombie.CMCmonStat.starForce));  //����� ������
            ultiZombie.TakeAny(TakeAct.Heal, a_CalcDmg * ultiZombie.ZomUltiRatio);    //���� ����� * ȸ�������� ü�� ȸ��
        }
    }
}
