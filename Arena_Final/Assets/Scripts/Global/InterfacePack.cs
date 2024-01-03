using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//�������̽����� ���ִ� ��
public interface IButtonClick      //��ư Ŭ�� �� �Լ������� ���� �������̽�
{
    public void ButtonOnClick(Button PushBtn);    
}

public interface ICMCList           //Mgr���� ���� �´� CMCList�� �����ϴ� �Լ��� �����ϱ� ���� �������̽�
{
    public List<CmnMonCtrl> CMCListFuncSet(string TagStr);
}

public interface ILoadScene         //�� �ҷ����� ������ ���� �������̽�
{
    public void LoadScene(SceneList NextScene);
}
