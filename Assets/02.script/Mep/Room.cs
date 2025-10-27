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
    public Door GetRandomDoor()
    {    // ���� ��ӹ��� �ڽĿ�����Ʈ �� �� �������� �ϳ� ����)
        return doors[Random.Range(0, doors.Count)];
    }
}
