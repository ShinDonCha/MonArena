using UnityEngine;
using UnityEngine.UI;
using System;

//������ ���� ������ ǥ���� �ִ� ��ũ��Ʈ
//UserInfoPanel �����տ� �ٿ��� ���
public class UserPanelCtrl : NetworkMgr, IButtonClick
{
    [SerializeField]
    private MonStorage monStore = null;     //�̹���,������Ʈ �����

    [Header("-------- LeftBackGround --------")]
    //---------------- ���� ����
    [SerializeField]
    private Text userUIDText = null;             //������ UniqueID ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Image userCrtImage = null;          //������ ���� ĳ���� �̹���
    [SerializeField]
    private Text userLevelText = null;          //������ ���� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField]
    private Text userNickText = null;            //�г��� ǥ�ÿ� �ؽ�Ʈ
    //---------------- ���� ����

    //---------------- ������
    [SerializeField]
    private GameObject nickPanelPrefab = null;  //�г��� ���� �ǳ� ������
    //---------------- ������

    private void Awake()
    {
        userCrtImage.sprite = monStore.characterImg[PlayerInfo.UserCrtNum]; //������ ĳ���� �̹��� ǥ��  
        userUIDText.text = "UID : " + PlayerInfo.UniqueID.ToString();       //������ ����IDǥ��
        userNickText.text = "�г��� : " + PlayerInfo.UserNick;               //������ �г��� ǥ��
        userLevelText.text = "���� : " + PlayerInfo.UserLevel.ToString();    //������ ���� ǥ��
    }

    protected override void Update()
    {
        base.Update();
    }

    public void ButtonOnClick(Button PushBtn)       //Hierarchy�� ��ư ������Ʈ���� OnClick()�� ���� ����
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch(BtnL)
        {
            case ButtonList.NickChangeButton:               //�г��� ���� Ŭ�� ��
                Instantiate(nickPanelPrefab, transform);    //�г��� ���� �ǳ� ����
                break;

            case ButtonList.CancelButton:       //��ҹ�ư Ŭ�� ��
                StopAllCoroutines();            //�������� �ڷ�ƾ(NetworkMgr���� ����Ǵ� ���� �ڷ�ƾ) ��� ����
                Destroy(gameObject);            //�� ������Ʈ(���� �����ǳ�) ����
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneList.LobbyScene.ToString());   //�κ�� �ٽ� �ҷ����� (�г��� �����ϰų� �ʻ�ȭ ���������� �ٷ� �������ֱ� ����)
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    public void CrtChange(int ChrNum)     //ĳ���� �̹��� ���� �Լ�(������ CharacterImgTab�� CrtImgTabCtrl ��ũ��Ʈ���� ȣ��)
    {
        PlayerInfo.UserCrtNum = ChrNum;     //�ٲ� �̹��� ��ȣ�� ����
        PushPacket(PacketType.UserCrt);     //����� �̹�����ȣ ���� ��û
        userCrtImage.sprite = monStore.characterImg[ChrNum];    //ĳ���� �̹��� ����
    }

    public void ReqNickCheck()   //�г��� Ȯ�� �Լ�(�г��� ���� �ǳ��� NickPanelCtrl���� ����)
    {
        PushPacket(PacketType.NickName);    //�г��� Ȯ��&���� ��û
    }

    public void NickChange(string Nick)     //�г��� ���� �Լ�(NetworkMgr���� �ߺ� �г��� �˻簡 ������ ����)
    {
        PlayerInfo.UserNick = Nick;                 //�г��� �������� ����
        userNickText.text = "�г��� : " + Nick;      //���� �г��� ǥ�� ����
    }
}
