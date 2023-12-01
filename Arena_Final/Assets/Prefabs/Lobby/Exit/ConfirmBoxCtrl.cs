using UnityEngine;
using UnityEngine.UI;
using System;

//종료 확인용 판넬 스크립트, 로비에서 게임종료 누르면 판넬생성
//ExitConfirmPrefab에 붙여서 사용
public class ConfirmBoxCtrl : NetworkMgr, IButtonClick
{
    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        switch (BtnL)
        {
            case ButtonList.OKButton:     //확인 버튼 누르면 중요한정보 다시 한번 저장요청 후 게임 종료 요청
                PushPacket(PacketType.UserLv);
                PushPacket(PacketType.MonList);
                PushPacket(PacketType.UserGold);
                PushPacket(PacketType.AutoTime);
                PushPacket(PacketType.CombatStage);
                reqExit = true;     //게임종료 요청
                break;

            case ButtonList.CancelButton:
                Destroy(gameObject);   //이 오브젝트 삭제
                break;
        }
    }
}
