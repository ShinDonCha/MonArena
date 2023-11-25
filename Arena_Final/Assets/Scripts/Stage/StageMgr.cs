using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//������ ���� ���� ���������� �����ֱ� ���� ��ũ��Ʈ
//StageScene�� StageMgr�� �ٿ��� ���
public class StageMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    [SerializeField]
    private GameObject loadingPanel = null;     //�ε� �ǳ� ������
    [SerializeField]
    private Text curStageText = null;            //���� ���������� ǥ���� �ؽ�Ʈ

    // Start is called before the first frame update
    void Start()
    {
        curStageText.text = PlayerInfo.CombatStage.ToString();  //���� �������� ǥ��
        BGMController.Instance.BGMChange(BGMList.Lobby);     //BGM ����
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.ChallengeButton:
                LoadScene(SceneList.InGameScene);   //�ΰ��� ������ �̵�
                break;

            case ButtonList.BackButton:
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());    //�κ� ������ �̵�
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    #region --------------------- �� �ҷ����� �Լ� ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- �� �ҷ����� �Լ� ---------------------
}
