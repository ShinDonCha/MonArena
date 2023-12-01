using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie�� ���� ��ũ��Ʈ
//Zombie�����տ� �ٿ��� ���
public class Zombie : CmnMonCtrl
{
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ�
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;      //ù��° ��ų Ȯ�� 30%
        SkSet.Cool[(int)Skill.Sk1] = 5.0f;    //ù��° ��ų ��Ÿ�� 5��

        SkSet.Prob[(int)Skill.Sk2] = 20;      //�ι�° ��ų Ȯ�� 20%  
        SkSet.Cool[(int)Skill.Sk2] = 8.0f;    //�ι�° ��ų ��Ÿ�� 8��
    }
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ�

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject zomAtkEffect = null;     //�Ϲݰ��� �ǰ� ����Ʈ

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject zomUltiPrefab = null;       //�ñر� ������
    private readonly int[] zomUltiDmg = { 10, 15, 20, 25, 30, 35 };    //�ñر� �����
    public int GetUltiDmg(int Index) { return zomUltiDmg[Index]; }     //�ܺο��� �ñر� ������� �������� ���� �Լ�
    public float ZomUltiRatio { get; } = 0.5f;     //�ñر� ����� ��ȯ�� 50%
    public float ZomUltiTime { get; } = 4.0f;      //�ñر� ���ӽð�
    public float ZomUltiRadius { get; } = 3.0f;    //�ñر� ����

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject zomSk1Effect = null;
    private readonly int[] zomSk1Dmg = { 10, 12, 14, 16, 18, 20 };   //Skill1 �����
    public int GetSk1Dmg(int Index) { return zomSk1Dmg[Index]; }     //�ܺο��� Skill1�� ������� �������� ���� �Լ�
    private readonly int[] zomSk1ReduVal = { 3, 4, 5, 6, 7, 8 };     //��,���� ���ҷ�
    public int GetSk1ReduVal(int Index) { return zomSk1ReduVal[Index]; } //�ܺο��� ��,���� ���ҷ��� �������� ���� �Լ�    
    public float ZomSk1SusTime { get; } = 3.0f;      //Skill1�� ���ӽð�

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject zomSk2Prefab = null;      //Skill2 ��� �� ������ ������
    private readonly float[] zomSk2Ratio = { 0.25f, 0.30f, 0.35f, 0.4f, 0.45f, 0.5f};   //���ݷ� ���� ����
    public float GetReduceRatio(int Index) { return zomSk2Ratio[Index]; }   //�ܺο��� Skill2�� ���ݷ� ���Һ����� �������� ���� �Լ�
    public float ZomSk2SusTime { get; } = 5.0f;     //���ݷ� ���� ���ӽð�
    public float ZomSk2Radius { get; } = 3.5f;      //��� ���� ����

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
        Instantiate(zomAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));  //����Ʈ ����
        GiveDamage(Enemy, CMCcurStat.attackDmg);   //����� ������
        base.Attack();                  //��ų �ߵ� üũ, �ñر� ������ ȹ�� �� ���� ���� �κ�
    }

    protected override void UltiSkill() //�ñر� : �ڽ��� ������ ���ָ� ���� �������� ���� ���鿡�� ������� ������ �ڽ��� ü���� ȸ���Ѵ�.
    {
        Instantiate(zomUltiPrefab, transform);     //�ñر� ������ ����
        base.UltiSkill();             //�ñر� ������ �ʱ�ȭ, �ִϸ��̼� �ʱ�ȭ �� ���� ���� �κ�
    }

    protected override void Skill1()   //��ų 1 : ���� ���� ������� ������ �渶�� ����
    {
        if (!Enemy.activeSelf)      //���� ����� �׾����� ���
            return;
                
        GiveDamage(Enemy, zomSk1Dmg[CMCmonStat.starForce]);     //����� ������
        CmnMonCtrl a_CMC = Enemy.GetComponent<CmnMonCtrl>();    //���� ����� ��Ʈ�� ��ũ��Ʈ ��������
        a_CMC.TakeAny(TakeAct.Defence, -zomSk1ReduVal[CMCmonStat.starForce], ZomSk1SusTime);      //�� ���� ����
        a_CMC.TakeAny(TakeAct.MDefence, -zomSk1ReduVal[CMCmonStat.starForce], ZomSk1SusTime);     //�� �������׷� ����
        Instantiate(zomSk1Effect, a_CMC.GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));      //����Ʈ ����
    }

    protected override void Skill2()    //��ų 2 : ���� �������� �� ���� ������ �����ð� ������� ���߰� �ڽ��� �����ϰ� �Ѵ�
    {
        Instantiate(zomSk2Prefab, transform);    //Skill2�� ����Ʈ ����
    }
}
