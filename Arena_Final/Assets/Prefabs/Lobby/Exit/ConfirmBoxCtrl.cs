using UnityEngine;
using UnityEngine.UI;
using System;

//���� Ȯ�ο� �ǳ� ��ũ��Ʈ, �κ񿡼� �������� ������ �ǳڻ���
//ExitConfirmPrefab�� �ٿ��� ���
public class ConfirmBoxCtrl : NetworkMgr, IButtonClick
{
    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        switch (BtnL)
        {
            case ButtonList.OKButton:     //Ȯ�� ��ư ������ �߿������� �ٽ� �ѹ� �����û �� ���� ���� ��û
                PushPacket(PacketType.UserLv);
                PushPacket(PacketType.MonList);
                PushPacket(PacketType.UserGold);
                PushPacket(PacketType.AutoTime);
                PushPacket(PacketType.CombatStage);
                reqExit = true;     //�������� ��û
                break;

            case ButtonList.CancelButton:
                Destroy(gameObject);   //�� ������Ʈ ����
                break;
        }
    }
}
