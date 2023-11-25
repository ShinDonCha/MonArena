using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����� �Ѿ��� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//DronBullet �����տ� �ٿ��� ���
[RequireComponent(typeof(Rigidbody))]
public class DronBulletCtrl : MonoBehaviour
{
    private Soldier dronBulletSoldier = null;      //�������� ���� ��Ʈ�� ��ũ��Ʈ�� ���� ����
    private Rigidbody rb = null;                   //�Ѿ��� ������ٵ�
    private readonly float BulletSpd = 10.0f;      //�Ѿ��� ���ư��� �ӵ�
        
    void Awake()
    {
        tag = transform.parent.tag;         //�� ������Ʈ�� �±� ����
        rb = GetComponent<Rigidbody>();     //������ٵ� ������Ʈ ��������
        dronBulletSoldier = FindClass.GetMonCMC(tag, MonsterName.Soldier).GetComponent<Soldier>();       //�±׸� ���� �� �Ѿ��� ������ ������ ��������
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.useGravity = false;          //�߷� ��������·� ����
        rb.AddForce(transform.forward * BulletSpd, ForceMode.Impulse);  //�Ѿ˿� �������� �� ���ϱ�
        Destroy(gameObject, 5.0f);      //5���Ŀ��� ������ ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))        //�����̶��
        {
            dronBulletSoldier.GiveDamage(other.transform.parent.gameObject, dronBulletSoldier.GetDronDmg(dronBulletSoldier.CMCmonStat.starForce));   //����� ������
            Destroy(gameObject);    //�Ѿ� �ı�
        }
    }


}
