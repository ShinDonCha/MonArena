using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Zombie�� �ñر� ��Ʈ�� ��ũ��Ʈ
//ZomUltimate�� �ٿ��� ���
public class ZomUltiCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject damageDummy = null;   //������ ������ ������� �ִ� ���� (������ ZomUltiDummy)

    private readonly WaitForSeconds onSeconds = new(0.1f);       //������ ������� ���� �� �ִ� �ð�
    private readonly WaitForSeconds offSeconds = new(0.4f);      //������ ������� ���� �� ���� �ð�

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
        while (true)
        {
            damageDummy.SetActive(true);
            yield return onSeconds;
            damageDummy.SetActive(false);
            yield return offSeconds;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();        //�ı��ɶ� ��� �ڷ�ƾ ����
    }
}
