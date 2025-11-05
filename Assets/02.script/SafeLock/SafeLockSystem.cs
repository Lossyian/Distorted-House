using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class SafeLockSystem : MonoBehaviour
{
    public static SafeLockSystem instance;
    public InvestigatePoint safePoint;
    
    [Header("금고 관련설정")]
    public bool isUnlocked = false;
    public List<string> enteredNumbers = new List<string>();
    private List<string> correctNumbers => GameManager.SafePassword;


    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }
    public void OpenSafeUI(InvestigatePoint safePoint)
    {
        this.safePoint = safePoint;
        UiManager.instance?.openSafeUi(safePoint);
    }

    public void InputNumber(string num)
    {
        if (isUnlocked) return;
        
        if (enteredNumbers.Count >=3)
            enteredNumbers.Clear();

        enteredNumbers.Add(num);
        UiManager.instance?.UpdateSafeDisplay(string.Join("", enteredNumbers));

    }
    public void CheckPassword()
    {
        if(isUnlocked) return;

        if(enteredNumbers.Count >=3)
        {
            UiManager.instance?.ShowDialog("3자리를 입력해야 한다");
            return;
        }

        //순서 상관없이 비교.
        var correctSet = new HashSet<string>(correctNumbers);
        var enteredSet = new HashSet<string>(enteredNumbers);

        bool allMatched = correctSet.IsSubsetOf(enteredSet);

        if (allMatched)
        {
            isUnlocked = true;
            UiManager.instance?.ShowDialog("금고가 열렸다. 열쇠를 얻얻다.");
            Inventory inv = FindObjectOfType<Inventory>();
            if (inv != null)
            {
                inv.Pickup("열쇠");
            }
            UiManager.instance?.closeSafeUi();
        }
        else
        {
            UiManager.instance?.ShowDialog("틀렸다.");
        }

        enteredNumbers.Clear();
    }
    public void ClearInput()
    {
        enteredNumbers.Clear();
        UiManager.instance?.UpdateSafeDisplay("");
    }
}
