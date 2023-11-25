using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GamblerCat의 궁극기 스크립트
//CatUltimate 프리팹에 붙여서 사용
[RequireComponent (typeof(SphereCollider))]
public class CatUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject buffEffect = null;      //궁극기를 받는 대상에게 표시할 이펙트
    private GamblerCat ultiGamblerCat = null;  //생성자의 몬스터 컨트롤 스크립트를 담을 변수
    private SphereCollider sCollider = null;   //이 오브젝트의 SphereCollider    

    private void Awake()
    {
        sCollider = GetComponent<SphereCollider>();     //SphereCollider 가져오기
        ultiGamblerCat = FindClass.GetMonCMC(transform.parent.tag, MonsterName.GamblerCat).GetComponent<GamblerCat>();  //생성자의 컨트롤 스크립트 가져오기
    }

    // Start is called before the first frame update
    void Start()
    {        
        sCollider.radius = ultiGamblerCat.CatUltiRadius;   //대상을 찾을 반지름 설정
        sCollider.isTrigger = true;                        //isTrigger상태 온
        Destroy(gameObject, 1.0f);                         //일정시간 후 이 오브젝트 삭제
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && ultiGamblerCat.CompareTag(other.transform.parent.tag))  //아군일 경우
        {
            Instantiate(buffEffect, other.transform.parent);        //궁극기 이펙트 생성
            other.GetComponentInParent<CmnMonCtrl>().TakeAny(TakeAct.Heal, ultiGamblerCat.GetUltiAmount(ultiGamblerCat.CMCmonStat.starForce));    //대상을 회복시키기
        }
    }
}
