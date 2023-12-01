using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jammo의 전용 스크립트
//Jammo 프리팹에 붙여서 사용
public class Jammo : CmnMonCtrl
{
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 30;      //첫번째 스킬 확률 30%
        SkSet.Cool[(int)Skill.Sk1] = 4.0f;    //첫번째 스킬 쿨타임 4초

        SkSet.Prob[(int)Skill.Sk2] = 20;      //두번쨰 스킬 확률 20%     
        SkSet.Cool[(int)Skill.Sk2] = 8.0f;    //두번째 스킬 쿨타임 8초
    }
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject jamAtkPrefab = null;        //일반공격 시 생성할 투사체
    [SerializeField]
    private GameObject jamAtkEffect = null;        //일반공격 피격 이펙트

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject jamUltiPrefab = null;       //궁극기 사용시 생성할 투사체
    [SerializeField]
    private GameObject jamUltiEffect = null;       //궁극기 피격 이펙트
    private readonly int[] jamUltDmg = { 40, 45, 50, 55, 60, 65 };  //궁극기 스킬 대미지
    public int GetUltiDmg(int Index) { return jamUltDmg[Index]; }   //외부에서 궁극기의 대미지를 가져가기 위한 함수    

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject jamSk1Prefab = null;        //Skill1 사용 시 생성할 파티클이 포함된 오브젝트
    private bool jamSk1Charge = false;             //Skill1 스킬 충전 여부
    private readonly float[] jamSk1Mul = { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f };   //Skill1 발동 시 증가하는 대미지 배율
    public float GetSk1Mul(int Index) { return jamSk1Mul[Index]; }   //외부에서 Skill1의 배율을 가져가기 위한 함수    

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject jamSk2Prefab = null;       //Skill2 프리팹
    public float JamSk2Time { get; } = 5.0f;      //Skill2 지속시간
    private readonly float[] jamSk2Value = { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f };   //Skill2 공격속도 증가 수치
    public float GetSk2Value(int Index) { return jamSk2Value[Index]; }  //외부에서 Skill2의 증가치를 가져가기 위한 함수    
    public float JamSk2Radius { get; } = 2.5f;    //Skill2의 범위

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
        GameObject a_GO = Instantiate(jamAtkPrefab, transform);       //기본공격 프리팹 생성
        a_GO.GetComponent<LightningBoltScript>().SetPos(firePos.gameObject, Enemy, jamAtkEffect);     //프리팹의 시작점부터 끝점까지 길이 설정
        Destroy(a_GO, 0.36f);     //일정시간이 지난 후에 삭제

        int a_Damage;        //지금 공격에서 입힐 대미지를 담을 지역변수
        if(jamSk1Charge)     //Skill1이 적용돼있는 상태라면
        {
            LineRenderer a_LR = a_GO.GetComponent<LineRenderer>(); //프리팹의 LinRenderer에 접근
            a_LR.startColor = new Color32(40, 132, 255, 255);      //색상 변경
            a_LR.startWidth *= 1.5f;        //시작 넓이 1.5배
            a_LR.endWidth *= 2.5f;          //끝 넓이 2.5배
            a_Damage = (int)(CMCcurStat.attackDmg * jamSk1Mul[CMCmonStat.starForce]);     //증가한 대미지를 적용
            jamSk1Charge = false;           //스킬사용상태 리셋
        }
        else
            a_Damage = CMCcurStat.attackDmg;   //현재 공격력을 적용

        GiveDamage(Enemy, a_Damage);              //대미지 입히기
        base.Attack();        //스킬 발동 체크, 궁극기 게이지 획득 등 공통 실행 부분
    }

    protected override void UltiSkill()     //궁극기 : 적 1명에게 강한 공격
    {
        GameObject a_GO = Instantiate(jamUltiPrefab, transform);    //궁극기 프리팹 생성
        a_GO.GetComponent<LightningBoltScript>().SetPos(firePos.gameObject, Enemy, jamUltiEffect);     //프리팹의 시작점부터 끝점까지 길이 설정
        Destroy(a_GO, 0.62f);         //일정시간이 지난 후에 삭제

        GiveDamage(Enemy, jamUltDmg[CMCmonStat.starForce]);   //대미지 입히기

        base.UltiSkill();             //궁극기 게이지 초기화, 애니메이션 초기화 등 공통 실행 부분
    }

    protected override void Skill1()    //스킬 1 : 전격을 충전해 다음공격에 대미지를 추가한다
    {
        Instantiate(jamSk1Prefab, transform);   //이펙트 생성
        jamSk1Charge = true;     //충전상태로 변경
    }

    protected override void Skill2()    //스킬 2 : 일정시간동안 주변 아군들의 공격속도를 빠르게 한다
    {
        Instantiate(jamSk2Prefab, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));     //프리팹 생성
    }
}
