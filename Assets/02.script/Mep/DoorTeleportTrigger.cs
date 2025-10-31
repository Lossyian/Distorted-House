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
    {   //�� ��ũ��Ʈ�� ���� ���� ������Ʈ�� �ִ� �ݶ��̴� Ž��
        var col = GetComponent<BoxCollider2D>();
        if (col == null)  //�ݶ��̴�2D ������Ʈ�� ���ٸ�,���� �����.
            col = gameObject.AddComponent<BoxCollider2D>();
        //�ش� �ݶ��̴��� Ʈ���Ÿ� üũ�� �����Ѵ�.
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedTrigger == null || isOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        // ��Ÿ���� ���ְ�,, �ڷ���Ʈ ������ ����Ǿ��ְ�, �±װ� �÷��̾��� �ڷ���Ʈ ����.
        StartCoroutine(TeleportWithCooldown(other));
    }

    private IEnumerator TeleportWithCooldown(Collider2D player)
    {
        // ��Ÿ�ӵ��� �۵� ����
        isOnCooldown = true;

        //  �ݴ��� �ݶ��̴� Ʈ���ŵ� ��Ȱ��ȭ
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = true;

        //  �ݴ��� �ݶ��̴� Ʈ������ position���� �÷��̾� �ڷ���Ʈ
        Vector3 targetPos = linkedTrigger.transform.position;
        player.transform.position = targetPos + Vector3.up * 0.5f;

        //  �ݴ��� �ݶ��̴� ��ġ�� TP������, ������ ��� TP�� �Ǵ°��� ������ ���� �ڷ�ƾ�� �̿��� TP ��Ÿ���� ����.
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = false;
    }
}