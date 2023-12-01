using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie의 전용 스크립트
//Zombie프리팹에 붙여서 사용
public class Zombie : CmnMonCtrl
{
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;      //첫번째 스킬 확률 30%
        SkSet.Cool[(int)Skill.Sk1] = 5.0f;    //첫번째 스킬 쿨타임 5초

        SkSet.Prob[(int)Skill.Sk2] = 20;      //두번째 스킬 확률 20%  
        SkSet.Cool[(int)Skill.Sk2] = 8.0f;    //두번째 스킬 쿨타임 8초
    }
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject zomAtkEffect = null;     //일반공격 피격 이펙트

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject zomUltiPrefab = null;       //궁극기 프리팹
    private readonly int[] zomUltiDmg = { 10, 15, 20, 25, 30, 35 };    //궁극기 대미지
    public int GetUltiDmg(int Index) { return zomUltiDmg[Index]; }     //외부에서 궁극기 대미지를 가져가기 위한 함수
    public float ZomUltiRatio { get; } = 0.5f;     //궁극기 대미지 전환률 50%
    public float ZomUltiTime { get; } = 4.0f;      //궁극기 지속시간
    public float ZomUltiRadius { get; } = 3.0f;    //궁극기 범위

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject zomSk1Effect = null;
    private readonly int[] zomSk1Dmg = { 10, 12, 14, 16, 18, 20 };   //Skill1 대미지
    public int GetSk1Dmg(int Index) { return zomSk1Dmg[Index]; }     //외부에서 Skill1의 대미지를 가져가기 위한 함수
    private readonly int[] zomSk1ReduVal = { 3, 4, 5, 6, 7, 8 };     //방,마저 감소량
    public int GetSk1ReduVal(int Index) { return zomSk1ReduVal[Index]; } //외부에서 방,마저 감소량을 가져가기 위한 함수    
    public float ZomSk1SusTime { get; } = 3.0f;      //Skill1의 지속시간

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject zomSk2Prefab = null;      //Skill2 사용 시 생성할 프리팹
    private readonly float[] zomSk2Ratio = { 0.25f, 0.30f, 0.35f, 0.4f, 0.45f, 0.5f};   //공격력 감소 비율
    public float GetReduceRatio(int Index) { return zomSk2Ratio[Index]; }   //외부에서 Skill2의 공격력 감소비율을 가져가기 위한 함수
    public float ZomSk2SusTime { get; } = 5.0f;     //공격력 감소 지속시간
    public float ZomSk2Radius { get; } = 3.5f;      //대상 추적 범위

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
        Instantiate(zomAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));  //이펙트 생성
        GiveDamage(Enemy, CMCcurStat.attackDmg);   //대미지 입히기
        base.Attack();                  //스킬 발동 체크, 궁극기 게이지 획득 등 공통 실행 부분
    }

    protected override void UltiSkill() //궁극기 : 자신의 주위에 저주를 내려 일정범위 안의 적들에게 대미지를 입히고 자신의 체력을 회복한다.
    {
        Instantiate(zomUltiPrefab, transform);     //궁극기 프리팹 생성
        base.UltiSkill();             //궁극기 게이지 초기화, 애니메이션 초기화 등 공통 실행 부분
    }

    protected override void Skill1()   //스킬 1 : 적을 물어 대미지를 입히고 방마저 감소
    {
        if (!Enemy.activeSelf)      //공격 대상이 죽었으면 취소
            return;
                
        GiveDamage(Enemy, zomSk1Dmg[CMCmonStat.starForce]);     //대미지 입히기
        CmnMonCtrl a_CMC = Enemy.GetComponent<CmnMonCtrl>();    //공격 대상의 컨트롤 스크립트 가져오기
        a_CMC.TakeAny(TakeAct.Defence, -zomSk1ReduVal[CMCmonStat.starForce], ZomSk1SusTime);      //적 방어력 감소
        a_CMC.TakeAny(TakeAct.MDefence, -zomSk1ReduVal[CMCmonStat.starForce], ZomSk1SusTime);     //적 마법저항력 감소
        Instantiate(zomSk1Effect, a_CMC.GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));      //이펙트 생성
    }

    protected override void Skill2()    //스킬 2 : 주위 일정범위 내 적을 도발해 일정시간 대미지를 낮추고 자신을 공격하게 한다
    {
        Instantiate(zomSk2Prefab, transform);    //Skill2의 이펙트 생성
    }
}
