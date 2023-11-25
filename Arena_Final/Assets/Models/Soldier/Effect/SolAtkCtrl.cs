using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier의 일반공격 투사체를 컨트롤 하기위한 스크립트
//SolAttack프리팹에 붙여서 사용
public class SolAtkCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject hitEffect = null;    //타격시 나타나는 이펙트

    private Soldier SolAtkSoldier = null;   //생성자(Soldier)의 컨트롤 스크립트
    private CmnMonCtrl enemyCMC = null;     //공격 대상의 CmnMonCtrl

    private void Awake()
    {
        SolAtkSoldier = FindClass.GetMonCMC(transform.parent.tag, MonsterName.Soldier).GetComponent<Soldier>();    //생성자(Soldier)의 컨트롤 스크립트 가져오기
        enemyCMC = SolAtkSoldier.Enemy.GetComponent<CmnMonCtrl>();   //공격 대상의 CmnMonCtrl 가져오기
    }

    // Update is called once per frame
    void Update()
    {      
        if (!enemyCMC.gameObject.activeSelf)      //공격 목표가 죽은상태면 삭제
            Destroy(gameObject);

        transform.position = Vector3.MoveTowards(transform.position, enemyCMC.GetHitPoint.position, 10.0f * Time.deltaTime);      //목표를 향해 계속 이동
        transform.LookAt(enemyCMC.GetHitPoint);     //공격 목표를 바라보도록 회전
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && enemyCMC.gameObject.Equals(other.transform.parent.gameObject))      //충돌 대상이 공격 목표라면
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity, transform.parent);    //이펙트 생성
            SolAtkSoldier.GiveDamage(enemyCMC.gameObject, SolAtkSoldier.CMCcurStat.attackDmg);   //대미지 입히기
            Destroy(gameObject);    //이 오브젝트 삭제
        }
    }
}
