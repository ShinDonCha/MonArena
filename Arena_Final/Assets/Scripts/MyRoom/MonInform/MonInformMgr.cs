using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//MonInformScene의 버튼동작 및 몬스터 오브젝트를 생성 및 몬스터 정보표시, 스킬표시 등 RightBoard 하위의 오브젝트 컨트롤을 담당하는 스크립트
//MonInformScene의 MonInformMgr에 붙여서 사용
public class MonInformMgr : MonoBehaviour, IButtonClick
{
    public static MonInformMgr Instance;            //싱글턴

    [Header("----------- 게임 오브젝트 연결 -----------")]
    [SerializeField]
    private MonStorage monStore = null;           //몬스터 이미지 및 오브젝트 저장소
    public MonsterStat MIMmonStat;                //정보를 표시하려는 몬스터의 스탯

    #region --------------- RightBoard ---------------
    [Header("------ 버튼, 강화 등등 ------")]
    [SerializeField]
    private Transform starLineTr = null;           //별이미지가 들어있는 게임오브젝트의 transform
    [SerializeField]
    private Transform skillTabTr = null;           //몬스터별 스킬을 넣을 곳의 transform

    [Header("------ 몬스터 정보 표시 (MonStatWindow) ------")]
    [SerializeField]
    private Text monNameText = null;               //몬스터 이름을 표시해줄 텍스트
    [SerializeField]
    private Sprite[] aTypeSprites = new Sprite[2]; //공격 타입 스프라이트
    [SerializeField]
    private Image aTypeImg = null;                 //공격 타입에 따른 이미지를 넣어줄 이미지 컴포넌트
    [SerializeField]
    private Text aTypeText = null;                 //공격 타입을 표시해줄 텍스트
    [SerializeField]
    private Text hpText = null;                    //HP를 표시해줄 텍스트
    [SerializeField]
    private Text aDamageText = null;               //공격력을 표시해줄 텍스트
    [SerializeField]
    private Text defText = null;                   //방어력을 표시해줄 텍스트
    [SerializeField]
    private Text mdefText = null;                  //마법저항력을 표시해줄 텍스트
    [SerializeField]
    private Text aSpdText = null;                  //공격속도를 표시해줄 텍스트
    [SerializeField]
    private Text mSpdText = null;                  //이동속도를 표시해줄 텍스트
    [SerializeField]
    private Text aRangeText = null;                //공격사거리를 표시해줄 텍스트
    #endregion ---------------------------------------------

    private void Awake()
    {
        Instance = this;        //싱글턴

        MIMmonStat = PlayerInfo.MonList[FindClass.MISelNum];  //유저가 보유한 몬스터 중 MyRoom에서 선택한 번호에 맞는 몬스터 스탯 가져오기
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < MIMmonStat.starForce; i++)         //성급에 맞게 별 오브젝트 켜기
            starLineTr.GetChild(i).gameObject.SetActive(true);

        monNameText.text = MIMmonStat.monName.ToString();      //몬스터 이름 표시

        (aTypeImg.sprite, aTypeText.text) = MIMmonStat.attackType switch  //현재 몬스터의 공격타입에 따라 이미지와 텍스트 바꿔주기
        {
            AttackType.Physical => (aTypeSprites[(int)AttackType.Physical], "물리대미지"),
            AttackType.Magical => (aTypeSprites[(int)AttackType.Magical], "마법대미지"),
            _=> (null, "오류")
        };

        hpText.text = MIMmonStat.hp.ToString();                 //HP 표시
        aDamageText.text = MIMmonStat.attackDmg.ToString();     //공격력 표시
        defText.text = MIMmonStat.defPower.ToString();          //방어력 표시
        mdefText.text = MIMmonStat.mdefPower.ToString();        //마법저항력 표시
        aSpdText.text = MIMmonStat.attackSpd.ToString();        //공격속도 표시
        mSpdText.text = MIMmonStat.moveSpd.ToString();          //이동속도 표시
        aRangeText.text = MIMmonStat.attackRange.ToString();    //공격사거리 표시

        Instantiate(monStore.monstersObj[(int)MIMmonStat.monName], Vector3.zero, Quaternion.Euler(0, 180, 0), transform);  //해당 몬스터 오브젝트 생성
        Instantiate(monStore.monstersSkGroup[(int)MIMmonStat.monName], skillTabTr);   //현재 몬스터에 맞는 스킬그룹 생성
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;
        
        switch(BtnL)
        {
            case ButtonList.LeftButton:     //왼쪽 화살표 클릭 시
                FindClass.MISelNum = SelNumCalc(FindClass.MISelNum - 1);    //현재 선택되어있는 몬스터 번호에 -1(이전 몬스터 선택)
                SceneManager.LoadScene(SceneList.MonInformScene.ToString());   //MonInformScene 다시 불러오기
                break;

            case ButtonList.RightButton:    //오른쪽 화살표 클릭 시
                FindClass.MISelNum = SelNumCalc(FindClass.MISelNum + 1);    //현재 선택되어있는 몬스터 번호에 +1(다음 몬스터 선택)
                SceneManager.LoadScene(SceneList.MonInformScene.ToString());   //MonInformScene 다시 불러오기
                break;

            case ButtonList.BackButton:     //돌아가기 클릭 시
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());     //MyRoomScene으로 이동
                break;               
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    private int SelNumCalc(int SelNum)      //정보보기를 원하는 몬스터의 번호 계산 함수
    {
        if (SelNum < 0)     //선택한 번호가 음수면 유저가 보유한 마지막 몬스터의 번호로 변경
            SelNum = PlayerInfo.MonList.Count -1;     
        else if (SelNum == PlayerInfo.MonList.Count)    //선택한 번호가 몬스터의 수와 같으면 처음몬스터의 번호로 변경
            SelNum = 0;

        return SelNum;      //계산된 몬스터 번호를 리턴
    }
}
