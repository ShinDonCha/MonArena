using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier�� �ñر� �̻��� ��Ʈ�� ��ũ��Ʈ
//SolUltimate �����տ� �ٿ��� ����
public class SolUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform firePos = null;        //������ �Ͼ ��ġ (������ FirePos ������Ʈ)
    [SerializeField]
    private GameObject exploEffect = null;   //���� ����Ʈ ������
    [SerializeField]
    private GameObject warningPrefab = null; //Ÿ�� ���� ǥ�� ������

    private void Start()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit Hit, Mathf.Infinity, 1 << (int)LayerName.Terrain))    //�ٴڿ� �������Ʈ ǥ��
            Instantiate(warningPrefab, Hit.point, Quaternion.Euler(new(90, 0, 0)), transform.parent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("UltiWarning"))    //ǥ�İ� �浹���� ���
        {
            Instantiate(exploEffect, firePos.position, Quaternion.identity, transform.parent);  //���� ����Ʈ ����
            Destroy(other.gameObject);      //ǥ�� ����
            Destroy(gameObject);            //�̻��� ������Ʈ ����
        }
    }
}
