using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//정적변수, 델리게이트, 구조체 등을 모아놓은 곳
public struct MonsterStat        //몬스터 정보 구조체(기본 틀)
{
    public MonsterName monName;            //몬스터 이름
    public AttackType attackType;          //공격타입(물리 or 마법)
    public int hp;                         //몬스터 체력
    public int attackDmg;                  //공격력
    public int defPower;                   //방어력
    public int mdefPower;                  //마법방어력
    public float attackSpd;                //공격속도
    public float moveSpd;                  //이동속도
    public float attackRange;              //공격사거리
    public int starForce;                  //성급
}

public class GameManager      //전투시작, 일시정지, 종료 시 여러함수를 동시에 실행시키기 위한 클래스
{
    public delegate void GMDele();
    public static event GMDele StartEvent;

    public static void GMEventAct()     //이벤트 실행 함수(현재는 StartEvent만 실행 중)
    {
        StartEvent();
    }
}

public class ResultData     //전투결과제공에 필요한 정보를 받아오기 위한 정적변수를 포함하고있는 클래스
{
    public static Result CombatResult = Result.Victory;             //전투 결과를 받아오기 위한 변수(기본값 승리)
    public static SceneList PrevScene = SceneList.InGameScene;      //전투결과를 요청한 Scene을 받아오기 위한 변수(기본값 InGameScene)   
}

public class FindClass        //외부에서 다른 스크립트에 있는 함수 또는 스크립트 자체에 접근하기 위한 클래스
{
    public static Func<string, Transform> AreaTrFunc;       //이펙트 오브젝트 등을 생성할 Area를 찾기위한 Function(생성자의 팀태그와 같은 Tag를 지닌 Area)

    public static Func<string, List<CmnMonCtrl>> GetCMCListFunc;     //각 Mgr스크립트의 CmnMonCtrl 리스트를 가져오기 위한 Function
    public static CmnMonCtrl GetMonCMC(string Tag, MonsterName MN)   //Tag를 통해 얻은 전체 CmnMonCtrl리스트에서 매개변수로 들어온 몬스터에 맞는 CmnMonCtrl을 반환하는 함수
    {
        return GetCMCListFunc(Tag).Find(a => a.CMCmonStat.monName.Equals(MN));
    }

    public static SetPointCtrl CurSetPoint;   //현재 씬의 SetPointCtrl을 사용하기 위한 정적 변수
    public static ContentCtrl CurContent;     //현재 씬의 ContentCtrl을 사용하기 위한 정적 변수

    public static SceneList LoadSceneName;    //LoadingPanel에서 불러와야할 씬을 저장하기위한 변수

    public static int MISelNum;     //MonInformScene에서 정보를 보려는 몬스터의 번호 (MyRoomScene과 MonInformScene에서 사용)
    public static int SISelNum;     //MonInformScene에서 설명을 보려는 스킬의 번호 (MonInformScene의 SkillGroup에서 사용)

    public static string RankNick;          //RankGameScene에서 선택한 랭커의 닉네임을 찾기위한 정적 변수
    public static MonsterName[] RankDName;  //RankGameScene에서 선택한 랭커의 몬스터 목록을 찾기위한 정적 변수
    public static int[] RankDStar;          //RankGameScene에서 선택한 랭커의 몬스터 성급 목록을 찾기위한 정적 변수
}

public class TextRequest   //버프, 대미지, 힐 등의 텍스트 출력을 담당하는 클래스
{
    public static Action<string, int, Vector3, TxtAnimList, TakeAct> BuffTxtReqAct;     //버프, 디버프의 텍스트 출력을 실행시키기 위한 액션(TextAreaCtrl에서 등록하고 다른곳에서 이 액션을 사용해 실행)
    public static Action<Vector3, TxtAnimList, int> InstantTxtReqAct;        //대미지나 힐 등 한번 출력 후 사라지는 텍스트 출력을 실행시키기 위한 액션(TextAreaCtrl에서 등록하고 다른곳에서 이 액션을 사용해 실행)
}

public class PlayerInfo       //유저의 정보를 담당하는 클래스
{
    public static int UniqueID = 0;             //유저의 고유 번호
    public static int UserCrtNum = 0;           //유저의 캐릭터 이미지 번호
    public static int UserLevel = 0;            //유저의 레벨
    public static string UserNick = "";         //유저의 닉네임
    public static int UserGold = 0;             //유저가 보유하고 있는 골드
    public static DateTime AutoExpTime;         //자동 보상 시간
    public static int CombatStage = 1;          //유저의 현재 스테이지

    public static List<MonsterStat> MonList = new();       //유저가 보유한 전체 몬스터
    public static List<MonsterName> CombatDeck = new();    //유저가 설정한 전투 덱
    public static List<int> CombatStarF = new();           //유저가 설정한 전투 덱의 몬스터 성급
    public static List<MonsterName> DefDeck = new();       //유저가 설정한 방어 덱
    public static List<int> DefStarF = new();              //유저가 설정한 방어 덱의 몬스터 성급
    public static List<MonsterName> RankDeck = new();      //유저가 설정한 랭크 덱
    public static List<int> RankStarF = new();             //유저가 설정한 랭크 덱의 몬스터 성급
}

public class MonsterData        //모든 몬스터들의 데이터를 보관하는 클래스
{
    public static Dictionary<MonsterName, List<MonsterStat>> MonDic = new();    //모든 몬스터들의 데이터가 들어있는 딕셔너리
}
