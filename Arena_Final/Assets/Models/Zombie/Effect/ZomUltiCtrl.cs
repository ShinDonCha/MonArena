using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie�� �ñر� ��Ʈ�� ��ũ��Ʈ
//ZomUltimate�� �ٿ��� ���
public class ZomUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject damageDummy = null;   //������ ������ ������� �ִ� ���� (������ ZomUltiDummy)

    private void Awake()
    {
        tag = transform.parent.tag;         //�±� ����
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DamageCo());     //����� �ڷ�ƾ ����
    }

    private IEnumerator DamageCo()      //0.5�ʸ��� ������� �ֱ����� �ڷ�ƾ
    {
        damageDummy.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageDummy.SetActive(false);
        yield return new WaitForSeconds(0.4f);        
        StartCoroutine(DamageCo());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();        //�ı��ɶ� ��� �ڷ�ƾ ����
    }
}
