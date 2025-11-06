using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UiManager : MonoBehaviour
{
    
    public static UiManager instance;
    [Header("UI참조")]
    public GameObject safeLockPanel;
    public SafeLockUI safeLockUI;
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

    [Header("대화/단서")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;
    public float dialogShowTime = 3f;

    public GameObject provisoPanel;
    public UnityEngine.UI.Image provisoImage;
    public float provisoShowTime = 4f;

    private Queue<string> dialogQueue = new Queue<string>();
    private bool isDialogShowing = false;

    private Coroutine provisoRoutine;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() => HideAll();


    private void HideAll()
    {
        safeLockPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        gameClearPanel?.SetActive(false);
        dialogPanel?.SetActive(false);
        provisoPanel?.SetActive(false);
    }

   

    public void openSafeUi (InvestigatePoint safePoint)
    {
        StartCoroutine(openSafeUiCoroutine());
    }

    private IEnumerator openSafeUiCoroutine()
    {
        
        safeLockPanel.SetActive(true);
        safeLockUI?.open();
        yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = 0f;
    }
    public void closeSafeUi ()
    {
        safeLockUI?.close();
        safeLockPanel?.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateSafeDisplay(string text)
    {
        safeLockUI?.UpdateDisplay(text);
    }



    public void ShowDialog(string msg, float duration = -1f)
    {
        if (string.IsNullOrEmpty(msg)) return;
        dialogQueue.Enqueue(msg);
        if (!isDialogShowing)
            StartCoroutine(ProcessDialogQueue(duration > 0 ? duration : dialogShowTime));
    }

    private IEnumerator ProcessDialogQueue(float duration)
    {
        isDialogShowing = true;

        while (dialogQueue.Count > 0)
        {
            string nextMsg = dialogQueue.Dequeue();

            if (dialogPanel != null && dialogText != null)
            {
                dialogPanel.SetActive(true);
                dialogText.text = nextMsg;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                // 금고 UI 열려 있으면 잠시 대기 (겹침 방지)
                if (safeLockPanel != null && safeLockPanel.activeSelf)
                    yield return null;
                else
                    elapsed += Time.unscaledDeltaTime;

                yield return null;
            }

            dialogPanel.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f); // 메시지 간 약간의 간격
        }

        isDialogShowing = false;
    }

    private IEnumerator DialogRoutine(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        dialogPanel.SetActive(false);
    }

    public void ShowProvisoImage(Sprite s, float duration = -1f)
    {
        if (provisoPanel == null || provisoImage == null)
        {
            
            return;
        }

        StartCoroutine(ShowProvisoCoroutine(s, duration));
    }
    private IEnumerator ShowProvisoCoroutine(Sprite s, float duration)
    {
        yield return null;

        provisoImage.sprite = s;
        provisoPanel.SetActive(true);

        if (provisoRoutine != null)
        {
            StopCoroutine(provisoRoutine);
        }
        provisoRoutine = StartCoroutine(ProvisoRoutine(duration > 0 ? duration : provisoShowTime));
    }

    private IEnumerator ProvisoRoutine(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        provisoPanel.SetActive(false);
    }
    public void ShowProvisoByName(string provisoName, float duration = -1f)
    {
        if (string.IsNullOrEmpty(provisoName))
        {
            return;
        }

        Sprite s = Resources.Load<Sprite>($"Provisos/{provisoName}");

        if (s != null)
        {
            ShowProvisoImage(s, duration);
        }
       
    }
    public void ShowGameOver()
    {
        HideAll();
        gameOverPanel?.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(StopNPLAY());
    }
    public void ShowGameClear()
    {
        HideAll();
        gameClearPanel?.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(StopNPLAY());
    }
    public void OnClickReturnToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator StopNPLAY()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
