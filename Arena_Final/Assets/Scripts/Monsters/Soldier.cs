using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier�� ���� ��ũ��Ʈ
//Soldier �����տ� �ٿ��� ���
public class Soldier : CmnMonCtrl
{       
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ�
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 15;      //ù��° ��ų Ȯ�� 15%
        SkSet.Cool[(int)Skill.Sk1] = 6.0f;    //ù��° ��ų ��Ÿ�� 6��

        SkSet.Prob[(int)Skill.Sk2] = 10;      //�ι�° ��ų Ȯ�� 10%
        SkSet.Cool[(int)Skill.Sk2] = 10.0f;   //�ι�° ��ų ��Ÿ�� 10��
    }
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ�    

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject solAtkPrefab = null;       //�Ϲݰ��� �� ������ ����ü
    [SerializeField]
    private GameObject solAtkEffect = null;       //�Ϲݰ��� �Ѿ� �߻� ����Ʈ

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject solUltiPrefab = null;      //�ñر� ��� �� ������ �̻���
    private readonly int[] solUltiDmg = { 30, 35, 40, 45, 50, 55 };   //�ñر� ����� (������ Starforce�� ���� �ٸ� �����)
    public int GetUltiDmg(int Index) { return solUltiDmg[Index]; }    //�ܺο��� �ñر��� ������� �������� ���� �Լ�
    public float SolUltiRadius { get; } = 1.5f;    //�ñر� Ÿ�� ����

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject solSk1Prefab = null;       //Skill1�� ����Ʈ ������
    private readonly float[] solSk1DecRatio = { 0.15f, 0.13f, 0.11f, 0.1f, 0.09f, 0.08f };  //��ų ��� �� ü�� ���� ����
    public float GetDecRatio(int Index) { return solSk1DecRatio[Index]; }   //�ܺο��� Skill1�� ���� ������ �������� ���� �Լ�
    public int SolSk1UltiG { get; } = 20;         //��ų ��� �� ȸ���Ǵ� �ñر� ������

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject solSk2Effect = null;    //Skill2�� ����Ʈ
    [SerializeField]
    private GameObject[] dronsPrefab;          //��� ������Ʈ�� (��� ������ ����)
    [SerializeField]
    private Vector3[] dronsAddPos;             //��� ���� ��ġ�� ���� �ϱ� ���� ������ (����Ƽ���� ����)
    private readonly int[] DronDamage  = { 10, 11, 12, 13, 14, 15 };   //����� �Ѿ� �����
    public int GetDronDmg(int Index) { return DronDamage[Index]; }     //�ܺο��� Skill2�� ������� �������� ���� �Լ�
    public float DronLifeT { get; } = 2.0f;    //����� ���ӽð�
    public float DronASpd { get; } = 0.5f;     //����� ���ݼӵ�

    protected override void Awake()
    {
        base.Awake();
        SkillSetting();     //��ų Ȯ�� �� ��Ÿ�� ����
    }

    protected override void Start()
    {        
        base.Start();
    }

    protected override void Attack()    //�Ϲ� ����
    {
        Instantiate(solAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //�⺻���� ����Ʈ ����
        Instantiate(solAtkPrefab, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //�⺻���� ����ü ����
        base.Attack();       //��ų �ߵ� üũ, �ñر� ������ ȹ�� �� ���� ���� �κ�
    }

    protected override void UltiSkill()   //�ñر� : �ٹ̻����� ������ ����Ʈ�� �ֺ��� ������� ������.
    {
        Vector3 a_Vec = Enemy.transform.position;   //���� ��ġ ��������
        a_Vec.y += 5.0f;   //���� ��ġ���� 5��ŭ �� ���� ��ġ�� �����ϵ��� ��ġ ����

        Instantiate(solUltiPrefab, a_Vec, Quaternion.LookRotation(Enemy.transform.position - a_Vec), FindClass.AreaTrFunc(tag));    //�ñر� �̻��� ����

        base.UltiSkill();                 //�ñر� ������ �ʱ�ȭ, �ִϸ��̼� �ʱ�ȭ �� ���� ���� �κ�
    }

    protected override void Skill1()        //��ų 1 : ���������� HP�� �Ҹ��ϰ� �ñر� ������ ����
    {
        int a_CurHP = CMCcurStat.hp;        //���� ü�� ��������
        a_CurHP -= (int)(CMCmonStat.hp * solSk1DecRatio[CMCmonStat.starForce]); //��ų �ߵ����� ��� ���ԵǴ� HP���

        if (a_CurHP <= (CMCmonStat.hp * 0.2f))    //��ų �ߵ� �� ���� ü���� �ִ�ü���� 20%���ϸ� ��ų�ߵ� ����
            return;

        Instantiate(solSk1Prefab, transform);     //����Ʈ ����
        CMCcurStat.hp -= (int)(CMCmonStat.hp * solSk1DecRatio[CMCmonStat.starForce]);    //�ڽ��� ü�� �������� ����
        hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //���� �Ӹ����� HP�� ǥ�� ����
        AddUltiGage(20);       //UltiGageȹ��
    }

    protected override void Skill2()     //��ų 2 : ��� 4�븦 �����Ͽ� ����
    {
        StartCoroutine(DronSet());      //��л��� �ڷ�ƾ ����
    }

    private IEnumerator DronSet()       //Skill2�� ��л��� �ڷ�ƾ
    {
        Instantiate(solSk2Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));       //����Ʈ ����
        yield return new WaitForSeconds(0.3f);

        //�� ���Ͱ� �����ִ� ������ �ٶ󺸵��� ��� ����
        for (int i = 0; i < dronsPrefab.Length; i++)
            Instantiate(dronsPrefab[i], (transform.position + dronsAddPos[i]), Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));     //��� ����
    }
}
