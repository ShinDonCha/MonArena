using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//MonInformScene�� ��ư���� �� ���� ������Ʈ�� ���� �� ���� ����ǥ��, ��ųǥ�� �� RightBoard ������ ������Ʈ ��Ʈ���� ����ϴ� ��ũ��Ʈ
//MonInformScene�� MonInformMgr�� �ٿ��� ���
public class MonInformMgr : MonoBehaviour, IButtonClick
{
    public static MonInformMgr Instance;            //�̱���

    [Header("----------- ���� ������Ʈ ���� -----------")]
    [SerializeField]
    private MonStorage monStore = null;           //���� �̹��� �� ������Ʈ �����
    public MonsterStat MIMmonStat;                //������ ǥ���Ϸ��� ������ ����

    #region --------------- RightBoard ---------------
    [Header("------ ��ư, ��ȭ ��� ------")]
    [SerializeField]
    private Transform starLineTr = null;           //���̹����� ����ִ� ���ӿ�����Ʈ�� transform
    [SerializeField]
    private Transform skillTabTr = null;           //���ͺ� ��ų�� ���� ���� transform

    [Header("------ ���� ���� ǥ�� (MonStatWindow) ------")]
    [SerializeField]
    private Text monNameText = null;               //���� �̸��� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Sprite[] aTypeSprites = new Sprite[2]; //���� Ÿ�� ��������Ʈ
    [SerializeField]
    private Image aTypeImg = null;                 //���� Ÿ�Կ� ���� �̹����� �־��� �̹��� ������Ʈ
    [SerializeField]
    private Text aTypeText = null;                 //���� Ÿ���� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text hpText = null;                    //HP�� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text aDamageText = null;               //���ݷ��� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text defText = null;                   //������ ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text mdefText = null;                  //�������׷��� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text aSpdText = null;                  //���ݼӵ��� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text mSpdText = null;                  //�̵��ӵ��� ǥ������ �ؽ�Ʈ
    [SerializeField]
    private Text aRangeText = null;                //���ݻ�Ÿ��� ǥ������ �ؽ�Ʈ
    #endregion ---------------------------------------------

    private void Awake()
    {
        Instance = this;        //�̱���

        MIMmonStat = PlayerInfo.MonList[FindClass.MISelNum];  //������ ������ ���� �� MyRoom���� ������ ��ȣ�� �´� ���� ���� ��������
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < MIMmonStat.starForce; i++)         //���޿� �°� �� ������Ʈ �ѱ�
            starLineTr.GetChild(i).gameObject.SetActive(true);

        monNameText.text = MIMmonStat.monName.ToString();      //���� �̸� ǥ��

        (aTypeImg.sprite, aTypeText.text) = MIMmonStat.attackType switch  //���� ������ ����Ÿ�Կ� ���� �̹����� �ؽ�Ʈ �ٲ��ֱ�
        {
            AttackType.Physical => (aTypeSprites[(int)AttackType.Physical], "���������"),
            AttackType.Magical => (aTypeSprites[(int)AttackType.Magical], "���������"),
            _=> (null, "����")
        };

        hpText.text = MIMmonStat.hp.ToString();                 //HP ǥ��
        aDamageText.text = MIMmonStat.attackDmg.ToString();     //���ݷ� ǥ��
        defText.text = MIMmonStat.defPower.ToString();          //���� ǥ��
        mdefText.text = MIMmonStat.mdefPower.ToString();        //�������׷� ǥ��
        aSpdText.text = MIMmonStat.attackSpd.ToString();        //���ݼӵ� ǥ��
        mSpdText.text = MIMmonStat.moveSpd.ToString();          //�̵��ӵ� ǥ��
        aRangeText.text = MIMmonStat.attackRange.ToString();    //���ݻ�Ÿ� ǥ��

        Instantiate(monStore.monstersObj[(int)MIMmonStat.monName], Vector3.zero, Quaternion.Euler(0, 180, 0), transform);  //�ش� ���� ������Ʈ ����
        Instantiate(monStore.monstersSkGroup[(int)MIMmonStat.monName], skillTabTr);   //���� ���Ϳ� �´� ��ų�׷� ����
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;
        
        switch(BtnL)
        {
            case ButtonList.LeftButton:     //���� ȭ��ǥ Ŭ�� ��
                FindClass.MISelNum = SelNumCalc(FindClass.MISelNum - 1);    //���� ���õǾ��ִ� ���� ��ȣ�� -1(���� ���� ����)
                SceneManager.LoadScene(SceneList.MonInformScene.ToString());   //MonInformScene �ٽ� �ҷ�����
                break;

            case ButtonList.RightButton:    //������ ȭ��ǥ Ŭ�� ��
                FindClass.MISelNum = SelNumCalc(FindClass.MISelNum + 1);    //���� ���õǾ��ִ� ���� ��ȣ�� +1(���� ���� ����)
                SceneManager.LoadScene(SceneList.MonInformScene.ToString());   //MonInformScene �ٽ� �ҷ�����
                break;

            case ButtonList.BackButton:     //���ư��� Ŭ�� ��
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());     //MyRoomScene���� �̵�
                break;               
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    private int SelNumCalc(int SelNum)      //�������⸦ ���ϴ� ������ ��ȣ ��� �Լ�
    {
        if (SelNum < 0)     //������ ��ȣ�� ������ ������ ������ ������ ������ ��ȣ�� ����
            SelNum = PlayerInfo.MonList.Count -1;     
        else if (SelNum == PlayerInfo.MonList.Count)    //������ ��ȣ�� ������ ���� ������ ó�������� ��ȣ�� ����
            SelNum = 0;

        return SelNum;      //���� ���� ��ȣ�� ����
    }
}
