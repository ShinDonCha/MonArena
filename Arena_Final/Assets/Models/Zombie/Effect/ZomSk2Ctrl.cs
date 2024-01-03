using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie의 두번째 스킬을 컨트롤 하기위한 스크립트
//ZomSkill2 프리팹에 붙여서 사용
[RequireComponent(typeof(SphereCollider))]
public class ZomSk2Ctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject provokeEffect = null;       //도발에 당한 상대에게 표시해줄 이펙트
    private Zombie provokeZombie = null;           //생성자(Zombie)의 컨트롤 스크립트를 가져올 변수
    private SphereCollider sCollider = null;       //도발대상 추적용 SphereCollider
    private readonly float traceTime = 1.0f;       //도발 대상을 찾는 시간
        
    private void Awake()
    {
        provokeZombie = GetComponentInParent<Zombie>(); //생성자의 컨트롤 스크립트 가져오기
        sCollider = GetComponent<SphereCollider>();     //이 오브젝트의 SphereCollider 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        if (sCollider.radius < provokeZombie.ZomSk2Radius)      //현재 대상 추적용 콜라이더의 크기가 정해진 추적 범위보다 작다면
            sCollider.radius += (provokeZombie.ZomSk2Radius / traceTime) * Time.deltaTime;  //traceTime 동안 최대 ZomSk2Radius 만큼 확장시키면서 대상을 찾기
        else
            sCollider.enabled = false;                  //추적 시간 후 적 감지 하는 SphereCollider 꺼주기
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !provokeZombie.CompareTag(other.transform.parent.tag))        //적군일 경우
        {
            CmnMonCtrl a_CMC = other.GetComponentInParent<CmnMonCtrl>();    //적군의 몬스터 컨트롤 스크립트 가져오기
            a_CMC.EnemySet(provokeZombie.gameObject);                       //적들의 공격 목표를 Zombie로 변경           
            a_CMC.TakeAny(TakeAct.Dmg, -provokeZombie.GetReduceRatio(provokeZombie.CMCmonStat.starForce), provokeZombie.ZomSk2SusTime);   //대미지 감소 디버프 적용
            a_CMC.GetComponentInChildren<StCanvasCtrl>().EffectSet(provokeEffect, provokeZombie.ZomSk2SusTime);  //적에게 도발 이펙트 생성
        }
    }
}
