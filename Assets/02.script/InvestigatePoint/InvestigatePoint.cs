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

            //�������� ����������, ������� �ȴ�.
        }

        else
        {
            string dropsItem = inven.DropItem();
            //�տ���� �������� ������ ȹ��

            string temp = dataName;
            dataName = dropsItem;
            inven.Pickup(temp);


        }
    }


}
