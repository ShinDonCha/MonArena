using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GamblerCat�� �ñر� ��ũ��Ʈ
//CatUltimate �����տ� �ٿ��� ���
[RequireComponent (typeof(SphereCollider))]
public class CatUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject buffEffect = null;      //�ñر⸦ �޴� ��󿡰� ǥ���� ����Ʈ
    private GamblerCat ultiGamblerCat = null;  //�������� ���� ��Ʈ�� ��ũ��Ʈ�� ���� ����
    private SphereCollider sCollider = null;   //�� ������Ʈ�� SphereCollider    

    private void Awake()
    {
        sCollider = GetComponent<SphereCollider>();     //SphereCollider ��������
        ultiGamblerCat = FindClass.GetMonCMC(transform.parent.tag, MonsterName.GamblerCat).GetComponent<GamblerCat>();  //�������� ��Ʈ�� ��ũ��Ʈ ��������
    }

    // Start is called before the first frame update
    void Start()
    {        
        sCollider.radius = ultiGamblerCat.CatUltiRadius;   //����� ã�� ������ ����
        sCollider.isTrigger = true;                        //isTrigger���� ��
        Destroy(gameObject, 1.0f);                         //�����ð� �� �� ������Ʈ ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals((int)LayerName.MonBody)
            && ultiGamblerCat.CompareTag(other.transform.parent.tag))  //�Ʊ��� ���
        {
            Instantiate(buffEffect, other.transform.parent);        //�ñر� ����Ʈ ����
            other.GetComponentInParent<CmnMonCtrl>().TakeAny(TakeAct.Heal, ultiGamblerCat.GetUltiAmount(ultiGamblerCat.CMCmonStat.starForce));    //����� ȸ����Ű��
        }
    }
}
