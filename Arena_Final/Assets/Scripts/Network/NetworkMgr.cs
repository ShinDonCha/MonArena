using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System.Linq;

//�����ͺ��̽��� ���������� ��û�ϱ� ���� ��ũ��Ʈ
//���������� ��û�� �ʿ��� ��ũ��Ʈ�� ����Ѵ�.
public class NetworkMgr : MonoBehaviour
{
    //-------- URL
    private readonly string nickSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveNickName.php";       //�г��� ���� �ּ�
    private readonly string userCrtURL = "http://dhosting.dothome.co.kr/Arena/SaveCrt.php";             //���� ĳ�����̹��� ���� �ּ�
    private readonly string userLvSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveLevel.php";        //���� ���� ���� �ּ�
    private readonly string monListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveMonList.php";     //������ �������� ���� �ּ�
    private readonly string goldSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveGold.php";           //������ ��� ���� �ּ�
    private readonly string autoExpSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveAutoExp.php";     //�ڵ�Ž�� ���� ���� �ּ�
    private readonly string stageSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveStage.php";         //�������� ���� �ּ�
    private readonly string combatSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveCombatList.php";   //������ ���� �ּ�
    private readonly string defListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveDefList.php";     //�� ���� �ּ�
    private readonly string rankListSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveRankList.php";   //��ũ�� ���� �ּ�
    private readonly string rankingSaveURL = "http://dhosting.dothome.co.kr/Arena/SaveRanking.php";     //��ŷ ���� �ּ�
    //-------- URL

    private readonly Queue<PacketType> packetsQueue = new();    //���� ������ ��û�� ��Ŷ ����� ���� ť
    private bool isNetworkLock = false;             //���� ������ ������ ���� ������ �Ǻ��ϱ� ���� ����
    protected bool reqExit = false;                 //���� ���� ��û�� ���Դ��� �˱����� ����

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isNetworkLock)     //������ ������ �������� �ƴ� ��
        {
            if (packetsQueue.Count > 0)     //��û���� ��Ŷ�� �ִٸ�
                ReqNetwork(packetsQueue.Dequeue());   //�������� ��û �Լ� ����
            else if (packetsQueue.Count <= 0 && reqExit)    //��û���� ��Ŷ�� ����, �����û�� ���Դٸ�
                Application.Quit();         //���α׷� ����
        }
    }

    private void ReqNetwork(PacketType PT)  //�������� ��û �Լ�
    {
        switch (PT)
        {
            case PacketType.NickName:
                StartCoroutine(NickChangeCo());  //�г��� ���� �ڷ�ƾ ����
                break;

            case PacketType.UserCrt:             
                StartCoroutine(UserCrtCo());     //���� ĳ���� �̹��� ���� �ڷ�ƾ ����
                break;

            case PacketType.UserLv:
                StartCoroutine(UserLvCo());      //���� ���� ���� �ڷ�ƾ ����
                break;

            case PacketType.MonList:
                StartCoroutine(MonListCo());     //������ �������� ���� �ڷ�ƾ ����
                break;

            case PacketType.AutoTime:
                StartCoroutine(AutoTimeCo());    //�ڵ�Ž�� ���� �ڷ�ƾ ����
                break;

            case PacketType.UserGold:
                StartCoroutine(UserGoldCo());    //���� ��� ���� �ڷ�ƾ ����
                break;

            case PacketType.CombatStage:
                StartCoroutine(StageCo());       //�������� ���� �ڷ�ƾ ����
                break;

            case PacketType.CombatDeck:
                StartCoroutine(CombatDeckCo());  //������ ���� �ڷ�ƾ ����
                break;

            case PacketType.DefDeck:
                StartCoroutine(DefDeckCo());   //�� ���� �ڷ�ƾ ����
                break;

            case PacketType.RankDeck:
                StartCoroutine(RankDeckCo());   //��ũ�� ���� �ڷ�ƾ ����
                break;

            case PacketType.Ranking:
                StartCoroutine(RankingCo());    //��ŷ ���� �ڷ�ƾ ����
                break;
        }
    }

    protected void PushPacket(PacketType Type)    //��ӹ��� ��ũ��Ʈ���� �������� ��û�Ҷ� �θ��� �Լ�
    {
        if (packetsQueue.Contains(Type))          //�̹� �����û�� �� ��Ŷ�� ���� ��Ŷ�� ��û�� ��� ���
            return;

        packetsQueue.Enqueue(Type);     //��Ŷ ����
    }    

    private IEnumerator NickChangeCo()  //�г��� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        NickPanelCtrl a_NPCtrl = GetComponentInChildren<NickPanelCtrl>();     //�г��� ���� �ǳ��� ��ũ��Ʈ ��������

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("Input_Nick", a_NPCtrl.GetReqNick);

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(nickSaveURL, a_form);  
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        System.Text.Encoding Encd = System.Text.Encoding.UTF8;
        string a_GetStr = Encd.GetString(a_www.downloadHandler.data);

        if (a_GetStr.Contains("exist"))   //���� �г����� ������ �޾ƿ��� ���ڿ�
            StartCoroutine(a_NPCtrl.ErrorOnOff("������� �г��� �Դϴ�."));      //�г��� ���� �ǳ��� �����޼��� ǥ�� �Լ� ����
        else if (a_GetStr.Contains("SaveSuccess"))   //�г��� ���濡 �������� ��
        {
            GetComponent<UserPanelCtrl>().NickChange(a_NPCtrl.GetReqNick);      //�г��� ����
            Destroy(a_NPCtrl.gameObject);       //�г��� �ǳ� ����
        }

        isNetworkLock = false;   //���� ���� �������� ����
    }

    private IEnumerator UserCrtCo()      //���� ĳ���� �̹��� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("User_Crt", PlayerInfo.UserCrtNum.ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(userCrtURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;    //���� ���� �������� ����
    }

    private IEnumerator UserLvCo()      //���� ���� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;
        
        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("Input_Level", PlayerInfo.UserLevel.ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(userLvSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;    //���� ���� �������� ����
    }

    private IEnumerator MonListCo()     //������ �������� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };

        if (PlayerInfo.MonList.Count > 0)                //���͸� �����ϰ� �ִٸ�
        {
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();        //���� ����

            for (int i = 0; i < PlayerInfo.MonList.Count; i++)
            {
                a_JSArr[0].Add(PlayerInfo.MonList[i].monName.ToString());     //������ �̸� ����
                a_JSArr[1].Add(PlayerInfo.MonList[i].starForce);      //������ starforce ����
            }
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("MonList", a_JSArr[0].ToString());
        a_form.AddField("MonStar", a_JSArr[1].ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(monListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator UserGoldCo()      //���� ��� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("UserGold", PlayerInfo.UserGold);

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(goldSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator AutoTimeCo()        //�ڵ� ���� �ð� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("AutoExpTime", PlayerInfo.AutoExpTime.ToString("yyyy-MM-dd HH:mm:ss"));

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(autoExpSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator StageCo()      //���� �������� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("CombatStage", PlayerInfo.CombatStage.ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(stageSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator CombatDeckCo()      //������ ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;    //���� ���������� ����

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0���� ���� �̸� �����ϴ°�, 1���� �� ������ ���� �����ϴ� ��

        for (int i = 0; i < PlayerInfo.CombatDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.CombatDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.CombatStarF[i]);
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("CombatList", a_JSArr[0].ToString());
        a_form.AddField("CombatMonStar", a_JSArr[1].ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(combatSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator DefDeckCo()     //�� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)   //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����
                
        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0���� ���� �̸� �����ϴ°�, 1���� �� ������ ���� �����ϴ� ��
                
        for(int i = 0; i < PlayerInfo.DefDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.DefDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.DefStarF[i]);
        }
        
        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("DefMonList", a_JSArr[0].ToString());
        a_form.AddField("DefMonStar", a_JSArr[1].ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(defListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����

        GetComponent<DefDeckMgr>().LoadScene(SceneList.RankLobbyScene);      //��ũ �κ������ �̵�
    }

    private IEnumerator RankDeckCo()    //��ũ�� ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)        //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;    //���� ���������� ����

        JSONArray[] a_JSArr = { new JSONArray(), new JSONArray() };     //0���� ���� �̸� �����ϴ°�, 1���� �� ������ ���� �����ϴ� ��

        for (int i = 0; i < PlayerInfo.RankDeck.Count; i++)
        {
            a_JSArr[0].Add(PlayerInfo.RankDeck[i].ToString());
            a_JSArr[1].Add(PlayerInfo.RankStarF[i]);
        }

        WWWForm a_form = new();
        a_form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_form.AddField("RankMonName", a_JSArr[0].ToString());
        a_form.AddField("RankMonStar", a_JSArr[1].ToString());

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(rankListSaveURL, a_form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

    private IEnumerator RankingCo()     //��ŷ ���� �ڷ�ƾ
    {
        if (PlayerInfo.UniqueID <= 0)   //�ùٸ� ������ �ƴ϶��
            yield break;

        isNetworkLock = true;   //���� ���������� ����

        WWWForm a_Form = new();
        a_Form.AddField("Input_UID", PlayerInfo.UniqueID.ToString());
        a_Form.AddField("Input_Nick", FindClass.RankNick);

        //--------- ���� ���� ��û
        UnityWebRequest a_www = UnityWebRequest.Post(rankingSaveURL, a_Form);
        yield return a_www.SendWebRequest();
        //--------- ���� ���� ��û
        a_www.uploadHandler.Dispose();

        if (a_www.error != null)
            Debug.Log(a_www.error);

        isNetworkLock = false;  //���� ���� �������� ����
    }

}
