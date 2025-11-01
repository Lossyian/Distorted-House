using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Door> doors;
    public List<InvestigatePoint> points = new List<InvestigatePoint>();
   

    private void Awake()
    {
        points.AddRange(GetComponentsInChildren<InvestigatePoint>());
    }

    

    public Door GetRandomDoor(Door ex = null)
    {    // ���� ��ӹ��� �ڽĿ�����Ʈ �� �� �������� �ϳ� ����)
        List<Door> available = new List<Door>(doors);
        if (ex != null)
        {
            available.Remove(ex);
        }
        if (available.Count == 0)
        {
            return null;
        }
        return available[Random.Range(0, available.Count)];
    }

    public void LockrandomDoor()
    {
        if (doors == null || doors.Count == 0) return;

        Door target = doors[Random.Range(0, doors.Count)];
        Collider2D col = target.GetComponent<Collider2D>();
        if (col != null )
        {
            col.enabled = false;
            Debug.Log("���� ���ٿ�!");
        }
    }


}
