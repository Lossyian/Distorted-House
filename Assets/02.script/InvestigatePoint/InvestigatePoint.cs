using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InvestigateType
{
    Empty,  //헌팅타임시 숨을곳
    item,   //아이템
    Trap,   //함정
    proviso,// 단서
    special //격퇴아이템,금고,바닥문

}

public class InvestigatePoint : MonoBehaviour
{
    public InvestigateType type;
    public string dataName;
   

    public void Interact()
    {
        switch(type)
        {
            case InvestigateType.Empty:

                break;
            case InvestigateType.item:

                break;
            case InvestigateType.Trap:

                break;
            case InvestigateType.proviso:

                break;
            case InvestigateType.special:

                break;
        }
    }

    public void HandItemChainge(Inventory inven)
    {
        if (!inven.hasItem)
        {
            inven.Pickup(dataName);
            type = InvestigateType.Empty;
            dataName = "";

            //아이템을 가져감으로, 빈공간이 된다.
        }

        else
        {
            string dropsItem = inven.DropItem();
            //손에든걸 내려놓고 아이템 획득

            string temp = dataName;
            dataName = dropsItem;
            inven.Pickup(temp);


        }
    }


}
