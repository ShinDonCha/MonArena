using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//����ü, ��ų����Ʈ ���� ������Ʈ���� ������ Transform�� �����ϱ� ���� ��ũ��Ʈ
//�� �������� �±׿� �´� AreaTr�� ������ ������Ʈ�� �����ǰ�, ������ ������Ʈ���� Area�� �±׸� ���� �θ��� �±׸� ������ �� �ִ�.
//������ �Ͼ�� ���� InstantArea ���ӿ�����Ʈ�� �ٿ��� ���
public class InstantAreaCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform playerAreaTr = null;   //Ally�� �±׸� ���� Area
    [SerializeField]
    private Transform enemyAreaTr = null;    //Enemy�� �±׸� ���� Area
    [SerializeField]
    private Transform team1AreaTr = null;    //Team1�� �±׸� ���� Area
    [SerializeField]
    private Transform team2AreaTr = null;    //Team2�� �±׸� ���� Area

    // Start is called before the first frame update
    void Start()
    {
        FindClass.AreaTrFunc = GetAreaTr;       //�ܺο��� Area�� Transform�� ã�� �� �ֵ��� �������� ������ Func�� ���
    }

    private Transform GetAreaTr(string TeamTag)     //�±׿� �´� Area�� ã�� �Լ�
    {
        if (!Enum.TryParse(TeamTag, out Team Tm))
            return null;

        switch(Tm)
        {
            case Team.Ally:
                return playerAreaTr;

            case Team.Enemy:
                return enemyAreaTr;

            case Team.Team1:
                return team1AreaTr;

            case Team.Team2:
                return team2AreaTr;

            default:
                return null;
        }
    }
}
