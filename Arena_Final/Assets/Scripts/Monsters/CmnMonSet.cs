using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Photon.Pun;

//������ ��ų�̳� ���� �� �ʱ� ������ ����ϴ� ��ũ��Ʈ
//CmnMonCtrl�� ���
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class CmnMonSet : MonoBehaviour
{
    protected enum MonsterState   //���� ���� ���� ���
    {
        Idle,        //�ΰ��ӿ��� �� ������ �� �ʱ� ����
        Search,      //���� ����� Ž���ϴ� ����
        Trace,       //���� ����
        Attack,      //�Ϲ� ������ ���ϰ� �ִ� ����
        Rest,        //�Ϲ� ������ �ִϸ��̼� �� ���� ����ؾ��� �ִϸ��̼��� ã������ ª�� ���� ����
        Wait,        //�ִϸ��̼� ��ȯ ���̿� ���� ���¿� ���� ����(monState�� ���� Update���� ����Ǵ� ����)�� ���ߵ��� ����� ���� ������
        Die          //���� ����
    }

    #region ------------------------------- ��ų ���� ----------------------------------------------    
    public struct SkillSet    //��ų ��Ÿ�Ӱ� Ȯ���� �����ϱ� ���� ����ü
    {
        public int[] Prob { get; private set; }     //�� ��ų�� Ȯ���� ���� �迭
        public float[] Cool { get; private set; }   //�� ��ų�� ��Ÿ���� ���� �迭

        public SkillSet(Skill Count)        //��ų������ ���� �迭 ���� ���� �ϴ� ������
        {
            Prob = new int[(int)Count];
            Cool = new float[(int)Count];
        }
    }
    public SkillSet SkSet { get; } = new(Skill.Count); //������ ���� ��ũ��Ʈ���� ����
    protected bool[] skillOnOff = { true, true };      //��ų�� ���� �ִ� �� Ȯ���ϱ� ���� ����
    #endregion ------------------------------- ��ų ���� ----------------------------------------------

    #region ------------------- �߰� ��ġ -------------------
    //------------------------ �޴� ����&����� ȿ�� ------------------------
    protected struct AddStat
    {
        public float MulAtkDmg;     //���� �� �������� ����� ����
        public float MulTakeDmg;    //�ǰ� �� �������� ����� ����
        public int AddAtkDmg;       //���� �� �������� �����
        public int DecDefence;      //���� ���ҷ�
        public int DecMDefence;     //�������׷� ���ҷ�
    }

    protected AddStat curAddStat;   //���� ����ǰ��ִ� �߰�����
    //------------------------ �޴� ����&����� ȿ�� ------------------------
    #endregion ------------------- �߰� ��ġ -------------------
        
    [SerializeField]
    protected Transform firePos = null;             //����ü�� �̿��� ������ �Ҷ� ����ü ������(���Ÿ� ���͸� ���)
    [SerializeField]
    private Transform hitPoint = null;              //������ ���� �� Ÿ���� �Ǵ°�(���� �߾�)
    public Transform GetHitPoint { get { return hitPoint; } }    //Ÿ������ transform�� �������� ���� ������Ƽ
    [SerializeField]
    protected Image hpImg = null;                   //HPCanvas ������ HpBar ���� (������ ü���� �ݿ��Ͽ� fillAmount�� �����ϱ� ���� Image)
    [SerializeField]
    protected GameObject ultiEffect = null;         //�ñر� ����Ʈ
    [SerializeField]
    protected GameObject deathEffect = null;        //��� ����Ʈ
    protected Rigidbody rigidBd = null;             //������ RigidBody
    protected Animator animator = null;             //������ Animator
    protected SphereCollider attackRangeCol = null; //���� Ÿ�� ������ SphereCollider
    protected MonsterState monState = MonsterState.Idle;        //������ ����
    protected float rotAngle = 8.0f;        //������ FixedTime�� ȸ������
    protected PhotonView photonV = null;    //��Ƽ ���ۿ� �ʿ��� PhotonView�� �������� ���� ����
    protected NavMeshAgent navAgent = null; //�� ������ �������� ��Ʈ�� �ϱ����� �׺�Ž�
    protected int movePriority = 50;        //�������� ���� �̵��� �� �� ���͸� ���ع� ������� �ʱ� ���� ����
    protected int atkPriority = 1;          //�������� ���� �̵��� �� �� ���͸� ���ع� ����ϱ� ���� ����

    //-------------------- ���� ����
    public MonsterStat CMCmonStat { get; private set; }  //������ �⺻���� (������ �ʴ� �ش� ������ ���� ����)
    public MonsterStat CMCcurStat;                       //������ ���罺�� (���� ��ų�̳� ���� ���� � ���� ������ �ݿ��ϱ� ���� ����)
    public int UltiGage { get; protected set; } = 0;     //������ �ñر� ������
    public int TotalDmg { get; protected set; } = 0;     //������ ���� �� ����� (���� ���� �� ǥ��)
    public int TotalHP { get; protected set; } = 0;      //������ ���� �� ���� (���� ���� �� ǥ��)
    //-------------------- ���� ����

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigidBd = GetComponent<Rigidbody>();            //������ RigidBody ������Ʈ ��������
        animator = GetComponent<Animator>();            //������ Animator ��������
        attackRangeCol = GetComponent<SphereCollider>();//������ ��� ������ SphereCollider ��������
        monState = MonsterState.Idle;                   //�ʱ� ���� ���� : ������ ����
        photonV = GetComponent<PhotonView>();           //��Ƽ �÷��̸� ���� PhotonView
        navAgent = GetComponent<NavMeshAgent>();        //�������� ���� �׺�Ž� ��������        
    }

    protected virtual void Start()
    {
        //--------------------- �̱ۿ����� ���� ---------------------
        if (ResultData.PrevScene != SceneList.MultiGameScene)
        {
            if (Enum.TryParse(tag, out Team TeamTag))
                switch (TeamTag)
                {
                    case Team.Ally:
                        CMCmonStat = FindClass.CurSetPoint.GetPointMSC(transform.parent.GetSiblingIndex()).MSCMonStat;       //���� ���� ����
                        hpImg.color = Color.green;      //�ٸ� ���� ���Ϳ� ������ �ǵ��� HP�� ���� ����
                        break;

                    case Team.Enemy:
                        //��ũ ������ �´� ���� ��������
                        if (ResultData.PrevScene.Equals(SceneList.InGameScene))
                            CMCmonStat = MonsterData.MonDic[Enum.Parse<MonsterName>(name.Substring(0, name.IndexOf("(")))][PlayerInfo.CombatStage - 1];
                        else if (ResultData.PrevScene.Equals(SceneList.RankGameScene))
                            CMCmonStat = MonsterData.MonDic[FindClass.RankDName[transform.parent.GetSiblingIndex()]][FindClass.RankDStar[transform.parent.GetSiblingIndex()]];
                        FindClass.GetCMCListFunc(tag).Add(GetComponent<CmnMonCtrl>());       //�� ������ CmnMonCtrl�� ����Ʈ�� �߰�
                        hpImg.color = Color.red;        //�ٸ� ���� ���Ϳ� ������ �ǵ��� HP�� ���� ����
                        break;
                }
        }
        //--------------------- �̱ۿ����� ���� ---------------------

        CMCcurStat = CMCmonStat;                        //�⺻���� ��ŭ ���罺�� ����
        navAgent.avoidancePriority = movePriority;      //������ ó�� �νĻ��¸� ���ع� ����� �ƴѻ��·� ����
        navAgent.stoppingDistance = CMCcurStat.attackRange; //���Ͱ� �̵��� �� ���� ������ ���ݻ�Ÿ��� �°� ����
        navAgent.speed = CMCcurStat.moveSpd;            //�׺�Ž��� �����̴� �ӵ��� �̵��ӵ��� �°� ����
        navAgent.acceleration = 10;                     //�׺�Ž��� ���� ����
        rigidBd.constraints = RigidbodyConstraints.FreezeRotation; //ȸ�� ����
        attackRangeCol.enabled = false;     //������ Collider �ʱ� ��Ȱ��ȭ
        attackRangeCol.radius = 0.0f;       //������ Collider �ʱⰪ ����
        attackRangeCol.isTrigger = true;    //������ Collider �ʱ� �浹���� ����
    }

    #region ----------------- ��Ƽ ���� -----------------        
    public void SetMonBasics(int MonNameIndex, int StarForce, Team TeamTag)     //��Ƽ���� ������ �⺻������ �������ֱ����� �Լ� (MtCbtSetPoint���� ����)
    {
        photonV.RPC("SetMonStat", RpcTarget.All, MonNameIndex, StarForce, TeamTag);      //���ð� ���� ��ο��� �⺻������ ���ֱ����� RPC ����
    }

    [PunRPC]
    public void SetMonStat(int MonNameIndex, int StarForce, Team TeamTag)
    {
        CMCmonStat = MonsterData.MonDic[(MonsterName)MonNameIndex][StarForce];  //���� �⺻���� ����
        tag = TeamTag.ToString();       //������ �� �±� ����
        FindClass.GetCMCListFunc(tag).Add(GetComponent<CmnMonCtrl>());          //�� ������ CmnMonCtrl�� �ش� ���� ����Ʈ�� �߰�

        switch(TeamTag)
        {
            case Team.Team1:
                hpImg.color = Color.red;        //���� ���� HP���� ����
                break;

            case Team.Team2:
                hpImg.color = Color.blue;       //���� ���� HP���� ����
                break;
        }
    }
    #endregion ----------------- ��Ƽ ���� -----------------
}
