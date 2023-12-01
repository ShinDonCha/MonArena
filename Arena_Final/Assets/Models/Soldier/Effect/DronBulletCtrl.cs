using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//드론의 총알을 컨트롤 하기위한 스크립트
//DronBullet 프리팹에 붙여서 사용
[RequireComponent(typeof(Rigidbody))]
public class DronBulletCtrl : MonoBehaviour
{
    private Soldier dronBulletSoldier = null;      //생성자의 몬스터 컨트롤 스크립트를 담을 변수
    private Rigidbody rb = null;                   //총알의 리지드바디
    private readonly float BulletSpd = 10.0f;      //총알이 날아가는 속도
        
    void Awake()
    {
        tag = transform.parent.tag;         //이 오브젝트의 태그 변경
        rb = GetComponent<Rigidbody>();     //리지드바디 컴포넌트 가져오기
        dronBulletSoldier = FindClass.GetMonCMC(tag, MonsterName.Soldier).GetComponent<Soldier>();       //태그를 통해 이 총알이 누구의 것인지 가져오기
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.useGravity = false;          //중력 미적용상태로 변경
        rb.AddForce(transform.forward * BulletSpd, ForceMode.Impulse);  //총알에 전방으로 힘 가하기
        Destroy(gameObject, 5.0f);      //5초후에는 무조건 삭제
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))        //적군이라면
        {
            dronBulletSoldier.GiveDamage(other.transform.parent.gameObject, dronBulletSoldier.GetDronDmg(dronBulletSoldier.CMCmonStat.starForce));   //대미지 입히기
            Destroy(gameObject);    //총알 파괴
        }
    }


}
