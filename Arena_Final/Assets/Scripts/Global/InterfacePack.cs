using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//인터페이스들이 모여있는 곳
public interface IButtonClick      //버튼 클릭 시 함수실행을 위한 인터페이스
{
    public void ButtonOnClick(Button PushBtn);    
}

public interface ICMCList           //Mgr에서 팀에 맞는 CMCList를 전달하는 함수를 생성하기 위한 인터페이스
{
    public List<CmnMonCtrl> CMCListFuncSet(string TagStr);
}

public interface ILoadScene         //씬 불러오기 동작을 위한 인터페이스
{
    public void LoadScene(SceneList NextScene);
}
