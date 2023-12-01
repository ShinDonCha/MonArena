using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Soldier�� 2��° ��ų �� ������ ����� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//drone�����տ� �ٿ��� ���
[RequireComponent(typeof(SphereCollider))]
public class DronCtrl : MonoBehaviour
{
    private enum DronState  //����� ���� ���¸� ǥ���� enum
    {
        Flying,
        Moving,
        Attack
    }
        
    private Soldier dronSoldier = null;           //�������� ���� ��Ʈ�� ��ũ��Ʈ
    private DronState dronState = DronState.Flying; //���� ����� ����
    private SphereCollider sCollider;              //��ǥ�� ã������ �ݶ��̴�
    private Vector3 destination;                  //����� ����� ������ ��ǥ
    private Vector3 targetVec;                    //������ ��ǥ�� ��ġ
    private readonly float moveSpd = 6.0f;        //����� �̵��ӵ�
    private float dronASpd = 0.0f;                //����� ���ݼӵ�
    private bool attackEnable = true;             //���� ���� ����

    [SerializeField]
    private Transform firePos = null;              //�Ѿ��� ���� ��ġ
    [SerializeField]
    private GameObject bulletObj = null;           //�Ѿ� ������Ʈ

    void Awake()
    {
        tag = transform.parent.tag;     //�� ������Ʈ�� �±� ����
        sCollider = GetComponent<SphereCollider>(); //�� ����� ��� ������ SphereCollider
        dronSoldier = FindClass.GetMonCMC(tag, MonsterName.Soldier).GetComponent<Soldier>();     //�������� ���� ��Ʈ�� ��ũ��Ʈ ��������        
    }

    // Start is called before the first frame update
    void Start()
    {
        sCollider.radius = 10.0f;   //�ݶ��̴��� ũ�� ����
        sCollider.enabled = false;  //�ݶ��̴� ����
        destination = new Vector3(transform.position.x, 1.0f, transform.position.z);    //����� ��� ������ ����
        dronASpd = dronSoldier.DronASpd;                //����� ���ݼӵ� ��������
        Destroy(gameObject, dronSoldier.DronLifeT);     //���ӽð� �� ��� ����
    }

    // Update is called once per frame
    void Update()
    {
        switch(dronState)   //����� ���� üũ
        {
            case DronState.Flying:  //����� ��»���
                transform.position = Vector3.MoveTowards(transform.position, destination, 2.0f * Time.deltaTime);   //��� �������� ���� �̵�

                if (transform.position == destination)  //����� �������� �������� ���
                {
                    dronState = DronState.Moving;   //��� �̵����·� ����
                    sCollider.enabled = true;       //���� �ݶ��̴� ��
                }
                break;

            case DronState.Moving:   //��� �̵�����
                transform.Translate(moveSpd * Time.deltaTime * transform.forward, Space.World);       //Ÿ���� ��ã������ ������ ��� �̵�
                break;

            case DronState.Attack:  //��� ���ݻ���
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetVec), 540.0f * Time.deltaTime); //���� ����� �ٶ󺸵��� ȸ��

                if (attackEnable)  //���� ���� ������ ���
                    StartCoroutine(BulletGenerate());   //�Ѿ� �߻�
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !CompareTag(other.transform.parent.tag))     //�Ʊ��� �ƴϸ�
        {
            sCollider.enabled = false;      //���� �ݶ��̴� ����
            dronState = DronState.Attack;   //����� ���ݻ��·� ����
            targetVec = (other.GetComponentInParent<CmnMonCtrl>().GetHitPoint.position - transform.position);   //����� ���� ��ġ���� ���� ����� ���� ���� ����
        }
    }

    private IEnumerator BulletGenerate()    //�Ѿ� �߻� �ڷ�ƾ
    {
        attackEnable = false;   //���� �Ұ��� ���·� ����
        Instantiate(bulletObj, firePos.position, Quaternion.LookRotation(targetVec), transform.parent);  //�Ѿ� ����
        yield return new WaitForSeconds(dronASpd);  //����� ���ݼӵ���ŭ ����� �����ֱ�
        attackEnable = true;    //���� ���� ���·� ����
    }
}
