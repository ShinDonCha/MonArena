using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//TabŰ�� ���� ���� ������Ʈ�� ������ �� �ֵ��� �ϱ����� ��ũ��Ʈ
//TitleScene�� LoginPanel, SignupPanel�� �ٿ��� ���
public class TabKeySet : MonoBehaviour
{
    [SerializeField]
    private InputField IDInputField = null;     //IDInputField
    [SerializeField]
    private InputField PWInputField = null;     //PWInputField

    private void OnEnable()
    {
        IDInputField.text = "";     //�Է�â �ʱ�ȭ
        PWInputField.text = "";     //�Է�â �ʱ�ȭ

        if (IDInputField != null)
            IDInputField.Select();      //IDInputField ���� ���·� ����
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))     //TabŰ Ŭ���� �׺���̼��� ���� ���� ��ġ�� Ŀ�� �̵�
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
    }
}
