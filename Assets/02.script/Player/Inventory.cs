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
    }

    public string DropItem ()
    {
        string drops = inTheHend;
        inTheHend = "";
        Debug.Log($" '{drops}' 을(를) 내려놓음");
        return drops;
    }
}
