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
        Debug.Log($" �κ��丮�� '{itemName}' �߰���");
    }

    public string DropItem ()
    {
        string drops = inTheHend;
        inTheHend = "";
        Debug.Log($" '{drops}' ��(��) ��������");
        return drops;
    }
}
