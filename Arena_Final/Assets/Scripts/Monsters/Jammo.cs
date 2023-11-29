using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jammo�� ���� ��ũ��Ʈ
//Jammo �����տ� �ٿ��� ���
public class Jammo : CmnMonCtrl
{
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ����
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;      //ù��° ��ų Ȯ�� 30%
        SkSet.Cool[(int)Skill.Sk1] = 4.0f;    //ù��° ��ų ��Ÿ�� 4��

        SkSet.Prob[(int)Skill.Sk2] = 20;      //�ι��� ��ų Ȯ�� 20%     
        SkSet.Cool[(int)Skill.Sk2] = 8.0f;    //�ι�° ��ų ��Ÿ�� 8��
    }
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ����

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject jamAtkPrefab = null;        //�Ϲݰ��� �� ������ ����ü
    [SerializeField]
    private GameObject jamAtkEffect = null;        //�Ϲݰ��� �ǰ� ����Ʈ

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject jamUltiPrefab = null;       //�ñر� ���� ������ ����ü
    [SerializeField]
    private GameObject jamUltiEffect = null;       //�ñر� �ǰ� ����Ʈ
    private readonly int[] jamUltDmg = { 40, 45, 50, 55, 60, 65 };  //�ñر� ��ų �����
    public int GetUltiDmg(int Index) { return jamUltDmg[Index]; }   //�ܺο��� �ñر��� ������� �������� ���� �Լ�    

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject jamSk1Prefab = null;        //Skill1 ��� �� ������ ��ƼŬ�� ���Ե� ������Ʈ
    private bool jamSk1Charge = false;             //Skill1 ��ų ���� ����
    private readonly float[] jamSk1Mul = { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f };   //Skill1 �ߵ� �� �����ϴ� ����� ����
    public float GetSk1Mul(int Index) { return jamSk1Mul[Index]; }   //�ܺο��� Skill1�� ������ �������� ���� �Լ�    

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject jamSk2Prefab = null;       //Skill2 ������
    public float JamSk2Time { get; } = 5.0f;      //Skill2 ���ӽð�
    private readonly float[] jamSk2Value = { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f };   //Skill2 ���ݼӵ� ���� ��ġ
    public float GetSk2Value(int Index) { return jamSk2Value[Index]; }  //�ܺο��� Skill2�� ����ġ�� �������� ���� �Լ�    
    public float JamSk2Radius { get; } = 2.5f;    //Skill2�� ����

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
        GameObject a_GO = Instantiate(jamAtkPrefab, transform);       //�⺻���� ������ ����
        a_GO.GetComponent<LightningBoltScript>().SetPos(firePos.gameObject, Enemy, jamAtkEffect);     //�������� ���������� �������� ���� ����
        Destroy(a_GO, 0.36f);     //�����ð��� ���� �Ŀ� ����

        int a_Damage;        //���� ���ݿ��� ���� ������� ���� ��������
        if(jamSk1Charge)     //Skill1�� ������ִ� ���¶��
        {
            LineRenderer a_LR = a_GO.GetComponent<LineRenderer>(); //�������� LinRenderer�� ����
            a_LR.startColor = new Color32(40, 132, 255, 255);      //���� ����
            a_LR.startWidth *= 1.5f;        //���� ���� 1.5��
            a_LR.endWidth *= 2.5f;          //�� ���� 2.5��
            a_Damage = (int)(CMCcurStat.attackDmg * jamSk1Mul[CMCmonStat.starForce]);     //������ ������� ����
            jamSk1Charge = false;           //��ų������ ����
        }
        else
            a_Damage = CMCcurStat.attackDmg;   //���� ���ݷ��� ����

        GiveDamage(Enemy, a_Damage);              //����� ������
        base.Attack();        //��ų �ߵ� üũ, �ñر� ������ ȹ�� �� ���� ���� �κ�
    }

    protected override void UltiSkill()     //�ñر� : �� 1���� ���� ����
    {
        GameObject a_GO = Instantiate(jamUltiPrefab, transform);    //�ñر� ������ ����
        a_GO.GetComponent<LightningBoltScript>().SetPos(firePos.gameObject, Enemy, jamUltiEffect);     //�������� ���������� �������� ���� ����
        Destroy(a_GO, 0.62f);         //�����ð��� ���� �Ŀ� ����

        GiveDamage(Enemy, jamUltDmg[CMCmonStat.starForce]);   //����� ������

        base.UltiSkill();             //�ñر� ������ �ʱ�ȭ, �ִϸ��̼� �ʱ�ȭ �� ���� ���� �κ�
    }

    protected override void Skill1()    //��ų 1 : ������ ������ �������ݿ� ������� �߰��Ѵ�
    {
        Instantiate(jamSk1Prefab, transform);   //����Ʈ ����
        jamSk1Charge = true;     //�������·� ����
    }

    protected override void Skill2()    //��ų 2 : �����ð����� �ֺ� �Ʊ����� ���ݼӵ��� ������ �Ѵ�
    {
        Instantiate(jamSk2Prefab, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));     //������ ����
    }
}
