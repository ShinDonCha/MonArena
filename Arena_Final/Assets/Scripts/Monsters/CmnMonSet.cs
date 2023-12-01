using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using Photon.Pun;

//몬스터의 스킬이나 스탯 등 초기 설정을 담당하는 스크립트
//CmnMonCtrl에 상속
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class CmnMonSet : MonoBehaviour
{
    protected enum MonsterState   //몬스터 현재 상태 목록
    {
        Idle,        //인게임에서 덱 구성할 때 초기 상태
        Search,      //공격 대상을 탐색하는 상태
        Trace,       //추적 상태
        Attack,      //일반 공격을 가하고 있는 상태
        Rest,        //일반 공격의 애니메이션 후 다음 재생해야할 애니메이션을 찾기위한 짧은 쉬는 상태
        Wait,        //애니메이션 전환 사이에 현재 상태에 따른 동작(monState에 따라 Update에서 실행되는 동작)을 멈추도록 만들기 위한 대기상태
        Die          //죽은 상태
    }

    #region ------------------------------- 스킬 설정 ----------------------------------------------    
    public struct SkillSet    //스킬 쿨타임과 확률을 설정하기 위한 구조체
    {
        public int[] Prob { get; private set; }     //각 스킬의 확률을 넣을 배열
        public float[] Cool { get; private set; }   //각 스킬의 쿨타임을 넣을 배열

        public SkillSet(Skill Count)        //스킬개수에 따라 배열 개수 결정 하는 생성자
        {
            Prob = new int[(int)Count];
            Cool = new float[(int)Count];
        }
    }
    public SkillSet SkSet { get; } = new(Skill.Count); //몬스터의 전용 스크립트에서 설정
    protected bool[] skillOnOff = { true, true };      //스킬을 쓸수 있는 지 확인하기 위한 변수
    #endregion ------------------------------- 스킬 설정 ----------------------------------------------

    #region ------------------- 추가 수치 -------------------
    //------------------------ 받는 버프&디버프 효과 ------------------------
    protected struct AddStat
    {
        public float MulAtkDmg;     //공격 시 곱해지는 대미지 비율
        public float MulTakeDmg;    //피격 시 곱해지는 대미지 비율
        public int AddAtkDmg;       //공격 시 더해지는 대미지
        public int DecDefence;      //방어력 감소량
        public int DecMDefence;     //마법저항력 감소량
    }

    protected AddStat curAddStat;   //현재 적용되고있는 추가스탯
    //------------------------ 받는 버프&디버프 효과 ------------------------
    #endregion ------------------- 추가 수치 -------------------
        
    [SerializeField]
    protected Transform firePos = null;             //투사체를 이용한 공격을 할때 투사체 생성점(원거리 몬스터만 사용)
    [SerializeField]
    private Transform hitPoint = null;              //공격을 받을 때 타겟이 되는곳(몸의 중앙)
    public Transform GetHitPoint { get { return hitPoint; } }    //타격점의 transform을 가져가기 위한 프로퍼티
    [SerializeField]
    protected Image hpImg = null;                   //HPCanvas 하위의 HpBar 연결 (몬스터의 체력을 반영하여 fillAmount를 변경하기 위한 Image)
    [SerializeField]
    protected GameObject ultiEffect = null;         //궁극기 이펙트
    [SerializeField]
    protected GameObject deathEffect = null;        //사망 이펙트
    protected Rigidbody rigidBd = null;             //몬스터의 RigidBody
    protected Animator animator = null;             //몬스터의 Animator
    protected SphereCollider attackRangeCol = null; //공격 타겟 추적용 SphereCollider
    protected MonsterState monState = MonsterState.Idle;        //몬스터의 상태
    protected float rotAngle = 8.0f;        //몬스터의 FixedTime당 회전각도
    protected PhotonView photonV = null;    //멀티 동작에 필요한 PhotonView를 가져오기 위한 변수
    protected NavMeshAgent navAgent = null; //이 몬스터의 움직임을 컨트롤 하기위한 네비매쉬
    protected int movePriority = 50;        //목적지를 향해 이동할 때 이 몬스터를 방해물 취급하지 않기 위한 변수
    protected int atkPriority = 1;          //목적지를 향해 이동할 때 이 몬스터를 방해물 취급하기 위한 변수

    //-------------------- 몬스터 정보
    public MonsterStat CMCmonStat { get; private set; }  //몬스터의 기본스탯 (변하지 않는 해당 몬스터의 고유 스탯)
    public MonsterStat CMCcurStat;                       //몬스터의 현재스탯 (적의 스킬이나 팀의 버프 등에 의한 변동을 반영하기 위한 스탯)
    public int UltiGage { get; protected set; } = 0;     //몬스터의 궁극기 게이지
    public int TotalDmg { get; protected set; } = 0;     //적에게 가한 총 대미지 (전투 끝난 후 표시)
    public int TotalHP { get; protected set; } = 0;      //적에게 받은 총 피해 (전투 끝난 후 표시)
    //-------------------- 몬스터 정보

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigidBd = GetComponent<Rigidbody>();            //몬스터의 RigidBody 컴포넌트 가져오기
        animator = GetComponent<Animator>();            //몬스터의 Animator 가져오기
        attackRangeCol = GetComponent<SphereCollider>();//몬스터의 대상 추적용 SphereCollider 가져오기
        monState = MonsterState.Idle;                   //초기 몬스터 상태 : 숨쉬기 상태
        photonV = GetComponent<PhotonView>();           //멀티 플레이를 위한 PhotonView
        navAgent = GetComponent<NavMeshAgent>();        //움직임을 위한 네비매쉬 가져오기        
    }

    protected virtual void Start()
    {
        //--------------------- 싱글에서만 실행 ---------------------
        if (ResultData.PrevScene != SceneList.MultiGameScene)
        {
            if (Enum.TryParse(tag, out Team TeamTag))
                switch (TeamTag)
                {
                    case Team.Ally:
                        CMCmonStat = FindClass.CurSetPoint.GetPointMSC(transform.parent.GetSiblingIndex()).MSCMonStat;       //몬스터 스탯 설정
                        hpImg.color = Color.green;      //다른 팀의 몬스터와 구분이 되도록 HP의 색상 변경
                        break;

                    case Team.Enemy:
                        //랭크 정보에 맞는 스탯 가져오기
                        if (ResultData.PrevScene.Equals(SceneList.InGameScene))
                            CMCmonStat = MonsterData.MonDic[Enum.Parse<MonsterName>(name.Substring(0, name.IndexOf("(")))][PlayerInfo.CombatStage - 1];
                        else if (ResultData.PrevScene.Equals(SceneList.RankGameScene))
                            CMCmonStat = MonsterData.MonDic[FindClass.RankDName[transform.parent.GetSiblingIndex()]][FindClass.RankDStar[transform.parent.GetSiblingIndex()]];
                        FindClass.GetCMCListFunc(tag).Add(GetComponent<CmnMonCtrl>());       //이 몬스터의 CmnMonCtrl을 리스트에 추가
                        hpImg.color = Color.red;        //다른 팀의 몬스터와 구분이 되도록 HP의 색상 변경
                        break;
                }
        }
        //--------------------- 싱글에서만 실행 ---------------------

        CMCcurStat = CMCmonStat;                        //기본스탯 만큼 현재스탯 설정
        navAgent.avoidancePriority = movePriority;      //몬스터의 처음 인식상태를 방해물 취급이 아닌상태로 변경
        navAgent.stoppingDistance = CMCcurStat.attackRange; //몬스터가 이동할 때 멈출 구간을 공격사거리에 맞게 변경
        navAgent.speed = CMCcurStat.moveSpd;            //네비매쉬의 움직이는 속도를 이동속도에 맞게 변경
        navAgent.acceleration = 10;                     //네비매쉬의 가속 설정
        rigidBd.constraints = RigidbodyConstraints.FreezeRotation; //회전 막기
        attackRangeCol.enabled = false;     //추적용 Collider 초기 비활성화
        attackRangeCol.radius = 0.0f;       //추적용 Collider 초기값 설정
        attackRangeCol.isTrigger = true;    //추적용 Collider 초기 충돌상태 설정
    }

    #region ----------------- 멀티 전용 -----------------        
    public void SetMonBasics(int MonNameIndex, int StarForce, Team TeamTag)     //멀티에서 몬스터의 기본정보를 세팅해주기위한 함수 (MtCbtSetPoint에서 실행)
    {
        photonV.RPC("SetMonStat", RpcTarget.All, MonNameIndex, StarForce, TeamTag);      //로컬과 원격 모두에서 기본설정을 해주기위한 RPC 실행
    }

    [PunRPC]
    public void SetMonStat(int MonNameIndex, int StarForce, Team TeamTag)
    {
        CMCmonStat = MonsterData.MonDic[(MonsterName)MonNameIndex][StarForce];  //몬스터 기본스탯 설정
        tag = TeamTag.ToString();       //몬스터의 팀 태그 설정
        FindClass.GetCMCListFunc(tag).Add(GetComponent<CmnMonCtrl>());          //이 몬스터의 CmnMonCtrl을 해당 팀의 리스트에 추가

        switch(TeamTag)
        {
            case Team.Team1:
                hpImg.color = Color.red;        //팀에 따라 HP색상 변경
                break;

            case Team.Team2:
                hpImg.color = Color.blue;       //팀에 따라 HP색상 변경
                break;
        }
    }
    #endregion ----------------- 멀티 전용 -----------------
}
