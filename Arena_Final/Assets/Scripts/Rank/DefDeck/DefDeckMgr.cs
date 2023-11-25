using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//방어편성에서 방어덱 저장, 불러오기를 위한 스크립트 (방어덱은 멀티플레이 시 사용됨)
//DefDeckScene의 DefDeckMgr에 붙여서 사용
public class DefDeckMgr : NetworkMgr, IButtonClick
{
    public static DefDeckMgr Instance = null;

    private void Awake()
    {
        Instance = this;
        BGMController.Instance.BGMChange(BGMList.Battle);  //배경음악 변경
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //상속받은 NetworkMgr의 Update실행
    }

    public void ButtonOnClick(Button PushBtn)   //Hierarchy의 버튼 오브젝트들의 OnClick()을 통해 실행
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))       //버튼 오브젝트의 이름을 enum형으로 변경
            return;

        switch (BtnL)
        {
            //------------------------ 배치 저장 ------------------------
            case ButtonList.BackButton:
                PlayerInfo.DefDeck.Clear();    //방어덱 리스트 초기화
                PlayerInfo.DefStarF.Clear();   //방어덱 성급 리스트 초기화

                for (int i = 0; i < FindClass.CurSetPoint.transform.childCount; i++)      //모든 SetPoints오브젝트 하위의 Point 수만큼 실행
                {
                    if (FindClass.CurSetPoint[i].childCount > 1)     //몬스터가 배치된 Point의 MonSlotCtrl이면.
                    {
                        PlayerInfo.DefDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.monName);       //해당 몬스터의 이름을 저장
                        PlayerInfo.DefStarF.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.starForce);    //해당 몬스터의 성급을 저장
                    }
                    else                                    //몬스터가 배치되지 않은 Point의 MonSlotCtrl이면..
                    {
                        PlayerInfo.DefDeck.Add(MonsterName.None);       //비어있음을 저장
                        PlayerInfo.DefStarF.Add(-1);          //비어있음을 저장
                    }
                }
                PushPacket(PacketType.DefDeck);   //NetworkMgr에 몬스터 배치도 저장 요청                
                break;
            //------------------------ 배치 저장 -----------------------
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }
}
