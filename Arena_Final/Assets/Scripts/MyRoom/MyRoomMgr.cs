using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//MyRoomScene ��ư���� ������ �����ϱ� ���� ��ũ��Ʈ
//MyRoomScene�� MyRoomMgr ������Ʈ�� �ٿ��� ����
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

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }
}
