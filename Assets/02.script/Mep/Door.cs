using System.Xml.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door connectedDoor;
    public DoorTeleportTrigger teleportTrigger;

    private void Awake()
    {
        // �ڽ� �߿� DoorTeleportTrigger�� ���� ������Ʈ�� �̹� ������ ȣ���ϱ�
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();

        // ������ ���� �����ϱ�.
        if (teleportTrigger == null)
        {
            GameObject triggerObj = new GameObject("DoorTrigger_" + name);
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;

            teleportTrigger = triggerObj.AddComponent<DoorTeleportTrigger>();

            BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.0f, 1.0f); // �� �� ���� ����
        }

        // �ڽ� Ʈ������ door �ʵ� ���� 
        teleportTrigger.ownerDoor = this;
    }

    public void ConnectDoor(Door otherDoor)
    {
        // ������ ���� �����ϱ� �� ���� ��ũ��Ʈ ������ ���θ� �Ҵ�.
        connectedDoor = otherDoor;
        otherDoor.connectedDoor = this;

        // �� �� ��� Ʈ���� ���� �Ϸ� �� Ʈ���ų��� ����
        if (teleportTrigger == null)
        { //Ʈ���ź����� ����ִٸ�, �ڽ� ������Ʈ��, DoorTeleportTrigger ��ũ��Ʈ�� ���� ������Ʈ�� Ʈ���� ������ �Ҵ�.
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();
        }
        if (otherDoor.teleportTrigger == null)
            otherDoor.teleportTrigger = otherDoor.GetComponentInChildren<DoorTeleportTrigger>();

        if (teleportTrigger != null && otherDoor.teleportTrigger != null)
        { // ���� ���� �ִ� Ʈ������ linkedTrigger����, �ݴ��� ���� Ʈ���Ÿ� �Ҵ�
            teleportTrigger.linkedTrigger = otherDoor.teleportTrigger;
            //�ݴ��� ���� �Ȱ��� ����.
            otherDoor.teleportTrigger.linkedTrigger = teleportTrigger;

            Debug.Log($"{name} �� {otherDoor.name} Ʈ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning($"{name} �Ǵ� {otherDoor.name} Ʈ���� ���� ���� (Ʈ���Ű� ���� ����)");
        }
    }
}