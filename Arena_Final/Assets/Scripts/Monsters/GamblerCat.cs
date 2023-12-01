using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//GamblerCat�� ���� ��ũ��Ʈ
//GamblerCat �����տ� �ٿ��� ���
public class GamblerCat : CmnMonCtrl
{
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ����
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;       //ù��° ��ų Ȯ�� 30%
        SkSet.Cool[(int)Skill.Sk1] = 3.0f;    //ù��° ��ų ��Ÿ�� 3��

        SkSet.Prob[(int)Skill.Sk2] = 20;      //�ι��� ��ų Ȯ�� 20%    
        SkSet.Cool[(int)Skill.Sk2] = 6.0f;   //�ι�° ��ų ��Ÿ�� 6��
    }
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ����

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject catAtkPrefab = null;        //�Ϲ� ���� �� ������ ����ü

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject catUltiPrefab = null;     //�ñر� ����Ʈ ������
    private readonly int[] catUltiAmount = { 20, 25, 35, 45, 60, 100 };     //�ñر� ȸ����
    public int GetUltiAmount(int Index) { return catUltiAmount[Index]; }    //�ܺο��� �ñر��� ȸ������ �������� ���� �Լ�
    public float CatUltiRadius { get; } = 2.5f;  //�ñر� ����

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject catSk1Prefab = null;      //Skill1�� ����Ʈ
    private readonly int[] catSk1HealAmount = { 10, 13, 16, 19, 22, 25 };      //Skill1�� ȸ����
    public int GetHealAmount(int Index) { return catSk1HealAmount[Index]; }    //�ܺο��� Skill1�� ȸ������ �������� ���� �Լ�
    private readonly int catSk1healCount = 2;    //Skill1�� ���� ��� ��

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject catSk2Prefab = null;        //Skill2�� ����Ʈ
    private readonly float[] catSk2BuffTime = { 3.0f, 3.5f, 4.0f, 4.5f, 5.0f, 6.0f }; //Skill2�� ���ӽð�
    public float GetBuffTime(int Index) { return catSk2BuffTime[Index]; }             //�ܺο��� Skill2�� ���ӽð��� �������� ���� �Լ�   
    public float CatSk2IncAmount { get; } = 0.2f;  //������ ����� ����
    private readonly int catSk2BuffCount = 2;      //Skill2�� ���� ��� ��

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
        Instantiate(catAtkPrefab, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));   //�⺻���� ����ü ����
        base.Attack();      //��ų �ߵ� üũ, �ñر� ������ ȹ�� �� ���� ���� �κ�
    }

    protected override void UltiSkill()   //�ñر� : ���� ������ �Ʊ� ����� ü���� ȸ����Ų��
    {
        Instantiate(catUltiPrefab, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));    //�ñر� ������ ����
        base.UltiSkill();            //�ñر� ������ �ʱ�ȭ, �ִϸ��̼� �ʱ�ȭ �� ���� ���� �κ�
    }

    protected override void Skill1()     //��ų 1 : ���� ü���� ���� �Ʊ� 2�� ��
    {
        List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //������ �������� �Ʊ� ���� ����Ʈ ��������

        if (a_CMCList == null)      //���� ����
            return;

        //���� �������� ���� ����Ʈ�� �����켱���� ������ �� ü���� ���� ������ ����
        a_CMCList = a_CMCList.OrderByDescending(val => val.gameObject.activeSelf).ThenBy(val => val.CMCcurStat.hp).ToList();

        for (int i = 0; i < Mathf.Min(catSk1healCount, a_CMCList.Count); i++)   //��ų ����� ����ŭ �ݺ�
        {
            if (!a_CMCList[i].gameObject.activeSelf)        //�ش� ���Ͱ� �������¸� �Ѿ��(2�� �����ε� ���忡 1�� ���� ��쵵 �����Ƿ�)
                continue;

            a_CMCList[i].TakeAny(TakeAct.Heal, catSk1HealAmount[CMCmonStat.starForce]);   //ȸ��
            Instantiate(catSk1Prefab, a_CMCList[i].GetHitPoint);      //�� ����Ʈ ����
        }
    }

    protected override void Skill2()     //��ų 2: ���� ���ݷ��� ���� �Ʊ� 2���� ����� ���� ����
    {
        List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //������ �������� �Ʊ� ���� ����Ʈ ��������

        if (a_CMCList == null)      //���� ����
            return;

        // ���� �������� ���� ����Ʈ�� �����켱���� ������ �� ���ݷ��� ���� ������ ����
        a_CMCList = a_CMCList.OrderByDescending(val => val.gameObject.activeSelf).ThenByDescending(val => val.CMCcurStat.attackDmg).ToList();

        for (int i = 0; i < Mathf.Min(catSk2BuffCount, a_CMCList.Count); i++)     //��ų ����� ����ŭ �ݺ�
        {
            if (!a_CMCList[i].gameObject.activeSelf)        //�ش� ���Ͱ� �������¸� �Ѿ��(2�� �����ε� ���忡 1�� ���� ��쵵 �����Ƿ�)
                continue;

            a_CMCList[i].TakeAny(TakeAct.Dmg, CatSk2IncAmount, catSk2BuffTime[CMCmonStat.starForce]);      //����� ����
            Instantiate(catSk2Prefab, a_CMCList[i].transform);      //���� ����Ʈ ����
        }
    }
}
