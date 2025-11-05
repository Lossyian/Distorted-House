using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SafeLockUI : MonoBehaviour
{
    [Header("UI ÂüÁ¶")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private GameObject safePanel;

    [SerializeField] private Button[] numberbuttons;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button clearButton;
    void Start()
    {
        safePanel.SetActive(false);

        for (int i = 0; i< numberbuttons.Length; i++)
        {
            int digit = i + 1;
            numberbuttons[i].onClick.AddListener(() => OnNumberPressed(digit));
        }
        enterButton.onClick.AddListener(OnEnterPressed);
        closeButton.onClick.AddListener(OnClosePressed);
        if (clearButton != null) clearButton.onClick.AddListener(OnClearPressed);

    }

    public void open() => gameObject.SetActive(true);
    public void close() => gameObject.SetActive(false);

   
    public void UpdateDisplay(string text)
    {
        if (displayText != null)
            displayText.text = text;
    }
    public void OnNumberPressed(int digit)
    {
        SafeLockSystem.instance?.InputNumber(digit.ToString());
    }

    public void OnEnterPressed()
    {
        SafeLockSystem.instance?.CheckPassword();
    }

    public void OnClosePressed()
    {
        SafeLockSystem.instance?.enteredNumbers.Clear();
        UiManager.instance?.closeSafeUi();
    }

    public void OnClearPressed()
    {
        SafeLockSystem.instance?.ClearInput();
    }
}
