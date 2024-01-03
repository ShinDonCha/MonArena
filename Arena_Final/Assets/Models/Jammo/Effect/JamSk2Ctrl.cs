using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jammo의 2번째 스킬(공속증가) 스크립트
//JamSkill2 프리팹에 붙여서 사용
[RequireComponent(typeof(SphereCollider))]
public class JamSk2Ctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject buffEffect = null;     //버프 이펙트
    private Jammo aSpdBuffJammo = null;       //생성자(Jammo)의 컨트롤 스크립트를 담을 변수
    private SphereCollider sphereCol = null;  //이 오브젝트의 SphereCollider

    private void Awake()
    {
        tag = transform.parent.tag;     //이 오브젝트의 태그 변경
        aSpdBuffJammo = FindClass.GetMonCMC(tag, MonsterName.Jammo).GetComponent<Jammo>();        //생성자(Jammo)의 컨트롤 스크립트 가져오기
        sphereCol = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sphereCol.radius = aSpdBuffJammo.JamSk2Radius;     //적용 범위 설정
        Destroy(gameObject, 1.0f);    //일정 시간이 지난 후 삭제
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && aSpdBuffJammo.CompareTag(other.transform.parent.tag))     //아군일때만 버프제공
        {
            CmnMonCtrl a_CMC = other.GetComponentInParent<CmnMonCtrl>();    //대상의 공용 컨트롤 스크립트 가져오기
            a_CMC.TakeAny(TakeAct.ASpd, aSpdBuffJammo.GetSk2Value(aSpdBuffJammo.CMCmonStat.starForce), aSpdBuffJammo.JamSk2Time);  //버프 적용
            Instantiate(buffEffect, a_CMC.transform);       //버프 이펙트 생성
        }
    }
}
