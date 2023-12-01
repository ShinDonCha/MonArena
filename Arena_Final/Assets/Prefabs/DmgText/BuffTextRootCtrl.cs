using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���º�ȭ(����&�����), ����� ���� ǥ������ �ؽ�Ʈ�� ��ġ���� �� �ִϸ��̼� ������ ���� ��ũ��Ʈ
//BuffTxtRoot �����տ� �ٿ��� ���
public class BuffTextRootCtrl : MonoBehaviour
{
    [SerializeField]
    private Text buffText = null;       //������ BuffText ����
    [SerializeField]
    private Text deBuffText = null;     //������ DeBuffText ����

    private Animation Anim = null;      //�ִϸ��̼� ������Ʈ�� ���� ����
    private readonly Queue<TakeAct> requestTA = new();    //��û���� ���º�ȭ ������ ������� ť

    private void Awake()
    {
        Anim = GetComponent<Animation>();       //�� ������Ʈ�� �ִϸ��̼� ������Ʈ ��������
    }

    public void AnimPlay(Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)  //�ؽ�Ʈ �ִϸ��̼� ��� �Լ� (������ TextArea ���ӿ�����Ʈ�� TextAreaCtrl ��ũ��Ʈ���� ����)
    {
        Vector3 a_Vt3 = new (ObjPos.x, ObjPos.y + 1.5f, ObjPos.z);          //�ؽ�Ʈ ������ World���� ��ġ 
        transform.position = Camera.main.WorldToScreenPoint(a_Vt3);         //World -> Screen���� ���� �� ����

        Anim.CrossFadeQueued(AnimList.ToString(), 0.0f);   //�������� �ִϸ��̼��� ���� �� �� �����û
        requestTA.Enqueue(Act);                            //� ���º�ȭ�� ��û�޾Ҵ��� ����
    }

    public void TextSet(TxtAnimList AnimList)       //��û���� TakeAct�� ���� ����� �ؽ�Ʈ�� �����ϴ� �Լ�(BuffTxtAnim�� DeBuffTxtAnim���� �̺�Ʈ�� ����, �������� �ִϸ��̼ǰ� ��ġ�ϴ� �Ű����� ����)
    {
        if (requestTA.Count == 0)
            return;

        string a_ActStr = "";

        switch (requestTA.Dequeue())    //���� �ؾ��� ȿ���� ���� �ؽ�Ʈ ����
        {
            case TakeAct.Dmg:
                a_ActStr = "���ݷ�";
                break;

            case TakeAct.ASpd:
                a_ActStr = "���ݼӵ�";
                break;

            case TakeAct.Defence:
                a_ActStr = "����";
                break;

            case TakeAct.MDefence:
                a_ActStr = "�������׷�";
                break;
        }

        switch(AnimList)
        {
            case TxtAnimList.BuffTxtAnim:       //���� �ִϸ��̼��� ��� ���̶��
                buffText.text = a_ActStr + " ����";
                break;

            case TxtAnimList.DeBuffTxtAnim:     //����� �ִϸ��̼��� ��� ���̶��
                deBuffText.text = a_ActStr + " ����";
                break;
        }
    }
}
