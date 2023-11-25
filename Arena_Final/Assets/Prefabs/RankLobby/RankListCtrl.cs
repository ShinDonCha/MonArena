using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//랭커의 정보를 표시해주기위한 스크립트
//RankListPrefab에 붙여서 사용
public class RankListCtrl : MonoBehaviour, IButtonClick, ILoadScene
{
    [SerializeField]
    private MonStorage monStore;        //이미지, 오브젝트 저장소
    [SerializeField]
    private GameObject loadingPanel = null;     //로딩 판넬 프리팹
    [SerializeField]
    private Image crtImage = null;      //해당 랭커의 캐릭터 이미지를 표시할 이미지
    [SerializeField]
    private Text nickText = null;       //해당 랭커의 닉네임을 표시할 텍스트
    [SerializeField]
    private Text rankText = null;       //해당 랭커의 랭킹을 표시할 텍스트
    [SerializeField]
    private Transform monInfoTr = null; //해당 랭커의 배치몬스터 목록을 생성할 Transform

    private string rankNick;            //랭커의 닉네임을 임시 저장하기 위한 변수
    private MonsterName[] rankDNameList;//랭커의 덱 리스트를 임시 저장하기 위한 변수
    private int[] rankDStarList;        //랭커의 덱 성급 리스트를 임시 저장하기 위한 변수

    // Start is called before the first frame update
    void Start()
    {
        rankNick = RankLobbyMgr.Instance.GetNickList(transform.GetSiblingIndex());          //RankLobbyMgr에 저장되어있던 랭커들의 닉네임 정보 중 이 오브젝트의 순서에 맞는 정보 가져오기
        rankDNameList = RankLobbyMgr.Instance.GetDNameList(transform.GetSiblingIndex());    //RankLobbyMgr에 저장되어있던 랭커들의 덱 정보 중 이 오브젝트의 순서에 맞는 정보 가져오기
        rankDStarList = RankLobbyMgr.Instance.GetDStarList(transform.GetSiblingIndex());    //RankLobbyMgr에 저장되어있던 랭커들의 성급 정보 중 이 오브젝트의 순서에 맞는 정보 가져오기

        crtImage.sprite = monStore.characterImg[RankLobbyMgr.Instance.GetCrtList(transform.GetSiblingIndex())]; //캐릭터 이미지 변경
        rankText.text = "순위 : " + RankLobbyMgr.Instance.GetRankList(transform.GetSiblingIndex());   //랭킹 표시
        nickText.text = rankNick;   //닉네임 표시

        for (int i = 0; i < rankDNameList.Length; i++)      //보유 몬스터 목록 생성
        {
            if (rankDNameList[i].Equals(MonsterName.None))
                continue;

            GameObject a_GO = Instantiate(monStore.monSlot[rankDStarList[i]], monInfoTr);
            a_GO.tag = MonSlotTag.NameParse.ToString();
            a_GO.name = ((int)rankDNameList[i]).ToString();
        }
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CombatButton:       //도전 버튼 클릭 시 해당 랭커의 정보 정적변수로 전달
                FindClass.RankNick = rankNick;
                FindClass.RankDName = rankDNameList;
                FindClass.RankDStar = rankDStarList;
                LoadScene(SceneList.RankGameScene);
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //버튼 클릭 효과음 재생
    }

    #region --------------------- 씬 불러오기 함수 ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- 씬 불러오기 함수 ---------------------
}
