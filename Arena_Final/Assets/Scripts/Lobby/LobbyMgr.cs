using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Linq;

//LobbyScene���� �ʿ��� ���۵��� ����ϴ� ��ũ��Ʈ
//LobbyScene�� LobbyMgr�� �ٿ��� ���
public class LobbyMgr : NetworkMgr, IButtonClick
{
    [SerializeField]
    private MonStorage monStore = null;             //�̹���,������Ʈ �����

    [Header("-------- ���� ���� --------")]
    [SerializeField]
    private GameObject userPanelPrefab = null;     //���������ǳ� ������
    [SerializeField]
    private Text userNickText = null;              //���� �г��� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Image userCrtImage = null;             //���� ĳ���� �̹���
    [SerializeField]
    private Text userLevelText = null;             //���� ���� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Text userGoldText = null;              //������ ���� ������ ��� ǥ�ÿ� �ؽ�Ʈ

    [Header("-------- �ڵ� ���� --------")]
    [SerializeField]
    private Text autoTText = null;                  //�ڵ� ������ �ð� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Text rewardText = null;                 //�ڵ� �������� ȹ���� �� �ִ� ��� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Image autoExpImage = null;              //�ڵ� �����ư �̹���
    [SerializeField]
    private Sprite[] goldSprite = new Sprite[4];    //�ڵ� ���� ǥ������ ��� �̹���
    [SerializeField]
    private AudioSource goldAudio = null;           //�ڵ� ���� ���� ȿ���� ����� ���� AudioSource

    private TimeSpan autoTSpan;                     //�ڵ� ������ ���� �ð��� ������ ����
    private int rewardGold = 0;                     //�ڵ� �������� ȹ���� �� �ִ� ���

    [Header("-------- ���� ���� --------")]
    [SerializeField]
    private GameObject confirmBoxPrefab = null;      //���� Ȯ�� �ǳ� ������

    private void Start()
    {
        BGMController.Instance.BGMChange(BGMList.Lobby);            //BGM ����
        userNickText.text = PlayerInfo.UserNick;                    //������ �г��� ǥ��
        userCrtImage.sprite = monStore.characterImg[PlayerInfo.UserCrtNum];   //������ ĳ���� �̹��� ǥ��
        userLevelText.text = "���� " + PlayerInfo.UserLevel.ToString();       //������ ���� ǥ��
        userGoldText.text = PlayerInfo.UserGold.ToString();         //������ ������� ǥ��
        StartCoroutine(AutoExpCheck());                             //�ڵ����� ���ǥ�� �ڷ�ƾ ����

        //������ ���͸� ������ ���� ������� ���� �� MonSterName ������� ����
        if (PlayerInfo.MonList.Count != 0)
            PlayerInfo.MonList = PlayerInfo.MonList.OrderByDescending((a) => a.starForce).ThenByDescending((a) => a.monName).ToList();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //AutoExpCheck();             //�ڵ� ���� ǥ��
        base.Update();              //��ӹ��� NetworkMgr�� Update() ����
    }

    #region -------------------------- ��ư Ŭ�� �Լ� --------------------------    
    public void ButtonOnClick(Button PushBtn)        //�κ�����ִ� ��ư Ŭ�� �� ����(��ư�� OnClick���� ����)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {            
            case ButtonList.UserInfoButton:     //�������� ��ư
                Instantiate(userPanelPrefab, PushBtn.transform.parent);     //���� �����ǳ� ����
                break;

            case ButtonList.CombatButton:       //���� ��ư
                SceneManager.LoadScene(SceneList.StageScene.ToString());
                break;

            case ButtonList.ShopButton:         //���� ��ư
                SceneManager.LoadScene(SceneList.ShopScene.ToString());
                break;

            case ButtonList.MyRoomButton:       //���̷� ��ư
                SceneManager.LoadScene(SceneList.MyRoomScene.ToString());
                break;

            case ButtonList.RankLobbyButton:      //��ũ�κ� ��ư
                SceneManager.LoadScene(SceneList.RankLobbyScene.ToString());
                break;

            case ButtonList.ExitButton:         //���� ���� ��ư
                Instantiate(confirmBoxPrefab, PushBtn.transform.parent.parent);    //ConfirmBox ����
                break;

            case ButtonList.AutoExpButton:      //���� �ޱ� ��ư
                PlayerInfo.UserGold += rewardGold;          //���� ����
                userGoldText.text = PlayerInfo.UserGold.ToString();     //���� ��� ǥ��
                autoTSpan = TimeSpan.Zero;                  //���ð� �ʱ�ȭ
                PlayerInfo.AutoExpTime = DateTime.Now;      //���� �ð� ����                
                PushPacket(PacketType.UserGold);            //��� �����û
                PushPacket(PacketType.AutoTime);            //�ڵ� Ž�� �ð� �����û
                break;

            case ButtonList.MultiButton:     //��Ƽ ��ư
                SceneManager.LoadScene(SceneList.MultiLobbyScene.ToString());         
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }
    #endregion -------------------------- ��ư Ŭ�� �Լ� --------------------------

    #region -------------------------- �ڵ� ���� üũ �Լ� --------------------------
    private IEnumerator AutoExpCheck()    //�ڵ� ���� ��� ǥ�� �Լ�
    {
        while(true)
        {
            autoTSpan = DateTime.Now - PlayerInfo.AutoExpTime;  //�ڵ����� �ð����
            int a_Hours;        //�ڵ������� �ð��� ���� ����
            int a_Minutes;      //�ڵ������� ���� ���� ����
            int a_Seconds;      //�ڵ������� �ʸ� ���� ����

            if (autoTSpan.Days < 1)        //�ڵ����� �ð��� 24�ð� �̸��̶��
            {
                a_Hours = autoTSpan.Hours;
                a_Minutes = autoTSpan.Minutes;
                a_Seconds = autoTSpan.Seconds;
            }
            else //(autoTSpan.Days >= 1)   //�ڵ����� �ð��� 24�̰� �̻��̶��
            {
                a_Hours = 24;              //�ִ� 24�ð� ����
                a_Minutes = 0;
                a_Seconds = 0;
            }

            autoExpImage.sprite = goldSprite[a_Hours / 8];    //�ڵ����� �ð��� ���� ��� �̹��� ����(��� �̹����� 8�ð� ������ �ٲ�Ƿ� ���� �̿��� �̹��� ��ȣ ����)
            autoTText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", a_Hours, a_Minutes, a_Seconds);          //�ڵ����� �ð�ǥ��
            rewardGold = ((a_Hours * 360) + (a_Minutes * 6) + (a_Seconds / 10)) * PlayerInfo.CombatStage;   //�ڵ� �������� ȹ���� �� �ִ� ��� ���(10�ʸ��� ��� �߰�)
            rewardText.text = rewardGold.ToString();          //�ڵ� �������� ȹ���� �� �ִ� ��� ǥ��

            if (a_Hours != 24 && (a_Seconds % 10) == 0)     //��尡 �߰��Ǵ½ð�(10��)���� ȿ���� ���
                goldAudio.Play();

            yield return new WaitForSeconds((1000 - (float)autoTSpan.Milliseconds) / 1000);     //���� �ð����� ���
        }
    }
    #endregion -------------------------- �ڵ� ���� üũ �Լ� --------------------------

    private void OnDestroy()
    {
        StopCoroutine(AutoExpCheck());      //�ڵ� ������ ǥ�� �ڷ�ƾ ����
    }

}
