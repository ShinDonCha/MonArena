using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GamblerCat의 기본공격 투사체를 컨트롤 하기위한 스크립트
//CatAttack 프리팹에 붙여서 사용
[RequireComponent(typeof(Rigidbody))]
public class CatAtkCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject hitEffect = null;            //적을 맞혔을 때 생성할 이펙트
    private CmnMonCtrl gamblerCatCMC = null;        //생성자 GamblerCat의 컨트롤 스크립트를 담을 변수
    private CmnMonCtrl enemyCMC = null;             //공격 대상의 컨트롤 스크립트
    private Rigidbody rigidBd = null;               //이 오브젝트의 RigidBody 컴포넌트

    private readonly float forceSpd = 7.0f;         //투사체 초당 이동 속도
    private readonly float torqueSpd = 720.0f;      //투사체 초당 회전각

    private void Awake()
    {
        rigidBd = GetComponent<Rigidbody>();        //리지드바디 컴포넌트 가져오기
        gamblerCatCMC = FindClass.GetMonCMC(transform.parent.tag, MonsterName.GamblerCat);     //생성자의 컨트롤 스크립트 가져오기
        enemyCMC = gamblerCatCMC.Enemy.GetComponent<CmnMonCtrl>();   //공격 대상의 컨트롤 스크립트 가져오기
    }

    private void Update()
    {
        if (!enemyCMC.gameObject.activeSelf)      //목표가 죽은상태면 삭제
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Vector3 a_ReqRot = Quaternion.LookRotation(enemyCMC.GetHitPoint.position - rigidBd.position).eulerAngles;             //적을 바라보기 위해 필요한 회전값
        Vector3 a_CalcRot = new(a_ReqRot.x, a_ReqRot.y, rigidBd.rotation.eulerAngles.z + torqueSpd * Time.fixedDeltaTime);    //적을 바라보며 빙글빙글 돌기위해 필요한 회전값 계산
        rigidBd.rotation = Quaternion.Euler(a_CalcRot);          //회전값 설정
        rigidBd.position = Vector3.MoveTowards(rigidBd.position, enemyCMC.GetHitPoint.position, forceSpd * Time.fixedDeltaTime);     //목표를 향해 계속 이동        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && enemyCMC.gameObject.Equals(other.transform.parent.gameObject))     //충돌 대상이 공격 목표라면
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity, transform.parent);   //공격 적중 이펙트 생성
            gamblerCatCMC.GiveDamage(enemyCMC.gameObject, gamblerCatCMC.CMCcurStat.attackDmg);   //대미지 입히기
            Destroy(gameObject);    //이 오브젝트 삭제
        }
    }
}
