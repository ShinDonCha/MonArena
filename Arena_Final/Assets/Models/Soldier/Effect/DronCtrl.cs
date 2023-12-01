using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier의 2번째 스킬 시 생성된 드론을 컨트롤 하기위한 스크립트
//drone프리팹에 붙여서 사용
[RequireComponent(typeof(SphereCollider))]
public class DronCtrl : MonoBehaviour
{
    private enum DronState  //드론의 현재 상태를 표시할 enum
    {
        Flying,
        Moving,
        Attack
    }
        
    private Soldier dronSoldier = null;           //생성자의 몬스터 컨트롤 스크립트
    private DronState dronState = DronState.Flying; //현재 드론의 상태
    private SphereCollider sCollider;              //목표를 찾기위한 콜라이더
    private Vector3 destination;                  //드론이 상승할 목적지 좌표
    private Vector3 targetVec;                    //공격할 목표의 위치
    private readonly float moveSpd = 6.0f;        //드론의 이동속도
    private float dronASpd = 0.0f;                //드론의 공격속도
    private bool attackEnable = true;             //공격 가능 여부

    [SerializeField]
    private Transform firePos = null;              //총알이 나갈 위치
    [SerializeField]
    private GameObject bulletObj = null;           //총알 오브젝트

    void Awake()
    {
        tag = transform.parent.tag;     //이 오브젝트의 태그 변경
        sCollider = GetComponent<SphereCollider>(); //이 드론의 대상 추적용 SphereCollider
        dronSoldier = FindClass.GetMonCMC(tag, MonsterName.Soldier).GetComponent<Soldier>();     //생성자의 몬스터 컨트롤 스크립트 가져오기        
    }

    // Start is called before the first frame update
    void Start()
    {
        sCollider.radius = 10.0f;   //콜라이더의 크기 설정
        sCollider.enabled = false;  //콜라이더 오프
        destination = new Vector3(transform.position.x, 1.0f, transform.position.z);    //드론의 상승 목적지 설정
        dronASpd = dronSoldier.DronASpd;                //드론의 공격속도 가져오기
        Destroy(gameObject, dronSoldier.DronLifeT);     //지속시간 후 드론 삭제
    }

    // Update is called once per frame
    void Update()
    {
        switch(dronState)   //드론의 상태 체크
        {
            case DronState.Flying:  //드론이 상승상태
                transform.position = Vector3.MoveTowards(transform.position, destination, 2.0f * Time.deltaTime);   //상승 목적지를 향해 이동

                if (transform.position == destination)  //드론이 목적지에 도착했을 경우
                {
                    dronState = DronState.Moving;   //드론 이동상태로 변경
                    sCollider.enabled = true;       //추적 콜라이더 온
                }
                break;

            case DronState.Moving:   //드론 이동상태
                transform.Translate(moveSpd * Time.deltaTime * transform.forward, Space.World);       //타겟을 못찾았으면 앞으로 계속 이동
                break;

            case DronState.Attack:  //드론 공격상태
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetVec), 540.0f * Time.deltaTime); //공격 대상을 바라보도록 회전

                if (attackEnable)  //공격 가능 상태일 경우
                    StartCoroutine(BulletGenerate());   //총알 발사
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))     //아군이 아니면
        {
            sCollider.enabled = false;      //추적 콜라이더 오프
            dronState = DronState.Attack;   //드론을 공격상태로 변경
            targetVec = (other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.position - transform.position);   //드론의 현재 위치에서 공격 대상을 보는 벡터 설정
        }
    }

    private IEnumerator BulletGenerate()    //총알 발사 코루틴
    {
        attackEnable = false;   //공격 불가능 상태로 변경
        Instantiate(bulletObj, firePos.position, Quaternion.LookRotation(targetVec), transform.parent);  //총알 생성
        yield return new WaitForSeconds(dronASpd);  //드론의 공격속도만큼 제어권 돌려주기
        attackEnable = true;    //공격 가능 상태로 변경
    }
}
