using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ ǥ�õǴ� ���� �Ӹ����� HP�ٸ� ��Ʈ�� �ϱ����� ��ũ��Ʈ
//���� ������Ʈ ������ StateCanvas�� �ٿ��� ���
public class StCanvasCtrl : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);   //�ʱ� Off����
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.position;  //�׻� ī�޶� ������ ���̵��� �ϱ�
    }

    public void EffectSet(GameObject Prefab, float SusTime)  //����Ʈ ���� �ڷ�ƾ (����Ʈ ������, ���ӽð�)
    {
        GameObject a_GO = Instantiate(Prefab, transform);    //����Ʈ ����
        Destroy(a_GO, SusTime);     //���ӽð� �� ����
    }
}
