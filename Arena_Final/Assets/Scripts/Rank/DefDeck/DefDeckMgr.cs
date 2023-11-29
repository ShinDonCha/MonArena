using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//��������� �� ����, �ҷ����⸦ ���� ��ũ��Ʈ (���� ��Ƽ�÷��� �� ����)
//DefDeckScene�� DefDeckMgr�� �ٿ��� ���
public class DefDeckMgr : NetworkMgr, IButtonClick
{
    public static DefDeckMgr Instance = null;

    private void Awake()
    {
        Instance = this;
        BGMController.Instance.BGMChange(BGMList.Battle);  //������� ����
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //��ӹ��� NetworkMgr�� Update����
    }

    public void ButtonOnClick(Button PushBtn)   //Hierarchy�� ��ư ������Ʈ���� OnClick()�� ���� ����
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))       //��ư ������Ʈ�� �̸��� enum������ ����
            return;

        switch (BtnL)
        {
            //------------------------ ��ġ ���� ------------------------
            case ButtonList.BackButton:
                PlayerInfo.DefDeck.Clear();    //�� ����Ʈ �ʱ�ȭ
                PlayerInfo.DefStarF.Clear();   //�� ���� ����Ʈ �ʱ�ȭ

                for (int i = 0; i < FindClass.CurSetPoint.transform.childCount; i++)      //��� SetPoints������Ʈ ������ Point ����ŭ ����
                {
                    if (FindClass.CurSetPoint[i].childCount > 1)     //���Ͱ� ��ġ�� Point�� MonSlotCtrl�̸�.
                    {
                        PlayerInfo.DefDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.monName);       //�ش� ������ �̸��� ����
                        PlayerInfo.DefStarF.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.starForce);    //�ش� ������ ������ ����
                    }
                    else                                    //���Ͱ� ��ġ���� ���� Point�� MonSlotCtrl�̸�..
                    {
                        PlayerInfo.DefDeck.Add(MonsterName.None);       //��������� ����
                        PlayerInfo.DefStarF.Add(-1);          //��������� ����
                    }
                }
                PushPacket(PacketType.DefDeck);   //NetworkMgr�� ���� ��ġ�� ���� ��û                
                break;
            //------------------------ ��ġ ���� -----------------------
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }
}
