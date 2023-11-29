using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GamblerCat�� �⺻���� ����ü�� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//CatAttack �����տ� �ٿ��� ���
[RequireComponent(typeof(Rigidbody))]
public class CatAtkCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject hitEffect = null;            //���� ������ �� ������ ����Ʈ
    private CmnMonCtrl gamblerCatCMC = null;        //������ GamblerCat�� ��Ʈ�� ��ũ��Ʈ�� ���� ����
    private CmnMonCtrl enemyCMC = null;             //���� ����� ��Ʈ�� ��ũ��Ʈ
    private Rigidbody rigidBd = null;               //�� ������Ʈ�� RigidBody ������Ʈ

    private readonly float forceSpd = 7.0f;         //����ü �ʴ� �̵� �ӵ�
    private readonly float torqueSpd = 720.0f;      //����ü �ʴ� ȸ����

    private void Awake()
    {
        rigidBd = GetComponent<Rigidbody>();        //������ٵ� ������Ʈ ��������
        gamblerCatCMC = FindClass.GetMonCMC(transform.parent.tag, MonsterName.GamblerCat);     //�������� ��Ʈ�� ��ũ��Ʈ ��������
        enemyCMC = gamblerCatCMC.Enemy.GetComponent<CmnMonCtrl>();   //���� ����� ��Ʈ�� ��ũ��Ʈ ��������
    }

    private void Update()
    {
        if (!enemyCMC.gameObject.activeSelf)      //��ǥ�� �������¸� ����
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Vector3 a_ReqRot = Quaternion.LookRotation(enemyCMC.GetHitPoint.position - rigidBd.position).eulerAngles;             //���� �ٶ󺸱� ���� �ʿ��� ȸ����
        Vector3 a_CalcRot = new(a_ReqRot.x, a_ReqRot.y, rigidBd.rotation.eulerAngles.z + torqueSpd * Time.fixedDeltaTime);    //���� �ٶ󺸸� ���ۺ��� �������� �ʿ��� ȸ���� ���
        rigidBd.rotation = Quaternion.Euler(a_CalcRot);          //ȸ���� ����
        rigidBd.position = Vector3.MoveTowards(rigidBd.position, enemyCMC.GetHitPoint.position, forceSpd * Time.fixedDeltaTime);     //��ǥ�� ���� ��� �̵�        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && enemyCMC.gameObject.Equals(other.transform.parent.gameObject))     //�浹 ����� ���� ��ǥ���
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity, transform.parent);   //���� ���� ����Ʈ ����
            gamblerCatCMC.GiveDamage(enemyCMC.gameObject, gamblerCatCMC.CMCcurStat.attackDmg);   //����� ������
            Destroy(gameObject);    //�� ������Ʈ ����
        }
    }
}
