using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

//���͵��� ��ġ�ϴ� ���� SetPoints������Ʈ���� �߻��ϴ� �巡�� �� ����� ���� ���������� ��Ʈ�� �ϱ� ���� ��ũ��Ʈ
//�̹� �� Point�� ��ġ�Ǿ��ִ� ���͵��� ��ġ ����ϰų� �ٸ� Point�� �ű�� ������ �Ѵ�.
//InGameScene�� DefDeckScene �� ���͸� ��ġ�ϴ� ���� SetPoints ���ӿ�����Ʈ�� �ٿ� ���
public class SetPointCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("-------- Common --------")]
    [SerializeField]
    private MonStorage monStore = null;      //MonStorage ����

    [SerializeField]
    private Transform uiCanvasTr = null;     //UICanvas ���ӿ�����Ʈ ����

    [SerializeField]
    private GameObject errorText = null;     //ErrorText ���ӿ�����Ʈ ����
    private readonly float errorTime = 2.0f; //ErrorText ��� �ð�

    [SerializeField]
    private Material pointMtrl = null;      //Point�� ���Ǵ� Material
    private Color32 defaultPColor = new(0, 0, 0, 255);   //Point�� �⺻ ����
    private Color32 dragPColor = new(0, 0, 255, 255);    //�巡�� ���¿��� Point�� ����

    private int startPointIndex = -1;       //�巡�׸� ������ Point�� GetSiblingIndex
    private int endPointIndex = -1;         //�巡�װ� ���� Point�� GetSiblingIndex

    private GameObject dragGO = null;       //�巡�� �� ���콺�� ����ٴ� DragSlot ���ӿ�����Ʈ�� ������ ����

    private MonSlotCtrl[] MSCIndex;         //���� Point���� � ��ȣ�� MonSlotCtrl�� ����Ǿ��ִ��� �˱� ���� �迭
    public MonSlotCtrl GetPointMSC(int PointNum) {  return MSCIndex[PointNum]; }   //�ܺο��� Point�� ����Ǿ��ִ� MonSlotCtrl�� �������� ���� �Լ�
    
    private Transform[] PointsTr;          //SetPoints ������ Point���� transform
    public Transform this[int index] { get { return PointsTr[index]; } }      //���� Point���� transform�� �������� ���� �ε���    

    // Start is called before the first frame update
    private void Awake()
    {
        if(errorText != null)
            errorText.SetActive(false);     //���� �ؽ�Ʈ ���ӿ�����Ʈ ����

        PointsTr = new Transform[transform.childCount];    //Point ����ŭ �迭 ����
        for (int i = 0; i < PointsTr.Length; i++)          //Point�� ������ �°� transform ����
            PointsTr[i] = transform.GetChild(i);

        MSCIndex = new MonSlotCtrl[transform.childCount];   //Point ����ŭ �迭 ����        
        Array.Clear(MSCIndex, 0, MSCIndex.Length);          //�迭 �ʱ�ȭ

        FindClass.CurSetPoint = this;      //�ܺο��� �� ��ũ��Ʈ�� ã������ ���������� ���
    }

    #region -------------------- �巡�� �� ��� --------------------
    public void OnBeginDrag(PointerEventData eventData)     //�巡�� ���� �� �� ����
    {
        //������ �� ���س��� Layer�� ��ġ�ϴ� ������Ʈ ã��(Point). SetPoint�� �����ϵ��� layerMask ����
        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
            startPointIndex = rayHit.transform.parent.GetSiblingIndex();       //���(Point)�� ��� �ڽ����� ��������
                
        if (MSCIndex[startPointIndex] == null)  //������ ���� Point�� ���Ͱ� ��ġ�Ǿ����� ������ ���
            return;

        PColorChange(true);     //Point���󺯰� �Լ� ����
        dragGO = Instantiate(MSCIndex[startPointIndex].gameObject, Input.mousePosition, Quaternion.identity, uiCanvasTr);   //�巡�� ���� ������Ʈ ����
        dragGO.tag = MonSlotTag.Drag.ToString();                     //�±� ����

        Destroy(PointsTr[startPointIndex].GetChild(1).gameObject);   //������ Point�� �ִ� ���� ������Ʈ ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragGO == null)      //OnBeginDrag�� ��ҵƴٸ�
            return;

        dragGO.transform.position = Input.mousePosition;    //�巡�� ������ ���콺�� ����ٴϰ� �ϱ�
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGO == null)      //OnBeginDrag�� ��ҵƴٸ�
            return;

        //���� ��ġ Point���� ����� ��������
        if (Physics.Raycast(Camera.main.ScreenPointToRay(eventData.position), out RaycastHit rayHit, Mathf.Infinity, 1 << (int)LayerName.SetPoint))
        {
            EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.MonCreate); //���� ������Ʈ ���� ȿ���� ���
            endPointIndex = rayHit.transform.parent.GetSiblingIndex();  //�巡�װ� ���� Point�� ��ȣ�� ��������
            MonSlotCtrl a_StartMSC = MSCIndex[startPointIndex];         //�巡�� ���� Point�� ����Ǿ��ִ� MonSlotCtrl �ӽ�����
            MonSlotCtrl a_EndMSC = MSCIndex[endPointIndex];             //�巡�� ���� Point�� ����Ǿ��ִ� MonSlotCtrl �ӽ�����
            MSCIndex[startPointIndex] = null;                           //�巡�� ���� Point�� ����Ǿ��ִ� MonSlotCtrl �ʱ�ȭ
            MSCIndex[endPointIndex] = null;                             //�巡�� ���� Point�� ����Ǿ��ִ� MonSlotCtrl �ʱ�ȭ
            MonObjCreate(startPointIndex, a_EndMSC);                    //�巡�� ���� Point�� ����� MonSlotCtrl�� �ٲ��ְ� ���� ����
            MonObjCreate(endPointIndex, a_StartMSC);                    //�巡�� ���� Point�� ����� MonSlotCtrl�� �ٲ��ְ� ���� ����
        }
        else    //��ġPoint�� �ƴ� �ٸ����� �������� (��ġ ���)
        {
            MSCIndex[startPointIndex].UsingSet(false);    //���� Point�� ����Ǿ��ִ� MonSlot�� ������� �ƴ� ���·� ����
            MSCIndex[startPointIndex] = null;             //���� Point�� ����� MonSlotCtrl�� ������ ����
        }

        Destroy(dragGO);         //�巡�� ���� �ı�
        PColorChange(false);     //Point���󺯰� �Լ� ����
    }
    #endregion -------------------- �巡�� �� ��� --------------------

    #region ------------------- ���� ������Ʈ ���� -------------------    
    public void MonObjCreate(int PointNum, MonSlotCtrl SlotCtrl)  //(���͸� ������ Point��ȣ, ������������ MonSlot��ȣ)
    {
        if (SlotCtrl == null)   //���� ���Ͱ� ���� Point���� ������ ���(�Ű����� SlotCtrl�� Null) ���� ���� ����
            return;

        //-------------- ���� ���� ��ġ ���� & ���� ���͸� ���� �ڸ��� ���� ��� ��ü
        for (int i = 0; i < MSCIndex.Length; i++)    //Point�� ����Ǿ��ִ� ��� MonSlotCtrl�� ���� ����
        {            
            if (MSCIndex[i] == null)        //��ü ��� �� ����ִ� ���� �Ѿ��
                continue;
            else if (MSCIndex[i].MSCMonStat.monName.Equals(SlotCtrl.MSCMonStat.monName))    //��ü ��� �� ������� ���� ���� ��ġ�� ���� �̸��� ��û���� ������ �̸��� ���� ��(���� ���͸� ��ġ��û)
                if (i.Equals(PointNum))     //������ Point�� ���Ͱ� ��ġ�� Point�� ���� ���̶�� ��ġ�ϱ�(���� �̸��� ���Ͱ� �ִ� ���̸� ��ü�ϵ��� �ϱ� ����)
                    break;
                else  //������ Point�� ���Ͱ� ��ġ�� Point�� ���� ���̾ƴ϶�� �����߻�
                {
                    StartCoroutine(ErrorOnOff());       //���� �ؽ�Ʈ ��� �ڷ�ƾ ����(�̹� ���� ���Ͱ� �����մϴ�.)
                    EffSoundCtrl.Instance.EffSoundPlay(EffSoundList.fail);      //���� ȿ���� ���
                    return;     //���� ��ġ ���
                }
        }
        //-------------- ���� ���� ��ġ ���� & ���� ���͸� ���� �ڸ��� ���� ��� ��ü

        //------------ ���� ������Ʈ ����
        if (PointsTr[PointNum].childCount > 1)      //�ش� ��ġ�� �̹� ���Ͱ� �����ϸ�
        {
            Destroy(PointsTr[PointNum].GetChild(1).gameObject); //�����ϴ� ���� ����

            if (MSCIndex[PointNum] != null)         //������ ��ġ�Ǿ��ִ� ������ ������ ���� MonSlot ��밡�� ���·� ����
                MSCIndex[PointNum].UsingSet(false); 
        }

        MSCIndex[PointNum] = SlotCtrl;        //�ش� Point�� MonSlotCtrl ����

        if(SceneManager.GetActiveScene().name.Contains(SceneList.MultiGameScene.ToString()))      //��Ƽ�� ���
            Instantiate(monStore.monstersObj[(int)SlotCtrl.MSCMonStat.monName], PointsTr[PointNum]);     //���� ����
        else    //�̱��� ���
            Instantiate(monStore.monstersObj[(int)SlotCtrl.MSCMonStat.monName], PointsTr[PointNum]).tag = Team.Ally.ToString();     //���� ����

        SlotCtrl.UsingSet(true);        //���� ��ġ�ϴ� ������ ������ ���� MonSlot �����·� ����
        //------------ ���� ������Ʈ ����
    }
    #endregion ------------------- ���� ������Ʈ ���� -------------------

    #region ----------------- ���� ����, ���� ��� -----------------
    public void PColorChange(bool Dragging)     //���� Point���� ���󺯰� �Լ�
    {
        pointMtrl.color = Dragging ? dragPColor : defaultPColor;    //�巡�� ���¿� ���� Point�� ���� ����
    }

    private IEnumerator ErrorOnOff()        //���� �ؽ�Ʈ ����� ���� �ڷ�ƾ
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(errorTime);
        errorText.SetActive(false);
    }
    #endregion ----------------- ���� ����, ���� ��� -----------------

    #region -------------------------- ��Ƽ --------------------------
    //������ ���۵� �� MtGameMgr���� SetPoint�� ������Ű�µ� �׶� ����ȴ�. �� SetPoint��� ������ CombatSetPoint�� MtCbtSetPoint ��ũ��Ʈ���� ������ ������ ����
    private void OnDisable()
    {
        if (!SceneManager.GetActiveScene().name.Contains(SceneList.MultiGameScene.ToString()))        //��Ƽ�� ����
            return;

        int[] a_MonArrName = new int[PointsTr.Length];     //���� ��ġ�� ���͵��� �̸� �迭. Point���� ������Ʈ�� ���ڸ�ŭ ����(6��)
        int[] a_MonArrStar = new int[PointsTr.Length];     //���� ��ġ�� ���͵��� ���� �迭. Point���� ������Ʈ�� ���ڸ�ŭ ����(6��)

        for (int i = 0; i < PointsTr.Length; i++)
        {
            if (PointsTr[i].childCount > 1)     //���Ͱ� ��ġ�� Point
            {
                MonsterStat a_MonStat = MSCIndex[i].MSCMonStat;       //�� Point�� ����Ǿ��ִ� MonSlotCtrl���� MonStat�� �޾ƿ���
                a_MonArrName[i] = (int)a_MonStat.monName;             //������ �̸��� �� Point�� ��ȣ�� �°� int�������� ����
                a_MonArrStar[i] = a_MonStat.starForce;                //������ ������ �� Point�� ��ȣ�� �°� ����
                Destroy(PointsTr[i].GetChild(1).gameObject);          //���� ������Ʈ ����
            }
            else//(Tr.childCount <= 1)  //���Ͱ� ��ġ���� ���� Point
            {
                a_MonArrName[i] = -1;
                a_MonArrStar[i] = -1;
            }
        }
        Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(new() { { "MonArrName", a_MonArrName }, { "MonArrStar", a_MonArrStar } });     //���� �÷��̾��� ���� �߰�
    }
    #endregion -------------------------- ��Ƽ --------------------------

    private void OnDestroy()
    {
        StopAllCoroutines();        //�ı��� �� ��� �ڷ�ƾ ����
    }
}
