using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//몬스터 소환과 관련된 동작을 하는 스크립트
//ShopScene의 ShopMgr에 붙여서 사용
public class ShopMgr : NetworkMgr, IButtonClick
{
    [Header("----------- 저장소 -----------")]
    [SerializeField]
    private MonStorage monStore = null;     //이미지, 오브젝트 저장소

    [Header("----------- UI -----------")]
    [SerializeField]
    private Canvas mainCanvas = null;           //UI와 이미지들이 들어있는 이 씬의 Canvas를 담을 변수
    [SerializeField]
    private GameObject errorText = null;        //에러 텍스트 게임오브젝트를 담을 변수
    private readonly float errorTime = 2.0f;    //에러 텍스트 표시해줄 시간
    [SerializeField]
    private Animation[] ObjsAnimArray;          //배경에 있는 오브젝트들의 애니메이션 컴포넌트를 담을 변수
    private readonly float animTime = 5.0f;     //애니메이션 종료까지 걸리는 시간
    [SerializeField]
    private GameObject uiTab = null;            //UITab 게임오브젝트를 담을 변수 (애니메이션 재생할 때 UI들을 숨겨주기 위함)
    [SerializeField]
    private Text userGoldText = null;           //유저가 보유한 골드를 표시해주기위한 텍스트
    [SerializeField]
    private GameObject resultPanel = null;      //ResultPanel 게임오브젝트를 담을 변수
    [SerializeField]
    private Transform monSlotTabTr = null;      //소환된 몬스터 이미지를 넣을 곳의 Transform (ResultPanel의 하위오브젝트)

    private readonly int summonCost = 300;      //1회 소환비용
    private readonly Queue<int> summonQueue = new();   //소환결과로 나온 몬스터 목록을 담을 큐

    private void Start()
    {
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;       //Canvas의 초반 설정
        uiTab.SetActive(true);        //처음에 버튼 판넬 켜기
        resultPanel.SetActive(false); //처음에 결과 판넬 꺼주기
        errorText.SetActive(false);   //에러문구 꺼주기
        userGoldText.text = PlayerInfo.UserGold.ToString();        //유저의 보유 골드 표시하기
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();     //NetworkMgr의 Update() 실행
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생

        switch (BtnL)
        {
            case ButtonList.OnceSumButton:      //1회소환 버튼 클릭 시
                StartCoroutine(Summon(1));                
                break;

            case ButtonList.FiveTSumButton:     //5회소환 버튼 클릭 시
                StartCoroutine(Summon(5));
                break;

            case ButtonList.BackButton:         //돌아가기 버튼 클릭 시
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());        //로비씬으로 이동
                break;
        }
    }

    #region ------------------------ 소환 ------------------------
    private IEnumerator Summon(int Count)      //소환(횟수)
    {
        //------------------ 골드 계산 ------------------
        if (PlayerInfo.UserGold < summonCost * Count)        //골드가 충분하지 않다면
        {
            StartCoroutine(ErrorTextCo());      //에러문구 출력 코루틴 실행
            yield break;
        }

        uiTab.SetActive(false);                             //버튼들이 들어있는 게임오브젝트 오프로 변경
        PlayerInfo.UserGold -= summonCost * Count;          //유저 골드 소모
        userGoldText.text = PlayerInfo.UserGold.ToString(); //유저의 보유골드 표시 변경
        PushPacket(PacketType.UserGold);                    //골드 저장요청
        //------------------ 골드 계산 ------------------

        BGMController.Instance.BGMChange(BGMList.Stop);         //배경음 재생 중지
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Shop);  //소환 효과음 재생

        //------------------- 소환 -------------------
        for (int i = 0; i < Count; i++)     //요청한 소환 횟수만큼 실행
        {
            int a_SummonNum = UnityEngine.Random.Range(0, (int)MonsterName.MonsterCount);   //랜덤으로 몬스터 번호 뽑기
            PlayerInfo.MonList.Add(MonsterData.MonDic[(MonsterName)a_SummonNum][0]);        //소환된 몬스터 번호에 맞는 몬스터 정보를 딕셔너리로부터 가져오기(기본 0성)
            summonQueue.Enqueue(a_SummonNum);     //소환된 몬스터 큐에 저장(ResultPanel에 결과표시 할때 사용)
        }
        PushPacket(PacketType.MonList);           //몬스터 리스트 저장요청
        Camera.main.fieldOfView = 38;             //메인카메라의 옵션 변경

        foreach (Animation Anim in ObjsAnimArray) //목록에있는 애니메이션 실행
        {
            if (Anim.isPlaying)     //진행중인 애니메이션 취소(같은 애니메이션 중복실행 방지)
                Anim.Stop();

            Anim.Play();
        }
        //------------------- 소환 -------------------

        yield return new WaitForSeconds(animTime);      //애니메이션 종료 후 제어권 돌려받기

        //------------------- 결과 표시 -------------------
        resultPanel.SetActive(true);        //ResultPanel 켜기
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;  //Canvas의 설정 변경

        while(summonQueue.Count > 0)        //소환된 몬스터 수 만큼 실행
        {
            GameObject a_GO = Instantiate(monStore.monSlot[0], monSlotTabTr);   //몬스터 이미지 생성
            a_GO.name = summonQueue.Dequeue().ToString();                       //소환 목록에서 정보 가져오기
            a_GO.tag = MonSlotTag.NameParse.ToString();                         //태그 변경
        }
        //------------------- 결과 표시 -------------------
    }

    private IEnumerator ErrorTextCo()  //에러문구 출력 코루틴
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }

    public void ResultOnClick()     //ResultPanel 클릭시 동작하는 함수(활성화된 ResultPanel 게임오브젝트의 OnClick()에서 실행)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);   //버튼 클릭 효과음 재생
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;   //Canvas의 설정 변경
        resultPanel.SetActive(false);       //ResultPanel 끄기
        uiTab.SetActive(true);              //UI탭 켜기
        for (int i = 0; i < monSlotTabTr.childCount; i++)      //소환된 몬스터 이미지 전부 삭제
            Destroy(monSlotTabTr.GetChild(i).gameObject);
        BGMController.Instance.BGMChange(BGMList.Lobby);       //배경음 변경
    }
    #endregion ------------------------ 소환 ------------------------
}
