using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mutant의 궁극기 컨트롤 스크립트
//MutUltiDum프리팹에 붙여서 사용
[RequireComponent(typeof(CapsuleCollider))]
public class MutUltiDumCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject ultiDumEffect = null;        //피격 이펙트
    private Mutant ultiDumMutant = null;            //생성자의 컨트롤 스크립트를 받아올 변수

    private void Awake()
    {
        tag = transform.parent.tag;     //태그 변경
        ultiDumMutant = FindClass.GetMonCMC(tag, MonsterName.Mutant).GetComponent<Mutant>();    //생성자의 컨트롤 스크립트 가져오기
        transform.localScale = new Vector3(1.0f, 1.0f, ultiDumMutant.MutUltiMoveR / ultiDumMutant.MutUltiRange);     //이펙트 크기 & 피해 범위 조정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))  //적일 경우
        {
            ultiDumMutant.GiveDamage(other.transform.parent.gameObject, ultiDumMutant.GetUltiDmg(ultiDumMutant.CMCmonStat.starForce));        //대미지 입히기
            Instantiate(ultiDumEffect, other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.transform.position, Quaternion.identity, FindClass.AreaTrFunc(tag));        //피격 이펙트 생성
        }
    }
}
