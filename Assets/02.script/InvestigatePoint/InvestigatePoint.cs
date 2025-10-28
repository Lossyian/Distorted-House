using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InvestigateType
{
    Empty,  //����Ÿ�ӽ� ������
    item,   //������
    Trap,   //����
    proviso,// �ܼ�
    special //���������,�ݰ�,�ٴڹ�

}

public class InvestigatePoint : MonoBehaviour
{
    public InvestigateType type;
    public string dataName;
   

    public void Interact()
    {

        var inventory = FindObjectOfType<Inventory>();

        switch(type)
        {

            case InvestigateType.Empty:
                if (!inventory.hasItem)
                {
                    inventory.Pickup(dataName);
                    type = InvestigateType.Empty;
                    dataName = "";

                    //�������� ����������, ������� �ȴ�.
                }
                else
                {  //���� �������� �������� �� �������� ��������.
                    string drops = inventory.DropItem();
                    inventory.Pickup(dataName);
                    dataName = drops;
                }
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

   


}
