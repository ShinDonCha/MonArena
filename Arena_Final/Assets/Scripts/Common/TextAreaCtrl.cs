using UnityEngine.UI;
using UnityEngine;
using System;
using Photon.Pun;

//����&�����, ����� �� ���� �� ���Ͱ� �޴� ȿ���� �ؽ�Ʈ�� ����ϱ����� ��ũ��Ʈ
//�� ��ũ��Ʈ�� �ִ� �Լ��� ���� Ŭ������ TextRequest Ŭ������ �׼��� ���� �����Ѵ�.(����� ��ϸ� ���ִ� ��)
//����ȭ���� �ִ� �� ���� CombatUI -> TextArea ���ӿ�����Ʈ�� �ٿ��� ���
public class TextAreaCtrl : MonoBehaviour
{
    [Header("----------- �̱� -----------")]
    //-------------- �̱�(GameMgr)������ ���
    [SerializeField]
    private Transform playerTabTr = null;       //���� Player ���ӿ�����Ʈ�� Transform(���� ���Ͱ� �޴� ȿ�� �ؽ�Ʈ ���)
    [SerializeField]
    private Transform enemyTabTr = null;        //���� Enemy ���ӿ�����Ʈ�� Transform(�� ���Ͱ� �޴� ȿ�� �ؽ�Ʈ ���)
    //-------------- �̱�(GameMgr)������ ���

    [Header("----------- ��Ƽ -----------")]
    //-------------- ��Ƽ(MtGameMgr)������ ���
    [SerializeField]
    private Transform team1TabTr = null;        //���� Team1 ���ӿ�����Ʈ�� Transform(��Ƽ���� Team1�� ���Ͱ� �޴� ȿ�� �ؽ�Ʈ ���)
    [SerializeField]
    private Transform team2TabTr = null;        //���� Team2 ���ӿ�����Ʈ�� Transform(��Ƽ���� Team2�� ���Ͱ� �޴� ȿ�� �ؽ�Ʈ ���)
    //-------------- ��Ƽ(MtGameMgr)������ ���

    [Header("----------- ���� -----------")]
    [SerializeField]
    private GameObject dmgTextPrefab = null;    //����� �ؽ�Ʈ�� ������ִ� ������
    [SerializeField]
    private GameObject healTextPrefab = null;   //�� �ؽ�Ʈ�� ������ִ� ������

    private PhotonView PhotonV = null;          //��Ƽ���� �ʿ��� Photon View�� ���� ����

    private void Awake()
    {
        PhotonV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TextRequest.BuffTxtReqAct = BuffTextReq;        //����&����� �ؽ�Ʈ ��� �׼ǿ� �Լ� �߰�
        TextRequest.InstantTxtReqAct = HealTextReq;     //��&����� �ؽ�Ʈ ��� �׼ǿ� �Լ� �߰�
    }

    #region ----------------- ��&����� �ؽ�Ʈ ��û �Լ� -----------------
    //��&����� �ؽ�Ʈ ��û �Լ�
    private void HealTextReq(Vector3 ObjPos, TxtAnimList AnimList, int Value)     //1ȸ�� �ؽ�Ʈ ��û �Լ�
    {
        if (PhotonV == null)        //�̱�
            HealTextInstant(ObjPos, AnimList, Value);       //1ȸ�� �ؽ�Ʈ ���� �Լ� ����
        else//(PhotonV != null)     //��Ƽ
            PhotonV.RPC("HealTextInstant", RpcTarget.All, ObjPos, AnimList, Value);  //���ð� ���ݿ� 1ȸ�� �ؽ�Ʈ ���� �Լ� ����
    }

    [PunRPC]
    private void HealTextInstant(Vector3 ObjPos, TxtAnimList AnimList, int Value)       //1ȸ�� �ؽ�Ʈ ���� �Լ�
    {
        Vector2 a_PosVtr2 = Camera.main.WorldToScreenPoint(new Vector3(ObjPos.x, ObjPos.y + 1.8f, ObjPos.z));   //�ؽ�Ʈ�� ������ World���� ��ġ

        GameObject a_GO;    //�ؽ�Ʈ ������Ʈ�� ���� ����

        switch (AnimList)
        {
            case TxtAnimList.DmgTxtAnim:
                if (dmgTextPrefab == null)    //����� �ؽ�Ʈ ��� �������� ������� �ʾ����� ���(���� ����)
                    return;

                a_GO = Instantiate(dmgTextPrefab, a_PosVtr2, Quaternion.identity, transform);      //����� �ؽ�Ʈ ����
                a_GO.GetComponentInChildren<Text>().text = Value + " Dmg";  //����� ǥ�� ����
                Destroy(a_GO, 0.5f);        //�����ð� �ڿ� �ؽ�Ʈ ������Ʈ ����
                break;

            case TxtAnimList.HealTxtAnim:
                if (healTextPrefab == null)   //�� �ؽ�Ʈ ��� �������� ������� �ʾ����� ���(���� ����)
                    return;

                a_GO = Instantiate(healTextPrefab, a_PosVtr2, Quaternion.identity, transform);      //�� �ؽ�Ʈ ����
                a_GO.GetComponentInChildren<Text>().text = "+ " + Value + " HP";  //���� ǥ�� ����
                Destroy(a_GO, 0.8f);        //�����ð� �ڿ� �ؽ�Ʈ ������Ʈ ����
                break;
        }
    }
    #endregion ----------------- ��&����� �ؽ�Ʈ ��û �Լ� -----------------

    #region ----------------- ����&����� �ؽ�Ʈ ��û �Լ� -----------------
    //����&����� �ؽ�Ʈ ��û �Լ�
    private void BuffTextReq(string TeamTag, int SiblingIndex, Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)      //(���±�, ���° �ڽ�����, ���� ������Ʈ�� ��ġ, ����or�����, ���º�ȭ ����)
    {
        if (!Enum.TryParse(TeamTag, out Team TeamName))
            return;

        if (PhotonV == null)        //�̱�
            BuffTextPlay(TeamName, SiblingIndex, ObjPos, AnimList, Act);    //����&����� �ؽ�Ʈ ���� �Լ� ����
        else//(PhotonV != null)     //��Ƽ
            PhotonV.RPC("BuffTextPlay", RpcTarget.All, TeamName, SiblingIndex, ObjPos, AnimList, Act);  //���ð� ���ݿ� ����&����� �ؽ�Ʈ ���� �Լ� ����
    }

    [PunRPC]
    private void BuffTextPlay(Team TeamName, int SiblingIndex, Vector3 ObjPos, TxtAnimList AnimList, TakeAct Act)      //����&����� �ؽ�Ʈ ���� �Լ�
    {
        Transform a_ReqTr = null;

        switch (TeamName)
        {
            case Team.Ally:
                a_ReqTr = playerTabTr;
                break;

            case Team.Enemy:
                a_ReqTr = enemyTabTr;
                break;

            case Team.Team1:
                a_ReqTr = team1TabTr;
                break;

            case Team.Team2:
                a_ReqTr = team2TabTr;
                break;
        }

        //switch�� ���� ���� ���� ������Ʈ ������ BuffTextRoot�� BuffTextRootCtrl�� ������ �ؽ�Ʈ ���� �Լ� ����
        a_ReqTr.GetChild(SiblingIndex).GetComponent<BuffTextRootCtrl>().AnimPlay(ObjPos, AnimList, Act);
    }
    #endregion ----------------- ����&����� �ؽ�Ʈ ��û �Լ� -----------------
}
