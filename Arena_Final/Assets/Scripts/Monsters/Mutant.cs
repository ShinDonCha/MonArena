using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Mutant�� ���� ��ũ��Ʈ
//Mutant �����տ� �ٿ��� ���
public class Mutant : CmnMonCtrl
{
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ� (Mutant�� ��ų�� Active ������ �ƴ�)
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 0;      //ù��° ��ų Ȯ�� 0%
        SkSet.Cool[(int)Skill.Sk1] = 0.0f;   //ù��° ��ų ��Ÿ�� 0��

        SkSet.Prob[(int)Skill.Sk2] = 0;      //�ι�° ��ų Ȯ�� 0%
        SkSet.Cool[(int)Skill.Sk2] = 0.0f;   //�ι�° ��ų ��Ÿ�� 0��
    }
    //---------- �� ������ ��ų Ȯ��&��Ÿ�� ���� �Լ� (Mutant�� ��ų�� Active ������ �ƴ�)

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject mutAtkEffect = null;     //�Ϲݰ��� �ǰ� ����Ʈ

    //------ Ultimate
    [Header("-------------- Ultimate --------------")]
    [SerializeField]
    private GameObject mutUltiEffect = null;       //�ñر� ����Ʈ ������
    [SerializeField]
    private GameObject mutUltiDummy = null;         //�ñر� ����� ���� ������
    private readonly int[] mutUltiDmg = { 20, 23, 26, 29, 32, 35 };  //�ñر� �����
    public int GetUltiDmg(int Index) { return mutUltiDmg[Index]; }   //�ܺο��� �ñر��� ������� �������� ���� �Լ�
    public float MutUltiRange { get; } = 3.5f;      //�ִ� �̵��Ÿ�
    public float MutUltiMoveR { get; private set; } = 0.0f;    //�̹� �ñر⿡ �̵��� �Ÿ�

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject mutSk1Effect = null;     //Skill1�� ����Ʈ
    private readonly int[] mutSk1Dmg = { 11, 13, 15, 17, 19, 21 }; //Skill1�� �߰� �����
    public int GetSk1Dmg(int Index) { return mutSk1Dmg[Index]; }   //�ܺο��� Skill1�� �߰� ������� �������� ���� �Լ�
    private Team myTeam;                        //Skill1�� ���ݴ���� ã������ ���� �±�

    //------ Skill2
    public float MutSk2AMul { get; } = 0.3f;     //Skill2�� ���ݷ� ������
    public float MutSk2TMul { get; } = 0.4f;     //Skill2�� �޴����� ������

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();    //���� ���� ���� ����
        Skill2();        //Skill2 �ߵ�(�нú� ȿ�� ����)
    }

    protected override void StartFight()    //���� ���� �� ����Ǵ� �Լ�
    {
        GetComponentInChildren<StCanvasCtrl>(true).gameObject.SetActive(true);     //HP�� ǥ�ø� ���� ĵ���� On���·� ����

        if (photonV != null && !photonV.IsMine)     //��Ƽ ������ ��� ���
            return;

        Skill1();
    }

    protected override void Attack()        //�Ϲ� ����
    {
        Instantiate(mutAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));  //����Ʈ ����
        GiveDamage(Enemy, CMCcurStat.attackDmg);     //����� ������
        base.Attack();      //��ų �ߵ� üũ, �ñر� ������ ȹ�� �� ���� ���� �κ�
    }

    protected override void UltiSkill()     //�ñر� : �������� ������ �̵� �� ��ο��ִ� �� ����
    {
        MutUltiMoveR = MutUltiRange;       //�� ���Ͱ� �̵��� �� �ִ� �ִ� �Ÿ�

        if (Physics.Raycast(GetHitPoint.position, transform.forward, out RaycastHit rayHit, MutUltiRange, 1 << (int)LayerName.Wall))   //�ִ� �̵��Ÿ��ȿ� ���� �������
            MutUltiMoveR = ((rayHit.point - transform.forward * 0.5f) - GetHitPoint.position).magnitude;    //�̵��Ÿ� ����

        Vector3 a_DummyPos = transform.position + (transform.forward * (MutUltiMoveR / 2));    //������� ������ ���� ������ ������ġ
        a_DummyPos.y += 0.8f;       //��ġ�� ����

        Instantiate(mutUltiEffect, transform.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));     //�ñر� ����Ʈ ����

        transform.position += transform.forward * MutUltiMoveR;    //���� �Ÿ���ŭ �̵�

        StartCoroutine(UltiDumSet(a_DummyPos));     //������� ������ ���� ���� �����ڷ�ƾ ����

        base.UltiSkill();       //�ñر� ������ �ʱ�ȭ, �ִϸ��̼� �ʱ�ȭ �� ���� ���� �κ� �ñر� �������� ����
    }

    protected override void Skill1()        //��ų 1 : �ϻ� - ü���� ���� ���� ���� �ڷ� �̵� �� ��ȭ����
    {
        if (!System.Enum.TryParse(tag, out myTeam))     //�� ������ �±׸� Enum�������� ����
            return;

        Sk1TargetSet();      //Skill1�� ���ݴ�� ã�� ����
    }

    protected override void Skill2()        //��ų 2 : ���ݷ��� �����ϴ� ��� �޴� ����� ����(�нú�)
    {
        curAddStat.MulAtkDmg += MutSk2AMul;     //������ ����� ����
        curAddStat.MulTakeDmg += MutSk2TMul;    //�޴� ����� ����
    }

    private void Sk1TargetSet()      //Skill1�� ���� ����� ã������ �Լ�
    {      
        List<CmnMonCtrl> a_CMCList = new();

        switch (myTeam)
        {
            case Team.Ally:
                a_CMCList = FindClass.GetCMCListFunc(Team.Enemy.ToString());
                break;

            case Team.Enemy:
                a_CMCList = FindClass.GetCMCListFunc(Team.Ally.ToString());
                break;

            case Team.Team1:
                a_CMCList = FindClass.GetCMCListFunc(Team.Team2.ToString());
                break;

            case Team.Team2:
                a_CMCList = FindClass.GetCMCListFunc(Team.Team1.ToString());
                break;
        }

        if (a_CMCList.Count == 0)   //Ž���� �� ���� ����Ʈ�� ������� ���
            return;

        a_CMCList.Sort((a, b) => a.CMCcurStat.hp.CompareTo(b.CMCcurStat.hp));   //�� ���� ����� HP�� ���� ������� ����

        if (!a_CMCList[0].gameObject.name.Contains(MonsterName.Mutant.ToString()))  //����Ʈ -> ����Ʈ�� �Ұ�
            EnemySet(a_CMCList[0].gameObject);     //���� ��ǥ ����
        else
            if (a_CMCList.Count > 1)       //�ٸ� ������ ����� ������
            EnemySet(a_CMCList[1].gameObject);     //���� ��ǥ ����

        if (Enemy == null)      //��������� ��ǥ�� ã�� ���� ��Ȳ�̸�
        {
            base.StartFight();  //�⺻ �����Լ� ����
            return;
        }

        //---------- ���ݴ���� ã���� ��츸 ������ ����
        if (photonV == null)     //�̱�
            Sk1On();
        else                    //��Ƽ(IsMine�� ����)
            photonV.RPC("Sk1On", RpcTarget.All);
        //---------- ���ݴ���� ã���� ��츸 ������ ����
    }

    [PunRPC]
    private void Sk1On()        //���� ���ӻ���(�̱�,��Ƽ)�� ���� RPC�� ����Ͽ� ��ų�� �����ϱ����� �Լ�
    {
        StartCoroutine(Assassinate());
    }

    private IEnumerator Assassinate()   //�ϻ� - ü���� ���� ���� ���� �ڷ� �̵� �� ��ȭ����
    {        
        GameObject a_GO = Instantiate(mutSk1Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));   //��ų ����Ʈ ����
        AudioSource a_Sk1Audio = a_GO.GetComponent<AudioSource>();
        a_Sk1Audio.Play();      //Skill1�� ȿ���� ���
        yield return new WaitForSeconds(0.2f);

        rigidBd.useGravity = false;     //�� ���͸� �̵���Ű�� ���� ��� �߷� ����
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);      //�� ���� ���Ż��� ����(������ ��� ���ӿ�����Ʈ ����)

        Vector3 a_dirVec = (Enemy.transform.position - transform.position).normalized;       //�� ���Ͱ� ���ݸ�ǥ�� �ٶ󺼶��� ���⺤�� ���

        transform.position = Enemy.transform.position + a_dirVec * CMCcurStat.attackRange;  //��ǥ�κ��� �� ������ ���ݰŸ���ŭ ������ ��ġ�� �̵�
        transform.forward = (Enemy.transform.position - transform.position).normalized;     //��ǥ�� �ٶ󺸵��� ȸ��
        yield return new WaitForSeconds(0.6f);

        Instantiate(mutSk1Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));   //��ų ����Ʈ ����
        yield return new WaitForSeconds(0.4f);

        rigidBd.useGravity = true;      //�� ������ �߷� ����
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);      //�� ���� ���Ż��� ����(������ ��� ���ӿ�����Ʈ �ѱ�)

        curAddStat.AddAtkDmg += mutSk1Dmg[CMCmonStat.starForce];   //���� ���ݿ� �߰��� ����� ����
        animator.Play(MonsterState.Attack.ToString());             //���� �ִϸ��̼� ����
    }

    private IEnumerator UltiDumSet(Vector3 DumPos)  //�ñر� ���� �����ڷ�ƾ
    {
        yield return new WaitForSeconds(0.7f);      //0.7�� �� ����
        Instantiate(mutUltiDummy, DumPos, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //����� ���� ����
    }
}
