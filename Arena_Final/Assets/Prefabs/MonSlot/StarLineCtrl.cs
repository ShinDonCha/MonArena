using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//MonSlot의 사이즈 변화에 따른 별크기 조절을 위한 스크립트
//MonSlot의 StarImageLine에 붙여서 사용
public class StarLineCtrl : MonoBehaviour
{
    private GridLayoutGroup parentGridLayout = null;    //최상위 부모의 GridLayoutGroup 컴포넌트를 담을 변수
    private GridLayoutGroup thisGridLayout = null;      //이 오브젝트의 GridLayoutGroup 컴포넌트를 담을 변수
    private Vector2 parentWH = new(110, 120);           //별 크기의 기준이 되는 부모의 기본 사이즈

    private void Awake()
    {
        parentGridLayout = transform.parent.GetComponentInParent<GridLayoutGroup>();     //최상위 부모의 GridLayoutGroup 가져오기
        thisGridLayout = GetComponent<GridLayoutGroup>();       //이 오브젝트의 GridLayoutGroup 가져오기
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 a_PGLCellSize;    //부모(최상위 부모)의 CellSize를 담을 변수

        if (parentGridLayout == null)       //최상위 부모가 없을 경우
            a_PGLCellSize = transform.parent.GetComponent<RectTransform>().sizeDelta;  //부모의 Width, Height값 가져오기
        else
            a_PGLCellSize = parentGridLayout.cellSize;        //최상위 부모의 GridLayoutGroup에 설정된 CellSize 가져오기
                
        Vector2 a_SizeRatio = new (a_PGLCellSize.x / parentWH.x, a_PGLCellSize.y / parentWH.y);    //현재 부모의 크기와 기본크기를 비교해 바뀐 비율 측정

        if(thisGridLayout != null)
            thisGridLayout.cellSize *= a_SizeRatio;         //자식의 CellSize를 비율에 맞춰 변경
    }
}
