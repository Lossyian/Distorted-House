using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public string inTheHend;
    public bool hasItem => !string.IsNullOrEmpty(inTheHend);
    private ItemMnanger itemMnanger;
    private void Start()
    {
        itemMnanger = FindObjectOfType<ItemMnanger>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUseCurrentItem();
        }
    }

    public void Pickup(string itemName)
    {
        if (hasItem)
        {
            UiManager.instance?.ShowDialog("들고있는 아이템과 바꾸었다.");
            DropItem();

        }
        inTheHend = itemName;
        UiManager.instance?.ShowDialog($"손에 '{itemName}'을(를) 들었다.");

        ApplyPassiveEffect(itemName);
    }

    public string DropItem()
    {
        string drops = inTheHend;

        if (!string.IsNullOrEmpty(drops))
        {
            RemovePassiveEffect(drops);
        }
        inTheHend = "";
        return drops;
    }

    private void ApplyPassiveEffect(string itemName)
    {//패시브 아이템
        switch(itemName)
        {
            case"수상한 부적":
                GameManager.hasCharm = true;
                break;

            case "낡은소화기":
                GameManager.hasExtinguisher = true;
                break; 
        }
    }

    private void RemovePassiveEffect(string itemName)
    {
        switch (itemName)
        {
            case "수상한 부적":
                if (GameManager.hasCharm)
                {
                    GameManager.hasCharm = false;
                    Debug.Log("부적이 부서졌다요...");
                }
                break;

            case "낡은소화기":
                if (GameManager.hasExtinguisher)
                {
                    GameManager.hasExtinguisher = false;
                    Debug.Log("소화기를 다썻다요...");
                }
                break;
        }
    }


    private void TryUseCurrentItem()
    {
        if (!hasItem) return;

        string item = inTheHend;

        if (item == "수상한 부적"||item == "낡은소화기")
        {
            UiManager.instance?.ShowDialog("사용할 수 없어.");
            return;
        }
        
        itemMnanger?.UseItem(item);
        ConsumeItem(item);
    }

    
    public void ConsumeItem(string itemName)
    {
        if (inTheHend == itemName)
        {
            UiManager.instance?.ShowDialog("더는 못쓸것같다.");
            DropItem();
        }
    }
}
