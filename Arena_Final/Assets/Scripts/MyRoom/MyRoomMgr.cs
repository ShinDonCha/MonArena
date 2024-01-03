using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//MyRoomScene 버튼들의 동작을 실행하기 위한 스크립트
//MyRoomScene의 MyRoomMgr 오브젝트에 붙여서 실행
public class MyRoomMgr : MonoBehaviour, IButtonClick
{
    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.BackButton:
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());
                break;

            case ButtonList.EvolveButton:
                SceneManager.LoadScene(SceneList.EvolveScene.ToString());
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }
}
