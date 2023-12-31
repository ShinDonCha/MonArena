using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//스킬 아이콘 클릭 시 선택된 스킬 종류와 성급에 따른 설명을 보여주기 위한 스크립트
//SkillInform 프리팹에 붙여서 사용
public class SkillInformCtrl : MonoBehaviour, IPointerClickHandler
{
    [Header("----------- 게임 오브젝트 연결 -----------")]
    [SerializeField]
    private Text nameText = null;       //스킬 이름을 보여줄 텍스트
    [SerializeField]
    private Text explainText = null;    //스킬 설명을 보여줄 텍스트
    [SerializeField]
    private Transform starTab = null;   //별 버튼들이 들어있는 별 탭

    private MonsterStat monStat;       //현재 몬스터의 정보를 받아오기 위한 변수
    private ExplainList CurExList;     //SkillGroup에서 선택된 스킬이 어떤것인지 알기 위한 변수

    private Color32 defaultColor = new(255, 255, 255, 255);     //기본 버튼의 색상
    private Color32 selectColor = new(130, 130, 130, 255);      //선택된 버튼의 색상

    private readonly string explainfmt1 = "{0}\n<color=#0D47A1>{1}</color>\n\n";  //스킬 정보(동작 설명)을 표시할 문자열 서식
    private readonly string explainfmt2 = "<color=#FF0000>{0} :</color> {1}"; //스킬 정보(쿨타임, 범위 등)를 표시할 문자열 서식

    private void Awake()
    {
        monStat = PlayerInfo.MonList[FindClass.MISelNum];    //현재 보고있는 몬스터의 스탯 받아오기
        CurExList = (ExplainList)FindClass.SISelNum;         //선택된 스킬번호를 Enum으로 변경   
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMonEx(monStat.starForce);    //처음엔 현재 몬스터의 성급에 맞는 스킬설명 출력
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.name.Contains(name))       //설명창이 아닌 바깥쪽 클릭하면 설명창 삭제
        {
            EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
            Destroy(transform.parent.gameObject);
        }
    }

    public void SetMonEx(int StarF)       //선택한 별버튼의 색상변경, 요청받은 스킬설명 리스트에 따라 상세 설명을 출력하는 함수 (StartButton들의 OnClick()에서 매개변수로 버튼의 번호를 받아온다)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        //--------------- 선택한 별탭인지에 따라 색상 변경 ---------------
        for (int i = 0; i < starTab.childCount; i++)      //별 탭 전체에 실행
        {
            if (i.Equals(StarF))        
                starTab.GetChild(i).GetComponent<Image>().color = selectColor;
            else
                starTab.GetChild(i).GetComponent<Image>().color = defaultColor;
        }
        //--------------- 선택한 별탭인지에 따라 색상 변경 ---------------

        if(monStat.monName.Equals(MonsterName.Soldier))
        {
            Soldier a_SoldierCtrl = MonInformMgr.Instance.GetComponentInChildren<Soldier>();   //MonInformMgr오브젝트의 하위에 있는 몬스터 오브젝트로부터 스크립트 가져오기

            switch (CurExList)   //SkillGroup에서 선택된 스킬에 따라 출력할 설명 결정
            {
                case ExplainList.Ultimate:
                    nameText.text = "<color=#ffff00>핵폭발</color>";
                    explainText.text = string.Format(explainfmt1, "목표를 조준하여 핵미사일을 유도한다.", "핵미사일은 땅에 부딪혀 폭발하여 주변에 있는 적들에게 피해를 준다.") +
                                        string.Format(explainfmt2, "범위", "폭발로부터 " + a_SoldierCtrl.SolUltiRadius + " 거리\n") +
                                        string.Format(explainfmt2, "대미지", a_SoldierCtrl.GetUltiDmg(StarF));
                    break;

                case ExplainList.Skill1:
                    nameText.text = "약물 투여";
                    explainText.text = string.Format(explainfmt1, "자신에게 약물을 투여하여 신진대사를 활성화한다.", "궁극기 게이지가 충전되고 그 반동으로 일정량의 HP가 감소한다.") +
                                       string.Format(explainfmt2, "발동 확률", a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                       string.Format(explainfmt2, "게이지 충전량", a_SoldierCtrl.SolSk1UltiG + "\n") +
                                       string.Format(explainfmt2, "범위", "자기 자신\n") +
                                       string.Format(explainfmt2, "쿨타임", a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk1] + "초");
                    break;

                case ExplainList.Skill2:
                    nameText.text = "지원요청";
                    explainText.text = string.Format(explainfmt1, "지원사격을 해줄 드론 4대를 호출한다.", "드론은 전방으로 이동하며 적을 발견하면 공격을 가한다.") +
                                       string.Format(explainfmt2, "발동 확률", a_SoldierCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                       string.Format(explainfmt2, "범위", "일직선상에 있는 무작위 적군\n") +
                                       string.Format(explainfmt2, "드론 지속시간", a_SoldierCtrl.DronLifeT + "초\n") +
                                       string.Format(explainfmt2, "드론 공격속도", "초당 " + 1 / a_SoldierCtrl.DronASpd + "발\n") +
                                       string.Format(explainfmt2, "드론 대미지", "발당 " + a_SoldierCtrl.GetDronDmg(StarF) + "\n") +
                                       string.Format(explainfmt2, "쿨타임", a_SoldierCtrl.SkSet.Cool[(int)Skill.Sk2] + "초");
                    break;
            }
        }
        else if(monStat.monName.Equals(MonsterName.Zombie))
        {
            Zombie a_ZombieCtrl = MonInformMgr.Instance.GetComponentInChildren<Zombie>();   //MonInformMgr오브젝트의 하위에 있는 몬스터 오브젝트로부터 스크립트 가져오기

            switch (CurExList)     //SkillGroup에서 선택된 스킬에 따라 출력할 설명 결정
            {
                case ExplainList.Ultimate:
                    nameText.text = "<color=#ffff00>끝없는 갈증</color>";
                    explainText.text = string.Format(explainfmt1, "피에 대한 끝없는 갈증으로 주위로부터 생명력을 빼앗는다.", "범위 내의 적들에게 피해를 주고 피해량의 일부를 자신의 체력으로 전환한다.") +
                                       string.Format(explainfmt2, "지속시간", a_ZombieCtrl.ZomUltiTime + "초\n") +
                                       string.Format(explainfmt2, "범위", "자신으로부터 " + a_ZombieCtrl.ZomUltiRadius + " 거리\n") +
                                       string.Format(explainfmt2, "체력전환율", a_ZombieCtrl.ZomUltiRatio * 100 + "%\n") +
                                       string.Format(explainfmt2, "대미지", "초당 " + a_ZombieCtrl.GetUltiDmg(StarF));
                    break;

                case ExplainList.Skill1:
                    nameText.text = "감염";
                    explainText.text = string.Format(explainfmt1, "적을 물어 감염시킨다.", "감염된 적은 피해를 입고 일정 시간 약화된다.") +
                                       string.Format(explainfmt2, "발동 확률", a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                       string.Format(explainfmt2, "지속시간", a_ZombieCtrl.ZomSk1SusTime + "초\n") +
                                       string.Format(explainfmt2, "범위", "현재 공격 중인 대상\n") +
                                       string.Format(explainfmt2, "방/마저 감소량", a_ZombieCtrl.GetSk1ReduVal(StarF) + "\n") +
                                       string.Format(explainfmt2, "대미지", a_ZombieCtrl.GetSk1Dmg(StarF) + "\n") +
                                       string.Format(explainfmt2, "쿨타임", a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk1] + "초");
                    break;

                case ExplainList.Skill2:
                    nameText.text = "포효";
                    explainText.text = string.Format(explainfmt1, "괴성을 내질러 적들의 주의를 끈다.", "일정 거리 안에 있는 적을 도발해 공격력을 감소시키고 자신을 공격하게 한다.") +
                                       string.Format(explainfmt2, "발동 확률", a_ZombieCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                       string.Format(explainfmt2, "지속시간", a_ZombieCtrl.ZomSk2SusTime + "초\n") +
                                       string.Format(explainfmt2, "범위", "자신으로부터 " + a_ZombieCtrl.ZomSk2Radius + " 거리\n") +
                                       string.Format(explainfmt2, "공격력 감소율", a_ZombieCtrl.GetReduceRatio(StarF) * 100 + "%\n") +
                                       string.Format(explainfmt2, "쿨타임", a_ZombieCtrl.SkSet.Cool[(int)Skill.Sk2] + "초");
                    break;
            }
        }
        else if (monStat.monName.Equals(MonsterName.GamblerCat))
        {
            GamblerCat a_GamblerCatCtrl = MonInformMgr.Instance.GetComponentInChildren<GamblerCat>();   //MonInformMgr오브젝트의 하위에 있는 몬스터 오브젝트로부터 스크립트 가져오기

            switch (CurExList)     //SkillGroup에서 선택된 스킬에 따라 출력할 설명 결정
            {
                case ExplainList.Ultimate:
                    nameText.text = "<color=#ffff00>기사회생</color>";
                    explainText.text = string.Format(explainfmt1, "위기에 처한 아군을 구해낸다.", "주위 일정 범위 내 아군의 체력을 크게 회복시킨다.") +
                                       string.Format(explainfmt2, "범위", "자신으로부터 " + a_GamblerCatCtrl.CatUltiRadius + " 거리\n") +
                                       string.Format(explainfmt2, "회복량", a_GamblerCatCtrl.GetUltiAmount(StarF));
                    break;

                case ExplainList.Skill1:
                    nameText.text = "치유";
                    explainText.text = string.Format(explainfmt1, "내재한 재능을 발현하여 아군을 돕는다.", "아군 두 명의 체력을 회복시킨다.") +
                                       string.Format(explainfmt2, "발동 확률", a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                       string.Format(explainfmt2, "범위", "현재 체력이 가장 낮은 아군 두 명\n") +
                                       string.Format(explainfmt2, "회복량", a_GamblerCatCtrl.GetHealAmount(StarF) + "\n") +
                                       string.Format(explainfmt2, "쿨타임", a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk1] + "초");
                    break;

                case ExplainList.Skill2:
                    nameText.text = "달변가";
                    explainText.text = string.Format(explainfmt1, "현란한 말솜씨로 아군의 사기를 올린다.", "아군 두 명의 대미지를 증가시킨다.") +
                                       string.Format(explainfmt2, "발동 확률", a_GamblerCatCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                       string.Format(explainfmt2, "지속시간", a_GamblerCatCtrl.GetBuffTime(StarF) + "초\n") +
                                       string.Format(explainfmt2, "범위", "현재 공격력이 가장 높은 아군 두 명\n") +
                                       string.Format(explainfmt2, "대미지 증가율", a_GamblerCatCtrl.CatSk2IncAmount * 100 + "%\n") +
                                       string.Format(explainfmt2, "쿨타임", a_GamblerCatCtrl.SkSet.Cool[(int)Skill.Sk2] + "초");
                    break;
            }
        }
        else if (monStat.monName.Equals(MonsterName.Mutant))
        {
            Mutant a_MutantCtrl = MonInformMgr.Instance.GetComponentInChildren<Mutant>();   //MonInformMgr오브젝트의 하위에 있는 몬스터 오브젝트로부터 스크립트 가져오기

            switch (CurExList)     //SkillGroup에서 선택된 스킬에 따라 출력할 설명 결정
            {
                case ExplainList.Ultimate:
                    nameText.text = "<color=#ffff00>난도질</color>";
                    explainText.text = string.Format(explainfmt1, "소리보다 빠른 속도로 이동하며 날카로운 손톱으로 적들을 난도질한다.", "정면 범위 안의 적들에게 피해를 준다.") +
                                       string.Format(explainfmt2, "범위", "자신의 정면방향으로 최대 " + a_MutantCtrl.MutUltiRange + " 거리내의 적(지형과 충돌 시 범위 감소)\n") +
                                       string.Format(explainfmt2, "대미지", a_MutantCtrl.GetUltiDmg(StarF));
                    break;

                case ExplainList.Skill1:
                    nameText.text = "암살";
                    explainText.text = string.Format(explainfmt1, "주위 환경에 스며들어 몸을 숨긴 후 적의 급소를 공격한다.", "전투 시작 시 몸을 숨겨 적의 타깃이 되지 않으며, 잠시 후 뒤에서 나타나 적을 공격한다.") +
                                       string.Format(explainfmt2, "발동 확률", "100 %\n") +
                                       string.Format(explainfmt2, "범위", "현재 체력이 가장 낮은 적 한명\n") +
                                       string.Format(explainfmt2, "대미지", "기본 공격력 + " + a_MutantCtrl.GetSk1Dmg(StarF) + "\n") +
                                       string.Format(explainfmt2, "쿨타임", "전투 시작 시 1회 발동");
                    break;

                case ExplainList.Skill2:
                    nameText.text = "변이";
                    explainText.text = string.Format(explainfmt1, "돌연변이로 인해 전투에 특화된 신체를 지녔다.", "적에게 가하는 피해와 자신이 받는 피해가 증가한다.") +
                                       string.Format(explainfmt2, "발동 확률", "패시브\n") +
                                       string.Format(explainfmt2, "범위", "자기자신\n") +
                                       string.Format(explainfmt2, "주는 대미지 증가율", a_MutantCtrl.MutSk2AMul * 100 + "%\n") +
                                       string.Format(explainfmt2, "받는 대미지 증가율", a_MutantCtrl.MutSk2TMul * 100 + "%\n") +
                                       string.Format(explainfmt2, "쿨타임", "없음");
                    break;
            }
        }
        else if (monStat.monName.Equals(MonsterName.Jammo))
        {
            Jammo a_JammoCtrl = MonInformMgr.Instance.GetComponentInChildren<Jammo>();   //MonInformMgr오브젝트의 하위에 있는 몬스터 오브젝트로부터 스크립트 가져오기

            switch (CurExList)     //SkillGroup에서 선택된 스킬에 따라 출력할 설명 결정
            {
                case ExplainList.Ultimate:
                    nameText.text = "<color=#ffff00>백만볼트</color>";
                    explainText.text = string.Format(explainfmt1, "내부의 핵으로부터 강력한 전기를 생성해 적을 섬멸한다", "단일 적에게 큰 피해를 준다.") +
                                       string.Format(explainfmt2, "범위", "현재 공격 중인 대상\n") +
                                       string.Format(explainfmt2, "대미지", a_JammoCtrl.GetUltiDmg(StarF));
                    break;

                case ExplainList.Skill1:
                    nameText.text = "서지";
                    explainText.text = string.Format(explainfmt1, "체내의 회로를 순간적으로 활성화해 과부하 상태가 된다.", "다음 기본공격이 강화된다.") +
                                       string.Format(explainfmt2, "발동 확률", a_JammoCtrl.SkSet.Prob[(int)Skill.Sk1] + "%\n") +
                                       string.Format(explainfmt2, "범위", "현재 공격 중인 대상\n") +
                                       string.Format(explainfmt2, "강화된 대미지", "기본 공격력의 " + a_JammoCtrl.GetSk1Mul(StarF) + "배\n") +
                                       string.Format(explainfmt2, "쿨타임", a_JammoCtrl.SkSet.Cool[(int)Skill.Sk1] + "초");
                    break;

                case ExplainList.Skill2:
                    nameText.text = "가속";
                    explainText.text = string.Format(explainfmt1, "미약한 전류를 흘려보내 움직임을 향상시킨다.", "주위 아군의 공격 속도가 향상된다.") +
                                       string.Format(explainfmt2, "발동 확률", a_JammoCtrl.SkSet.Prob[(int)Skill.Sk2] + "%\n") +
                                       string.Format(explainfmt2, "지속시간", a_JammoCtrl.JamSk2Time + "초\n") +
                                       string.Format(explainfmt2, "범위", "자신으로부터 " + a_JammoCtrl.JamSk2Radius + " 거리\n") +
                                       string.Format(explainfmt2, "증가량", a_JammoCtrl.GetSk2Value(StarF) + "\n") +
                                       string.Format(explainfmt2, "쿨타임", a_JammoCtrl.SkSet.Cool[(int)Skill.Sk2] + "초");
                    break;
            }
        }        
    }
}
