using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

//���� ������ �巡�׾� ��� �ϱ� ���� ��ũ��Ʈ
//��ũ�Ѻ� ������ Content������Ʈ�� �ٿ��� ���
//InGameScene, DefDeckScene, MultiGameScene �� ���� ��ġ�ϴ°��� Content�� ����
public class ContentCtrl : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private MonStorage monStore = null;    //MonStorage ����

    [SerializeField]
    private Transform uiCanvasTr = null;   //UICanvas ���ӿ�����Ʈ ����

    private int startSlotIndex = -1;       //�巡�׸� ������ MonSlot�� GetSiblingIndex
    private int endPointIndex = -1;        //�巡�װ� ���� Point�� GetSiblingIndex

    private GameObject dragGO = null;      //�巡�� �� ���콺�� ����ٴ� DragSlot�� ������ ����

    private bool isDrag = false;           //���� MonSlot�� �巡�������� Ȯ���ϱ����� ����
    private ScrollRect parentScroll = null;     //�θ�(ScrollRect View)�� ScrollRect ������Ʈ�� ������������ ����
    private float calcHeight = 0.0f;       //DragSlot�� ������ ���̸� ����� ��

    private void Awake()
    {
        FindClass.CurContent = this;       //���� ���� Content������Ʈ�� ContentCtrl�� ����
        parentScroll = GetComponentInParent<ScrollRect>();  //�θ��� ScrollRect ������Ʈ ��������
    }

    private void Start()
    {
        for (int i = 0; i < PlayerInfo.MonList.Count; i++)        //������ ������ ���� ����ŭ ����
            Instantiate(monStore.monSlot[PlayerInfo.MonList[i].starForce], transform).tag = MonSlotTag.Content.ToString();    //���� ���� ���� �� �±׺���              

        calcHeight = (Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0)).y * GetComponent<RectTransform>().rect.height) / 720;     //DragSlot ������ ���� ���� ���
        StartCoroutine(LoadDeck());     //����� �� �ҷ����� �ڷ�ƾ ����
    }

    private void Update()
    {
        if (!isDrag)     //���콺�� MonSlot�� �巡���ϴ� ���°� �ƴ϶��
            return;

        if (Input.mousePosition.y >= calcHeight && dragGO == null)   //���콺�� �� ������Ʈ(Content)���� ���ʿ� �ְ�, �巡�� ������ �������� �ʾҴٸ�
        {
            parentScroll.enabled = false;       //Scroll View�� ScrollRect ����(���͸� ��ġ�Ҷ� Scroll View�� �����̴°� ���� ����)
            FindClass.CurSetPoint.PColorChange(true);   //Point�� ������ �巡�� ������ �������� ����
            dragGO = Instantiate(transform.GetChild(startSlotIndex).gameObject, Input.mousePosition, Quaternion.identity, uiCanvasTr);   //���ӿ�����Ʈ ����
            dragGO.tag = MonSlotTag.Drag.ToString();    //�±� ����
        }
        else if (dragGO != null)        //�巡�� ������ ������ ���¶��
            dragGO.transform.position = Input.mousePosition;    //�巡�� ������ ���콺�� ����ٴϰ� �ϱ�

        if (Input.GetMouseButtonUp(0))       //���콺 ���ʹ�ư�� ����
        {
            isDrag = false;     //�巡�� ���� ���
            parentScroll.enabled = true;   //Scroll Rect�� �巡�� �����ϵ��� ����

            if (dragGO == null)     //�巡�� ������ ����ִ� ���¸� ���(�巡�� ������ �ȸ���� �׳� ���콺�� ������ ���� ���)
                return;

            Destroy(dragGO);    //�巡�� ���� �ı�
            FindClass.CurSetPoint.PColorChange(false);   //Point�� ������ �⺻ ������ �������� ����

            //�巡�� ���� ��ġ�� Point���
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
            {
                endPointIndex = rayHit.transform.parent.GetSiblingIndex();  //�巡�װ� ���� Point�� ��ȣ�� ��������
                EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.MonCreate); //���� ������Ʈ ���� ȿ���� ���
                FindClass.CurSetPoint.MonObjCreate(endPointIndex, transform.GetChild(startSlotIndex).GetComponent<MonSlotCtrl>());  //���� ������Ʈ ����
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;          //�巡�� �� ���·� ����
        startSlotIndex = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();    //ó�� ������ ���ͽ����� �ڽĹ�ȣ ��������
    }

    private IEnumerator LoadDeck()   //����� �� �ҷ����� �ڷ�ƾ
    {
        if (!System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneList curScene))  //���� ���� �̸��� SceneList �������� ��ȯ(���� �� �ڷ�ƾ ����)
            yield break;

        yield return new WaitForFixedUpdate();      //���� Update() ���� ����ǵ��� �ϱ�

        var (a_CurDeck, a_DeckStarF) = curScene switch  //���� ���� ���� �´� �� �ҷ�����(���� ���õ� ��, ���� ���õ� ���� ����)
        {
            SceneList.InGameScene => (PlayerInfo.CombatDeck, PlayerInfo.CombatStarF),
            SceneList.RankGameScene => (PlayerInfo.RankDeck, PlayerInfo.RankStarF),
            SceneList.DefDeckScene => (PlayerInfo.DefDeck, PlayerInfo.DefStarF),
            _ => (null, null)
        };

        if (a_CurDeck == null || a_DeckStarF == null) yield break;  //�ҷ��� ���� ������ �ڷ�ƾ ����

        //�ҷ��� ���� ��ġ�ϴ� ���� ������ ���� MonSlot ���ӿ�����Ʈ�� ��Ʈ�� ��ũ��Ʈ�� ������ ����Ʈ(���� �ڵ忡�� MonSlot ���ӿ�����Ʈ�� ������ ��ܿ��µ� ���)
        List<MonSlotCtrl> a_DeckMSCList = new();

        for (int i = 0; i < a_CurDeck.Count; i++)    //�ҷ��� ���� ��ü ���� �� ��ŭ �ݺ�
        {
            if (a_CurDeck[i].Equals(MonsterName.None))   //i�� ���� ����� �̸��� None(�������)�̶�� �Ѿ��
                continue;

            //������ ������ ��ü ���� ��Ͽ��� �̸��� ������ ��ġ�ϴ� ���� ù��° ������ Index ��������
            int FIndex = PlayerInfo.MonList.FindIndex(MonStat => MonStat.monName.Equals(a_CurDeck[i]) && MonStat.starForce.Equals(a_DeckStarF[i]));
            MonSlotCtrl a_MSC = transform.GetChild(FIndex).GetComponent<MonSlotCtrl>();    //ã�� Index�� ��ġ�ϴ� ��ġ���ִ� MonSlot���ӿ�����Ʈ�� ��Ʈ�� ��ũ��Ʈ ��������
            a_DeckMSCList.Add(a_MSC);       //����Ʈ�� �߰�
            FindClass.CurSetPoint.MonObjCreate(i, a_MSC);  //���� ���� SetPoint�� ���� ������Ʈ ������ ��û(i�� ����Ʈ ��ġ�� FIndex�� MonSlot�� ���� �Ѱ��ֱ�)
        }

        a_DeckMSCList = a_DeckMSCList.OrderByDescending((MSC) => MSC.MSCMonStat.starForce).ThenByDescending((MSC) => MSC.MSCMonStat.monName).ToList();  //����Ʈ ����

        for (int i = 0; i < a_DeckMSCList.Count; i++)
            a_DeckMSCList[i].transform.SetSiblingIndex(i);      //����Ʈ�� �̿��� MonSlot ���ӿ�����Ʈ�� ���� ����(�ҷ������� ���Ͱ� �������� ��ġ�ϵ���)
    }
}
