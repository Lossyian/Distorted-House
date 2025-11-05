using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    
    public static UiManager instance;
    [Header("UI참조")]
    public GameObject safeLockPanel;
    public SafeLockUI safeLockUI;

    public GameObject gameOverPanel;
    public GameObject gameClearPanel;
    public GameObject hintPopupPanel;

    [Header("대화/단서")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;
    public float dialogShowTime = 3f;

    public GameObject provisoPanel;
    public UnityEngine.UI.Image provisoImage;
    public float provisoShowTime = 4f;

    private Coroutine hintRoutine;
    private Coroutine dialogRoutine;
    private Coroutine provisoRoutine;



    private void Awake()
    {
        if (instance != null && instance !=this)
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
        hintPopupPanel?.SetActive(false);
        dialogPanel?.SetActive(false);
        provisoPanel?.SetActive(false);
    }

    public void openSafeUi (InvestigatePoint safePoint)
    {
        safeLockPanel.SetActive(true);
        safeLockUI?.open();
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

    

    public void ShowDialog(string msg,float duration= -1f)
    {
        if (dialogPanel == null || dialogText == null) return;

        dialogText.text = msg;
        dialogPanel.SetActive(true);

        if (dialogRoutine != null) StopCoroutine(dialogRoutine);
        dialogRoutine = StartCoroutine(DialogRoutine(duration > 0 ? duration : dialogShowTime));

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
            
            ShowDialog("단서 이미지를 표시할 수 없습니다.");
            return;
        }

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
            ShowDialog("단서 이름이 비어 있다요.");
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
    }
    public void ShowGameClear()
    {
        HideAll();
        gameClearPanel?.SetActive(true);
        Time.timeScale = 0f;
    }

}
