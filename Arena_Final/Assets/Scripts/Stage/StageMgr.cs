using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//유저의 현재 전투 스테이지를 보여주기 위한 스크립트
//StageScene의 StageMgr에 붙여서 사용
public class StageMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    [SerializeField]
    private GameObject loadingPanel = null;     //로딩 판넬 프리팹
    [SerializeField]
    private Text curStageText = null;            //현재 스테이지를 표시할 텍스트

    // Start is called before the first frame update
    void Start()
    {
        curStageText.text = PlayerInfo.CombatStage.ToString();  //현재 스테이지 표시
        BGMController.Instance.BGMChange(BGMList.Lobby);     //BGM 변경
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.ChallengeButton:
                LoadScene(SceneList.InGameScene);   //인게임 씬으로 이동
                break;

            case ButtonList.BackButton:
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());    //로비 씬으로 이동
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
