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
            Debug.Log("들고있는 아이템과 바꾼다요!");
            DropItem();

        }
        inTheHend = itemName;
        Debug.Log($" 인벤토리에 '{itemName}' 추가됨");

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
            Debug.Log("그건 지금못쓴다요.");
            return;
        }
        Debug.Log(" 아이템을 사용한다요!");
        itemMnanger?.UseItem(item);
        ConsumeItem(item);
    }

    
    public void ConsumeItem(string itemName)
    {
        if (inTheHend == itemName)
        {
            Debug.Log("아이템을 사용한다요.");
            DropItem();
        }
    }
}
