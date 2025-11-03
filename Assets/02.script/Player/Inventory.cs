using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public string inTheHend;
    public bool hasItem => !string.IsNullOrEmpty(inTheHend);

    public void Pickup(string itemName)
    {
        inTheHend = itemName;
        Debug.Log($" 인벤토리에 '{itemName}' 추가됨");

        ApplyPassiveEffect(itemName);
    }

    public string DropItem()
    {
        string drops = inTheHend;

        if(!string.IsNullOrEmpty(drops))
        {
            RemovePassiveEffect(drops);
            Debug.Log($" '{drops}' 을(를) 내려놓음");
        }

        inTheHend = "";
        return drops;
    }

    private void ApplyPassiveEffect(string itemName)
    {
        switch(itemName)
        {
            case"수상한 부적":
                GameManager.hasCharm = true;
                Debug.Log("부적이 널 지켜줬다요.");
                break;

            case "낡은소화기":
                GameManager.hasExtinguisher = false;
                Debug.Log("소화기로 널 지켯다요.");
                break; 
        }
    }

    private void RemovePassiveEffect(string itemName)
    {
        switch (itemName)
        {
            case "수상한 부적":
                GameManager.hasCharm = false;
                Debug.Log("부적이 효과가 사라졌다요.");
                break;

            case "낡은소화기":
                GameManager.hasExtinguisher = false;
                Debug.Log("소화기를 다썻다요.");
                break;
        }
    }

    public void ConsumeItem(string itemName)
    {
        if (inTheHend == itemName)
        {
            Debug.Log("아이템을 사용했다요.");
            DropItem();
        }
    }
}
