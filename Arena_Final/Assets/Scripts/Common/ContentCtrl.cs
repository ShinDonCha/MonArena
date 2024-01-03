using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        StartCoroutine(SaveDeckSettings());     //����� �� ��ġ �Լ� ����
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

    private IEnumerator SaveDeckSettings()   //����� �� ��ġ �Լ�
    {
        if (!System.Enum.TryParse(SceneManager.GetActiveScene().name, out SceneList curScene))  //���� ���� �̸��� SceneList �������� ��ȯ(���� �� �ڷ�ƾ ����)
            yield break;

        yield return new WaitForFixedUpdate();      //Update() ���� ����ǵ���

        List<MonsterName> a_Deck;           //����� �� ����Ʈ�� ���� ����
        List<int> a_StarF;                  //����� ���� ���� ����Ʈ�� ���� ����
        Stack<Transform> a_MonSlotStack = new();    //����� ���� ��ġ�ϴ� ���� ������ ���� MonSlot�� Transform�� ������ ����

        switch (curScene)        //�� ���� �´� �� ��������
        {
            case SceneList.InGameScene:
                a_Deck = PlayerInfo.CombatDeck;
                a_StarF = PlayerInfo.CombatStarF;
                break;

            case SceneList.RankGameScene:
                a_Deck = PlayerInfo.RankDeck;
                a_StarF = PlayerInfo.RankStarF;
                break;

            case SceneList.DefDeckScene:
                a_Deck = PlayerInfo.DefDeck;
                a_StarF = PlayerInfo.DefStarF;
                break;

            default:
                a_Deck = null;
                a_StarF = null;
                break;
        }

        if (a_Deck == null || a_StarF == null)
            yield break;

        for (int i = 0; i < a_Deck.Count; i++)   //�ҷ��� ���� ���� �� ��ŭ �ݺ�
        {
            if (a_Deck[i].Equals(MonsterName.None))     //i�� ���� ����� �̸��� None(�������)�̶�� �Ѿ��
                continue;

            for (int k = 0; k < PlayerInfo.MonList.Count; k++)    //������ ������ ��ü ���� ����ŭ �ݺ�
            {
                //i�� ���� �����̸�,������ ���� ���Ͱ� ��ü ���� ����Ʈ�� k���� �ִٸ�(���� ���Ͷ��) ex)i = 3���� k = 0�� ���͸���Ʈ                
                if (a_Deck[i].Equals(PlayerInfo.MonList[k].monName) && a_StarF[i].Equals(PlayerInfo.MonList[k].starForce))
                {
                    Transform a_MonSlotTr = transform.GetChild(k);  //������ MonSlot��(��ü ���� ����Ʈ�� ����)�� k���� MonSlot�� Transform�� ��������
                    a_MonSlotStack.Push(a_MonSlotTr);               //k�� MonSlot�� Transform�� Stack�� ����
                    FindClass.CurSetPoint.MonObjCreate(i, a_MonSlotTr.GetComponent<MonSlotCtrl>());    //�� ���� SetPoint�� ���� ������Ʈ ������ ��û(i�� ����Ʈ ��ġ�� k�� MonSlot�� ���� �Ѱ��ֱ�)
                    break;
                }
                else //i�� ���� k�� ���� ����Ʈ�� �����̸� Ȥ�� ������ �ٸ���(���� ���Ͱ� �ƴϸ�) �Ѿ��
                    continue;
            }
        }

        while (a_MonSlotStack.Count > 0)        //������ ���� Stack ����ŭ �ݺ�
            a_MonSlotStack.Pop().SetAsFirstSibling();   //MonSlot�� ���� ����        
    }
}
