using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using System.Text;

//�α��ΰ� �ű԰���, �����ͺ��̽��κ��� ������������ ���� ����ϴ� ��ũ��Ʈ
//TitleScene�� TitleMgr�� �ٿ��� ���
public class TitleMgr : MonoBehaviour, IButtonClick, ILoadScene
{
    private enum RetryType  //��õ� ��ûŸ��
    {
        MonData,   //���� ������
        Login,     //�α���
        SignUp     //��������
    }

    [SerializeField]
    private GameObject loadingPanel = null;     //�ε� �ǳ� ������

    [Header("------- Login -------")]
    [SerializeField]
    private GameObject loginPanel = null;        //�α��� �ǳ� ���ӿ�����Ʈ
    [SerializeField]
    private InputField loginIDIFD = null;        //�α��� �ǳ��� ID_InputField
    [SerializeField]
    private InputField loginPWIFD = null;        //�α��� �ǳ��� PW_InputField
    [SerializeField]
    private GameObject startPanel = null;        //���ӽ��� �ǳ�
    private string inputID = "";        //�α��� �� �Է��ߴ� ID�� �ӽ������� ���� ����
    private string inputPW = "";        //�α��� �� �Է��ߴ� PW�� �ӽ������� ���� ����

    [Header("------- Signup -------")]
    [SerializeField]
    private GameObject signupPanel = null;       //SignUp �ǳ�
    [SerializeField]
    private InputField signupIDIFD = null;       //SignUp �ǳ��� ID_InputField
    [SerializeField]
    private InputField signupPWIFD = null;       //SignUp �ǳ��� PW_InputField

    [Header("------- Connecting -------")]
    [SerializeField]
    private Text connectText = null;            //���� �� �������� ǥ�ÿ� �ؽ�Ʈ
    private bool isTrying = false;              //���� ������ ������������ �˱����� ����
    private Color connectColor = Color.white;   //���� ���� ǥ�ÿ� �ؽ�Ʈ ����
    private Color errorColor = Color.red;       //���� ���� ǥ�ÿ� �ؽ�Ʈ ����
    private readonly string loginURL = "http://dhosting.dothome.co.kr/Arena/Login.php";     //�α��� ���� �ּ�
    private readonly string dicDataURL = "http://dhosting.dothome.co.kr/Arena/MonData.php"; //���� ������ ���� �ּ�
    private readonly string signupURL = "http://dhosting.dothome.co.kr/Arena/SignUp.php";   //���� ���� ���� �ּ�
    private readonly Queue<RetryType> retryQueue = new();       //��õ� ��û ť


    // Start is called before the first frame update
    void Start()
    {
        startPanel.SetActive(false);        //���� �ǳ� ����
        loginPanel.SetActive(false);        //�α��� �ǳ� ����
        signupPanel.SetActive(false);       //�������� �ǳ� ����
        connectText.gameObject.SetActive(true); //���� �ؽ�Ʈ �ѱ�

        StartCoroutine(DataInit());      //��ųʸ� ����(���� ������ ��������)
        BGMController.Instance.BGMChange(BGMList.Title);     //BGM ����
    }

    // Update is called once per frame
    void Update()
    {      
        //------------ ��õ� ��û���� ��� ����
        if (retryQueue.Count > 0 && !isTrying)     //�������� �����û�� �ʿ��ϰ�, �����۾� �������� �ƴ϶��
        {
            switch (retryQueue.Dequeue())
            {
                case RetryType.MonData:     //���� ������ �ٽ� ��û
                    StartCoroutine(DataInit());
                    break;

                case RetryType.Login:       //�α��� �ٽ� ��û
                    StartCoroutine(LoginCo(inputID, inputPW));
                    break;

                case RetryType.SignUp:      //�������� �ٽ� ��û
                    StartCoroutine(SignUpCo());
                    break;
            }           
        }
        //------------ ��õ� ��û���� ��� ����

    }
    #region ---------------------------- ��ư ���� �Լ� ----------------------------    
    public void ButtonOnClick(Button PushBtn)    //Ÿ��Ʋ�����ִ� ��ư Ŭ�� �� ����(��ư�� OnClick���� ����)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))  //�޾ƿ� ���ӿ�����Ʈ�� �̸��� ButtonName�� enum������ ����
            return;

        switch(BtnL)
        {
            case ButtonList.LoginButton:        //�α��� ��ư
                if (isTrying) return;           //�̹� �������� ������ �õ����̶�� ���
                StartCoroutine(LoginCo(loginIDIFD.text.Trim(), loginPWIFD.text.Trim()));   //�α��� �ڷ�ƾ(����) ����
                break;

            case ButtonList.CreateButton:       //���� ��ư
                if (isTrying) return;           //�̹� �������� ������ �õ����̶�� ���
                StartCoroutine(SignUpCo());     //�������� �ڷ�ƾ ����
                break;

            case ButtonList.SignupButton:       //�������� �ǳ� ���� ��ư
                loginPanel.SetActive(false);    //�α��� �ǳ� ����
                signupPanel.SetActive(true);    //Sign Up�ǳ� Ȱ��ȭ
                break;            

            case ButtonList.CancelButton:       //�������� �ǳ� ���� ��ư
                signupPanel.SetActive(false);   //Sign Up�ǳ� ����
                loginPanel.SetActive(true);     //�α��� �ǳ� �ѱ�
                break;

            case ButtonList.StartButton:         //���ӽ��� ��ư(StartPanel�� ����)
                LoadScene(SceneList.LobbyScene); //���̵� �Լ� ����
                break;

            case ButtonList.LogoutButton:        //������ȯ ��ư(StartPanel�� ����)
                startPanel.SetActive(false);     //���� �ǳ� ����
                loginPanel.SetActive(true);      //�α��� �ǳ� �ѱ�
                connectText.text = "";           //���� �ؽ�Ʈ �ʱ�ȭ
                connectText.gameObject.SetActive(true); //���� �ؽ�Ʈ �ѱ�
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }
    #endregion ---------------------------- ��ư ���� �Լ� ----------------------------

    #region --------------------- �� �ҷ����� �Լ� ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- �� �ҷ����� �Լ� ---------------------

    #region ---------------------------- ���� ������ �ҷ����� ----------------------------
    private IEnumerator DataInit()         //������ ����Ǿ��ִ� ���͵��� ������ ��ųʸ��� ��������
    {
        if (isTrying)   //�̹� �������� ������ �õ����̶�� ���
            yield break;

        isTrying = true;        //���� ������ ���·� ����
        connectText.color = connectColor;
        connectText.text = "�����κ��� �����͸� �޾ƿ��� ���Դϴ�. ��ø� ��ٷ��ּ���.";

        WWWForm a_Form = new();                

        for (int i = 0; i < (int)MonsterName.MonsterCount; i++)         //������ ������ŭ ������ Ű���� �����ֱ�
            a_Form.AddField(("Mon" + i).ToString(), ((MonsterName)i).ToString());

        a_Form.AddField("MonCount", (int)MonsterName.MonsterCount);     //PHP�� �ݺ����� ���� �� ��ŭ �ϱ����� ��

        UnityWebRequest a_WWW = UnityWebRequest.Post(dicDataURL, a_Form);
        yield return a_WWW.SendWebRequest();    //���� ���� ��û
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)            //���� ���ῡ ������ �ִٸ�
        {
            ReqRetry(RetryType.MonData);    //��õ� ��û
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //�޾ƿ� �����͸� ���ڿ��� ����

        if (!a_GetStr.Contains("Success!!"))    //���� ������ �������µ� �������� ���ߴٸ� (�����ͺ��̽� ���ٺҰ�, �ش� ������ ������ �������� ����)
        {
            ErrorMessage(a_GetStr);     //�����޼��� ǥ��
            yield break;
        }
        
        JSONNode a_JSON = JSON.Parse(a_GetStr);       //������ ���ڿ� JSON�������� �Ľ�

        if (a_JSON == null)     //�Ľ��� ���� ������ �ִٸ�
        {
            ReqRetry(RetryType.MonData);    //��õ� ��û
            yield break;
        }

        //----------------- ��ųʸ� ����
        for (int monnum = 0; monnum < (int)MonsterName.MonsterCount; monnum++)          //���� �� ��ŭ �ݺ�
        {
            List<MonsterStat> a_List = new();   //0�� ~ 5�������� ���� ������ ���� ����Ʈ
            MonsterStat a_Stat = new();     //���� ����Ʈ�� �߰��� ���� ����

            //Soldier ���� Jammo���� ���޿� ���� �޶����� ��ġ �и� (HP ~ AttackSpd)(������ JSONArray �������� ���� �Ǿ�����)
            a_JSON[monnum]["HP"] = JSON.Parse(a_JSON[monnum]["HP"]);
            a_JSON[monnum]["AttackDmg"] = JSON.Parse(a_JSON[monnum]["AttackDmg"]);
            a_JSON[monnum]["DefPower"] = JSON.Parse(a_JSON[monnum]["DefPower"]);
            a_JSON[monnum]["MDefPower"] = JSON.Parse(a_JSON[monnum]["MDefPower"]);
            a_JSON[monnum]["AttackSpd"] = JSON.Parse(a_JSON[monnum]["AttackSpd"]);
            //Soldier ���� Jammo���� ���޿� ���� �޶����� ��ġ �и� (HP ~ AttackSpd)(������ JSONArray �������� ���� �Ǿ�����)

            for (int starforce = 0; starforce < 6; starforce++)      //������ ���� (0�� ~ 5��)�� ���� ���� ����
            {
                a_Stat.monName = Enum.Parse<MonsterName>(a_JSON[monnum]["MonsterName"]);
                a_Stat.attackType = Enum.Parse<AttackType>(a_JSON[monnum]["AttackType"]);
                a_Stat.hp = a_JSON[monnum]["HP"][starforce];
                a_Stat.attackDmg = a_JSON[monnum]["AttackDmg"][starforce];
                a_Stat.defPower = a_JSON[monnum]["DefPower"][starforce];
                a_Stat.mdefPower = a_JSON[monnum]["MDefPower"][starforce];
                a_Stat.attackSpd = a_JSON[monnum]["AttackSpd"][starforce];
                a_Stat.moveSpd = a_JSON[monnum]["MoveSpd"];
                a_Stat.attackRange = a_JSON[monnum]["AttackRange"];
                a_Stat.starForce = starforce;

                a_List.Add(a_Stat);
            }
            MonsterData.MonDic.Add((MonsterName)monnum, a_List);     //���� ������ ��ųʸ��� ����
        }
        //----------------- ��ųʸ� ����

        isTrying = false;       //���� ���� ���� ���·� ����

        if(PlayerPrefs.GetString("UserID") == "")       //����� ������ ID�� ������
        {
            connectText.color = errorColor;
            connectText.text = "��ϵ� ���� ������ �����ϴ�. ������ ���� �����ϰų� ���� �α��� ���ּ���.";
            loginPanel.SetActive(true);
        }
        else //����� ������ ID�� ������
            StartCoroutine(LoginCo(PlayerPrefs.GetString("UserID"), PlayerPrefs.GetString("UserPW")));      //�ڵ� �α��� ����
    }
    #endregion ---------------------------- ���� ������ �ҷ����� ----------------------------

    #region ---------------------------- �α��� ----------------------------
    private IEnumerator LoginCo(string IDStr, string PWStr)    //�α��ι�ư Ŭ�� �� ����Ǵ� �α��� �ڷ�ƾ  
    {
        isTrying = true;    //���� ������ ���·� ����
        connectText.color = connectColor;
        connectText.text = "������ ������ �������� ��... ��ø� ��ٷ��ּ���.";

        inputID = IDStr;     //���� ������ ���� ��õ� ��û�� ���� �Էµ� ID ����
        inputPW = PWStr;     //���� ������ ���� ��õ� ��û�� ���� �Էµ� PW ����

        if (IDStr.Length <= 0)       //ID �Է����� �ʾ��� ��
        {
            ErrorMessage(IDStr);     //���� �޼��� ǥ��
            yield break;
        }
        else if (PWStr.Length <= 0)  //PW �Է����� �ʾ��� ��
        {
            ErrorMessage(PWStr);    //���� �޼��� ǥ��
            yield break;
        }

        loginPanel.SetActive(false);        //�α��� �ǳ� ����

        WWWForm a_Form = new();
        a_Form.AddField("User_ID", inputID);
        a_Form.AddField("User_PW", inputPW);

        UnityWebRequest a_WWW = UnityWebRequest.Post(loginURL, a_Form);
        yield return a_WWW.SendWebRequest();    //���� ���� ��û
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)            //���� ���ῡ ������ �ִٸ� ���� ID�� PW�� ��õ�
        {
            ReqRetry(RetryType.Login);      //��õ� ��û
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //�޾ƿ� �����͸� ���ڿ��� ����

        if (!a_GetStr.Contains("Success"))   //�α��� ���� �� (�����ͺ��̽��� ������ ���߰ų� ���̵� Ȥ�� ��й�ȣ�� Ʋ���� ��)
        {
            ErrorMessage(a_GetStr);     //�����޼��� ǥ��
            yield break;
        }

        JSONNode a_JSON = JSON.Parse(a_GetStr);

        if (a_JSON == null)     //�Ľ��� ���� ������ �ִٸ� ���� ID�� PW�� ��õ�
        {
            ReqRetry(RetryType.Login);  //��õ� ��û
            yield break;
        }

        //--------------- ����Ǿ��ִ� �÷��̾��� ���� �޾ƿ���
        PlayerInfo.UserNick = a_JSON["User_Nick"];      //�����ͺ��̽��� NULL�� �����ϰ� ���� "��" �������� �޾ƿ� �ٵ� JSON��ü���� �˾Ƽ� int�� �ٲ��ִµ�
        PlayerInfo.UniqueID = a_JSON["Unique_ID"];
        PlayerInfo.UserCrtNum = a_JSON["User_Crt"];
        PlayerInfo.UserLevel = a_JSON["User_Lv"];
        PlayerInfo.UserGold = a_JSON["User_Gold"];
        PlayerInfo.CombatStage = a_JSON["User_Stage"];

        if (a_JSON["User_ATime"] != null)          //���� ������ ���
            PlayerInfo.AutoExpTime = DateTime.Parse(a_JSON["User_ATime"]);            
        else                                       //�ű� ������ ���
            PlayerInfo.AutoExpTime = DateTime.Now;

        //--------- ���� ������ ���� ����Ʈ ��ü �ʱ�ȭ
        PlayerInfo.MonList.Clear();
        PlayerInfo.CombatDeck.Clear();
        PlayerInfo.CombatStarF.Clear();
        PlayerInfo.DefDeck.Clear();
        PlayerInfo.DefStarF.Clear();
        PlayerInfo.RankDeck.Clear();
        PlayerInfo.RankStarF.Clear();
        //--------- ���� ������ ���� ����Ʈ ��ü �ʱ�ȭ

        if (a_JSON["MonList"] != null)       //���� ������ ���
        {
            JSONNode a_MonJSON = JSON.Parse(a_JSON["MonList"]);       //string �������� ����ϱ� ���� �ѹ��� �Ľ� // ����� [\"Soldier\"] <-- �̻���) // �Ľ��ϰ��� ["Soldier", "Zombie"] �̷��� ��
            JSONNode a_StarJSON = JSON.Parse(a_JSON["MonStar"]);      //�迭�� �ٲٱ����� �ѹ��� �Ľ� //����� "[0,0,0,0]" <�̷��� ������

            for (int i = 0; i < a_MonJSON.Count; i++)      //����Ǿ��ִ� ���� ������ ���� ����
            {
                if (Enum.TryParse(a_MonJSON[i], out MonsterName MonName))
                    PlayerInfo.MonList.Add(MonsterData.MonDic[MonName][a_StarJSON[i]]);    //����� ������ �����̸��� ���޿� �´� ������ ��ųʸ����� ã�� �ֱ�
            }
        }

        if (a_JSON["CombatDeck"] != null)     //���� ������ ���
        {
            JSONNode a_CombatJSON = JSON.Parse(a_JSON["CombatDeck"]);
            for (int i = 0; i < a_CombatJSON.Count; i++)     //����Ǿ��ִ� ���� �� ������ ���� ����
            {
                if (Enum.TryParse(a_CombatJSON[i], out MonsterName MonName))
                    PlayerInfo.CombatDeck.Add(MonName);   //���� �̸� �����ͼ� ����
            }
        }

        if (a_JSON["CombatStarF"] != null)     //���� ������ ���
        {          
            JSONNode a_StarJSON = JSON.Parse(a_JSON["CombatStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //����Ǿ��ִ� ���� ���� ������ �����ͼ� ����
                PlayerInfo.CombatStarF.Add(a_StarJSON[i]);
        }

        if (a_JSON["DefDeck"] != null)      //���� ������ ���
        {
            JSONNode a_DefJSON = JSON.Parse(a_JSON["DefDeck"]);
            for (int i = 0; i < a_DefJSON.Count; i++)        //����Ǿ��ִ� ��� �� ������ ���� ����
            {
                if (Enum.TryParse(a_DefJSON[i], out MonsterName MonName))
                    PlayerInfo.DefDeck.Add(MonName);         //���� �̸� �����ͼ� ����
            }                  
        }

        if (a_JSON["DefStarF"] != null)     //���� ������ ���
        {
            JSONNode a_StarJSON = JSON.Parse(a_JSON["DefStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //����Ǿ��ִ� ��� ���� ������ �����ͼ� ����
                PlayerInfo.DefStarF.Add(a_StarJSON[i]);
        }

        if (a_JSON["RankDeck"] != null)     //���� ������ ���
        {
            JSONNode a_RankJSON = JSON.Parse(a_JSON["RankDeck"]);
            for (int i = 0; i < a_RankJSON.Count; i++)     //����Ǿ��ִ� ��ũ �� ������ ���� ����
            {
                if (Enum.TryParse(a_RankJSON[i], out MonsterName MonName))
                    PlayerInfo.RankDeck.Add(MonName);   //���� �̸� �����ͼ� ����
            }
        }

        if (a_JSON["RankStarF"] != null)     //���� ������ ���
        {
            JSONNode a_StarJSON = JSON.Parse(a_JSON["RankStarF"]);
            for (int i = 0; i < a_StarJSON.Count; i++)     //����Ǿ��ִ� ��ũ ���� ������ �����ͼ� ����
                PlayerInfo.RankStarF.Add(a_StarJSON[i]);
        }
        //--------------- ����Ǿ��ִ� �÷��̾��� ���� �޾ƿ���

        PlayerPrefs.SetString("UserID", inputID);       //������ ���̵� ���ÿ� ����(���� ���� ���� �� �ڵ��α����� ����)
        PlayerPrefs.SetString("UserPW", inputPW);       //������ ��й�ȣ ���ÿ� ����(���� ���� ���� �� �ڵ��α����� ����)

        isTrying = false;       //���� ���� ���� ���·� ����

        connectText.gameObject.SetActive(false);        //���� �ؽ�Ʈ ����
        startPanel.SetActive(true);     //�����ǳ� ����
    }
    #endregion ---------------------------- �α��� ----------------------------

    #region ---------------------------- �������� ----------------------------
    private IEnumerator SignUpCo()   //�������� Ȯ�ι�ư Ŭ�� �� ����Ǵ� �������� �ڷ�ƾ
    {
        isTrying = true;    //���� ������ ���·� ����
        connectText.color = connectColor;
        connectText.text = "������ �����ϴ� ��... ��ø� ��ٷ��ּ���.";

        string a_IDStr = signupIDIFD.text.Trim();   //���⸦ ������ �Էµ� �ؽ�Ʈ ��������
        string a_PWStr = signupPWIFD.text.Trim();   //���⸦ ������ �Էµ� �ؽ�Ʈ ��������

        if (a_IDStr.Length <= 0)    //ID�� �Է����� �ʾ��� ��
        {
            ErrorMessage(a_IDStr);  //���� �޼��� ǥ��
            yield break;
        }
        else if (a_PWStr.Length <= 0)   //PW�� �Է����� �ʾ��� ��
        {
            ErrorMessage(a_PWStr);  //���� �޼��� ǥ��
            yield break;
        }

        signupPanel.SetActive(false);       //�������� �ǳ� ����

        WWWForm a_Form = new();
        a_Form.AddField("Creat_ID", a_IDStr, Encoding.UTF8);
        a_Form.AddField("Creat_PW", a_PWStr, Encoding.UTF8);

        UnityWebRequest a_WWW = UnityWebRequest.Post(signupURL, a_Form);
        yield return a_WWW.SendWebRequest();    //���� ���� ��û
        a_WWW.uploadHandler.Dispose();

        if (a_WWW.error != null)    //���� ���ῡ �������� ��
        {
            ReqRetry(RetryType.SignUp); //��õ� ��û
            yield break;
        }

        Encoding a_Encd = Encoding.UTF8;
        string a_GetStr = a_Encd.GetString(a_WWW.downloadHandler.data);     //�޾ƿ� �����͸� ���ڿ��� ����

        if (!a_GetStr.Contains("Success"))   //���� ���� ���� �� (�����ͺ��̽��� ���� �Ұ� or ���� ���̵� ���� or ������ �����ϴ� �������� ������ �߻����� ��)
        {
            ErrorMessage(a_GetStr);
            yield break;
        }

        isTrying = false;   //���� ���� ���� ���·� ����
        loginPanel.SetActive(true);   //�α��� �ǳ� �ѱ�
        connectText.text = "���� ������ �Ϸ�Ǿ����ϴ�. �α��� �� ������ ������ �� �ֽ��ϴ�.";
    }
    #endregion ---------------------------- �������� ----------------------------

    #region ---------------------------- ���� ���� ----------------------------
    private void ReqRetry(RetryType Type)   //�������� ���� �� ��õ� ��û �Լ�
    {
        if (!retryQueue.Contains(Type))      //��õ� ��Ͽ� ���ٸ� �߰�
            retryQueue.Enqueue(Type);

        isTrying = false;
    }

    private void ErrorMessage(string Str)   //���� �߻� �� �޼��� ��� Ȥ�� ��õ� ��û�� ���� �Լ�
    {
        connectText.color = errorColor;

        if (Str.Length <= 0)    //ID Ȥ�� PW�� �Է����� ����(LoginCo, SignUpCo)
            connectText.text = "ID Ȥ�� PW�� ����� �Էµ��� �ʾҽ��ϴ�.";        
        else if (Str.Contains("ID does not exist."))    //ID�� �߸� �Է����� ���(LoginCo)
        {
            connectText.text = "�������� �ʴ� ID�Դϴ�. �α����� �ٽ� �õ��� �ּ���.";
            loginPanel.SetActive(true);
        }
        else if (Str.Contains("The PW doesn't fit."))   //PW�� �߸� �Է����� ���(LoginCo)
        {
            connectText.text = "��й�ȣ�� Ʋ�Ƚ��ϴ�. �α����� �ٽ� �õ��� �ּ���.";
            loginPanel.SetActive(true);
        }
        else if (Str.Contains("ID does exist."))    //�������� �� �ߺ� ID�� ���(SignupCo)
        {
            connectText.text = "���� ID�� �̹� �����մϴ�. ID�� �����Ͽ� �ٽ� �õ��� �ּ���.";
            signupPanel.SetActive(true);
        }
        else if (Str.Contains("Creat Error"))       //�������� ������ �ܰ迡�� ���� �߻� (SignupCo)
            ReqRetry(RetryType.SignUp);
        else if (Str.Contains("Could not Connect"))      //�����ͺ��̽��� �ƿ� �������� ������ ���(PHP������ �����ͺ��̽� ������ ���� ���̵�� ��й�ȣ � ������ ����)
            connectText.text = "������ ���� �� ������ �߻��߽��ϴ�. ���Ǹ� �����ּ���.";
        else if (Str.Contains("Data does not exist"))   //���� ������ �������� ������ ���(PHP������ ��û�ϴ� ������ �̸��� ������ ����)
            connectText.text = "������ �����͸� �������� �� ������ �߻��߽��ϴ�. ���Ǹ� �����ּ���.";

        isTrying = false;
    }
    #endregion ---------------------------- ���� ���� ----------------------------
}
