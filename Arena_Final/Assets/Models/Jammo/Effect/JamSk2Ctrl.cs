using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jammo�� 2��° ��ų(��������) ��ũ��Ʈ
//JamSkill2 �����տ� �ٿ��� ���
[RequireComponent(typeof(SphereCollider))]
public class JamSk2Ctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject buffEffect = null;     //���� ����Ʈ
    private Jammo aSpdBuffJammo = null;       //������(Jammo)�� ��Ʈ�� ��ũ��Ʈ�� ���� ����
    private SphereCollider sphereCol = null;  //�� ������Ʈ�� SphereCollider

    private void Awake()
    {
        tag = transform.parent.tag;     //�� ������Ʈ�� �±� ����
        aSpdBuffJammo = FindClass.GetMonCMC(tag, MonsterName.Jammo).GetComponent<Jammo>();        //������(Jammo)�� ��Ʈ�� ��ũ��Ʈ ��������
        sphereCol = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sphereCol.radius = aSpdBuffJammo.JamSk2Radius;     //���� ���� ����
        Destroy(gameObject, 1.0f);    //���� �ð��� ���� �� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && aSpdBuffJammo.CompareTag(other.transform.parent.tag))     //�Ʊ��϶��� ��������
        {
            CmnMonCtrl a_CMC = other.GetComponentInParent<CmnMonCtrl>();    //����� ���� ��Ʈ�� ��ũ��Ʈ ��������
            a_CMC.TakeAny(TakeAct.ASpd, aSpdBuffJammo.GetSk2Value(aSpdBuffJammo.CMCmonStat.starForce), aSpdBuffJammo.JamSk2Time);  //���� ����
            Instantiate(buffEffect, a_CMC.transform);       //���� ����Ʈ ����
        }
    }
}
