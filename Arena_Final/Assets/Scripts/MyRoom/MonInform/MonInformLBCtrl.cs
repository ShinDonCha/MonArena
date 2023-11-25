using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//MonInformScene에서 카메라를 회전시키면서 몬스터를 보기위한 스크립트, 왼쪽보드에서 담당
//MonInformScene의 LeftBoard 오브젝트에 붙여서 사용
public class MonInformLBCtrl : MonoBehaviour, IDragHandler
{
    private Vector3 cameraPos = Vector3.zero;       //맨처음 메인카메라의 위치를 저장하기 위한 변수
    private Vector3 cameraRot = Vector3.zero;       //마우스 움직임에 따라 적용시켜줄 카메라의 회전값을 넣는 변수
    private readonly float cameraSpeed = 5.0f;      //카메라 회전 속도

    private Vector3 targetVec = new(0.0f, 0.8f, 0.0f);   //카메라가 바라볼 Vector3

    // Start is called before the first frame update
    void Start()
    {
        cameraPos = Camera.main.transform.position;         //처음 카메라 위치 저장
        cameraRot = transform.localRotation.eulerAngles;    //처음 카메라 로컬회전값 저장
    }

    public void OnDrag(PointerEventData eventData)      //드래그 중일 때
    {
        Vector3 a_Vec = cameraPos - targetVec;      //바라볼 점과 카메라 사이의 벡터 계산
        cameraRot.y += Input.GetAxis("Mouse X") * cameraSpeed;  //마우스 좌우 이동값에 정해놓은 속도를 곱해 카메라의 회전각을 더해준다.
        Camera.main.transform.position = Quaternion.Euler(cameraRot) * a_Vec + targetVec;     //위에서 구한 벡터를 cameraRot의 회전축으로 회전시켜 카메라가 대상주위를 회전하게 하기
        Camera.main.transform.LookAt(targetVec);         //카메라가 대상을 바라보게 하기
    }
}
