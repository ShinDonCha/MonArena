using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//���� ��ȯ�� ���õ� ������ �ϴ� ��ũ��Ʈ
//ShopScene�� ShopMgr�� �ٿ��� ���
public class ShopMgr : NetworkMgr, IButtonClick
{
    [Header("----------- ����� -----------")]
    [SerializeField]
    private MonStorage monStore = null;     //�̹���, ������Ʈ �����

    [Header("----------- UI -----------")]
    [SerializeField]
    private Canvas mainCanvas = null;           //UI�� �̹������� ����ִ� �� ���� Canvas�� ���� ����
    [SerializeField]
    private GameObject errorText = null;        //���� �ؽ�Ʈ ���ӿ�����Ʈ�� ���� ����
    private readonly float errorTime = 2.0f;    //���� �ؽ�Ʈ ǥ������ �ð�
    [SerializeField]
    private Animation[] ObjsAnimArray;          //��濡 �ִ� ������Ʈ���� �ִϸ��̼� ������Ʈ�� ���� ����
    private readonly float animTime = 5.0f;     //�ִϸ��̼� ������� �ɸ��� �ð�
    [SerializeField]
    private GameObject uiTab = null;            //UITab ���ӿ�����Ʈ�� ���� ���� (�ִϸ��̼� ����� �� UI���� �����ֱ� ����)
    [SerializeField]
    private Text userGoldText = null;           //������ ������ ��带 ǥ�����ֱ����� �ؽ�Ʈ
    [SerializeField]
    private GameObject resultPanel = null;      //ResultPanel ���ӿ�����Ʈ�� ���� ����
    [SerializeField]
    private Transform monSlotTabTr = null;      //��ȯ�� ���� �̹����� ���� ���� Transform (ResultPanel�� ����������Ʈ)

    private readonly int summonCost = 300;      //1ȸ ��ȯ���
    private readonly Queue<int> summonQueue = new();   //��ȯ����� ���� ���� ����� ���� ť

    private void Start()
    {
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;       //Canvas�� �ʹ� ����
        uiTab.SetActive(true);        //ó���� ��ư �ǳ� �ѱ�
        resultPanel.SetActive(false); //ó���� ��� �ǳ� ���ֱ�
        errorText.SetActive(false);   //�������� ���ֱ�
        userGoldText.text = PlayerInfo.UserGold.ToString();        //������ ���� ��� ǥ���ϱ�
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();     //NetworkMgr�� Update() ����
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���

        switch (BtnL)
        {
            case ButtonList.OnceSumButton:      //1ȸ��ȯ ��ư Ŭ�� ��
                StartCoroutine(Summon(1));                
                break;

            case ButtonList.FiveTSumButton:     //5ȸ��ȯ ��ư Ŭ�� ��
                StartCoroutine(Summon(5));
                break;

            case ButtonList.BackButton:         //���ư��� ��ư Ŭ�� ��
                SceneManager.LoadScene(SceneList.LobbyScene.ToString());        //�κ������ �̵�
                break;
        }
    }

    #region ------------------------ ��ȯ ------------------------
    private IEnumerator Summon(int Count)      //��ȯ(Ƚ��)
    {
        //------------------ ��� ��� ------------------
        if (PlayerInfo.UserGold < summonCost * Count)        //��尡 ������� �ʴٸ�
        {
            StartCoroutine(ErrorTextCo());      //�������� ��� �ڷ�ƾ ����
            yield break;
        }

        uiTab.SetActive(false);                             //��ư���� ����ִ� ���ӿ�����Ʈ ������ ����
        PlayerInfo.UserGold -= summonCost * Count;          //���� ��� �Ҹ�
        userGoldText.text = PlayerInfo.UserGold.ToString(); //������ ������� ǥ�� ����
        PushPacket(PacketType.UserGold);                    //��� �����û
        //------------------ ��� ��� ------------------

        BGMController.Instance.BGMChange(BGMList.Stop);         //����� ��� ����
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.Shop);  //��ȯ ȿ���� ���

        //------------------- ��ȯ -------------------
        for (int i = 0; i < Count; i++)     //��û�� ��ȯ Ƚ����ŭ ����
        {
            int a_SummonNum = UnityEngine.Random.Range(0, (int)MonsterName.MonsterCount);   //�������� ���� ��ȣ �̱�
            PlayerInfo.MonList.Add(MonsterData.MonDic[(MonsterName)a_SummonNum][0]);        //��ȯ�� ���� ��ȣ�� �´� ���� ������ ��ųʸ��κ��� ��������(�⺻ 0��)
            summonQueue.Enqueue(a_SummonNum);     //��ȯ�� ���� ť�� ����(ResultPanel�� ���ǥ�� �Ҷ� ���)
        }
        PushPacket(PacketType.MonList);           //���� ����Ʈ �����û
        Camera.main.fieldOfView = 38;             //����ī�޶��� �ɼ� ����

        foreach (Animation Anim in ObjsAnimArray) //��Ͽ��ִ� �ִϸ��̼� ����
        {
            if (Anim.isPlaying)     //�������� �ִϸ��̼� ���(���� �ִϸ��̼� �ߺ����� ����)
                Anim.Stop();

            Anim.Play();
        }
        //------------------- ��ȯ -------------------

        yield return new WaitForSeconds(animTime);      //�ִϸ��̼� ���� �� ����� �����ޱ�

        //------------------- ��� ǥ�� -------------------
        resultPanel.SetActive(true);        //ResultPanel �ѱ�
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;  //Canvas�� ���� ����

        while(summonQueue.Count > 0)        //��ȯ�� ���� �� ��ŭ ����
        {
            GameObject a_GO = Instantiate(monStore.monSlot[0], monSlotTabTr);   //���� �̹��� ����
            a_GO.name = summonQueue.Dequeue().ToString();                       //��ȯ ��Ͽ��� ���� ��������
            a_GO.tag = MonSlotTag.NameParse.ToString();                         //�±� ����
        }
        //------------------- ��� ǥ�� -------------------
    }

    private IEnumerator ErrorTextCo()  //�������� ��� �ڷ�ƾ
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }

    public void ResultOnClick()     //ResultPanel Ŭ���� �����ϴ� �Լ�(Ȱ��ȭ�� ResultPanel ���ӿ�����Ʈ�� OnClick()���� ����)
    {
        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);   //��ư Ŭ�� ȿ���� ���
        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;   //Canvas�� ���� ����
        resultPanel.SetActive(false);       //ResultPanel ����
        uiTab.SetActive(true);              //UI�� �ѱ�
        for (int i = 0; i < monSlotTabTr.childCount; i++)      //��ȯ�� ���� �̹��� ���� ����
            Destroy(monSlotTabTr.GetChild(i).gameObject);
        BGMController.Instance.BGMChange(BGMList.Lobby);       //����� ����
    }
    #endregion ------------------------ ��ȯ ------------------------
}
