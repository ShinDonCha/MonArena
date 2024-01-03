using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//��������, ��������Ʈ, ����ü ���� ��Ƴ��� ��
public struct MonsterStat        //���� ���� ����ü(�⺻ Ʋ)
{
    public MonsterName monName;            //���� �̸�
    public AttackType attackType;          //����Ÿ��(���� or ����)
    public int hp;                         //���� ü��
    public int attackDmg;                  //���ݷ�
    public int defPower;                   //����
    public int mdefPower;                  //��������
    public float attackSpd;                //���ݼӵ�
    public float moveSpd;                  //�̵��ӵ�
    public float attackRange;              //���ݻ�Ÿ�
    public int starForce;                  //����
}

public class GameManager      //��������, �Ͻ�����, ���� �� �����Լ��� ���ÿ� �����Ű�� ���� Ŭ����
{
    public delegate void GMDele();
    public static event GMDele StartEvent;

    public static void GMEventAct()     //�̺�Ʈ ���� �Լ�(����� StartEvent�� ���� ��)
    {
        StartEvent();
    }
}

public class ResultData     //������������� �ʿ��� ������ �޾ƿ��� ���� ���������� �����ϰ��ִ� Ŭ����
{
    public static Result CombatResult = Result.Victory;             //���� ����� �޾ƿ��� ���� ����(�⺻�� �¸�)
    public static SceneList PrevScene = SceneList.InGameScene;      //��������� ��û�� Scene�� �޾ƿ��� ���� ����(�⺻�� InGameScene)   
}

public class FindClass        //�ܺο��� �ٸ� ��ũ��Ʈ�� �ִ� �Լ� �Ǵ� ��ũ��Ʈ ��ü�� �����ϱ� ���� Ŭ����
{
    public static Func<string, Transform> AreaTrFunc;       //����Ʈ ������Ʈ ���� ������ Area�� ã������ Function(�������� ���±׿� ���� Tag�� ���� Area)

    public static Func<string, List<CmnMonCtrl>> GetCMCListFunc;     //�� Mgr��ũ��Ʈ�� CmnMonCtrl ����Ʈ�� �������� ���� Function
    public static CmnMonCtrl GetMonCMC(string Tag, MonsterName MN)   //Tag�� ���� ���� ��ü CmnMonCtrl����Ʈ���� �Ű������� ���� ���Ϳ� �´� CmnMonCtrl�� ��ȯ�ϴ� �Լ�
    {
        return GetCMCListFunc(Tag).Find(a => a.CMCmonStat.monName.Equals(MN));
    }

    public static SetPointCtrl CurSetPoint;   //���� ���� SetPointCtrl�� ����ϱ� ���� ���� ����
    public static ContentCtrl CurContent;     //���� ���� ContentCtrl�� ����ϱ� ���� ���� ����

    public static SceneList LoadSceneName;    //LoadingPanel���� �ҷ��;��� ���� �����ϱ����� ����

    public static int MISelNum;     //MonInformScene���� ������ ������ ������ ��ȣ (MyRoomScene�� MonInformScene���� ���)
    public static int SISelNum;     //MonInformScene���� ������ ������ ��ų�� ��ȣ (MonInformScene�� SkillGroup���� ���)

    public static string RankNick;          //RankGameScene���� ������ ��Ŀ�� �г����� ã������ ���� ����
    public static MonsterName[] RankDName;  //RankGameScene���� ������ ��Ŀ�� ���� ����� ã������ ���� ����
    public static int[] RankDStar;          //RankGameScene���� ������ ��Ŀ�� ���� ���� ����� ã������ ���� ����
}

public class TextRequest   //����, �����, �� ���� �ؽ�Ʈ ����� ����ϴ� Ŭ����
{
    public static Action<string, int, Vector3, TxtAnimList, TakeAct> BuffTxtReqAct;     //����, ������� �ؽ�Ʈ ����� �����Ű�� ���� �׼�(TextAreaCtrl���� ����ϰ� �ٸ������� �� �׼��� ����� ����)
    public static Action<Vector3, TxtAnimList, int> InstantTxtReqAct;        //������� �� �� �ѹ� ��� �� ������� �ؽ�Ʈ ����� �����Ű�� ���� �׼�(TextAreaCtrl���� ����ϰ� �ٸ������� �� �׼��� ����� ����)
}

public class PlayerInfo       //������ ������ ����ϴ� Ŭ����
{
    public static int UniqueID = 0;             //������ ���� ��ȣ
    public static int UserCrtNum = 0;           //������ ĳ���� �̹��� ��ȣ
    public static int UserLevel = 0;            //������ ����
    public static string UserNick = "";         //������ �г���
    public static int UserGold = 0;             //������ �����ϰ� �ִ� ���
    public static DateTime AutoExpTime;         //�ڵ� ���� �ð�
    public static int CombatStage = 1;          //������ ���� ��������

    public static List<MonsterStat> MonList = new();       //������ ������ ��ü ����
    public static List<MonsterName> CombatDeck = new();    //������ ������ ���� ��
    public static List<int> CombatStarF = new();           //������ ������ ���� ���� ���� ����
    public static List<MonsterName> DefDeck = new();       //������ ������ ��� ��
    public static List<int> DefStarF = new();              //������ ������ ��� ���� ���� ����
    public static List<MonsterName> RankDeck = new();      //������ ������ ��ũ ��
    public static List<int> RankStarF = new();             //������ ������ ��ũ ���� ���� ����
}

public class MonsterData        //��� ���͵��� �����͸� �����ϴ� Ŭ����
{
    public static Dictionary<MonsterName, List<MonsterStat>> MonDic = new();    //��� ���͵��� �����Ͱ� ����ִ� ��ųʸ�
}
