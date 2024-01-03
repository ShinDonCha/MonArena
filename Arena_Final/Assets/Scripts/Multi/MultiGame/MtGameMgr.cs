using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Pt = ExitGames.Client.Photon;

//��Ƽ������ ��ü���� ������ ����ϴ� ��ũ��Ʈ
//MultiGameScene�� MutiGameMgr�� �ٿ��� ���
public class MtGameMgr : MonoBehaviourPunCallbacks, IButtonClick, ICMCList, ILoadScene
{
    private enum MsgType        //�� �����ڿ��� ǥ���� �޽����� Ÿ��
    {
        LogMsg,
        CountMsg,
    }

    public static MtGameMgr Instance;

    [SerializeField]
    private MonStorage monStore = null;         //���� �̹���,������Ʈ �����
    private PhotonView photonV = null;          //PhotonView ������Ʈ�� ���� ����

    [Header("----------- ���� ���� -----------")]
    [SerializeField]
    private GameObject standByUI = null;       //���� �غ� �� ǥ���� �� UI���� ���� �� UICanvas�� StandByUI ����
    [SerializeField]
    private GameObject fightUI = null;         //���� ���� �� ������ Fight�̹����� ����ִ� UI��, UICanvas�� FightUI ����
    [SerializeField]
    private GameObject combatUI = null;        //���� �� ǥ���� �� UI���� ���� ��, UICanvas�� CombatUI ����
    [SerializeField]
    private Text team1NickText = null;         //Team1�� �г���(�������� �г���)�� �� �ؽ�Ʈ
    [SerializeField]
    private Text team2NickText = null;         //Team2�� �г���(�������� �г���)�� �� �ؽ�Ʈ

    [SerializeField]
    private GameObject standBySetPoint = null;  //���� �غ� �� ���� SetPoint    

    private List<CmnMonCtrl> team1CMCList = new();   //������ ������ �� �������� ���͵��� ���¸� �ľ��ϱ� ���� CmnMonCtrl ����Ʈ, ���� ���� �� ��ġ�Ǿ��ִ� ���Ͱ� ��Ͽ� �߰�
    private List<CmnMonCtrl> team2CMCList = new();   //������ ������ �� �������� ���͵��� ���¸� �ľ��ϱ� ���� CmnMonCtrl ����Ʈ, ���� ���� �� ��ġ�Ǿ��ִ� ���Ͱ� ��Ͽ� �߰�

    private int numofTeam1 = 0;     //Team1�� ���� ����
    private int numofTeam2 = 0;     //Team2�� ���� ����

    [Header("----------- �� ���� -----------")]
    [SerializeField]
    private Text logText = null;            //�濡 ����, ���� �� �α׸� ǥ�����ֱ� ���� �ؽ�Ʈ    
    [SerializeField]
    private Text eMonNickText = null;       //��� �г����� ǥ���ϱ� ���� �ؽ�Ʈ
    [SerializeField]
    private Transform eMonListTr = null;    //��� ���� �̹����� �� Transform (����� ��ǥ ���͸� �����ִ� �뵵)
    [SerializeField]
    private GameObject kickButton = null;   //�����ư(�����Ͱ� �ƴϸ� ���������ֱ�)
    [SerializeField]
    private Text playerNumText = null;      //���� �����ο��� �����ֱ� ���� �ؽ�Ʈ
    [SerializeField]
    private GameObject countPanel = null;   //�����ð� ǥ�ÿ� �ǳ��� ���ӿ�����Ʈ
    [SerializeField]
    private Text countText = null;          //���� ���۱��� ���� �ð��� �����ֱ� ���� �ؽ�Ʈ
    private readonly int combatCount = 15;  //���� ���۱��� �ɸ��� �� �ð�
    private int curCount = 0;               //���� ���۱��� ���� �ð�

    [SerializeField]
    private GameObject loadingPanel = null; //�ε� �ǳ� ������(�κ�� ���ư� �� ���)

    private void Awake()
    {
        Instance = this;
        photonV = GetComponent<PhotonView>();

        if (standByUI != null)
            standByUI.SetActive(true);      //�غ��� UI �ѱ�

        if (fightUI != null)
            fightUI.SetActive(false);       //���� ���� UI ����

        if (combatUI != null)
            combatUI.SetActive(false);      //������ UI ����

        if(countPanel != null)
            countPanel.SetActive(false);    //ī��Ʈ �ǳ� ����

        if (kickButton != null)
            kickButton.SetActive(false);    //���� ��ư ����

        ResultData.PrevScene = SceneList.MultiGameScene;    //���� ���� ���� ������ ����
        FindClass.GetCMCListFunc = CMCListFuncSet;          //���� �´� CmnMonCtrl ����Ʈ�� ã������ �Լ��� �߰�
        BGMController.Instance.BGMChange(BGMList.Battle);   //����� ����
    }

    // Start is called before the first frame update
    void Start()
    {
        eMonNickText.text = "";

        #region --------------- �濡 �������� �� �ڽ��� ���� ���� �Ѱ��ֱ� (�����Ϳ� Ŭ���̾�Ʈ ��� ����) ---------------
        //------------- ��뿡�� �� ���� �Ϻ��� ������ �ֱ� ���� ���� ����
        PhotonNetwork.EnableCloseConnection = true;     //���� ������ ���� ���� ����

        int[] a_MNumList = new int[(int)MathF.Min(PlayerInfo.MonList.Count, 5)];          //���� ���� ������ ���� ���� (�ִ� 5��������)
        int[] a_MStarList = new int[(int)MathF.Min(PlayerInfo.MonList.Count, 5)];         //���� ������ ���� ������ ���� ����  (�ִ� 5��������)

        for (int i = 0; i < a_MNumList.Length; i++)
        {
            a_MNumList[i] = (int)PlayerInfo.MonList[i].monName;       //���� �̸��� int�������� ����
            a_MStarList[i] = PlayerInfo.MonList[i].starForce;         //������ ������ ����
        }

        Pt.Hashtable a_Hash = new()        //Hashtable�� ����
        {
            { "MonList", a_MNumList },
            { "StarList", a_MStarList }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(a_Hash);      //PhotonNetwork�� ���� ����
         //------------- ��뿡�� �� ���� �Ϻ��� ������ �ֱ� ���� ���� ����
        #endregion --------------- �濡 �������� �� �ڽ��� ���� ���� �Ѱ��ֱ� (�����Ϳ� Ŭ���̾�Ʈ ��� ����)---------------
    }

    #region -------------------- �������̽� --------------------
    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        switch (BtnL)
        {
            case ButtonList.BackButton:         //�泪���� ��ư
                PhotonNetwork.LeaveRoom();
                break;

            case ButtonList.KickButton:         //���� ��ư
                foreach (Player FindP in PhotonNetwork.PlayerListOthers)  //���� ������ ��ü �÷��̾� �߿��� ã��
                    if (FindP.NickName.Contains(eMonNickText.text))       //�����ư Ŭ���� �г��Ӱ� ��ġ�ϴ� �÷��̾�� ����
                        PhotonNetwork.CloseConnection(FindP);             //�ش����� ����
                break;
        }
    }

    public void LoadScene(SceneList NextScene)      //�� �ҷ����� �Լ�
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    public List<CmnMonCtrl> CMCListFuncSet(string TagStr)       //�޾ƿ� �±׿� �´� CmnMonList ���� �Լ�
    {
        return Enum.Parse<Team>(TagStr) switch
        {
            Team.Team1 => team1CMCList,
            Team.Team2 => team2CMCList,
            _ => null
        };
    }
    #endregion -------------------- �������̽� --------------------

    #region ----------------- PunCallbacks, �� ���� -----------------
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Pt.Hashtable changedProps)    //�濡�ִ� ������ ������ ����Ǿ��� ��� �濡�ִ� ��ο��� ���� (���»�� ����)
    {
        if (changedProps.ContainsKey("MonArrName"))     //StandBySetPoint�� OnDisable�� �� �޾ƿ� ���� ��ġ ������ ��� ���
            return;
        else if (changedProps.ContainsKey("MonList"))   //ó�� ����� ������ �÷��̾��� ���� ������ ���
        {
            logText.text += "\n<color=#00ff00>[" + targetPlayer.NickName + "] ���� �濡 �����߽��ϴ�.</color>";  //�濡 ������ �÷��̾��� �г��� ǥ��

            //----------------- �ٸ� ������ ���� ��������(���� �� �ڽ��� ������ CustomProperties�� ����) -----------------
            if (!targetPlayer.Equals(PhotonNetwork.LocalPlayer))      //���� ������ �ƴ϶�� ����(��������� �����͸� ����)
            {
                kickButton.SetActive(true);             //�����ư ��

                int[] a_MonList = (int[])changedProps["MonList"];   //Hashtable�� Ű���� ���� ����� ���� ����Ʈ(��ȣ) �޾ƿ���
                int[] a_StarList = (int[])changedProps["StarList"]; //Hashtable�� Ű���� ���� ��� ������ ���� �޾ƿ���

                eMonNickText.text = targetPlayer.NickName;      //��� �г��� ǥ��

                for (int i = 0; i < a_MonList.Length; i++)
                {
                    GameObject a_GO = Instantiate(monStore.monSlot[a_StarList[i]], eMonListTr); //��� ���� ��� ���� (�ִ� 5��)
                    a_GO.name = a_MonList[i].ToString();         //�̸� ����
                    a_GO.tag = MonSlotTag.NameParse.ToString();  //�±� ����
                }
            }

            if (!PhotonNetwork.IsMasterClient)   //Ŭ���̾�Ʈ�� ����
            {
                //------------- �������� ���� ���� �޾ƿͼ� ���
                Pt.Hashtable a_MastHash = PhotonNetwork.MasterClient.CustomProperties;
                int[] a_MonList = (int[])a_MastHash["MonList"];   //Hashtable�� Ű���� ���� ����� ���� ����Ʈ(��ȣ) �޾ƿ���
                int[] a_StarList = (int[])a_MastHash["StarList"]; //Hashtable�� Ű���� ���� ��� ������ ���� �޾ƿ���

                eMonNickText.text = PhotonNetwork.MasterClient.NickName;      //������ �г��� ǥ��

                for (int i = 0; i < a_MonList.Length; i++)
                {
                    GameObject a_GO = Instantiate(monStore.monSlot[a_StarList[i]], eMonListTr); //��� ���� ��� ���� (�ִ� 5��)
                    a_GO.name = a_MonList[i].ToString();        //�̸� ����
                    a_GO.tag = MonSlotTag.NameParse.ToString(); //�±� ����
                }
                //------------- �������� ���� ���� �޾ƿͼ� ���
            }
            //----------------- �ٸ� ������ ���� ��������(���� �� �ڽ��� ������ CustomProperties�� ����) -----------------
        }

        playerNumText.text = "���� �ο�\n" + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;      //�濡 �����ִ� �ο� ǥ��
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)      //�� �ο��� ���� á�� ���
        {
            if (PhotonNetwork.IsMasterClient)               //�����Ͷ��
                PhotonNetwork.CurrentRoom.IsOpen = false;   //�� ������ϵ��� ����

            StartCoroutine(CountDown());    //�������۽ð� ī��Ʈ �ٿ� �ڷ�ƾ ����
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)   //PhotonNetwork.LeaveRoom(); ������ ���� ���� �� ���� ����鸸 ����(1:1 �����̹Ƿ� ��������� �����͸� ����)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)       //���� �ο��� ��ٸ�
        {
            StopAllCoroutines();        //�������� ��� �ڷ�ƾ ����
            playerNumText.text = "���� �ο�\n" + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;      //�濡 �����ִ� �ο� ǥ��
            countPanel.SetActive(false);                //ī��Ʈ �ǳ� ����
            PhotonNetwork.CurrentRoom.IsOpen = true;    //�� �������·� ����

            //------------- ��� ���� ǥ���� ����
            for (int i = 0; i < eMonListTr.childCount; i++)      //��� ���� ��� ����
                Destroy(eMonListTr.GetChild(i).gameObject);

            eMonNickText.text = "";                 //��� �г��� �ʱ�ȭ

            if (PhotonNetwork.IsMasterClient)       //�������� ���
                kickButton.SetActive(false);        //�����ư ����
            //------------- ��� ���� ǥ���� ����

            logText.text += "\n<color=#ff8700>[" + otherPlayer.NickName + "] ���� �����߽��ϴ�.</color>";   //���� �� �濡�� ���� ���� �г��� ǥ��
        }
    }

    public override void OnLeftRoom()   //PhotonNetwork.LeaveRoom(); �Լ� ���� �� (������ ���� ���� �� ���� ����� ����) (���� ���� �����̶� ��� ���õ� ���� ��� ����)
    {
        StopAllCoroutines();                    //��� �ڷ�ƾ ����
        LoadScene(SceneList.MultiLobbyScene);   //���� ������ �̵�
    }

    public override void OnMasterClientSwitched(Player newMasterClient)     //�����Ͱ� ����� ���(������ ������ ��)
    {
        logText.text += "\n<color=#ff0000>���� �����Ͱ� " + newMasterClient.NickName + "������ ����Ǿ����ϴ�.</color>";
        PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "MasterNick", newMasterClient.NickName } });    //������ ���� ����(�κ� ǥ�õǴ� ���������� �� �����͸� �ٲ��ִµ� ���ȴ�.)     
    }
    #endregion ----------------- PunCallbacks, �� ���� -----------------

    #region ------------------ ���� ���� ------------------
    //-------------------- ���� ���� --------------------
    private IEnumerator CountDown()     //�������۽ð� ī��Ʈ �ٿ� �ڷ�ƾ (�����Ͱ� ����)
    {
        countPanel.SetActive(true);     //ī��Ʈ �ǳ� �ѱ�
        curCount = combatCount;         //���۽ð� ����

        while (curCount > 0)
        {
            countText.text = curCount.ToString();           //�����ð� ǥ��
            yield return new WaitForSeconds(1.0f);

            curCount--;     //1�� ����
        }

        CombatReady();      //���� ���� �� �غ� ���� �Լ� ����
        yield return new WaitForSeconds(1.5f);      //Fight�ִϸ��̼� ����ð����� ���
        OnCombat();         //���� ���� �Լ� ����
    }

    private void CombatReady()     //���� ���� �� ����Ǵ� �Լ�(UI��ü �� ���� ������Ʈ ����, Fight�ִϸ��̼� ���)
    {
        standBySetPoint.SetActive(false);   //���� ���͸� ��ġ�ϴ� SetPoint ���� 
        standByUI.SetActive(false);         //�غ���� UI ����
        fightUI.SetActive(true);            //�������� UI �ѱ�
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Fight);     //�������� ȿ���� ���

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("CombatSetP1", Vector3.zero, Quaternion.identity);      //���� ������ ����ϴ� ������Ʈ ����
        else
            PhotonNetwork.Instantiate("CombatSetP2", Vector3.zero, Quaternion.identity);      //���� ������ ����ϴ� ������Ʈ ����
    }

    private void OnCombat()     //���� ���� �Լ�
    {
        fightUI.SetActive(false);     //�������� UI ����
        combatUI.SetActive(true);     //���� �� UI �ѱ�

        foreach (Player P in PhotonNetwork.PlayerList)
            if (P.NickName.Equals(PhotonNetwork.MasterClient.NickName))
                team1NickText.text = P.NickName;    //�������� �г��� ǥ��
            else
                team2NickText.text = P.NickName;    //�������� �г��� ǥ��

        numofTeam1 = team1CMCList.Count;        //�ʵ忡 ��ġ�� Team1�� ���� ���ڸ�ŭ ����(��Ƽ�� ��� CombatSetPoint���� ���͸� �����ϸ� CmnMonSet���� �ڽ��� �� ����Ʈ�� �߰���)
        numofTeam2 = team2CMCList.Count;        //�ʵ忡 ��ġ�� Team2�� ���� ���ڸ�ŭ ����(��Ƽ�� ��� CombatSetPoint���� ���͸� �����ϸ� CmnMonSet���� �ڽ��� �� ����Ʈ�� �߰���)

        //-------- �����Ϳ� Ŭ���̾�Ʈ �� �� �Ѹ��̶� ���͹�ġ�� �������� ��� ���� ���� �ƴϸ� ���� ���� --------
        if (numofTeam1 == 0)
        {
            if (PhotonNetwork.IsMasterClient)
                ResultData.CombatResult = Result.Defeat;      //�й�
            else
                ResultData.CombatResult = Result.Victory;     //�¸�

            StartCoroutine(EndAction());
        }
        else if (numofTeam2 == 0)
        {
            if (PhotonNetwork.IsMasterClient)
                ResultData.CombatResult = Result.Victory;      //�¸�
            else
                ResultData.CombatResult = Result.Defeat;     //�й�

            StartCoroutine(EndAction());
        }
        else
            GameManager.GMEventAct();
        //-------- �����Ϳ� Ŭ���̾�Ʈ �� �� �Ѹ��̶� ���͹�ġ�� �������� ��� ���� ���� �ƴϸ� ���� ���� --------
    }
    //-------------------- ���� ���� --------------------

    public void CalcResults(string Tag)    //���� ��� �� �ش� ������ �̸�, �����, ���ط��� �޾ƿ� ����, ����ϴ� �Լ� (IsMine�� �����)
    {
        switch (Enum.Parse<Team>(Tag))  //���� ������ Tag�� Team enum�������� ����
        {
            case Team.Team1:         //Team1�� ���
                numofTeam1--;        //���� �����ִ� Team1�� �� ����
                break;

            case Team.Team2:         //Team2�� ���
                numofTeam2--;        //���� �����ִ� Team2�� �� ����
                break;

            default:
                return;
        }

        //Team1 �Ǵ� Team2�� ���Ͱ� �� �׾��� �� (CalcResults �Լ� ��ü�� IsMine�� ���Ͱ� �׾��� �� ���� ���ÿ����� ����Ǳ� ������ ��� ������ �� �׾��ٴ°� �� ���Ͱ� �� �׾��ٴ� ��)
        if (numofTeam1 == 0 || numofTeam2 == 0)
            photonV.RPC("EndActionRPC", RpcTarget.All);     //���� ���� �ڷ�ƾ ���ð� ���ݿ� ��� ����
    }    

    [PunRPC]
    private void EndActionRPC()
    {     
        if (numofTeam1 == 0 || numofTeam2 == 0)
            ResultData.CombatResult = Result.Defeat;      //�й�
        else
            ResultData.CombatResult = Result.Victory;     //�¸�

        StartCoroutine(EndAction());      //ResultScene���� �Ѿ�� �ڷ�ƾ ����
    }

    private IEnumerator EndAction()   //ResultScene���� �Ѿ �� ȿ���� �ֱ����� �ڷ�ƾ (RPC�� ���� �����Ϳ� Ŭ���̾�Ʈ ���� ��ο��� ����)
    {
        //------------------ ���� ���� CustomProperties�� ����Ͽ� ������� ����
        int[] a_MonName;
        int[] a_Dmg;
        int[] a_HP;

        if (PhotonNetwork.IsMasterClient)       //�������� ���
        {
            team1CMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));   //�� ������� ���� ������� ����

            a_MonName = new int[team1CMCList.Count];        //������ ������ ���͵��� �̸��� ���� �迭����
            a_HP = new int[team1CMCList.Count];             //������ ������ ���͵��� ���� ���ط��� ���� �迭����
            a_Dmg = new int[team2CMCList.Count];            //������ ������ ���͵��� ���� ���ط��� ���� �迭����

            for (int i = 0; i < team1CMCList.Count; i++)
            {
                a_MonName[i] = (int)team1CMCList[i].CMCmonStat.monName;
                a_HP[i] = team1CMCList[i].TotalHP;
            }

            for(int i = 0; i < team2CMCList.Count; i++)
                a_Dmg[i] = team2CMCList[i].TotalDmg;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnNameNum", a_MonName } });     //�������� ���� ���� ����
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnHP", a_HP } });               //�������� ���� �� ���� ����
            //�����ڰ� ���� �� ���� ���� (���� ������ ��� ���� IsMine�� �ƴ� ������ �������ֱ� ������ �������� ȭ���� �������� ���͸� ���� ���޹޾ƾ� �Ѵ�.)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnDmg", a_Dmg } });             
        }
        else                                    //�������� ���
        {
            team2CMCList.Sort((a, b) => -a.TotalDmg.CompareTo((b.TotalDmg)));   //�� ������� ���� ������� ����

            a_MonName = new int[team2CMCList.Count];
            a_HP = new int[team2CMCList.Count];
            a_Dmg = new int[team1CMCList.Count];

            for (int i = 0; i < team2CMCList.Count; i++)
            {
                a_MonName[i] = (int)team2CMCList[i].CMCmonStat.monName;
                a_HP[i] = team2CMCList[i].TotalHP;
            }

            for (int i = 0; i < team1CMCList.Count; i++)
                a_Dmg[i] = team1CMCList[i].TotalDmg;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnNameNum", a_MonName } });     //�������� ���� ���� ����
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team2CmnHP", a_HP } });               //�������� ���� �� ���� ����
            //�����Ͱ� ���� �� ���� ���� (���� ������ ��� ���� IsMine�� �ƴ� ������ �������ֱ� ������ �������� ȭ���� �������� ���͸� ���� ���޹޾ƾ� �Ѵ�.)
            PhotonNetwork.CurrentRoom.SetCustomProperties(new() { { "Team1CmnDmg", a_Dmg } });
        }
        //------------------ ���� ���� CustomProperties�� ����Ͽ� ������� ����

        while (Time.timeScale > 0.3f)   //���� ��ġ��ŭ ������ ������ �ݺ�
        {
            Time.timeScale -= Time.deltaTime;       //�ӵ� ���������� �ϱ�
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Time.timeScale = 1.0f;       //timeScale ����ȭ

        SceneManager.LoadScene(SceneList.MultiResultScene.ToString());      //MultiResultScene���� �Ѿ��
    }
    #endregion ------------------ ���� ���� ------------------
}
