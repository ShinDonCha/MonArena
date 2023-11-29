using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

//ResultScene���� ǥ�� �� �͵��� �����ϴ� ��ũ��Ʈ (�̱۰� ��Ƽ���� ���� ���)
//ResultScene�� MtResultScene�� Mgr�� �ٿ��� ���
public class ResultMgr : NetworkMgr, IButtonClick, ILoadScene
{
    private enum ResultText
    {
        Win,
        Defeat
    }
    [SerializeField]
    private Image topTextImg = null;    //�ǳ� ��ܿ� ǥ���� ����̹����� ���� �̹��� ������Ʈ
    [SerializeField]
    private Sprite[] textImgs = new Sprite[2];    //��� �̹��� ��������Ʈ��
    [SerializeField]
    private GameObject loadingPanel = null;     //�ε� �ǳ� ������

    #region --------- �̱����� ���� ---------
    [Header("--------- �̱�����, ��Ƽ�� ��� ����α� ---------")]
    [SerializeField]
    private MonStorage monStore = null;     //�̹���, ������Ʈ �����
    [SerializeField]
    private GameObject retryTab = null;     //�絵�� ��ư
    [SerializeField]
    private Image mvpMonImg = null;         //MVP ������ �̹����� ���� �̹��� ������Ʈ
    #endregion --------- �̱����� ���� ---------

    #region --------- ��Ƽ���� ���� ---------
    [Header("--------- ��Ƽ����, �̱��� ��� ����α� ---------")]
    [SerializeField]
    private Text nameText1 = null;          //����(������)�� �̸��� ǥ���� �ؽ�Ʈ
    [SerializeField]
    private Text nameText2 = null;          //����(������)�� �̸��� ǥ���� �ؽ�Ʈ
    #endregion --------- ��Ƽ���� ���� ---------

    // Start is called before the first frame update
    void Start()
    {
        BGMController.Instance.BGMChange(BGMList.Stop);     //������� ������� ����

        if (retryTab != null)        //�̱� ����� ��
        {
            if (ResultData.CombatResult.Equals(Result.Defeat) && ResultData.PrevScene.Equals(SceneList.InGameScene))   //�������� �й����� ��(InGame���� ���� �����)
                retryTab.SetActive(true);     //�絵�� ��ư �ѱ�
            else
                retryTab.SetActive(false);    //�絵�� ��ư ����
        }

        if (mvpMonImg != null)       //�̱� ����� ��
            if(FindClass.GetCMCListFunc(Team.Ally.ToString()).Count != 0)
                mvpMonImg.sprite = monStore.monstersImg[(int)FindClass.GetCMCListFunc(Team.Ally.ToString())[0].CMCmonStat.monName];      //MVP(������ ���� ���� ������� ���� ����)�� �̹��� ��������
            else
                mvpMonImg.sprite = null;

        switch (ResultData.CombatResult)     //���� ���ο� ���� ��� �̹��� ����
        {
            case Result.Victory:
                topTextImg.sprite = textImgs[(int)ResultText.Win];
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ResultVictory);       //�¸� ȿ���� ���
                break;

            case Result.Defeat:
                topTextImg.sprite = textImgs[(int)ResultText.Defeat];
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ResultDefeat);        //�й� ȿ���� ���
                break;
        }

        switch(ResultData.PrevScene)
        {
            case SceneList.InGameScene:
                if (ResultData.CombatResult.Equals(Result.Victory))     //�¸� ��
                {
                    if(PlayerInfo.CombatStage < 5)                      //�ִ� ���������� �ƴ϶��
                        PlayerInfo.CombatStage++;                       //������ ���� �������� �ܰ� + 1

                    PushPacket(PacketType.CombatStage);                 //���� �������� �����û
                }
                break;

            case SceneList.RankGameScene:
                if (ResultData.CombatResult.Equals(Result.Victory))     //�¸� ��
                    PushPacket(PacketType.Ranking);                     //�ٲ� ��ŷ ���� ��û
                break;

            case SceneList.MultiGameScene:
                foreach (Player p in PhotonNetwork.PlayerList)
                    if (p.NickName.Equals(PhotonNetwork.MasterClient.NickName))   //�������� �г����̶�� ���� �ؽ�Ʈ�� �ֱ�
                        nameText1.text = p.NickName;
                    else                                                          //�� �������� �г����̶�� ���� �ؽ�Ʈ�� �ֱ�
                        nameText2.text = p.NickName;
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //��ӹ��� NetworkMgr�� Update()����
    }

    public void ButtonOnClick(Button PushBtn)       //��ưŬ�� �� �����ϴ� �Լ�
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.ResultPanel:      //��� �ǳ� Ŭ�� ��
                switch(ResultData.PrevScene)
                {
                    case SceneList.InGameScene:     
                        LoadScene(SceneList.StageScene);        //�������� ������ �̵� 
                        break;

                    case SceneList.RankGameScene:
                        LoadScene(SceneList.RankLobbyScene);    //��ũ�κ� ������ �̵�
                        break;

                    case SceneList.MultiGameScene:
                        LoadScene(SceneList.MultiLobbyScene);   //��Ƽ�κ� ������ �̵�
                        PhotonNetwork.LeaveRoom();              //�� ������
                        break;
                }
                break;

            case ButtonList.RetryButton:            //�絵�� ��ư Ŭ�� ��(�ΰ����� ���� �� ������� ����)
                LoadScene(SceneList.InGameScene);   //�ΰ��� ������ �̵�
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
