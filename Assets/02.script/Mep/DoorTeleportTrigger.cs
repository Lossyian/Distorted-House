using UnityEngine;

using System.Collections;

public class DoorTeleportTrigger : MonoBehaviour
{
    [Header("��ŷ�� �ݴ��� Ʈ����")]
    public DoorTeleportTrigger linkedTrigger;

    [HideInInspector] public Door ownerDoor;

    [Header("�ڷ���Ʈ ��Ÿ��")]
    [SerializeField] private float cooldownTime = 0.5f;

    private bool isOnCooldown = false;

    private void Awake()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedTrigger == null || isOnCooldown) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportWithCooldown(other));
    }

    private IEnumerator TeleportWithCooldown(Collider2D player)
    {
        // ��Ÿ�� ����
        isOnCooldown = true;

        //  ��� Ʈ���ŵ� ��� ��Ȱ��ȭ (���� ���� ����)
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = true;

        //  �÷��̾� �̵�
        Vector3 targetPos = linkedTrigger.transform.position;
        player.transform.position = targetPos + Vector3.up * 0.5f;

        //  ��� �� ��Ÿ�� ����
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = false;
    }
}