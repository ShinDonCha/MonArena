using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

//�г��� ���� �ǳ� ��ũ��Ʈ
//NickPanel �����տ� �ٿ��� ���
public class NickPanelCtrl : MonoBehaviour, IButtonClick
{
    [Header("----------- ���� ������Ʈ ���� -----------")]
    [SerializeField]
    private InputField nickIFd = null;                             //�г��� InputField
    public string GetReqNick { get { return nickIFd.text; } }      //UserPanelCtrl�� NetworkMgr���� �г��� ���� �� �г��� InputField�� �ؽ�Ʈ�� �޾ư��� ���� ������Ƽ

    [SerializeField]
    private Text errorText = null;                //�������� ǥ�ÿ� �ؽ�Ʈ
    private readonly float errorTime = 2.0f;      //�������� ǥ�� �ð�

    // Start is called before the first frame update
    void Start()
    {
        errorText.enabled = false;      //���� �� �����ؽ�Ʈ �������·� ����
        nickIFd.Select();               //���� �� Ŀ���� �г���InputField�� ����
    }

    public void ButtonOnClick(Button PushBtn)       //Hierarchy�� ��ư ������Ʈ���� OnClick()�� ���� ����
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.OKButton:           //�����ư Ŭ�� �� �г��� ���� ����
                if (nickIFd.text.Length <= 0)   //�г����� �Է����� �ʾ�����
                    StartCoroutine(ErrorOnOff("�г����� �ּ� 1���� �̻��̾�� �մϴ�."));
                else if (nickIFd.text.Length > 6)   //�г����� 6���ڸ� �ʰ�������
                    StartCoroutine(ErrorOnOff("�г����� �ִ� 6���ڸ� �����մϴ�."));
                else  //�̻� ������
                    GetComponentInParent<UserPanelCtrl>().ReqNickCheck();   //�г��� Ȯ�� ��û �Լ� ȣ��
                break;

            case ButtonList.CancelButton:
                Destroy(gameObject);   //��ҹ�ư Ŭ�� �� ���ӿ�����Ʈ �ı�
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    public IEnumerator ErrorOnOff(string Str)     //���� ���� �¿��� �ڷ�ƾ (�� ��ũ��Ʈ�� NetworkMgr�� NetworkMgr�� NickChangeCo �ڷ�ƾ���� ���)
    {
        errorText.text = Str;
        errorText.enabled = true;
        yield return new WaitForSecondsRealtime(errorTime);
        errorText.enabled = false;
    }
}
