using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System;

//PHP�κ��� ��Ŀ���� ������ �޾ƿ� �����ϱ� ���� ��ũ��Ʈ
//RankLobbyScene�� RankLobbyMgr�� �ٿ��� ���
public class RankLobbyMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    public static RankLobbyMgr Instance;

    private readonly string getRankURL = "http://dhosting.dothome.co.kr/Arena/GetRank.php";       //��ŷ������ ������ PHP �ּ�

    private readonly List<string> rankNickList = new();                             //��Ŀ���� ������ �г����� ������ ����Ʈ
    public string GetNickList(int Index) { return rankNickList[Index]; }            //�ܺο��� ������ ��Ŀ�� �г����� �������� ���� �Լ�
    private readonly List<int> rankCrtList = new();                                 //��Ŀ���� ������ ĳ���� �̹��� ��ȣ�� ������ ����Ʈ
    public int GetCrtList(int Index) { return rankCrtList[Index]; }                 //�ܺο��� ������ ��Ŀ�� ĳ���� �̹��� ��ȣ�� �������� ���� �Լ�
    private readonly List<int> rankNumList = new();                                 //��Ŀ���� ������ ������ ������ ����Ʈ
    public int GetRankList(int Index) { return rankNumList[Index]; }                //�ܺο��� ������ ��Ŀ�� ������ �������� ���� �Լ�
    private readonly List<MonsterName[]> rankDNameList = new();                     //��Ŀ���� ������ ���� ����� ������ ����Ʈ
    public MonsterName[] GetDNameList(int Index) { return rankDNameList[Index]; }   //�ܺο��� ������ ��Ŀ�� ���� ����� �������� ���� �Լ�
    private readonly List<int[]> rankDStarFList = new();                            //��Ŀ���� ������ ���� ������ ������ ����Ʈ
    public int[] GetDStarList(int Index) { return rankDStarFList[Index]; }          //�ܺο��� ������ ��Ŀ�� ���� ������ �������� ���� �Լ�

    [SerializeField]
    private MonStorage monStore = null;     //�̹���, ������Ʈ �����
    [SerializeField]
    private GameObject loadingPanel = null; //�ε� �ǳ� ������
    [SerializeField]
    private Image myCrtImg = null;          //���� ĳ���� �̹����� ���� �̹���
    [SerializeField]
    private Text myNickText = null;         //���� �г����� ǥ���� �ؽ�Ʈ
    [SerializeField]
    private Text myRankText = null;         //���� ������ ǥ���� �ؽ�Ʈ

    [SerializeField]
    private GameObject rankListPrefab = null;   //��Ŀ�� ������ ������ ��ŷ�� ������
    [SerializeField]
    private Transform rankListTr = null;        //rankListPrefab�� ������ Transform
    private bool getRanking = false;            //���� ��ŷ������ ������ ���� �ִ��� �˱����� ����(��Ʈ��ũ ���� ������)

    private void Awake()
    {
        Instance = this;        //�̱���
        myCrtImg.sprite = monStore.characterImg[PlayerInfo.UserCrtNum]; //���� ĳ���� �̹��� ǥ��
        myNickText.text = PlayerInfo.UserNick;                          //���� �г��� ǥ��
        BGMController.Instance.BGMChange(BGMList.RankLobby);            //������� ����
    }

    private void Start()
    {
        StartCoroutine(GetRankCo());        //��ŷ���� �������� �ڷ�ƾ ����
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.DefDeckButton:      //��� ��
                LoadScene(SceneList.DefDeckScene);
                break;

            case ButtonList.RetryButton:        //���ΰ�ħ
                if(!getRanking)
                    StartCoroutine(GetRankCo());
                break;

            case ButtonList.BackButton:         //���ư���
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    public void LoadScene(SceneList NextScene)  //�� �ҷ����� �Լ�
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }

    private IEnumerator GetRankCo()     //��ŷ ���� �������� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        for(int i = 0; i < rankListTr.childCount; i++)      //������ �����ϴ� RankList������ ��� ����
            Destroy(rankListTr.GetChild(i).gameObject);

        getRanking = true;

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(getRankURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
        {
            Debug.Log(a_www.error);
            getRanking = false;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_www.downloadHandler.data);     //�޾ƿ� �����͸� ���ڿ��� ����

        JSONNode a_JSON = JSON.Parse(a_GetStr);

        if (a_JSON == null)
        {
            getRanking = false;
            yield break;
        }

        for(int i = 0; i < a_JSON.Count; i++)       //�޾ƿ� ��Ŀ�� �� ��ŭ �ݺ�
        {
            if (a_JSON[i]["User_Nick"].Equals(PlayerInfo.UserNick))      //���� ������ ���
                myRankText.text = "���� : " + a_JSON[i]["User_Rank"];     //���� ǥ��
            else  //�ٸ� ������ ������ ���
            {
                rankNickList.Add(a_JSON[i]["User_Nick"]);       //�г��� �߰�
                rankCrtList.Add(a_JSON[i]["User_Crt"]);         //ĳ���� �̹��� �߰�
                rankNumList.Add(a_JSON[i]["User_Rank"]);        //��ŷ ���� �߰�

                JSONNode a_DefDeckJSON = JSON.Parse(a_JSON[i]["DefDeck"]);         //�� ����Ʈ �ѹ� �� �Ľ�
                JSONNode a_DefStarJSON = JSON.Parse(a_JSON[i]["DefStarF"]);        //�� ���� ����Ʈ �ѹ� �� �Ľ�

                if (a_DefDeckJSON == null)      //���� ����
                    continue;

                //-------------- ���� ��� �޾ƿ��� --------------
                MonsterName[] a_MNameArr = new MonsterName[a_DefDeckJSON.Count];
                int[] a_MStarFArr = new int[a_DefStarJSON.Count];

                for (int k = 0; k < a_DefDeckJSON.Count; k++)
                    if (Enum.TryParse(a_DefDeckJSON[k], out MonsterName MonName))
                    {
                        a_MNameArr[k] = MonName;
                        a_MStarFArr[k] = a_DefStarJSON[k];
                    }

                rankDNameList.Add(a_MNameArr);
                rankDStarFList.Add(a_MStarFArr);
                //-------------- ���� ��� �޾ƿ��� --------------

                Instantiate(rankListPrefab, rankListTr);    //��ŷ�� ����
            }
        }

        getRanking = false; 
    }
}
