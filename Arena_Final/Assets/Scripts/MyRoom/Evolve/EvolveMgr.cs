using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

//��ȭ �� ���õ� ����,��� ������ �̹����� �������� �����ϴ� ��ũ��Ʈ
//EvolveScene�� EvolveMgr�� �ٿ��� ���
public class EvolveMgr : NetworkMgr, IButtonClick
{
    public static EvolveMgr Instance;       //�̱���

    [Header("------------- BeforeEvolve -------------")]
    [SerializeField]
    private Transform mainMonTabTr = null;      //���� ������ �̹����� �� Transform
    [SerializeField]
    private Transform mtrlMonTabTr = null;      //��� ������ �̹����� �� Transform
    [SerializeField]
    private Text goldText = null;               //������ ������ ��带 �����ֱ����� �ؽ�Ʈ
    [SerializeField]
    private GameObject errorText = null;        //������ ������ ǥ�����ֱ� ���� �ؽ�Ʈ ���ӿ�����Ʈ
    private readonly float errorTime = 2.0f;    //���� ǥ�ýð�
    [SerializeField]
    private Text explainText = null;            //������ ǥ���� �ؽ�Ʈ(���� ���õ� ���¿� ���� �����ؽ�Ʈ�� �ٲ��ֱ�����)

    private const int resetMonIndex = -1;       //�ʱ�ȭ �� �ο��� ���� ��ȣ
    private int mainMonIndex = -1;              //���õ� ���� ������ ��ȣ
    private int mtrlMonIndex = -1;              //���õ� ��� ������ ��ȣ

    private readonly int[] evvProb = { 50, 40, 30, 20, 10 };           //��ȭ Ȯ��
    private readonly int[] goldCost = { 100, 300, 600, 1000, 1500 };   //�Ҹ� ���

    private Color32 defaultColor = new(255, 255, 255, 255);   //�⺻ ������ ����
    private Color32 mainSelColor = new(233, 19, 110, 180);    //���õ� ���� ������ ����
    private Color32 mtrlSelColor = new(0, 0, 0, 180);         //���õ� ��� ������ ����

    public delegate void ColorDelegate(int SlotNum, Color32 WColor);     //CttMonListCtrl�� ColorChange �Լ��� ��� �����ϱ� ���� ��������Ʈ

    [Header("------------- AfterEvolve -------------")]
    [SerializeField]
    private Animation panelAnim = null;       //Canvas�� Panel������Ʈ�� �ִϸ��̼�(��ȭ �ִϸ��̼� ��ü ���)
    [SerializeField]
    private Transform mainMonPaneltr = null;  //��ȭ��Ű���� ���� ������ �̹����� ���� Transform
    [SerializeField]
    private Transform mtrlMonPaneltr = null;  //��ȭ��Ű���� ��� ������ �̹����� ���� Transform
    [SerializeField]
    private Transform starTabTr = null;       //��ȭ�� ������ ������ ������ �����ֱ����� ���̹����� ��� ������Ʈ
    [SerializeField]
    private Text resultText = null;           //��ȭ����� ǥ�����ִ� �ؽ�Ʈ
    

    private void Awake()
    {
        Instance = this;

        //������ ������ ���� ������� ���� �� MonSterName ������� ����
        if (PlayerInfo.MonList.Count != 0)
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();

        BGMController.Instance.BGMChange(BGMList.Lobby);            //BGM ����
    }

    private void Start()
    {
        errorText.SetActive(false);
        goldText.text = PlayerInfo.UserGold.ToString();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        switch (BtnL)
        {
            case ButtonList.EvolveButton:
                if (mainMonIndex == -1 || mtrlMonIndex == -1)       //���θ��� Ȥ�� ������ �� �ϳ��� ���ٸ� ���
                    return;

                //------------------- ��� ��� -------------------
                int a_GoldCost = goldCost[PlayerInfo.MonList[mainMonIndex].starForce];      //��ȭ��Ű���� ���θ����� �������� ��ȭ�� �ʿ��� ��� ���

                if(PlayerInfo.UserGold >= a_GoldCost)       //��尡 ����ϸ�
                {
                    PlayerInfo.UserGold -= a_GoldCost;      //��� �Ҹ�
                    PushPacket(PacketType.UserGold);        //��� �����û
                }
                else //(PlayerInfo.UserGold < a_GoldCost)   //��尡 �����ϸ�
                {
                    StartCoroutine(ErrorTextCo());          //���� �ؽ�Ʈ ǥ��
                    return;
                }
                //------------------- ��� ��� -------------------

                //-------------------- ��ȭ --------------------
                Instantiate(mainMonTabTr.transform.GetChild(0).gameObject, mainMonPaneltr);     //���� ���Ϳ� ���� �̹��� ����
                Instantiate(mtrlMonTabTr.transform.GetChild(0).gameObject, mtrlMonPaneltr);     //��� ���Ϳ� ���� �̹��� ����

                if (UnityEngine.Random.Range(0, 100) < evvProb[PlayerInfo.MonList[mainMonIndex].starForce])       //��ȭ ����
                {
                    resultText.color = Color.green;       //�ؽ�Ʈ ���� ����
                    resultText.text = "��ȭ ����";

                    //���� ������ ������ ����
                    PlayerInfo.MonList[mainMonIndex] = MonsterData.MonDic[PlayerInfo.MonList[mainMonIndex].monName][PlayerInfo.MonList[mainMonIndex].starForce + 1];
                    for (int i = 0; i < PlayerInfo.MonList[mainMonIndex].starForce; i++)    //�߰��� ���޸�ŭ ǥ��
                        starTabTr.GetChild(i).gameObject.SetActive(true);                    
                }
                else        //��ȭ ����
                {
                    resultText.color = Color.red;     //�ؽ�Ʈ ���� ����
                    resultText.text = "��ȭ ����";
                }
                PlayerInfo.MonList.RemoveAt(mtrlMonIndex);      //������ ����
                PushPacket(PacketType.MonList);     //�ٲ� ���͸���Ʈ �����û
                panelAnim.Play();                   //��ȭ �ִϸ��̼� ����
                BGMController.Instance.BGMChange(BGMList.Stop);                 //������� ����
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Evolve);        //��ȭ ȿ���� ���
                //-------------------- ��ȭ --------------------
                break;

            case ButtonList.BackButton:     //���ư��� ��ư
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());
                break;

            case ButtonList.OKButton:       //Ȯ�� ��ư
                SceneManager.LoadScene(SceneList.EvolveScene.ToString());
                break;
        }
    }

    public void EvolveSelect(MonSlotCtrl ReqMSC, ColorDelegate ColorDel)      //����or��� ���� ���� �� �����ϴ� �Լ�(CttMonListCtrl���� ����)
    {
        int a_ReqMonIndex = ReqMSC.transform.GetSiblingIndex();     //���õ� ������ ��ȣ ��������

        if (mainMonIndex.Equals(resetMonIndex))      //���� ���Ͱ� ���õǾ� ���� ���� ���¶�� ���� ���� ����
        {
            if (ReqMSC.MSCMonStat.starForce >= 5)   //�ش� ���Ͱ� �ִ뼺���� ��� ���
                return;

            mainMonIndex = a_ReqMonIndex;            //���õ� ���� ���� ��ȣ ����
            Instantiate(ReqMSC.gameObject, mainMonTabTr).tag = MonSlotTag.Untagged.ToString();       //���� ���� �̹��� ����
            ColorDel(a_ReqMonIndex, mainSelColor);      //���õ� ������ ���� ����       
            explainText.text = "���� ����� ���͸� �������ּ���.\n��� ���ʹ� ���� ���Ϳ� ���� ���޸� ���� �����մϴ�.";
        }
        else if (mainMonIndex.Equals(a_ReqMonIndex))    //���� ������ ������ ������� ��
        {
            Destroy(mainMonTabTr.GetChild(0).gameObject);  //���� ���� �̹��� ����
            ColorDel(mainMonIndex, defaultColor);      //��ҵ� ������ ���� ����
            mainMonIndex = resetMonIndex;       //���õ� ���� ������ ��ȣ �ʱ�ȭ

            if (!mtrlMonIndex.Equals(resetMonIndex))    //���� ��� ���͵� ��ġ�Ǿ� �־��ٸ�
            {
                Destroy(mtrlMonTabTr.GetChild(0).gameObject);  //��� ���� �̹��� ����
                ColorDel(mtrlMonIndex, defaultColor);      //��ҵ� ������ ���� ����
                mtrlMonIndex = resetMonIndex;       //���õ� ��� ������ ��ȣ �ʱ�ȭ
            }
            explainText.text = "������ ���� ��Ͽ��� ��ȭ��ų ���� ���͸� �������ּ���.\n�ִ� ������ �޼��� ���ʹ� ��ȭ��ų �� �����ϴ�.";
        }
        else if (mtrlMonIndex.Equals(resetMonIndex)) //��� ���Ͱ� ���õǾ� ���� ���� ���¶�� ��� ���� ����
        {
            if (!PlayerInfo.MonList[mainMonIndex].starForce.Equals(ReqMSC.MSCMonStat.starForce))  //���ΰ� ��ᰡ �ٸ� ������ ��� ���
                return;

            mtrlMonIndex = a_ReqMonIndex;             //���õ� ��� ���� ��ȣ ����
            Instantiate(ReqMSC.gameObject, mtrlMonTabTr).tag = MonSlotTag.Untagged.ToString();       //��� ���� �̹��� ����
            ColorDel(a_ReqMonIndex, mtrlSelColor);      //���õ� ������ ���� ����
            explainText.text = "<color=#FF0000>���� Ȯ�� : </color>" + evvProb[ReqMSC.MSCMonStat.starForce] +
                "%\n<color=#FF0000>��ȭ ��� :</color> " + goldCost[ReqMSC.MSCMonStat.starForce] + "���\n" +
                "<color=#149000>��ȭ�� ������ ��� ������ �ö󰡸�, ������ ��� ���� ������ �����˴ϴ�.\n���� ���ο� ������� ��� ���ʹ� �Ҹ�˴ϴ�.</color>";
        }
        else if (mtrlMonIndex.Equals(a_ReqMonIndex))    //��� ������ ������ ������� ��(��Ḹ ���)
        {
            Destroy(mtrlMonTabTr.GetChild(0).gameObject);  //��� ���� �̹��� ����
            mtrlMonIndex = resetMonIndex;       //���õ� ��� ������ ��ȣ �ʱ�ȭ
            ColorDel(a_ReqMonIndex, defaultColor);      //��ҵ� ������ ���� ����
            explainText.text = "���� ����� ���͸� �������ּ���.\n��� ���ʹ� ���� ���Ϳ� ���� ���޸� ���� �����մϴ�.";
        }
    }

    private IEnumerator ErrorTextCo()       //���� �ؽ�Ʈ�� ����ϱ� ���� �ڷ�ƾ
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }
}
