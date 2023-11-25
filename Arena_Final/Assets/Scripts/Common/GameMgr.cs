using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//GameScene���� �ʿ��� �����̳� ������� �ϴ� ��ũ��Ʈ
//InGameScene�� InGameMgr�� RankGameScene�� RankGameMgr�� �ٿ��� ���
public class GameMgr : NetworkMgr, IButtonClick, ICMCList
{
    //------------- UICanvas�� UI�ǵ�(InGameScene�� RankGameScene������ ���)
    [Header("-------- UICanvas(InGame, RankGame) --------")]
    [SerializeField]
    private GameObject standByUI = null;      //������ �� ǥ���� �� UI���� ���� ��, UICanvas�� StandByUI ����
    [SerializeField]
    private GameObject fightUI = null;        //���� ���� �� �ִϸ��̼��� ����ֱ� ���� UI
    [SerializeField]
    private GameObject combatUI = null;       //�������� �� ǥ���� �� UI���� ���ΰ�, UICanvas�� CombatUI ����
    //------------- UICanvas�� UI�ǵ�(InGameScene�� RankGameScene������ ���)
    
    [SerializeField]
    private GameObject loadingPanel = null;   //�ε� �ǳ� ������

    public static GameMgr Instance;           //�̱���
    private List<CmnMonCtrl> myCMCList = new();      //���� �������� ���͵��� ���¸� �ľ��ϱ� ���� CmnMonCtrl, ���� ���� �� ���Ͱ� ��Ͽ� �߰��ȴ�.
    private List<CmnMonCtrl> enemyCMCList = new();   //���� �������� �� ���͵��� ���¸� �ľ��ϱ� ���� CmnMonCtrl, ���� ���� �� ���Ͱ� ��Ͽ� �߰��ȴ�.
    private int numofPMon = 0;           //������ ���� ���� (���� ���� �� �Ʊ� ������ ��ġ�� ���� ����ŭ ���ڰ� �������� 0�̵Ǹ� ���� ����)
    private int numofEMon = 0;           //���� ���� ���� (���� ���� �� �� ������ ��ġ�� ���� ����ŭ ���ڰ� �������� 0�̵Ǹ� ���� ����)

    private SceneList curSceneName;     //���� ���� �̸��� ���� ����

    private void Awake()
    {
        Instance = this;

        if (standByUI != null)
            standByUI.SetActive(true);      //�غ��� UI �ѱ�

        if (fightUI != null)
            fightUI.SetActive(false);       //���� ���� UI ����

        if (combatUI != null)
            combatUI.SetActive(false);      //������ UI ����

        if (Enum.TryParse(SceneManager.GetActiveScene().name, out curSceneName))  //���� ���� �̸��� SceneList �������� ����
            ResultData.PrevScene = curSceneName;           //���� ���� �̸� ����

        FindClass.GetCMCListFunc = CMCListFuncSet;         //���� �´� CmnMonCtrl ����Ʈ�� ã������ �Լ��� �߰�

        BGMController.Instance.BGMChange(BGMList.Battle);  //BGM ����
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();      //��ӹ��� NetworkMgr�� Update() ����
    }

    #region -------------------- �������̽� --------------------
    public void ButtonOnClick(Button PushBtn)   //Hierarchy�� ��ư ������Ʈ���� OnClick()�� ���� ����
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))       //��ư ������Ʈ�� �̸��� enum������ ����
            return;

        switch (BtnL)
        {            
            case ButtonList.BackButton:     //���ư��� ��ư
                switch(curSceneName)
                {
                    case SceneList.InGameScene:
                        LoadScene(SceneList.StageScene);        //�������� ������ �̵�
                        break;

                    case SceneList.RankGameScene:
                        LoadScene(SceneList.RankLobbyScene);    //��ũ �κ� ������ �̵�
                        break;
                }
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);   //��ư Ŭ�� ȿ���� ���
                break;

            case ButtonList.StartButton:     //�������� ��ư
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Fight);        //�������� ȿ���� ���
                StartCoroutine(StartSettings());
                break;
        }        
    }
    
    public void LoadScene(SceneList NextScene)  //�� �ҷ����� �Լ�
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    public List<CmnMonCtrl> CMCListFuncSet(string TagStr)   //�ܺο��� FindClass.GetCMCListFunc�� ���� CMCList�� �������� �Լ�
    {
        if (!Enum.TryParse(TagStr, out Team TeamTag))
            return null;

        switch (TeamTag)
        {
            case Team.Ally:
                return myCMCList;

            case Team.Enemy:
                return enemyCMCList;

            default:
                return null;
        }
    }
    #endregion -------------------- �������̽� --------------------

    #region ------------------- ���� ���۽� ���� �Լ� -------------------
    private IEnumerator StartSettings()     //�������� �� �����ؾ��� �͵��� ��Ƴ��� �Լ�(��ġ����, UI����)
    {
        //--------------------------- �� ���� ---------------------------
        List<MonsterName> a_NameDeck = new();
        List<int> a_StarDeck = new();
        PacketType a_PType = PacketType.CombatDeck;

        switch (curSceneName)
        {
            case SceneList.InGameScene:
                a_NameDeck = PlayerInfo.CombatDeck;
                a_StarDeck = PlayerInfo.CombatStarF;
                a_PType = PacketType.CombatDeck;
                break;

            case SceneList.RankGameScene:
                a_NameDeck = PlayerInfo.RankDeck;
                a_StarDeck = PlayerInfo.RankStarF;
                a_PType = PacketType.RankDeck;
                break;
        }

        a_NameDeck.Clear();    //�� ����Ʈ ���� ���� (�ٽ� �����ϱ� ���ؼ� ���� ��� ����)
        a_StarDeck.Clear();    //�� ���� ����Ʈ ���� ���� (�ٽ� �����ϱ� ���ؼ� ���� ��� ����)

        for (int i = 0; i < FindClass.CurSetPoint.transform.childCount; i++)
        {
            FindClass.CurSetPoint[i].GetChild(0).gameObject.SetActive(false);   //Point�� �̹����� ��Ÿ���� ���� ������Ʈ ����

            if (FindClass.CurSetPoint[i].childCount > 1)    //Ȱ��ȭ��(���Ͱ� �����ϴ�) Point�� ����
            {
                a_NameDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.monName);    //�ش� ������ �̸��� ����
                a_StarDeck.Add(FindClass.CurSetPoint.GetPointMSC(i).MSCMonStat.starForce);  //�ش� ������ ������ ����

                myCMCList.Add(FindClass.CurSetPoint[i].GetComponentInChildren<CmnMonCtrl>());   //��ġ�� ���͵��� CommonMonCtrl�� ����Ʈ�� �߰�
            }
            else
            {
                a_NameDeck.Add(MonsterName.None);   //��������� ����
                a_StarDeck.Add(-1);                 //��������� ����
            }
        }
        PushPacket(a_PType);   //NetworkMgr�� ���� ��ġ�� ���� ��û
        //--------------------------- �� ���� ---------------------------

        standByUI.SetActive(false);     //���� �غ��� UI ����
        fightUI.SetActive(true);        //�������� UI �ѱ�

        numofPMon = myCMCList.Count;   //������ ���Ͱ� ��ġ�� ����ŭ ���� ����
        numofEMon = enemyCMCList.Count;  //�� ���Ͱ� ��ġ�� ����ŭ ���� ����

        yield return new WaitForSeconds(1.5f);      //�ִϸ��̼� ����ð���ŭ ���

        fightUI.SetActive(false);       //�������� UI ����
        combatUI.SetActive(true);       //���� UI �ѱ�

        //-------------- ������ �� �� �����̶� ���͸� �ϳ��� ��ġ���� �ʰ� �����ߴٸ� ��� ���� �ƴϸ� �������� --------------
        if (numofPMon == 0)
        {
            ResultData.CombatResult = Result.Defeat;   //�й�
            StartCoroutine(EndAction());
        }
        else if (numofEMon == 0)
        {
            ResultData.CombatResult = Result.Victory;   //�¸�
            StartCoroutine(EndAction());
        }
        else
            GameManager.GMEventAct();   //�̺�Ʈ ����(��������), CmnMonCtrl�� StartFight�� ������
        //-------------- ������ �� �� �����̶� ���͸� �ϳ��� ��ġ���� �ʰ� �����ߴٸ� ��� ���� �ƴϸ� �������� --------------
    }
    #endregion ------------------- ���� ���۽� ���� �Լ� -------------------

    #region ------------------ ���� ��� ���, ���� ���� �׼� ------------------
    public void CalcResults (string TeamTag)    //���� ��� �� �ش� ������ �̸�, �����, ���ط��� �޾ƿ� ����, ����ϴ� �Լ�
    {
        if (!Enum.TryParse(TeamTag, out Team TeamName))      //���� ������ Tag�� Team enum�������� ����
            return;

        switch(TeamName)
        {
            case Team.Ally:         //�Ʊ��� ���
                numofPMon--;         //���� �����ִ� �Ʊ��� �� ����
                break;

            case Team.Enemy:        //������ ���
                numofEMon--;        //���� �����ִ� ������ �� ����
                break;
        }

        if (numofPMon == 0 || numofEMon == 0)     //�Ʊ��� ���� �� ������̵� �� ������ ����
        {
            if (numofPMon == 0)              //�Ʊ��� �� �׾�����
                ResultData.CombatResult = Result.Defeat;     //�й�
            else if (numofEMon == 0)
                ResultData.CombatResult = Result.Victory;   //�¸�

            StartCoroutine(EndAction());      //ResultScene���� �Ѿ�� �ڷ�ƾ ����
        }
    }

    private IEnumerator EndAction()   //ResultScene���� �Ѿ �� ȿ���� �ֱ����� �ڷ�ƾ
    {
        myCMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));      //�� ������� ���� ������� ����

        while (Time.timeScale > 0.3f)   //���� ��ġ��ŭ ������ ������ �ݺ�
        {
            Time.timeScale -= Time.deltaTime;       //�ӵ� ���������� �ϱ�
            yield return null;
        }

        Time.timeScale = 1.0f;       //timeScale ����ȭ

        SceneManager.LoadScene(SceneList.ResultScene.ToString());      //ResultScene���� �Ѿ��
    }
    #endregion ------------------ ���� ��� ���, ���� ���� �׼� ------------------
}
