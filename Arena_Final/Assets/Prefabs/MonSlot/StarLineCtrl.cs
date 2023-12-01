using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonSlot�� ������ ��ȭ�� ���� ��ũ�� ������ ���� ��ũ��Ʈ
//MonSlot�� StarImageLine�� �ٿ��� ���
public class StarLineCtrl : MonoBehaviour
{
    private GridLayoutGroup parentGridLayout = null;    //�ֻ��� �θ��� GridLayoutGroup ������Ʈ�� ���� ����
    private GridLayoutGroup thisGridLayout = null;      //�� ������Ʈ�� GridLayoutGroup ������Ʈ�� ���� ����
    private Vector2 parentWH = new(110, 120);           //�� ũ���� ������ �Ǵ� �θ��� �⺻ ������

    private void Awake()
    {
        parentGridLayout = transform.parent.GetComponentInParent<GridLayoutGroup>();     //�ֻ��� �θ��� GridLayoutGroup ��������
        thisGridLayout = GetComponent<GridLayoutGroup>();       //�� ������Ʈ�� GridLayoutGroup ��������
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 a_PGLCellSize;    //�θ�(�ֻ��� �θ�)�� CellSize�� ���� ����

        if (parentGridLayout == null)       //�ֻ��� �θ� ���� ���
            a_PGLCellSize = transform.parent.GetComponent<RectTransform>().sizeDelta;  //�θ��� Width, Height�� ��������
        else
            a_PGLCellSize = parentGridLayout.cellSize;        //�ֻ��� �θ��� GridLayoutGroup�� ������ CellSize ��������
                
        Vector2 a_SizeRatio = new (a_PGLCellSize.x / parentWH.x, a_PGLCellSize.y / parentWH.y);    //���� �θ��� ũ��� �⺻ũ�⸦ ���� �ٲ� ���� ����

        if(thisGridLayout != null)
            thisGridLayout.cellSize *= a_SizeRatio;         //�ڽ��� CellSize�� ������ ���� ����
    }
}
