using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

//진화 시 선택된 메인,재료 몬스터의 이미지와 정보들을 관리하는 스크립트
//EvolveScene의 EvolveMgr에 붙여서 사용
public class EvolveMgr : NetworkMgr, IButtonClick
{
    public static EvolveMgr Instance;       //싱글턴

    [Header("------------- BeforeEvolve -------------")]
    [SerializeField]
    private Transform mainMonTabTr = null;      //메인 몬스터의 이미지가 들어갈 Transform
    [SerializeField]
    private Transform mtrlMonTabTr = null;      //재료 몬스터의 이미지가 들어갈 Transform
    [SerializeField]
    private Text goldText = null;               //유저가 보유한 골드를 보여주기위한 텍스트
    [SerializeField]
    private GameObject errorText = null;        //골드부족 오류를 표시해주기 위한 텍스트 게임오브젝트
    private readonly float errorTime = 2.0f;    //오류 표시시간
    [SerializeField]
    private Text explainText = null;            //설명을 표시할 텍스트(현재 선택된 상태에 따라 설명텍스트를 바꿔주기위함)

    private const int resetMonIndex = -1;       //초기화 시 부여할 몬스터 번호
    private int mainMonIndex = -1;              //선택된 메인 몬스터의 번호
    private int mtrlMonIndex = -1;              //선택된 재료 몬스터의 번호

    private readonly int[] evvProb = { 50, 40, 30, 20, 10 };           //진화 확률
    private readonly int[] goldCost = { 100, 300, 600, 1000, 1500 };   //소모 골드

    private Color32 defaultColor = new(255, 255, 255, 255);   //기본 몬스터의 색깔
    private Color32 mainSelColor = new(233, 19, 110, 180);    //선택된 메인 몬스터의 색깔
    private Color32 mtrlSelColor = new(0, 0, 0, 180);         //선택된 재료 몬스터의 색깔

    public delegate void ColorDelegate(int SlotNum, Color32 WColor);     //CttMonListCtrl의 ColorChange 함수를 대신 실행하기 위한 델리게이트

    [Header("------------- AfterEvolve -------------")]
    [SerializeField]
    private Animation panelAnim = null;       //Canvas의 Panel오브젝트의 애니메이션(진화 애니메이션 전체 담당)
    [SerializeField]
    private Transform mainMonPaneltr = null;  //진화시키려는 메인 몬스터의 이미지를 넣을 Transform
    [SerializeField]
    private Transform mtrlMonPaneltr = null;  //진화시키려는 재료 몬스터의 이미지를 넣을 Transform
    [SerializeField]
    private Transform starTabTr = null;       //진화에 성공한 몬스터의 성급을 보여주기위한 별이미지가 담긴 오브젝트
    [SerializeField]
    private Text resultText = null;           //진화결과를 표시해주는 텍스트
    

    private void Awake()
    {
        Instance = this;

        //몬스터의 성급이 높은 순서대로 정렬 후 MonSterName 순서대로 정렬
        if (PlayerInfo.MonList.Count != 0)
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();

        BGMController.Instance.BGMChange(BGMList.Lobby);            //BGM 변경
    }

    private void Start()
    {
        errorText.SetActive(false);
        goldText.text = PlayerInfo.UserGold.ToString();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        switch (BtnL)
        {
            case ButtonList.EvolveButton:
                if (mainMonIndex == -1 || mtrlMonIndex == -1)       //메인몬스터 혹은 재료몬스터 중 하나라도 없다면 취소
                    return;

                //------------------- 골드 계산 -------------------
                int a_GoldCost = goldCost[PlayerInfo.MonList[mainMonIndex].starForce];      //진화시키려는 메인몬스터의 성급으로 진화에 필요한 골드 계산

                if(PlayerInfo.UserGold >= a_GoldCost)       //골드가 충분하면
                {
                    PlayerInfo.UserGold -= a_GoldCost;      //골드 소모
                    PushPacket(PacketType.UserGold);        //골드 저장요청
                }
                else //(PlayerInfo.UserGold < a_GoldCost)   //골드가 부족하면
                {
                    StartCoroutine(ErrorTextCo());          //에러 텍스트 표시
                    return;
                }
                //------------------- 골드 계산 -------------------

                //-------------------- 진화 --------------------
                Instantiate(mainMonTabTr.transform.GetChild(0).gameObject, mainMonPaneltr);     //메인 몬스터와 같은 이미지 생성
                Instantiate(mtrlMonTabTr.transform.GetChild(0).gameObject, mtrlMonPaneltr);     //재료 몬스터와 같은 이미지 생성

                if (UnityEngine.Random.Range(0, 100) < evvProb[PlayerInfo.MonList[mainMonIndex].starForce])       //진화 성공
                {
                    resultText.color = Color.green;       //텍스트 색상 변경
                    resultText.text = "진화 성공";

                    //메인 몬스터의 성급을 변경
                    PlayerInfo.MonList[mainMonIndex] = MonsterData.MonDic[PlayerInfo.MonList[mainMonIndex].monName][PlayerInfo.MonList[mainMonIndex].starForce + 1];
                    for (int i = 0; i < PlayerInfo.MonList[mainMonIndex].starForce; i++)    //추가된 성급만큼 표시
                        starTabTr.GetChild(i).gameObject.SetActive(true);                    
                }
                else        //진화 실패
                {
                    resultText.color = Color.red;     //텍스트 색상 변경
                    resultText.text = "진화 실패";
                }
                PlayerInfo.MonList.RemoveAt(mtrlMonIndex);      //재료몬스터 삭제
                PushPacket(PacketType.MonList);     //바뀐 몬스터리스트 저장요청
                panelAnim.Play();                   //진화 애니메이션 실행
                BGMController.Instance.BGMChange(BGMList.Stop);                 //배경음악 끄기
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Evolve);        //진화 효과음 재생
                //-------------------- 진화 --------------------
                break;

            case ButtonList.BackButton:     //돌아가기 버튼
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());
                break;

            case ButtonList.OKButton:       //확인 버튼
                SceneManager.LoadScene(SceneList.EvolveScene.ToString());
                break;
        }
    }

    public void EvolveSelect(MonSlotCtrl ReqMSC, ColorDelegate ColorDel)      //메인or재료 몬스터 선택 시 동작하는 함수(CttMonListCtrl에서 실행)
    {
        int a_ReqMonIndex = ReqMSC.transform.GetSiblingIndex();     //선택된 몬스터의 번호 가져오기

        if (mainMonIndex.Equals(resetMonIndex))      //메인 몬스터가 선택되어 있지 않은 상태라면 메인 몬스터 선택
        {
            if (ReqMSC.MSCMonStat.starForce >= 5)   //해당 몬스터가 최대성급일 경우 취소
                return;

            mainMonIndex = a_ReqMonIndex;            //선택된 메인 몬스터 번호 저장
            Instantiate(ReqMSC.gameObject, mainMonTabTr).tag = MonSlotTag.Untagged.ToString();       //메인 몬스터 이미지 생성
            ColorDel(a_ReqMonIndex, mainSelColor);      //선택된 몬스터의 색깔 변경       
            explainText.text = "재료로 사용할 몬스터를 선택해주세요.\n재료 몬스터는 메인 몬스터와 같은 성급만 선택 가능합니다.";
        }
        else if (mainMonIndex.Equals(a_ReqMonIndex))    //메인 몬스터의 선택을 취소했을 때
        {
            Destroy(mainMonTabTr.GetChild(0).gameObject);  //메인 몬스터 이미지 삭제
            ColorDel(mainMonIndex, defaultColor);      //취소된 몬스터의 색깔 변경
            mainMonIndex = resetMonIndex;       //선택된 메인 몬스터의 번호 초기화

            if (!mtrlMonIndex.Equals(resetMonIndex))    //만약 재료 몬스터도 배치되어 있었다면
            {
                Destroy(mtrlMonTabTr.GetChild(0).gameObject);  //재료 몬스터 이미지 삭제
                ColorDel(mtrlMonIndex, defaultColor);      //취소된 몬스터의 색깔 변경
                mtrlMonIndex = resetMonIndex;       //선택된 재료 몬스터의 번호 초기화
            }
            explainText.text = "보유한 몬스터 목록에서 진화시킬 메인 몬스터를 선택해주세요.\n최대 성급을 달성한 몬스터는 진화시킬 수 없습니다.";
        }
        else if (mtrlMonIndex.Equals(resetMonIndex)) //재료 몬스터가 선택되어 있지 않은 상태라면 재료 몬스터 선택
        {
            if (!PlayerInfo.MonList[mainMonIndex].starForce.Equals(ReqMSC.MSCMonStat.starForce))  //메인과 재료가 다른 성급일 경우 취소
                return;

            mtrlMonIndex = a_ReqMonIndex;             //선택된 재료 몬스터 번호 저장
            Instantiate(ReqMSC.gameObject, mtrlMonTabTr).tag = MonSlotTag.Untagged.ToString();       //재료 몬스터 이미지 생성
            ColorDel(a_ReqMonIndex, mtrlSelColor);      //선택된 몬스터의 색깔 변경
            explainText.text = "<color=#FF0000>성공 확률 : </color>" + evvProb[ReqMSC.MSCMonStat.starForce] +
                "%\n<color=#FF0000>진화 비용 :</color> " + goldCost[ReqMSC.MSCMonStat.starForce] + "골드\n" +
                "<color=#149000>진화에 성공할 경우 성급이 올라가며, 실패할 경우 현재 성급이 유지됩니다.\n성공 여부에 상관없이 재료 몬스터는 소멸됩니다.</color>";
        }
        else if (mtrlMonIndex.Equals(a_ReqMonIndex))    //재료 몬스터의 선택을 취소했을 때(재료만 취소)
        {
            Destroy(mtrlMonTabTr.GetChild(0).gameObject);  //재료 몬스터 이미지 삭제
            mtrlMonIndex = resetMonIndex;       //선택된 재료 몬스터의 번호 초기화
            ColorDel(a_ReqMonIndex, defaultColor);      //취소된 몬스터의 색깔 변경
            explainText.text = "재료로 사용할 몬스터를 선택해주세요.\n재료 몬스터는 메인 몬스터와 같은 성급만 선택 가능합니다.";
        }
    }

    private IEnumerator ErrorTextCo()       //오류 텍스트를 출력하기 위한 코루틴
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }
}
