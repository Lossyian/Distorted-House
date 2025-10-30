using System.Xml.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door connectedDoor;
    public DoorTeleportTrigger teleportTrigger;

    private void Awake()
    {
        // �ڽ� �߿� DoorTeleportTrigger�� �̹� ������ ��������
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();

        // ������ ���� ����
        if (teleportTrigger == null)
        {
            GameObject triggerObj = new GameObject("DoorTrigger_" + name);
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;

            teleportTrigger = triggerObj.AddComponent<DoorTeleportTrigger>();

            BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 0.8f); // �� �� ���� ����
        }

        // �ڽ� Ʈ������ door �ʵ� ���� (���߿� ��������)
        teleportTrigger.ownerDoor = this;
    }

    public void ConnectDoor(Door otherDoor)
    {
        // ������ ����
        connectedDoor = otherDoor;
        otherDoor.connectedDoor = this;

        // �� �� ��� Ʈ���� ����/�Ҵ� �Ϸ� �� Ʈ���ų��� ����
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();
        if (otherDoor.teleportTrigger == null)
            otherDoor.teleportTrigger = otherDoor.GetComponentInChildren<DoorTeleportTrigger>();

        if (teleportTrigger != null && otherDoor.teleportTrigger != null)
        {
            teleportTrigger.linkedTrigger = otherDoor.teleportTrigger;
            otherDoor.teleportTrigger.linkedTrigger = teleportTrigger;

            Debug.Log($"{name} �� {otherDoor.name} Ʈ���� ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning($"{name} �Ǵ� {otherDoor.name} Ʈ���� ���� ���� (Ʈ���Ű� ���� ����)");
        }
    }
}