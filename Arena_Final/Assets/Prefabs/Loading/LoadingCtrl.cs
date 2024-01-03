using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//씬을 넘어갈때 로딩 중을 표시하기위한 스크립트
//LoadingCanvas프리팹의 LoadingPanel에 붙여서 사용
public class LoadingCtrl : MonoBehaviour
{
    [SerializeField]
    private Image progressBarImg = null;        //로딩이 얼마나 진행되었는지 나타내기위한 바 이미지
    [SerializeField]
    private Text progressText = null;           //로딩이 얼마나 진행되었는지 나타내기위한 텍스트

    private readonly float completeProg = 0.9f;  //씬 이동 준비가 완료됐음을 알기 위한 변수(Progress가 완료된 수치 = 0.9f)
    private readonly float totalTime = 0.7f;     //씬이동이 일어날 최소 시간 (Progress가 완료되어도 최소한 이 시간만큼 지난 후 씬 이동을 하기위함)
    private float calcTime = 0.0f;               //최소시간을 위한 계산값을 저장하기위한 변수

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());        //씬 로딩 코루틴 실행
    }

    private IEnumerator LoadScene()         //씬 로딩 코루틴
    {
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(FindClass.LoadSceneName.ToString());       //해당 씬 로딩에 대한 정보를 받아온다.
        sceneAsync.allowSceneActivation = false;       //자동으로 씬을 넘길 수 없도록 막기

        while(!sceneAsync.isDone)
        {
            calcTime += Time.deltaTime / totalTime;    //최소시간을 위한 계산값 추가

            if (progressBarImg.fillAmount < completeProg)           //씬 이동을 준비중이라면
                progressBarImg.fillAmount = Mathf.Min(calcTime, sceneAsync.progress);    //최소시간을 위한 계산값과 실제 진행률 중 낮은 값을 가져오기
            else if (progressBarImg.fillAmount >= completeProg)     //씬 이동 준비가 완료되었다면
                progressBarImg.fillAmount += Time.deltaTime / totalTime;    //남은 최소시간 만큼 지연 or 화면에 띄워주는 진행률 정돈

            progressText.text = string.Format("로딩 중 ({0:N0}%)", progressBarImg.fillAmount * 100);

            if (progressBarImg.fillAmount >= 1.0f)     //최소시간을 포함한 모든 조건이 충족되었다면
                sceneAsync.allowSceneActivation = true;     //씬 이동 허가

            yield return null;
        }
    }
}
