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
    }

    public string DropItem ()
    {
        string drops = inTheHend;
        inTheHend = "";
        return drops;
    }
}
