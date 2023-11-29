using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

//ResultScene���� ��������� ǥ���ϴ� Board�� ��Ʈ�� ��ũ��Ʈ(ResultScene�� MultiResultScene���� ���)
//ResultScene������ RightBoard��, MultiResultScene������ LeftBoard, RightBoard�� �ٿ��� ���
public class ResultBoardCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject monPartPrefab = null; //������ ������ ���� �� ������

    //--------------- ���͵��� ��������� ����ִ� ����(Start���� �����Ǵ� MonPart���� ���� ������)
    public int[] CmnListNameNum { get; private set; }   //��������� ǥ�����ֱ����� ���������� ������ ���� ����(�̸�)
    public int[] CmnListDmg { get; private set; }       //��������� ǥ�����ֱ����� ���������� ������ ���� ����(���� �����)
    public int[] CmnListHP { get; private set; }        //��������� ǥ�����ֱ����� ���������� ������ ���� ����(���� �����)

    public int TotalMonDmg { get; private set; }   //�� �迭�� ���͵��� ���� �� �����
    public int TotalMonHP { get; private set; }    //�� �迭�� ���͵��� ���� �� ���ط�
    //--------------- ���͵��� ��������� ����ִ� ����(Start���� �����Ǵ� MonPart���� ���� ������)

    private void Awake()
    {
        if (!Enum.TryParse(tag, out Team TeamTag))      //ResultScene(�̱�)������ Ally, MultiResultScene(��Ƽ)������ Team1, Team2
            return;

        switch (TeamTag)
        {
            //------------- �̱�(InGameScene, RankGameScene ����)
            case Team.Ally:
                List<CmnMonCtrl> a_CMCList = FindClass.GetCMCListFunc(tag); //�� ���ӿ�����Ʈ�� �±׿� �´�CmnMonCtrl ����Ʈ�� ��������

                CmnListNameNum = new int[a_CMCList.Count];       //������ ����ŭ �迭 ����
                CmnListDmg = new int[a_CMCList.Count];           //������ ����ŭ �迭 ����
                CmnListHP = new int[a_CMCList.Count];            //������ ����ŭ �迭 ����

                for (int i = 0; i < CmnListNameNum.Length; i++)   //������ ������ �����ͼ� ����
                {
                    CmnListNameNum[i] = (int)a_CMCList[i].CMCmonStat.monName;
                    CmnListDmg[i] = a_CMCList[i].TotalDmg;
                    CmnListHP[i] = a_CMCList[i].TotalHP;
                }
                break;
            //------------- �̱�(InGameScene, RankGameScene ����)

            //------------ ��Ƽ�� ��� CustomProperties�� ����� ������ ��������
            case Team.Team1:    //LeftBoard�� ���
                CmnListNameNum = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnNameNum"];
                CmnListDmg = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnDmg"];
                CmnListHP = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team1CmnHP"];
                break;

            case Team.Team2:   //RightBoard�� ���
                CmnListNameNum = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnNameNum"];
                CmnListDmg = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnDmg"];
                CmnListHP = (int[])PhotonNetwork.CurrentRoom.CustomProperties["Team2CmnHP"];
                break;
           //------------ ��Ƽ�� ��� CustomProperties�� ����� ������ ��������
        }
    }    

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < CmnListDmg.Length; i++)
        {
            TotalMonDmg += CmnListDmg[i];   //���Ͱ� ���� �� ���ط�
            TotalMonHP += CmnListHP[i];     //���Ͱ� ���� �� ���ط�
            Instantiate(monPartPrefab, transform);      //���� ������(MonPart) ����
        }
    }
}
