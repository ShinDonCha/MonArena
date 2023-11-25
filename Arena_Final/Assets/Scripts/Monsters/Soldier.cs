using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier의 전용 스크립트
//Soldier 프리팹에 붙여서 사용
public class Soldier : CmnMonCtrl
{       
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수
    protected override void SkillSetting()
    {
        SkSet.Prob[(int)Skill.Sk1] = 15;      //첫번째 스킬 확률 15%
        SkSet.Cool[(int)Skill.Sk1] = 6.0f;    //첫번째 스킬 쿨타임 6초

        SkSet.Prob[(int)Skill.Sk2] = 10;      //두번째 스킬 확률 10%
        SkSet.Cool[(int)Skill.Sk2] = 10.0f;   //두번째 스킬 쿨타임 10초
    }
    //---------- 이 몬스터의 스킬 확률&쿨타임 설정 함수    

    [Header("-------------- Normal --------------")]
    //------ Normal
    [SerializeField]
    private GameObject solAtkPrefab = null;       //일반공격 시 생성할 투사체
    [SerializeField]
    private GameObject solAtkEffect = null;       //일반공격 총알 발사 이펙트

    [Header("-------------- Ultimate --------------")]
    //------ Ultimate
    [SerializeField]
    private GameObject solUltiPrefab = null;      //궁극기 사용 시 생성할 미사일
    private readonly int[] solUltiDmg = { 30, 35, 40, 45, 50, 55 };   //궁극기 대미지 (몬스터의 Starforce에 따라 다른 대미지)
    public int GetUltiDmg(int Index) { return solUltiDmg[Index]; }    //외부에서 궁극기의 대미지를 가져가기 위한 함수
    public float SolUltiRadius { get; } = 1.5f;    //궁극기 타격 범위

    [Header("-------------- Skill1 --------------")]
    //------ Skill1
    [SerializeField]
    private GameObject solSk1Prefab = null;       //Skill1의 이펙트 프리팹
    private readonly float[] solSk1DecRatio = { 0.15f, 0.13f, 0.11f, 0.1f, 0.09f, 0.08f };  //스킬 사용 시 체력 감소 비율
    public float GetDecRatio(int Index) { return solSk1DecRatio[Index]; }   //외부에서 Skill1의 감소 비율을 가져가기 위한 함수
    public int SolSk1UltiG { get; } = 20;         //스킬 사용 시 회복되는 궁극기 게이지

    [Header("-------------- Skill2 --------------")]
    //------ Skill2
    [SerializeField]
    private GameObject solSk2Effect = null;    //Skill2의 이펙트
    [SerializeField]
    private GameObject[] dronsPrefab;          //드론 오브젝트들 (드론 프리팹 연결)
    [SerializeField]
    private Vector3[] dronsAddPos;             //드론 생성 위치를 설정 하기 위한 보정값 (유니티에서 설정)
    private readonly int[] DronDamage  = { 10, 11, 12, 13, 14, 15 };   //드론의 총알 대미지
    public int GetDronDmg(int Index) { return DronDamage[Index]; }     //외부에서 Skill2의 대미지를 가져가기 위한 함수
    public float DronLifeT { get; } = 2.0f;    //드론의 지속시간
    public float DronASpd { get; } = 0.5f;     //드론의 공격속도

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
        Instantiate(solAtkEffect, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //기본공격 이펙트 생성
        Instantiate(solAtkPrefab, firePos.position, Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));    //기본공격 투사체 생성
        base.Attack();       //스킬 발동 체크, 궁극기 게이지 획득 등 공통 실행 부분
    }

    protected override void UltiSkill()   //궁극기 : 핵미사일을 적에게 떨어트려 주변에 대미지를 입힌다.
    {
        Vector3 a_Vec = Enemy.transform.position;   //적의 위치 가져오기
        a_Vec.y += 5.0f;   //적의 위치보다 5만큼 더 높은 위치에 생성하도록 위치 보정

        Instantiate(solUltiPrefab, a_Vec, Quaternion.LookRotation(Enemy.transform.position - a_Vec), FindClass.AreaTrFunc(tag));    //궁극기 미사일 생성

        base.UltiSkill();                 //궁극기 게이지 초기화, 애니메이션 초기화 등 공통 실행 부분
    }

    protected override void Skill1()        //스킬 1 : 일정비율의 HP를 소모하고 궁극기 게이지 충전
    {
        int a_CurHP = CMCcurStat.hp;        //현재 체력 가져오기
        a_CurHP -= (int)(CMCmonStat.hp * solSk1DecRatio[CMCmonStat.starForce]); //스킬 발동했을 경우 남게되는 HP계산

        if (a_CurHP <= (CMCmonStat.hp * 0.2f))    //스킬 발동 후 남는 체력이 최대체력의 20%이하면 스킬발동 안함
            return;

        Instantiate(solSk1Prefab, transform);     //이펙트 생성
        CMCcurStat.hp -= (int)(CMCmonStat.hp * solSk1DecRatio[CMCmonStat.starForce]);    //자신의 체력 일정비율 감소
        hpImg.fillAmount = (float)CMCcurStat.hp / CMCmonStat.hp;    //몬스터 머리위의 HP바 표시 변경
        AddUltiGage(20);       //UltiGage획득
    }

    protected override void Skill2()     //스킬 2 : 드론 4대를 생성하여 공격
    {
        StartCoroutine(DronSet());      //드론생성 코루틴 실행
    }

    private IEnumerator DronSet()       //Skill2의 드론생성 코루틴
    {
        Instantiate(solSk2Effect, transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));       //이펙트 생성
        yield return new WaitForSeconds(0.3f);

        //이 몬스터가 보고있는 방향을 바라보도록 드론 생성
        for (int i = 0; i < dronsPrefab.Length; i++)
            Instantiate(dronsPrefab[i], (transform.position + dronsAddPos[i]), Quaternion.LookRotation(transform.forward), FindClass.AreaTrFunc(tag));     //드론 생성
    }
}
