using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie의 궁극기 사용시 대미지를 주기위한 스크립트
//ZomUltimate프리팹의 ZomUltiDum에 붙여서 사용
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]

public class ZomUltiDumCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject ultiEffect = null;           //궁극기 피격 이펙트
    private Zombie ultiZombie = null;               //스킬의 생성자의 컨트롤 스크립트를 가져올 변수
    private SphereCollider ultiSColl = null;        //대상을 추적하기 위한 SphereCollider

    private void Awake()
    {
        ultiZombie = GetComponentInParent<Zombie>();    //생성자의 컨트롤 스크립트 가져오기
        ultiSColl = GetComponent<SphereCollider>();     //이 오브젝트의 SphereCollider 가져오기
        ultiSColl.radius = ultiZombie.ZomUltiRadius;    //적 추적 범위 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !transform.parent.CompareTag(other.transform.parent.tag))     //적일 경우
        {
            Instantiate(ultiEffect, other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(transform.parent.tag));
            int a_CalcDmg = ultiZombie.GiveDamage(other.transform.parent.gameObject, ultiZombie.GetUltiDmg(ultiZombie.CMCmonStat.starForce));  //대미지 입히기
            ultiZombie.TakeAny(TakeAct.Heal, a_CalcDmg * ultiZombie.ZomUltiRatio);    //입힌 대미지 * 회복비율로 체력 회복
        }
    }
}
