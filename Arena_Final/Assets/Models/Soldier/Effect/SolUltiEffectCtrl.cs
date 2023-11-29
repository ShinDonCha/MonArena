using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier�� �ñر� ����Ʈ�� ����� ��� ��ũ��Ʈ
//SolUltiEffect�����տ� �ٿ��� ����
[RequireComponent(typeof(SphereCollider))]
public class SolUltiEffectCtrl : MonoBehaviour
{
    private Soldier ultiDmgSoldier = null;        //�������� ���� ��Ʈ�� ��ũ��Ʈ�� ���� ����
    private SphereCollider sphereCol = null;      //�� ������Ʈ�� SphereCollider

    private void Awake()
    {
        tag = transform.parent.tag;
        ultiDmgSoldier = FindClass.GetMonCMC(transform.parent.tag, MonsterName.Soldier).GetComponent<Soldier>();   //�������� ���� ��Ʈ�� ��ũ��Ʈ ��������
        sphereCol = GetComponent<SphereCollider>();
        sphereCol.radius = ultiDmgSoldier.SolUltiRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))        //���� ���
        {
            ultiDmgSoldier.GiveDamage(other.transform.parent.gameObject, ultiDmgSoldier.GetUltiDmg(ultiDmgSoldier.CMCmonStat.starForce));     //����� ������
        }
    }
}
