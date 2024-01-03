using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//��Ŀ�� ������ ǥ�����ֱ����� ��ũ��Ʈ
//RankListPrefab�� �ٿ��� ���
public class RankListCtrl : MonoBehaviour, IButtonClick, ILoadScene
{
    [SerializeField]
    private MonStorage monStore;        //�̹���, ������Ʈ �����
    [SerializeField]
    private GameObject loadingPanel = null;     //�ε� �ǳ� ������
    [SerializeField]
    private Image crtImage = null;      //�ش� ��Ŀ�� ĳ���� �̹����� ǥ���� �̹���
    [SerializeField]
    private Text nickText = null;       //�ش� ��Ŀ�� �г����� ǥ���� �ؽ�Ʈ
    [SerializeField]
    private Text rankText = null;       //�ش� ��Ŀ�� ��ŷ�� ǥ���� �ؽ�Ʈ
    [SerializeField]
    private Transform monInfoTr = null; //�ش� ��Ŀ�� ��ġ���� ����� ������ Transform

    private string rankNick;            //��Ŀ�� �г����� �ӽ� �����ϱ� ���� ����
    private MonsterName[] rankDNameList;//��Ŀ�� �� ����Ʈ�� �ӽ� �����ϱ� ���� ����
    private int[] rankDStarList;        //��Ŀ�� �� ���� ����Ʈ�� �ӽ� �����ϱ� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        rankNick = RankLobbyMgr.Instance.GetNickList(transform.GetSiblingIndex());          //RankLobbyMgr�� ����Ǿ��ִ� ��Ŀ���� �г��� ���� �� �� ������Ʈ�� ������ �´� ���� ��������
        rankDNameList = RankLobbyMgr.Instance.GetDNameList(transform.GetSiblingIndex());    //RankLobbyMgr�� ����Ǿ��ִ� ��Ŀ���� �� ���� �� �� ������Ʈ�� ������ �´� ���� ��������
        rankDStarList = RankLobbyMgr.Instance.GetDStarList(transform.GetSiblingIndex());    //RankLobbyMgr�� ����Ǿ��ִ� ��Ŀ���� ���� ���� �� �� ������Ʈ�� ������ �´� ���� ��������

        crtImage.sprite = monStore.characterImg[RankLobbyMgr.Instance.GetCrtList(transform.GetSiblingIndex())]; //ĳ���� �̹��� ����
        rankText.text = "���� : " + RankLobbyMgr.Instance.GetRankList(transform.GetSiblingIndex());   //��ŷ ǥ��
        nickText.text = rankNick;   //�г��� ǥ��

        for (int i = 0; i < rankDNameList.Length; i++)      //���� ���� ��� ����
        {
            if (rankDNameList[i].Equals(MonsterName.None))
                continue;

            GameObject a_GO = Instantiate(monStore.monSlot[rankDStarList[i]], monInfoTr);
            a_GO.tag = MonSlotTag.NameParse.ToString();
            a_GO.name = ((int)rankDNameList[i]).ToString();
        }
    }

    public void ButtonOnClick(Button PushBtn)
    {
        if (!Enum.TryParse(PushBtn.name, out ButtonList BtnL))
            return;

        switch (BtnL)
        {
            case ButtonList.CombatButton:       //���� ��ư Ŭ�� �� �ش� ��Ŀ�� ���� ���������� ����
                FindClass.RankNick = rankNick;
                FindClass.RankDName = rankDNameList;
                FindClass.RankDStar = rankDStarList;
                LoadScene(SceneList.RankGameScene);
                break;
        }

        EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.ButtonClick);       //��ư Ŭ�� ȿ���� ���
    }

    #region --------------------- �� �ҷ����� �Լ� ---------------------
    public void LoadScene(SceneList NextScene)
    {
        FindClass.LoadSceneName = NextScene;
        Instantiate(loadingPanel);
    }
    #endregion --------------------- �� �ҷ����� �Լ� ---------------------
}
