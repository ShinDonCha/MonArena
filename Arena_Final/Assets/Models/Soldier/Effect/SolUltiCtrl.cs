using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier의 궁극기 미사일 컨트롤 스크립트
//SolUltimate 프리팹에 붙여서 실행
public class SolUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform firePos = null;        //폭발이 일어날 위치 (하위의 FirePos 오브젝트)
    [SerializeField]
    private GameObject exploEffect = null;   //폭발 이펙트 프리팹
    [SerializeField]
    private GameObject warningPrefab = null; //타격 지점 표시 프리팹

    private void Start()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit Hit, Mathf.Infinity, 1 << (int)LayerName.Terrain))    //바닥에 경고이펙트 표시
            Instantiate(warningPrefab, Hit.point, Quaternion.Euler(new(90, 0, 0)), transform.parent);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("UltiWarning"))    //표식과 충돌했을 경우
        {
            Instantiate(exploEffect, firePos.position, Quaternion.identity, transform.parent);  //폭발 이펙트 생성
            Destroy(other.gameObject);      //표식 삭제
            Destroy(gameObject);            //미사일 오브젝트 삭제
        }
    }
}
