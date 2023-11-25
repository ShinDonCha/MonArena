using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Mutant의 전용 스크립트
//Mutant 프리팹에 붙여서 사용
public class Mutant : CmnMonCtrl
{
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수 (Mutant는 스킬이 Active 형식이 아님)
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 0;      //첫번째 스킬 확률 0%
        SkSet.Cool[(int)Skill.Sk1] = 0.0f;   //첫번째 스킬 쿨타임 0초

        SkSet.Prob[(int)Skill.Sk2] = 0;      //두번째 스킬 확률 0%
        SkSet.Cool[(int)Skill.Sk2] = 0.0f;   //두번째 스킬 쿨타임 0초
    }
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수 (Mutant는 스킬이 Active 형식이 아님)

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject mutAtkEffect = null;     //일반공격 피격 이펙트

    //------ Ultimate
    [Header("-------------- Ultimate --------------")]
    [SerializeField]
    private GameObject mutUltiEffect = null;       //궁극기 이펙트 프리팹
    [SerializeField]
    private GameObject mutUltiDummy = null;         //궁극기 대미지 더미 프리팹
    private readonly int[] mutUltiDmg = { 20, 23, 26, 29, 32, 35 };  //궁극기 대미지
    public int GetUltiDmg(int Index) { return mutUltiDmg[Index]; }   //외부에서 궁극기의 대미지를 가져가기 위한 함수
    public float MutUltiRange { get; } = 3.5f;      //최대 이동거리
    public float MutUltiMoveR { get; private set; } = 0.0f;    //이번 궁극기에 이동할 거리

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject mutSk1Effect = null;     //Skill1의 이펙트
    private readonly int[] mutSk1Dmg = { 11, 13, 15, 17, 19, 21 }; //Skill1의 추가 대미지
    public int GetSk1Dmg(int Index) { return mutSk1Dmg[Index]; }   //외부에서 Skill1의 추가 대미지를 가져가기 위한 함수
    private Team myTeam;                        //Skill1의 공격대상을 찾기위한 나의 태그

    //------ Skill2
    public float MutSk2AMul { get; } = 0.3f;     //Skill2의 공격력 증가량
    public float MutSk2TMul { get; } = 0.4f;     //Skill2의 받는피해 증가량

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();    //몬스터 공통 설정 실행
        Skill2();        //Skill2 발동(패시브 효과 적용)
    }

    protected override void StartFight()    //전투 시작 시 실행되는 함수
    {
        GetComponentInChildren<StCanvasCtrl>(true).gameObject.SetActive(true);     //HP바 표시를 위한 캔버스 On상태로 변경

        if (photonV != null && !photonV.IsMine)     //멀티 원격인 경우 취소
            return;

        Skill1();
    }

    protected override void Attack()        //일반 공격
    {
        Instantiate(mutAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));  //이펙트 생성
        GiveDamage(Enemy, CMCcurStat.attackDmg);     //대미지 입히기
        base.Attack();      //스킬 발동 체크, 궁극기 게이지 획득 등 공통 실행 부분
    }

    protected override void UltiSkill()     //궁극기 : 전방으로 빠르게 이동 후 경로에있는 적 공격
    {
        MutUltiMoveR = MutUltiRange;       //이 몬스터가 이동할 수 있는 최대 거리

        if (Physics.Raycast(GetHitPoint.position, transform.forward, out RaycastHit rayHit, MutUltiRange, 1 << (int)LayerName.Wall))   //최대 이동거리안에 벽이 있을경우
            MutUltiMoveR = ((rayHit.point - transform.forward * 0.5f) - GetHitPoint.position).magnitude;    //이동거리 수정

        Vector3 a_DummyPos = transform.position + (transform.forward * (MutUltiMoveR / 2));    //대미지를 입히기 위한 더미의 생성위치
        a_DummyPos.y += 0.8f;       //위치값 보정

        Instantiate(mutUltiEffect, transform.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));     //궁극기 이펙트 생성

        transform.position += transform.forward * MutUltiMoveR;    //계산된 거리만큼 이동

        StartCoroutine(UltiDumSet(a_DummyPos));     //대미지를 입히기 위한 더미 생성코루틴 실행

        base.UltiSkill();       //궁극기 게이지 초기화, 애니메이션 초기화 등 공통 실행 부분 궁극기 마지막에 실행
    }

    protected override void Skill1()        //스킬 1 : 암살 - 체력이 제일 적은 적의 뒤로 이동 후 강화공격
    {
        if (!System.Enum.TryParse(tag, out myTeam))     //이 몬스터의 태그를 Enum형식으로 변경
            return;

        Sk1TargetSet();      //Skill1의 공격대상 찾기 실행
    }

    protected override void Skill2()        //스킬 2 : 공격력이 증가하는 대신 받는 대미지 증가(패시브)
    {
        curAddStat.MulAtkDmg += MutSk2AMul;     //입히는 대미지 증가
        curAddStat.MulTakeDmg += MutSk2TMul;    //받는 대미지 증가
    }

    private void Sk1TargetSet()      //Skill1의 공격 대상을 찾기위한 함수
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

        if (a_CMCList.Count == 0)   //탐색된 적 몬스터 리스트가 없을경우 취소
            return;

        a_CMCList.Sort((a, b) => a.CMCcurStat.hp.CompareTo(b.CMCcurStat.hp));   //적 몬스터 목록을 HP가 적은 순서대로 정렬

        if (!a_CMCList[0].gameObject.name.Contains(MonsterName.Mutant.ToString()))  //뮤턴트 -> 뮤턴트는 불가
            EnemySet(a_CMCList[0].gameObject);     //공격 목표 설정
        else
            if (a_CMCList.Count > 1)       //다른 추적할 대상이 있으면
            EnemySet(a_CMCList[1].gameObject);     //공격 목표 설정

        if (Enemy == null)      //결과적으로 목표를 찾지 못한 상황이면
        {
            base.StartFight();  //기본 시작함수 실행
            return;
        }

        //---------- 공격대상을 찾았을 경우만 나머지 실행
        if (photonV == null)     //싱글
            Sk1On();
        else                    //멀티(IsMine만 실행)
            photonV.RPC("Sk1On", RpcTarget.All);
        //---------- 공격대상을 찾았을 경우만 나머지 실행
    }

    [PunRPC]
    private void Sk1On()        //현재 게임상태(싱글,멀티)에 따라 RPC를 사용하여 스킬을 실행하기위한 함수
    {
        StartCoroutine(Assassinate());
    }

    private IEnumerator Assassinate()   //암살 - 체력이 제일 적은 적의 뒤로 이동 후 강화공격
    {        
        GameObject a_GO = Instantiate(mutSk1Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));   //스킬 이펙트 생성
        AudioSource a_Sk1Audio = a_GO.GetComponent<AudioSource>();
        a_Sk1Audio.Play();      //Skill1의 효과음 재생
        yield return new WaitForSeconds(0.2f);

        rigidBd.useGravity = false;     //이 몬스터를 이동시키기 위해 잠시 중력 제거
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);      //이 몬스터 은신상태 설정(하위의 모든 게임오브젝트 끄기)

        Vector3 a_dirVec = (Enemy.transform.position - transform.position).normalized;       //이 몬스터가 공격목표를 바라볼때의 방향벡터 계산

        transform.position = Enemy.transform.position + a_dirVec * CMCcurStat.attackRange;  //목표로부터 이 몬스터의 공격거리만큼 떨어진 위치로 이동
        transform.forward = (Enemy.transform.position - transform.position).normalized;     //목표를 바라보도록 회전
        yield return new WaitForSeconds(0.6f);

        Instantiate(mutSk1Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));   //스킬 이펙트 생성
        yield return new WaitForSeconds(0.4f);

        rigidBd.useGravity = true;      //이 몬스터의 중력 복구
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);      //이 몬스터 은신상태 해제(하위의 모든 게임오브젝트 켜기)

        curAddStat.AddAtkDmg += mutSk1Dmg[CMCmonStat.starForce];   //다음 공격에 추가할 대미지 설정
        animator.Play(MonsterState.Attack.ToString());             //공격 애니메이션 실행
    }

    private IEnumerator UltiDumSet(Vector3 DumPos)  //궁극기 더미 생성코루틴
    {
        yield return new WaitForSeconds(0.7f);      //0.7초 후 실행
        Instantiate(mutUltiDummy, DumPos, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //대미지 더미 생성
    }
}
