using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//��Ƽ���� �ش�����(Team1, Team2)�� �´� ���͸� �����ϱ����� SetPoint
//Resources������ CombatSetP1, CombatSetP2�� �ٿ��� ���
//MtGameMgr�� CombatReady()���� ��ġ�� ���� SetPoint�� �����鼭 ����, ��ġ�� ������ ������ ���޵ȴ�.
public class MtCbtSetPoint : MonoBehaviourPunCallbacks
{
    private PhotonView photonV = null;
    private Transform[] pointTr;    //���� Point���� Transform�� ���� �迭����

    private void Awake()
    {
        photonV = GetComponent<PhotonView>();

        pointTr = new Transform[transform.childCount];      //���� Point ����ŭ �迭�� �� ����

        for(int i = 0; i < pointTr.Length; i++)             //���� Point���� Transform ����
            pointTr[i] = transform.GetChild(i);
    }

    //�����Ϳ� Ŭ���̾�Ʈ�� ���� �濡 �ֱ� ������ ������ or Ŭ���̾�Ʈ ���� �ϳ��� ������ ����Ǿ CombatSetP1�� CombatSetP2��ο��� ����ȴ�.
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //----------------- ���� ������Ʈ ���� -----------------
        if (!changedProps.ContainsKey("MonArrName"))      //MultiGameScene�� StandBySetPoint�� OnDisable�� �� �޾ƿ� ���� ��ġ ������ ��츸 ����
            return;

        //CombatSetP1�� �������� ���͸� ���, CombatSetP2�� Ŭ���̾�Ʈ�� ���͸� ���, �����Ϳ� Ŭ���̾�Ʈ ��� �� �Լ��� ����Ͽ� ���͸� ��ġ�ϹǷ� �ڽſ��� �´� ���ÿ����� �����ϵ��� �ؾ���
        if (targetPlayer.Equals(photonV.Owner) && photonV.IsMine)       //�� CombatSetP�� �´� ��������� Ȯ��
        {
            int[] a_MonArrName = (int[])changedProps["MonArrName"];     //��ġ�� ������ �̸� �迭 �޾ƿ���
            int[] a_MonArrStar = (int[])changedProps["MonArrStar"];     //��ġ�� ������ ���� �迭 �޾ƿ���

            for (int i = 0; i < a_MonArrName.Length; i++)   //��ġ�� ���� ����ŭ ����
            {
                if (a_MonArrName[i] == -1)  //���� ������ ���� Index�� �Ѿ��
                    continue;

                GameObject a_GO;

                if (photonV.Owner.Equals(PhotonNetwork.MasterClient))       //�����ְ� �������� ���(�� ������Ʈ�� CombatSetP1�� ���)
                    a_GO = PhotonNetwork.Instantiate(((MonsterName)a_MonArrName[i]).ToString(), transform.GetChild(i).position, Quaternion.Euler(new(0, 90, 0)));      //���� ������Ʈ ����
                else                                                        //�����ְ� Ŭ���̾�Ʈ�� ���(�� ������Ʈ�� CombatSetP2�� ���)
                    a_GO = PhotonNetwork.Instantiate(((MonsterName)a_MonArrName[i]).ToString(), transform.GetChild(i).position, Quaternion.Euler(new(0, -90, 0)));     //���� ������Ʈ ����

                a_GO.GetComponent<CmnMonSet>().SetMonBasics(a_MonArrName[i], a_MonArrStar[i], PhotonNetwork.IsMasterClient ? Team.Team1 : Team.Team2);  //���� �⺻���� ����, ����ġ �Լ� ����
            }
        }
        //----------------- ���� ������Ʈ ���� -----------------          
    }
}
