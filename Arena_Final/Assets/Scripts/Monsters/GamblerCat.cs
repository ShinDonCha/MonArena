using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//GamblerCat의 전용 스크립트
//GamblerCat 프리팹에 붙여서 사용
public class GamblerCat : CmnMonCtrl
{
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;       //첫번째 스킬 확률 30%
        SkSet.Cool[(int)Skill.Sk1] = 3.0f;    //첫번째 스킬 쿨타임 3초

        SkSet.Prob[(int)Skill.Sk2] = 20;      //두번쨰 스킬 확률 20%    
        SkSet.Cool[(int)Skill.Sk2] = 6.0f;   //두번째 스킬 쿨타임 6초
    }
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject catAtkPrefab = null;        //일반 공격 시 생성할 투사체

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject catUltiPrefab = null;     //궁극기 이펙트 프리팹
    private readonly int[] catUltiAmount = { 20, 25, 35, 45, 60, 100 };     //궁극기 회복량
    public int GetUltiAmount(int Index) { return catUltiAmount[Index]; }    //외부에서 궁극기의 회복량을 가져가기 위한 함수
    public float CatUltiRadius { get; } = 2.5f;  //궁극기 범위

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject catSk1Prefab = null;      //Skill1의 이펙트
    private readonly int[] catSk1HealAmount = { 10, 13, 16, 19, 22, 25 };      //Skill1의 회복량
    public int GetHealAmount(int Index) { return catSk1HealAmount[Index]; }    //외부에서 Skill1의 회복량을 가져가기 위한 함수
    private readonly int catSk1healCount = 2;    //Skill1을 받을 대상 수

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject catSk2Prefab = null;        //Skill2의 이펙트
    private readonly float[] catSk2BuffTime = { 3.0f, 3.5f, 4.0f, 4.5f, 5.0f, 6.0f }; //Skill2의 지속시간
    public float GetBuffTime(int Index) { return catSk2BuffTime[Index]; }             //외부에서 Skill2의 지속시간을 가져가기 위한 함수   
    public float CatSk2IncAmount { get; } = 0.2f;  //증가될 대미지 배율
    private readonly int catSk2BuffCount = 2;      //Skill2를 받을 대상 수

    protected override void Awake()
    {
        base.Awake();
        SkillSetting();     //스킬 확률 및 쿨타임 설정
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()    //일반 공격
    {
        Instantiate(catAtkPrefab, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));   //기본공격 투사체 생성
        base.Attack();      //스킬 발동 체크, 궁극기 게이지 획득 등 공통 실행 부분
    }

    protected override void UltiSkill()   //궁극기 : 일정 범위내 아군 모두의 체력을 회복시킨다
    {
        Instantiate(catUltiPrefab, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));    //궁극기 프리팹 생성
        base.UltiSkill();            //궁극기 게이지 초기화, 애니메이션 초기화 등 공통 실행 부분
    }

    protected override void Skill1()     //스킬 1 : 현재 체력이 낮은 아군 2명 힐
    {
        List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //전투에 참여중인 아군 몬스터 리스트 가져오기

        if (a_CMCList == null)      //오류 방지
            return;

        //전투 참여중인 몬스터 리스트를 생존우선으로 정렬한 뒤 체력이 낮은 순서로 정렬
        a_CMCList = a_CMCList.OrderByDescending(val => val.gameObject.activeSelf).ThenBy(val => val.CMCcurStat.hp).ToList();

        for (int i = 0; i < Mathf.Min(catSk1healCount, a_CMCList.Count); i++)   //스킬 대상의 수만큼 반복
        {
            if (!a_CMCList[i].gameObject.activeSelf)        //해당 몬스터가 죽은상태면 넘어가기(2명 선택인데 전장에 1명만 남을 경우도 있으므로)
                continue;

            a_CMCList[i].TakeAny(TakeAct.Heal, catSk1HealAmount[CMCmonStat.starForce]);   //회복
            Instantiate(catSk1Prefab, a_CMCList[i].GetHitPoint);      //힐 이펙트 생성
        }
    }

    protected override void Skill2()     //스킬 2: 현재 공격력이 높은 아군 2명에게 대미지 증가 버프
    {
        List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //전투에 참여중인 아군 몬스터 리스트 가져오기

        if (a_CMCList == null)      //오류 방지
            return;

        // 전투 참여중인 몬스터 리스트를 생존우선으로 정렬한 뒤 공격력이 높은 순서로 정렬
        a_CMCList = a_CMCList.OrderByDescending(val => val.gameObject.activeSelf).ThenByDescending(val => val.CMCcurStat.attackDmg).ToList();

        for (int i = 0; i < Mathf.Min(catSk2BuffCount, a_CMCList.Count); i++)     //스킬 대상의 수만큼 반복
        {
            if (!a_CMCList[i].gameObject.activeSelf)        //해당 몬스터가 죽은상태면 넘어가기(2명 선택인데 전장에 1명만 남을 경우도 있으므로)
                continue;

            a_CMCList[i].TakeAny(TakeAct.Dmg, CatSk2IncAmount, catSk2BuffTime[CMCmonStat.starForce]);      //대미지 증가
            Instantiate(catSk2Prefab, a_CMCList[i].transform);      //버프 이펙트 생성
        }
    }
}
