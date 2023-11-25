using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie�� �ι�° ��ų�� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//ZomSkill2 �����տ� �ٿ��� ���
[RequireComponent(typeof(SphereCollider))]
public class ZomSk2Ctrl : MonoBehaviour
{
    [SerializeField]
    private GameObject provokeEffect = null;       //���߿� ���� ��뿡�� ǥ������ ����Ʈ
    private Zombie provokeZombie = null;           //������(Zombie)�� ��Ʈ�� ��ũ��Ʈ�� ������ ����
    private SphereCollider sCollider = null;       //���ߴ�� ������ SphereCollider
    private readonly float traceTime = 1.0f;       //���� ����� ã�� �ð�
        
    private void Awake()
    {
        provokeZombie = GetComponentInParent<Zombie>(); //�������� ��Ʈ�� ��ũ��Ʈ ��������
        sCollider = GetComponent<SphereCollider>();     //�� ������Ʈ�� SphereCollider ��������
    }

    // Update is called once per frame
    void Update()
    {
        if (sCollider.radius < provokeZombie.ZomSk2Radius)      //���� ��� ������ �ݶ��̴��� ũ�Ⱑ ������ ���� �������� �۴ٸ�
            sCollider.radius += (provokeZombie.ZomSk2Radius / traceTime) * Time.deltaTime;  //traceTime ���� �ִ� ZomSk2Radius ��ŭ Ȯ���Ű�鼭 ����� ã��
        else
            sCollider.enabled = false;                  //���� �ð� �� �� ���� �ϴ� SphereCollider ���ֱ�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && !provokeZombie.CompareTag(other.transform.parent.tag))        //������ ���
        {
            CmnMonCtrl a_CMC = other.GetComponentInParent<CmnMonCtrl>();    //������ ���� ��Ʈ�� ��ũ��Ʈ ��������
            a_CMC.EnemySet(provokeZombie.gameObject);                       //������ ���� ��ǥ�� Zombie�� ����           
            a_CMC.TakeAny(TakeAct.Dmg, -provokeZombie.GetReduceRatio(provokeZombie.CMCmonStat.starForce), provokeZombie.ZomSk2SusTime);   //����� ���� ����� ����
            a_CMC.GetComponentInChildren<StCanvasCtrl>().EffectSet(provokeEffect, provokeZombie.ZomSk2SusTime);  //������ ���� ����Ʈ ����
        }
    }
}
