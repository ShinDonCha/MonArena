using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//���� �Ѿ�� �ε� ���� ǥ���ϱ����� ��ũ��Ʈ
//LoadingCanvas�������� LoadingPanel�� �ٿ��� ���
public class LoadingCtrl : MonoBehaviour
{
    [SerializeField]
    private Image progressBarImg = null;        //�ε��� �󸶳� ����Ǿ����� ��Ÿ�������� �� �̹���
    [SerializeField]
    private Text progressText = null;           //�ε��� �󸶳� ����Ǿ����� ��Ÿ�������� �ؽ�Ʈ

    private readonly float completeProg = 0.9f;  //�� �̵� �غ� �Ϸ������ �˱� ���� ����(Progress�� �Ϸ�� ��ġ = 0.9f)
    private readonly float totalTime = 0.7f;     //���̵��� �Ͼ �ּ� �ð� (Progress�� �Ϸ�Ǿ �ּ��� �� �ð���ŭ ���� �� �� �̵��� �ϱ�����)
    private float calcTime = 0.0f;               //�ּҽð��� ���� ��갪�� �����ϱ����� ����

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());        //�� �ε� �ڷ�ƾ ����
    }

    private IEnumerator LoadScene()         //�� �ε� �ڷ�ƾ
    {
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(FindClass.LoadSceneName.ToString());       //�ش� �� �ε��� ���� ������ �޾ƿ´�.
        sceneAsync.allowSceneActivation = false;       //�ڵ����� ���� �ѱ� �� ������ ����

        while(!sceneAsync.isDone)
        {
            calcTime += Time.deltaTime / totalTime;    //�ּҽð��� ���� ��갪 �߰�

            if (progressBarImg.fillAmount < completeProg)           //�� �̵��� �غ����̶��
                progressBarImg.fillAmount = Mathf.Min(calcTime, sceneAsync.progress);    //�ּҽð��� ���� ��갪�� ���� ����� �� ���� ���� ��������
            else if (progressBarImg.fillAmount >= completeProg)     //�� �̵� �غ� �Ϸ�Ǿ��ٸ�
                progressBarImg.fillAmount += Time.deltaTime / totalTime;    //���� �ּҽð� ��ŭ ���� or ȭ�鿡 ����ִ� ����� ����

            progressText.text = string.Format("�ε� �� ({0:N0}%)", progressBarImg.fillAmount * 100);

            if (progressBarImg.fillAmount >= 1.0f)     //�ּҽð��� ������ ��� ������ �����Ǿ��ٸ�
                sceneAsync.allowSceneActivation = true;     //�� �̵� �㰡

            yield return null;
        }
    }
}
