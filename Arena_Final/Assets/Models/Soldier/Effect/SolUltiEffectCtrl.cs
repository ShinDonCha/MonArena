using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier의 궁극기 이펙트와 대미지 계산 스크립트
//SolUltiEffect프리팹에 붙여서 실행
[RequireComponent(typeof(SphereCollider))]
public class SolUltiEffectCtrl : MonoBehaviour
{
    private Soldier ultiDmgSoldier = null;        //생성자의 몬스터 컨트롤 스크립트를 담을 변수
    private SphereCollider sphereCol = null;      //이 오브젝트의 SphereCollider

    private void Awake()
    {
        tag = transform.parent.tag;
        ultiDmgSoldier = FindClass.GetMonCMC(transform.parent.tag, MonsterName.Soldier).GetComponent<Soldier>();   //생성자의 몬스터 컨트롤 스크립트 가져오기
        sphereCol = GetComponent<SphereCollider>();
        sphereCol.radius = ultiDmgSoldier.SolUltiRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))        //적일 경우
        {
            ultiDmgSoldier.GiveDamage(other.transform.parent.gameObject, ultiDmgSoldier.GetUltiDmg(ultiDmgSoldier.CMCmonStat.starForce));     //대미지 입히기
        }
    }
}
