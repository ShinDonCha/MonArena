using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mutant�� �ñر� ��Ʈ�� ��ũ��Ʈ
//MutUltiDum�����տ� �ٿ��� ���
[RequireComponent(typeof(CapsuleCollider))]
public class MutUltiDumCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject ultiDumEffect = null;        //�ǰ� ����Ʈ
    private Mutant ultiDumMutant = null;            //�������� ��Ʈ�� ��ũ��Ʈ�� �޾ƿ� ����

    private void Awake()
    {
        tag = transform.parent.tag;     //�±� ����
        ultiDumMutant = FindClass.GetMonCMC(tag, MonsterName.Mutant).GetComponent<Mutant>();    //�������� ��Ʈ�� ��ũ��Ʈ ��������
        transform.localScale = new Vector3(1.0f, 1.0f, ultiDumMutant.MutUltiMoveR / ultiDumMutant.MutUltiRange);     //����Ʈ ũ�� & ���� ���� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))  //���� ���
        {
            ultiDumMutant.GiveDamage(other.transform.parent.gameObject, ultiDumMutant.GetUltiDmg(ultiDumMutant.CMCmonStat.starForce));        //����� ������
            Instantiate(ultiDumEffect, other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));        //�ǰ� ����Ʈ ����
        }
    }
}
