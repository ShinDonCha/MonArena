using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//MonInformScene���� ī�޶� ȸ����Ű�鼭 ���͸� �������� ��ũ��Ʈ, ���ʺ��忡�� ���
//MonInformScene�� LeftBoard ������Ʈ�� �ٿ��� ���
public class MonInformLBCtrl : MonoBehaviour, IDragHandler
{
    private Vector3 cameraPos = Vector3.zero;       //��ó�� ����ī�޶��� ��ġ�� �����ϱ� ���� ����
    private Vector3 cameraRot = Vector3.zero;       //���콺 �����ӿ� ���� ��������� ī�޶��� ȸ������ �ִ� ����
    private readonly float cameraSpeed = 5.0f;      //ī�޶� ȸ�� �ӵ�

    private Vector3 targetVec = new(0.0f, 0.8f, 0.0f);   //ī�޶� �ٶ� Vector3

    // Start is called before the first frame update
    void Start()
    {
        cameraPos = Camera.main.transform.position;         //ó�� ī�޶� ��ġ ����
        cameraRot = transform.localRotation.eulerAngles;    //ó�� ī�޶� ����ȸ���� ����
    }

    public void OnDrag(PointerEventData eventData)      //�巡�� ���� ��
    {
        Vector3 a_Vec = cameraPos - targetVec;      //�ٶ� ���� ī�޶� ������ ���� ���
        cameraRot.y += Input.GetAxis("Mouse X") * cameraSpeed;  //���콺 �¿� �̵����� ���س��� �ӵ��� ���� ī�޶��� ȸ������ �����ش�.
        Camera.main.transform.position = Quaternion.Euler(cameraRot) * a_Vec + targetVec;     //������ ���� ���͸� cameraRot�� ȸ�������� ȸ������ ī�޶� ��������� ȸ���ϰ� �ϱ�
        Camera.main.transform.LookAt(targetVec);         //ī�޶� ����� �ٶ󺸰� �ϱ�
    }
}
